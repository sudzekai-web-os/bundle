using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Domain.Authentication;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class MySqlUserRepository(MySqlConnectionFactory connections) : IUserRepository
    {
        public async Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            await using var connection = connections.Create();
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT id, username, password_hash, role_id, is_blocked
                FROM users WHERE username = @username
                """;
            command.Parameters.AddWithValue("@username", username);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
                return null;

            return new(reader.GetInt64(0), reader.GetString(1), reader.GetString(2),
                (ClientRole)reader.GetInt32(3), reader.GetBoolean(4));
        }
    }
}
