using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CommonVending
{
  public static class Main
  {
    public static DateTime ConvertFromUnixTimestamp(double timestamp)
    {
      var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      return origin.AddSeconds(timestamp);
    }
    
    public static double ConvertToUnixTimestamp(DateTime date)
    {
      var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      var diff = date - origin;
      return Math.Floor(diff.TotalSeconds);
    }

    #region Ext methods

    public static void CopyObjectProperties(this object target, object source)
    {
      var sourceProperty = source.GetType().GetProperties();
      var targetProperties = target.GetType().GetProperties();

      foreach (var property in sourceProperty)
      {
        if (!property.CanRead) continue;

        foreach (var targetProperty in targetProperties)
        {
          if (!string.Equals(targetProperty.Name, property.Name, StringComparison.CurrentCultureIgnoreCase)) continue;
          if (!targetProperty.CanWrite) continue;
          if ((targetProperty.GetSetMethod(true) != null) && targetProperty.GetSetMethod(true).IsPrivate) continue;
          if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0) continue;
          if (!targetProperty.PropertyType.IsAssignableFrom(property.PropertyType)) continue;

          targetProperty.SetValue(target, property.GetValue(source, null), null);
        }

        //var targetProperty = target.GetType().GetProperty(property.Name);
        //if (targetProperty == null) continue;
        //if (!targetProperty.CanWrite) continue;
        //if ((targetProperty.GetSetMethod(true) != null) && targetProperty.GetSetMethod(true).IsPrivate) continue;
        //if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0) continue;
        //if (!targetProperty.PropertyType.IsAssignableFrom(property.PropertyType)) continue;

        //targetProperty.SetValue(target, property.GetValue(source, null), null);
      }
    }

    public static T GetNewObj<T>(this object sc) where T : new()
    {
      var newObj = new T();
      newObj.CopyObjectProperties(sc);
      return newObj;
    }

    public static string GetMd5ByString(string input)
    {
      if (string.IsNullOrEmpty(input)) return null;

      using var md5Hash = MD5.Create();

      // Convert the input string to a byte array and compute the hash. 
      var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

      // Create a new StringBuilder to collect the bytes 
      // and create a string.
      var sBuilder = new StringBuilder();

      // Loop through each byte of the hashed data  
      // and format each one as a hexadecimal string. 
      foreach (var t in data) { sBuilder.Append(t.ToString("x2")); }

      // Return the hexadecimal string. 
      return sBuilder.ToString();
    }

    #endregion
  }
}