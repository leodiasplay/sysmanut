using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SYSMANU.Models;
using System;

namespace SYSMANU.Controllers
{
    [Route("usuario")]
    public class UsuarioController : Controller
    {
        private readonly MySQLConnectionService _connectionService;

        public UsuarioController(MySQLConnectionService connectionService)
        {
            _connectionService = connectionService;
        }


        public IActionResult CadastroUsuario()
        {
            return View();
        }

        // Inserir usuário no banco
        [HttpGet("usuario/cadastro")]
        public IActionResult Inserir([FromForm] Usuario usuario)
        {
            using (MySqlConnection conn = _connectionService.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Verifica se o COD_USUARIO já existe no banco
                    string checkSql = "SELECT COUNT(*) FROM USUARIO WHERE COD_USUARIO = @COD_USUARIO";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@COD_USUARIO", usuario.COD_USUARIO);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            return Content("Erro: O código de usuário já existe!");
                        }
                    }

                    string sql = "INSERT INTO USUARIO (COD_USUARIO, MATRICULA, SENHA, EMAIL, NIVEL, CARGO, NOME) " +
                                 "VALUES (@COD_USUARIO, @MATRICULA, @SENHA, @EMAIL, @NIVEL, @CARGO, @NOME)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@COD_USUARIO", usuario.COD_USUARIO);
                        cmd.Parameters.AddWithValue("@MATRICULA", usuario.MATRICULA);
                        cmd.Parameters.AddWithValue("@SENHA", usuario.SENHA);
                        cmd.Parameters.AddWithValue("@EMAIL", usuario.EMAIL);
                        cmd.Parameters.AddWithValue("@NIVEL", usuario.NIVEL);
                        cmd.Parameters.AddWithValue("@CARGO", usuario.CARGO);
                        cmd.Parameters.AddWithValue("@NOME",  usuario.NOME);

                        cmd.ExecuteNonQuery();
                    }

                    return Content("Usuário inserido com sucesso!");
                }
                catch (Exception ex)
                {
                    return Content($"Erro ao inserir usuário: {ex.Message}");
                }
            }
        }
    }
}
