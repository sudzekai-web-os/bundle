using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class MySqlConnectionFactory(IConfiguration configuration)
    {
        private readonly string _connectionString = new MySqlConnectionStringBuilder
        {
            Server = Required(configuration, "Host"),
            Database = Required(configuration, "Database"),
            UserID = Required(configuration, "Username"),
            Password = Required(configuration, "Password"),
            SslMode = MySqlSslMode.Required
        }.ConnectionString;

        public MySqlConnection Create()
            => new(_connectionString);

        private static string Required(IConfiguration configuration, string key)
            => configuration[$"LinCtrlDatabase:{key}"] is { Length: > 0 } value
                ? value
                : throw new InvalidOperationException($"Missing configuration: LinCtrlDatabase:{key}");
    }
}
