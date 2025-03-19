using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SYSMANU.Models;
using System;

namespace SYSMANU.Controllers
{
    [Route("Cliente")]
    public class ClienteController : Controller
    {
        private readonly MySQLConnectionService _connectionService;

        public ClienteController(MySQLConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public IActionResult CadastroCliente()
        {
            return View();
        }



        [HttpPost("insert_cliente")]
        public IActionResult Inserir([FromForm] Cliente cliente)
        {
            using (MySqlConnection conn = _connectionService.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Obtém o maior COD_CLIENTE e adiciona +1, usando COALESCE para evitar NULL
                    string getMaxIdSql = "SELECT COALESCE(MAX(COD_CLIENTE), 0) + 1 FROM CLIENTE";
                    int V_NOVO_COD_CLIENTE;

                    using (MySqlCommand getMaxCmd = new MySqlCommand(getMaxIdSql, conn))
                    {
                        V_NOVO_COD_CLIENTE = Convert.ToInt32(getMaxCmd.ExecuteScalar());
                    }

                    string sql = @"INSERT INTO CLIENTE (COD_CLIENTE, NOME, TELEFONE, ENDERECO, CIDADE) 
                                VALUES (@COD_CLIENTE, @NOME, @TELEFONE, @ENDERECO, @CIDADE)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@COD_CLIENTE", V_NOVO_COD_CLIENTE);
                        cmd.Parameters.AddWithValue("@NOME", cliente.NOME);
                        cmd.Parameters.AddWithValue("@TELEFONE", cliente.TELEFONE);
                        cmd.Parameters.AddWithValue("@ENDERECO", cliente.ENDERECO);
                        cmd.Parameters.AddWithValue("@CIDADE", cliente.CIDADE);

                        cmd.ExecuteNonQuery();
                    }

                    // Define a mensagem de sucesso na ViewBag para exibir na mesma tela
                    ViewBag.Status_operacao = $"Cliente {V_NOVO_COD_CLIENTE} inserido com sucesso!";

                    return View("CadastroCliente"); // Retorna para a mesma página
                }
                catch (Exception ex)
                {
                    // Define mensagem de erro na ViewBag e retorna para a mesma tela
                    ViewBag.Status_operacao = $"Erro ao inserir Cliente: {ex.Message}";
                    return View("CadastroCliente");
                }
            }
        }





         //============================================//CONSULTA-CLIENTE//=============================================

        [HttpPost("buscar_cliente")]
        public IActionResult BuscarCliente([FromForm] string COD_CLIENTE)
        {
           /* if (string.IsNullOrEmpty(COD_CLIENTE))
            {
                ViewBag.Error = "Código do cliente não informado.";
                return View("CadastroCliente");
            } */

            Cliente? cliente = null; // Permitir que seja nulo explicitamente
            int? v_cod_retonado = null;

            using (MySqlConnection conn = _connectionService.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COD_CLIENTE, NOME, TELEFONE, ENDERECO, CIDADE FROM CLIENTE WHERE COD_CLIENTE = @cod";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cod", COD_CLIENTE);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                 v_cod_retonado = reader["COD_CLIENTE"] != DBNull.Value ? Convert.ToInt32(reader["COD_CLIENTE"]) : 0;


                                cliente = new Cliente
                                {                                                                    
                                    COD_CLIENTE = reader["COD_CLIENTE"] != DBNull.Value ? Convert.ToInt32(reader["COD_CLIENTE"]) : 0,
                                    NOME        = reader["NOME"]?.ToString() ?? string.Empty,
                                    TELEFONE    = reader["TELEFONE"]?.ToString() ?? string.Empty,
                                    ENDERECO    = reader["ENDERECO"]?.ToString() ?? string.Empty,
                                    CIDADE      = reader["CIDADE"]?.ToString() ?? string.Empty
                                };

                                if (v_cod_retonado == 0)
                                {   
                                     ViewBag.Status_operacao = "Nenhum cliente encontrado";
                                    
                                }
                            } 
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Erro ao buscar Cliente: {ex.Message}";
                    return View("CadastroCliente");
                }
            }

            return View("CadastroCliente", cliente);
        }



        //============================================//ALTERA-CLIENTE//=============================================
        [HttpPost("alterar_cliente")]
        public IActionResult AlterarCliente([FromForm] Cliente cliente)
        {
            if (cliente == null || cliente.COD_CLIENTE == 0)
            {
                return Content("Código do cliente não informado.");
                
            } 

            using (MySqlConnection conn = _connectionService.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Verifica se o cliente existe antes de atualizar
                    string checkSql = "SELECT COUNT(*) FROM CLIENTE WHERE COD_CLIENTE = @COD_CLIENTE";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@COD_CLIENTE", cliente.COD_CLIENTE);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            return Content($"Cliente com código {cliente.COD_CLIENTE} não encontrado.");
                        }
                    }

                    string sql = @"UPDATE CLIENTE 
                                    SET NOME     = @NOME, 
                                        TELEFONE = @TELEFONE, 
                                        ENDERECO = @ENDERECO, 
                                        CIDADE   = @CIDADE 
                                    WHERE COD_CLIENTE = @COD_CLIENTE";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@COD_CLIENTE", cliente.COD_CLIENTE);
                        cmd.Parameters.AddWithValue("@NOME", cliente.NOME);
                        cmd.Parameters.AddWithValue("@TELEFONE", cliente.TELEFONE);
                        cmd.Parameters.AddWithValue("@ENDERECO", cliente.ENDERECO);
                        cmd.Parameters.AddWithValue("@CIDADE", cliente.CIDADE);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                           ViewBag.Status_operacao = $"Os dados do cliente: {cliente.COD_CLIENTE} - {cliente.NOME} foram alterados ";
                           return View("CadastroCliente");
                        }
                        else
                            ViewBag.Status_operacao = $"ERRO: Nenhuma alteração foi aplicada para o cliente: {cliente.COD_CLIENTE} verifique!";
                            return View("CadastroCliente");
                    }
                }
                catch (Exception ex)
                {
                    return Content($"Erro ao atualizar Cliente: {ex.Message}");
                }
            }
        }

        //============================================//DELETA-CLIENTE//=============================================
        [HttpPost("Excluir_cliente")]
        public IActionResult ExcluirCliente([FromForm] Cliente cliente)
        {
            if (cliente == null || cliente.COD_CLIENTE == 0)
            {
                return Content("Código do cliente não informado.");
                
            } 

            using (MySqlConnection conn = _connectionService.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Verifica se o cliente existe antes de excluir
                    string checkSql = "SELECT COUNT(*) FROM CLIENTE WHERE COD_CLIENTE = @COD_CLIENTE";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@COD_CLIENTE", cliente.COD_CLIENTE);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            
                           ViewBag.Status_opracao = $"cliente nao encontrado {cliente.NOME} ";
                           return View("CadastroCliente");
                        }
                    }

                    string sql = @"DELETE FROM CLIENTE 
                                    WHERE COD_CLIENTE = @COD_CLIENTE";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@COD_CLIENTE", cliente.COD_CLIENTE);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                         // string v_cliente = cliente.NOME ?? "NAO RETORNADO"; 

                           ViewBag.Status_operacao = $"cliente: {cliente.NOME} Excluido ";
                           return View("CadastroCliente");
                        }
                        else
                            ViewBag.Status_operacao = $"ERRO: ao excluir cliente: {cliente.NOME} verifique!";
                            return View("CadastroCliente");
                    }
                }
                catch (Exception ex)
                {
                    return Content($"Erro ao excluir Cliente: {ex.Message}");
                }
            }
        }

    }
}
