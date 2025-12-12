using System.Security.Cryptography;
using System.Text;

namespace TechChallenge.Domain.Helpers;

public static class EncryptionHelper
{
    private static readonly byte[] Key = Convert.FromHexString(
        "6831132c1d00ce37ad2a61d4cc6d91d154561b135a3aea8bb460179e73af18ac"
    );
    
    private const int IvSize = 16;

    public static string EncryptDocumentNumber(string documentNumber)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = RandomNumberGenerator.GetBytes(IvSize);

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        byte[] encryptedBytes;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(documentNumber);
            }
            encryptedBytes = ms.ToArray();
        }

        var result = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    public static string DecryptDocumentNumber(string encryptedDocumentNumber)
    {
        const string prefix = "ENCRYPTED_";
        if (encryptedDocumentNumber.StartsWith(prefix))
        {
            encryptedDocumentNumber = encryptedDocumentNumber[prefix.Length..];
        }
        
        var buffer = Convert.FromBase64String(encryptedDocumentNumber);

        if (buffer.Length < IvSize)
            throw new ArgumentException("Encrypted data is too short to contain IV.");

        var iv = new byte[IvSize];
        Buffer.BlockCopy(buffer, 0, iv, 0, IvSize);

        var cipherTextLength = buffer.Length - IvSize;
        var cipherText = new byte[cipherTextLength];
        Buffer.BlockCopy(buffer, IvSize, cipherText, 0, cipherTextLength);

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }

    public static string CreateIndexHash(string documentNumber)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(documentNumber)));
}