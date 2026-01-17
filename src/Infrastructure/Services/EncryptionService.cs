using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CesiZen.Infrastructure.Services;
public sealed class EncryptionService(
    string key,
    string iv
) : IEncryptionService {
    
    #region PROPERTIES

        private readonly byte[] key = Encoding.UTF8.GetBytes(key);
        private readonly byte[] iv  = Encoding.UTF8.GetBytes(iv);

    #endregion
    #region CONSTRUCTORS

        public EncryptionService(IConfiguration configuration) : this(
            configuration["Users:Encryption:Key"]!,
            configuration["Users:Encryption:IV"]!
        ) {}

    #endregion
    #region METHODS

        public string Encrypt(string data) {

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV  = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))  {
                using var streamWriter = new StreamWriter(cryptoStream);
                streamWriter.Write(data);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string encryptedData) {

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV  = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedData));
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }

    #endregion
}