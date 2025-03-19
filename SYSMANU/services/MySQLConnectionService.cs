using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // Para log (opcional)

public class MySQLConnectionService
{
    private readonly string _connectionString;
    private readonly ILogger<MySQLConnectionService> _logger; // Para log (opcional)

    public MySQLConnectionService(IConfiguration configuration, ILogger<MySQLConnectionService> logger)
    {
        _logger = logger; // Inicializa o logger

        // Obtém a string de conexão do arquivo de configuração
        _connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(_connectionString))
        {
            _logger.LogError("A string de conexão 'DefaultConnection' não foi encontrada ou está vazia.");
            throw new InvalidOperationException("A string de conexão não pode ser nula ou vazia.");
        }
    }

    public MySqlConnection GetConnection()
    {
        try
        {
            return new MySqlConnection(_connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar a conexão com o banco de dados.");
            throw;
        }
    }
}
