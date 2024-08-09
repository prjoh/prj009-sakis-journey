using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


public class JSONDataService : IDataService
{
  private readonly string m_dataDirPath;

  public JSONDataService(string dataDirPath)
  {
    m_dataDirPath = dataDirPath;
  }

  public bool SaveData<TData>(string dataFileName, TData data, bool isEncrypted)
  {
    string fullPath = Path.Combine(m_dataDirPath, dataFileName);

    try
    {
      using StreamWriter sw = new(File.Create(fullPath));
      var serializedData = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
      {
          ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      });
      sw.Write(serializedData);

      return true;
    }
    catch (Exception e)
    {
      Debug.LogError("[JSONDataService] Error occured when trying to save data to file:\n" + fullPath + "\n" + e);
      return false;
    }
  }

  public bool LoadData<TData>(string dataFileName, bool isEncrypted, out TData dataOut)
  {
    string fullPath = Path.Combine(m_dataDirPath, dataFileName);

    try
    {
      using StreamReader sr = new(File.OpenRead(fullPath));
      dataOut = JsonConvert.DeserializeObject<TData>(sr.ReadToEnd());
      
      return true;
    }
    catch (Exception e)
    {
      Debug.LogError("[JSONDataService] Error occured when trying to load data from file:\n" + fullPath + "\n" + e);
      dataOut = default;
      return false;
    }
  }

  public bool DeleteData(string dataFileName)
  {
    string fullPath = Path.Combine(m_dataDirPath, dataFileName);

    try
    {
      File.Delete(fullPath);
      return true;
    }
    catch (Exception e)
    {
      Debug.LogError("[JSONDataService] Error occured when trying to delete file:\n" + fullPath + "\n" + e);
      return false;
    }
  }
}