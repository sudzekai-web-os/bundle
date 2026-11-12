using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    internal sealed class AppDbContext(string connectionString) : IDbContext
    {
        public DbConnection CreateConnection()
            => new MySqlConnection(connectionString);
    }
}
