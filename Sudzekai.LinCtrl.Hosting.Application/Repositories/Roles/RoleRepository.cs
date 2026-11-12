using Sudzekai.LinCtrl.Hosting.Application.Objects.Models;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    internal sealed class RoleRepository(IDbContext connections) : IRoleRepository
    {
        public async Task EnsureRolesAsync(IReadOnlyList<Role> roles, CancellationToken cancellationToken)
        {
            await using var connection = connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;

            foreach (var role in roles.OrderBy(role => role.Id))
            {
                command.CommandText = """
                    INSERT INTO roles (id, name) VALUES (@id, @name)
                    ON DUPLICATE KEY UPDATE id = roles.id
                    """;
                command.Parameters.Clear();
                var id = command.CreateParameter();
                id.ParameterName = "@id";
                id.Value = (int)role.Id;
                command.Parameters.Add(id);

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "@name";
                nameParameter.Value = role.Name;
                command.Parameters.Add(nameParameter);
                await command.ExecuteNonQueryAsync(cancellationToken);

                command.CommandText = "SELECT name FROM roles WHERE id = @id FOR UPDATE";
                var name = await command.ExecuteScalarAsync(cancellationToken) as string;
                if (!string.Equals(name, role.Name, StringComparison.Ordinal))
                    throw new InvalidOperationException($"Database role conflicts with ClientRole.{role.Id} ({(int)role.Id})");
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
