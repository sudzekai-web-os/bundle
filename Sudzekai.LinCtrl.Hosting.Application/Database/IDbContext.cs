using System.Data.Common;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    internal interface IDbContext
    {
        public DbConnection CreateConnection();

    }
}
