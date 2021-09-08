using System;
using System.IO;
using System.Security.Cryptography;

namespace CommonVending.Crypt
{
  public static class Crypto
  {
    private static readonly byte[] DESKey =
    {
      83, 104, 118, 101,
      116, 115, 0, 32
    };

    private static readonly byte[] DESInitializationVector =
    {
      16, 221, 118, 179,
      165, 130, 125, 143,
      200, 45
    };

    public static string Encrypt(string value) { return DESEncrypt(value); }

    public static string Decrypt(string value) { return DESDecrypt(value); }

    private static string DESEncrypt(string plainText)
    {
      using var cryptoProvider = new DESCryptoServiceProvider();
      using (var memoryStream = new MemoryStream())
      using (var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(DESKey, DESInitializationVector), CryptoStreamMode.Write))
      using (var writer = new StreamWriter(cryptoStream))
      {
        writer.Write(plainText);
        writer.Flush();
        cryptoStream.FlushFinalBlock();
        writer.Flush();
        return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
      }
    }

    private static string DESDecrypt(string cipherText)
    {
      try
      {
        using (var cryptoProvider = new DESCryptoServiceProvider())
        {
          using (var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText)))
          using (var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(DESKey, DESInitializationVector), CryptoStreamMode.Read))
          using (var reader = new StreamReader(cryptoStream)) { return reader.ReadToEnd(); }
        }
      }
      catch (Exception exception)
      {
        Console.WriteLine($"Crypto error - {exception.Message}", exception);
        return null;
      }
    }
  }
}
