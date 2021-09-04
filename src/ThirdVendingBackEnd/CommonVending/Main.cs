using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CommonVending
{
  //todo CLEAN UP
  public static class Main
  {
    //public static void ClearTableFromDuplicates(DataTable table, string columnName)
    //{
    //  var hTable = new Hashtable();
    //  var duplicateList = new ArrayList();

    //  foreach (DataRow row in table.Rows)
    //  {
    //    if (hTable.Contains(row[columnName])) duplicateList.Add(row);
    //    else hTable.Add(row[columnName], string.Empty);
    //  }

    //  foreach (DataRow dRow in duplicateList)
    //  {
    //    table.Rows.Remove(dRow);
    //  }
    //}

    //public static void ClearColumnTable(DataTable table, List<string> errorsCode = null, bool isUp = true, bool isDelDur = true)
    //{
    //  if ((table == null) || (table.Rows.Count == 0)) return;

    //  var indexList = new List<DataColumn>();
    //  foreach (DataColumn column in table.Columns)
    //  {
    //    var isNull = true;
    //    if ((errorsCode != null) && errorsCode.Contains(column.ColumnName))
    //    {
    //      foreach (DataRow row in table.Rows)
    //      {
    //        if (!string.IsNullOrEmpty(row[column.ColumnName].ToString()))
    //        {
    //          if (int.TryParse(row[column.ColumnName].ToString(), out var code))
    //          {
    //            if (code != 0)
    //            {
    //              isNull = false;
    //              break;
    //            }
    //          }
    //          else
    //          {
    //            isNull = false;
    //            break;
    //          }
    //        }
    //      }
    //    }
    //    else
    //    {
    //      foreach (DataRow row in table.Rows)
    //      {
    //        if (!string.IsNullOrEmpty(row[column.ColumnName].ToString()))
    //        {
    //          isNull = false;
    //          break;
    //        }
    //      }
    //    }

    //    if (isNull) indexList.Add(column);
    //  }

    //  foreach (var column in indexList)
    //  {
    //    if (table.Columns.Contains(column.ColumnName)) table.Columns.Remove(column);

    //    if (isDelDur)
    //    {
    //      if ((errorsCode != null) && errorsCode.Contains(column.ColumnName))
    //      {
    //        if (int.TryParse(column.ColumnName, out var errDur))
    //        {
    //          var errorDurId = isUp ? errDur + 1 : errDur - 1;
    //          if (table.Columns.Contains(errorDurId.ToString()))
    //          {
    //            table.Columns.Remove(errorDurId.ToString());
    //          }
    //        }
    //      }
    //    }
    //  }
    //}

    //public static string EjectExceptionMessage(Exception exception) { return ProcessExceptionMessage(exception); }

    private static string ProcessExceptionMessage(Exception exception)
    {
      return exception == null ? null : $"{exception.Message}\n{ProcessExceptionMessage(exception.InnerException)}";
    }

    public static bool CheckIsSummerTime(DateTime dateT)
    {
      var timeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
      if (!timeZone.IsDaylightSavingTime(dateT)) // для того чтобы, при переводе часов на летнее время, начало было с 3-го часа, а не 4-го 
        return timeZone.IsDaylightSavingTime(dateT.AddHours(1));

      return true;
    }
       
    public static IList<T> ToListByPropertyName<T>(this DataTable table) where T : new()
    {
      IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
      IList<T> result = new List<T>();

      foreach (var row in table.Rows)
      {
        var item = CreateItemFromRow<T>((DataRow) row, properties);
        result.Add(item);
      }

      return result;
    }

    public static IList<T> ToListDoubleByPropertyName<T>(this DataTable table) where T : new()
    {
      IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
      IList<T> result = new List<T>();

      foreach (var row in table.Rows)
      {
        var item = CreateItemDoubleFromRow<T>((DataRow)row, properties);
        result.Add(item);
      }

      return result;
    }

    private static T CreateItemDoubleFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
    {
      T item = new T();
      foreach (var property in properties)
      {
        if (row.Table.Columns.Contains(property.Name) && (row[property.Name] != DBNull.Value))
        {
          double dbVar;
          if (row.Table.Columns[property.Name].DataType == typeof(DateTime))
          {
            var date = Convert.ToDateTime(row[property.Name]);
            dbVar = date.ToOADate();
          }
          else
          {
            dbVar = Convert.ToDouble(row[property.Name]);
          }

          property.SetValue(item, dbVar, null);
        }
      }

      return item;
    }

    private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
    {
      T item = new T();
      foreach (var property in properties)
      {
        {
          if (row.Table.Columns.Contains(property.Name) && (row[property.Name] != DBNull.Value))
          {
            property.SetValue(item, row[property.Name], null);
            //var vl = row[property.Name];
            //var pType = property.PropertyType;
            //var rType = row[property.Name].GetType();
            //if (rType != typeof(DateTime) && rType != pType)
            //{
            //  var value = Convert.ChangeType(vl, pType);
            //  property.SetValue(item, value, null);
            //}
            //else property.SetValue(item, vl, null);
          }
        }
      }

      return item;
    }

    public static List<string> GetAllLocalIPAddress()
    {
      var hostName = Dns.GetHostName();
      var host = Dns.GetHostEntry(hostName);

      return (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).ToList();
    }

    public static bool ParseJson(JToken token, Dictionary<string, string> nodes, string parentLocation = "")
    {
      if (token.HasValues)
      {
        foreach (JToken child in token.Children())
        {
          if (token.Type == JTokenType.Property) parentLocation = ((JProperty)token).Name.ToLower().Trim();

          ParseJson(child, nodes, parentLocation);
        }

        // we are done parsing and this is a parent node
        return true;
      }
      else
      {
        // leaf of the tree
        if (nodes.ContainsKey(parentLocation))
        {
          // this was an array
          nodes[parentLocation] += "|" + token;
        }
        else
        {
          // this was a single property
          nodes.Add(parentLocation, token.ToString());
        }

        return false;
      }
    }

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

    /// <summary>
    ///   Конвертирует байт в ASCII строку
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    [Description("Конвертирует байт в ASCII строку")]
    public static string ToASCIIString(this byte item) { return ToASCIIString(new[] { item }); }

    /// <summary>
    ///   Конвертирует список байт в ASCII строку
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ToASCIIString(this List<byte> list) { return ToASCIIString(list.ToArray()); }

    /// <summary>
    ///   Конвертирует массив байт в ASCII строку
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static string ToASCIIString(this byte[] array) { return Encoding.ASCII.GetString(array); }

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
      using (var md5Hash = MD5.Create())
      {
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
    }
    //public static IList<T> ToList<T>(this DataTable table) where T : new()
    //{
    //  IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
    //  IList<T> result = new List<T>();

    //  foreach (var row in table.Rows)
    //  {
    //    var item = CreateItemDoubleFromRow<T>((DataRow) row, properties);
    //    result.Add(item);
    //  }

    //  return result;
    //}

    #endregion
  }
}