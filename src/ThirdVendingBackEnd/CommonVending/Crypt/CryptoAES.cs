using System;
using System.IO;
using System.Security.Cryptography;

namespace CommonVending.Crypt
{
  public static class CryptoAes
  {
    private const string AesKey = "XPYW/ysSLa6pnIDEFe3qeh8AHkyu2VMgyajcTobIcSQ=";
    private const string AesIv = "2PNly+555b8emLivdGReFw==";

    private static byte[] AesKeyBytes => Convert.FromBase64String(AesKey);

    private static byte[] AesIvBytes => Convert.FromBase64String(AesIv);

    public static byte[] EncryptAes(string plainText)
    {
      using var aes = new AesCryptoServiceProvider {Key = AesKeyBytes, IV = AesIvBytes, Mode = CipherMode.CBC, Padding = PaddingMode.Zeros};

      var enc = aes.CreateEncryptor(aes.Key, aes.IV);

      using var ms = new MemoryStream();
      using var cs = new CryptoStream(ms, enc, CryptoStreamMode.Write);
      using (var sw = new StreamWriter(cs)) { sw.Write(plainText); }

      var encrypted = ms.ToArray();

      return encrypted;
    }

    public static string DecryptAes(byte[] encryptedBytes)
    {
      using var aes = new AesCryptoServiceProvider {Key = AesKeyBytes, IV = AesIvBytes, Mode = CipherMode.CBC, Padding = PaddingMode.Zeros};

      var dec = aes.CreateDecryptor(aes.Key, aes.IV);

      using var ms = new MemoryStream(encryptedBytes);
      using var cs = new CryptoStream(ms, dec, CryptoStreamMode.Read);
      using var sr = new StreamReader(cs);
      var decrypted = sr.ReadToEnd();

      return decrypted;
    }
  }
}
