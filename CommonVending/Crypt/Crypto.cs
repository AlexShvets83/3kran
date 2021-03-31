using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable InconsistentNaming

namespace CommonVending.Crypt
{
  public static class Crypto
  {
    private const string Key = "2p@rB.s?";
    private const string IV = "z2M/ymUH";

    private static readonly byte[] RijndaelKey =
    {
      200, 5, 78, 232,
      9, 6, 0, 4,
      200, 5, 78, 232,
      9, 6, 0, 4,
      200, 5, 78, 232,
      9, 6, 0, 4,
      200, 5, 78, 232,
      9, 6, 0, 4
    };

    private static readonly byte[] RijndaelIV =
    {
      0, 1, 2, 3,
      4, 5, 6, 7,
      8, 9, 0, 1,
      2, 3, 4, 5
    };

    //private static byte[] DESKey = { 200, 5, 78, 232, 9, 6, 0, 4 };
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

    //public static string Encrypt(string text) { return EncryptStringToBytes(text, null, null); }
    //public static string Decrypt(string text) { return DecryptStringFromBytes(text, null, null); }

    public static string Encrypt(string value) { return DESEncrypt(value); }

    public static string Decrypt(string value) { return DESDecrypt(value); }

    private static string DESEncrypt(string plainText)
    {
      using (var cryptoProvider = new DESCryptoServiceProvider())
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

    // Protect the connectionStrings section.

    //private static void ProtectConfiguration(string filePath)
    //{
    //  // Get the application configuration file.
    //  Configuration config = //ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    //    ConfigurationManager.OpenExeConfiguration(filePath);

    //  //Configuration config = new Configuration();

    //  // Define the Rsa provider name.
    //  string provider = "RsaProtectedConfigurationProvider";

    //  // Get the section to protect.
    //  ConfigurationSection connStrings = config.ConnectionStrings;

    //  if (connStrings != null)
    //  {
    //    if (!connStrings.SectionInformation.IsProtected)
    //    {
    //      if (!connStrings.ElementInformation.IsLocked)
    //      {
    //        // Protect the section.
    //        connStrings.SectionInformation.ProtectSection(provider);

    //        connStrings.SectionInformation.ForceSave = true;
    //        config.Save(ConfigurationSaveMode.Full);

    //        MessageBox.Show($"Section {connStrings.SectionInformation.Name} is now protected by {connStrings.SectionInformation.ProtectionProvider.Name}");
    //        //Console.WriteLine("Section {0} is now protected by {1}", connStrings.SectionInformation.Name, connStrings.SectionInformation.ProtectionProvider.Name);
    //      }
    //      else
    //      {
    //        MessageBox.Show($"Can't protect, section {connStrings.SectionInformation.Name} is locked");
    //        //Console.WriteLine("Can't protect, section {0} is locked", connStrings.SectionInformation.Name);
    //      }
    //    }
    //    else
    //    {
    //      MessageBox.Show($"Section {connStrings.SectionInformation.Name} is already protected by {connStrings.SectionInformation.ProtectionProvider.Name}");
    //      //Console.WriteLine("Section {0} is already protected by {1}", connStrings.SectionInformation.Name, connStrings.SectionInformation.ProtectionProvider.Name);
    //    }
    //  }
    //  else
    //  {
    //    MessageBox.Show("Can't get the section of connStrings");
    //    //Console.WriteLine("Can't get the section {0}", connStrings.SectionInformation.Name);
    //  }
    //}

    //// Unprotect the connectionStrings section.

    //private static void UnProtectConfiguration(string filePath)
    //{
    //  // Get the application configuration file.
    //  Configuration config = //ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    //    ConfigurationManager.OpenExeConfiguration(filePath);

    //  // Get the section to unprotect.
    //  ConfigurationSection connStrings = config.ConnectionStrings;

    //  if (connStrings != null)
    //  {
    //    if (connStrings.SectionInformation.IsProtected)
    //    {
    //      if (!connStrings.ElementInformation.IsLocked)
    //      {
    //        // Unprotect the section.
    //        connStrings.SectionInformation.UnprotectSection();

    //        connStrings.SectionInformation.ForceSave = true;
    //        config.Save(ConfigurationSaveMode.Full);

    //        Console.WriteLine("Section {0} is now unprotected.", connStrings.SectionInformation.Name);
    //      }
    //      else
    //      {
    //        Console.WriteLine("Can't unprotect, section {0} is locked", connStrings.SectionInformation.Name);
    //      }
    //    }
    //    else
    //    {
    //      Console.WriteLine("Section {0} is already unprotected.", connStrings.SectionInformation.Name);
    //    }
    //  }
    //  else
    //  {
    //    Console.WriteLine(@"Can't get the section of connStrings");
    //  }
    //}

    public static void Protect(string filePath, string strData)
    {
      using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
      {
        using (var cryptic = new DESCryptoServiceProvider {Key = Encoding.ASCII.GetBytes(Key), IV = Encoding.ASCII.GetBytes(IV)})
        {
          using (var crStream = new CryptoStream(stream, cryptic.CreateEncryptor(), CryptoStreamMode.Write))
          {
            var data = Encoding.ASCII.GetBytes(strData);
            crStream.Write(data, 0, data.Length);
          }
        }
      }
    }

    public static void Protect(FileStream stream, string strData)
    {
      using (var cryptic = new DESCryptoServiceProvider {Key = Encoding.ASCII.GetBytes(Key), IV = Encoding.ASCII.GetBytes(IV)})
      {
        using (var crStream = new CryptoStream(stream, cryptic.CreateEncryptor(), CryptoStreamMode.Write))
        {
          var data = Encoding.ASCII.GetBytes(strData);
          crStream.Write(data, 0, data.Length);
        }
      }
    }

    public static string UnProtect(string filePath)
    {
      string data;
      using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        var cryptic = new DESCryptoServiceProvider {Key = Encoding.ASCII.GetBytes(Key), IV = Encoding.ASCII.GetBytes(IV)};
        using (var crStream = new CryptoStream(stream, cryptic.CreateDecryptor(), CryptoStreamMode.Read))
        {
          using (var reader = new StreamReader(crStream)) { data = reader.ReadToEnd(); }
        }
      }

      return data;
    }

    public static string GetMd5ByString(string input)
    {
      if (string.IsNullOrEmpty(input)) return null;

      return GetMd5(Encoding.UTF8.GetBytes(input));
    }

    public static string GetMd5(byte[] input)
    {
      using (var md5Hash = MD5.Create())
      {
        var data = md5Hash.ComputeHash(input);
        var sBuilder = new StringBuilder();
        foreach (var t in data) { sBuilder.Append(t.ToString("x2")); }

        return sBuilder.ToString();
      }
    }

    private static string EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
      // Check arguments.
      if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));

      if ((Key == null) || (Key.Length <= 0)) Key = RijndaelKey;

      //Key = Encoding.UTF8.GetBytes("`nMt2GL{MmR,t7))");
      if ((IV == null) || (IV.Length <= 0)) IV = RijndaelIV;

      //IV = Encoding.UTF8.GetBytes(",s5sax-mD9'9.7N+");
      string encrypted;

      // Create an RijndaelManaged object
      // with the specified key and IV.
      using (var rijAlg = new RijndaelManaged())
      {
        rijAlg.Key = Key;
        rijAlg.IV = IV;

        // Create a decrytor to perform the stream transform.
        var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

        // Create the streams used for encryption.
        using (var msEncrypt = new MemoryStream())
        {
          using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
              //Write all data to the stream.
              swEncrypt.Write(plainText);
            }

            encrypted = Encoding.UTF8.GetString(msEncrypt.ToArray());
          }
        }
      }

      // Return the encrypted bytes from the memory stream.
      return encrypted;
    }

    private static string DecryptStringFromBytes(string cipherText, byte[] Key, byte[] IV)
    {
      // Check arguments.
      if ((cipherText == null) || (cipherText.Length <= 0)) throw new ArgumentNullException(nameof(cipherText));

      if ((Key == null) || (Key.Length <= 0)) Key = RijndaelKey;

      //Key = Encoding.UTF8.GetBytes("`nMt2GL{MmR,t7))");
      if ((IV == null) || (IV.Length <= 0)) IV = RijndaelIV;

      //IV = Encoding.UTF8.GetBytes(",s5sax-mD9'9.7N+");

      // Declare the string used to hold
      // the decrypted text.
      string plaintext = null;

      // Create an RijndaelManaged object
      // with the specified key and IV.
      using (var rijAlg = new RijndaelManaged())
      {
        rijAlg.Key = Key;
        rijAlg.IV = IV;

        // Create a decrytor to perform the stream transform.
        var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
        var byteArray = Encoding.UTF8.GetBytes(cipherText);

        // Create the streams used for decryption.
        using (var msDecrypt = new MemoryStream(byteArray))
        {
          using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
              // Read the decrypted bytes from the decrypting stream
              // and place them in a string.
              plaintext = srDecrypt.ReadToEnd();
            }
          }
        }
      }

      return plaintext;
    }

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
      // Check arguments.
      if ((cipherText == null) || (cipherText.Length <= 0)) throw new ArgumentNullException("cipherText");
      if ((Key == null) || (Key.Length <= 0)) throw new ArgumentNullException("Key");
      if ((IV == null) || (IV.Length <= 0)) throw new ArgumentNullException("IV");

      // Declare the string used to hold
      // the decrypted text.
      string plaintext = null;

      // Create an RijndaelManaged object
      // with the specified key and IV.
      using (var rijAlg = new RijndaelManaged())
      {
        rijAlg.Key = Key;
        rijAlg.IV = IV;

        // Create a decrytor to perform the stream transform.
        var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        // Create the streams used for decryption.
        using (var msDecrypt = new MemoryStream(cipherText))
        {
          using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
              // Read the decrypted bytes from the decrypting stream
              // and place them in a string.
              plaintext = srDecrypt.ReadToEnd();
            }
          }
        }
      }

      return plaintext;
    }
  }
}
