using System;


public interface IPersistentDataBinding<TData> where TData : IPersistentData
{
  SerializableGuid Id { get; set; }

  void SaveData(ref TData data);
  void LoadData(TData data);
}
