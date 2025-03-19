using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Registra o MySQLConnectionService como um serviço Singleton
builder.Services.AddSingleton<MySQLConnectionService>();

// Adiciona os serviços ao contêiner
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Testa a conexão com o banco de dados MySQL antes de iniciar o aplicativo
try
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    using (var connection = new MySqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("Conexão bem-sucedida com o banco de dados MySQL!");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Erro ao conectar com o banco de dados MySQL: " + ex.Message);
    // Pode lançar uma exceção aqui se quiser impedir o início do aplicativo caso falhe a conexão.
    throw;
}

// Configura o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // HTTP Strict Transport Security (HSTS)
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Configuração de arquivos estáticos (se aplicável)
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
