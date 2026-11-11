using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Sudzekai.LinCtrl.Hosting
{
    public sealed class PasswordHashService
    {
        private const int _memorySize = 19456;
        private const int _iterations = 2;
        private const int _saltLength = 16;
        private const int _hashLength = 32;
        private const string _parameters = "m=19456,t=2,p=1";
        private readonly byte[] _dummySalt = RandomNumberGenerator.GetBytes(_saltLength);

        public string Hash(string password)
        {
            ArgumentException.ThrowIfNullOrEmpty(password);
            var salt = RandomNumberGenerator.GetBytes(_saltLength);
            var hash = Derive(password, salt);
            try
            {
                return $"$argon2id$v=19${_parameters}${Encode(salt)}${Encode(hash)}";
            }
            finally
            {
                CryptographicOperations.ZeroMemory(hash);
            }
        }

        public bool Verify(string password, string? passwordHash)
        {
            var salt = _dummySalt;
            byte[]? expected = null;
            if (passwordHash is { Length: <= 255 })
            {
                var parts = passwordHash.Split('$');
                if (parts is ["", "argon2id", "v=19", _parameters, var encodedSalt, var encodedHash])
                {
                    var decodedSalt = new byte[_saltLength];
                    var decodedHash = new byte[_hashLength];
                    if (Decode(encodedSalt, decodedSalt) && Decode(encodedHash, decodedHash))
                    {
                        salt = decodedSalt;
                        expected = decodedHash;
                    }
                }
            }

            var actual = Derive(password, salt);
            try
            {
                return expected != null && CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(actual);
                if (expected != null)
                    CryptographicOperations.ZeroMemory(expected);
            }
        }

        private static string Encode(byte[] value)
            => Convert.ToBase64String(value).TrimEnd('=');

        private static bool Decode(string value, byte[] destination)
            => Convert.TryFromBase64String(value.PadRight((value.Length + 3) / 4 * 4, '='), destination, out var length)
                && length == destination.Length;

        private static byte[] Derive(string password, byte[] salt)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            try
            {
                using var argon = new Argon2id(bytes)
                {
                    Salt = salt,
                    MemorySize = _memorySize,
                    Iterations = _iterations,
                    DegreeOfParallelism = 1
                };
                return argon.GetBytes(_hashLength);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(bytes);
            }
        }
    }
}
