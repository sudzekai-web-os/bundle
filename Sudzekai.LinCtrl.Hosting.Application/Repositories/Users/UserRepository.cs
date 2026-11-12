using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Application.Objects.Models;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    internal sealed class UserRepository(IDbContext connections) : IUserRepository
    {
        public async Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            await using var connection = connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT id, username, password_hash, role_id, is_blocked
                FROM users WHERE username = @username
                """;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@username";
            parameter.Value = username;
            command.Parameters.Add(parameter);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
                return null;

            return new(reader.GetInt64(0), reader.GetString(1), reader.GetString(2),
                (ClientRole)reader.GetInt32(3), reader.GetBoolean(4));
        }
    }
}
