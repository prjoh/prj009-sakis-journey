

public interface IDataService
{
  bool SaveData<TData>(string dataFileName, TData data, bool isEncrypted);
  bool LoadData<TData>(string dataFileName, bool isEncrypted, out TData dataOut);
  bool DeleteData(string dataFileName);
}
