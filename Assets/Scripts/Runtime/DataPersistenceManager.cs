using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DataPersistenceManager : SingletonBehaviour<DataPersistenceManager>
{
  private JSONDataService m_dataService = null;
  private GameData m_gameData = null;
  // private List<IPersistentDataBinding<SomeData>> m_someDataObjects = null;

  [SerializeField] string debugDataPath;

  protected override void Awake()
  {
    base.Awake();

    m_dataService = new JSONDataService(Application.persistentDataPath);

    debugDataPath = Application.persistentDataPath;
  }

  // private void Start()
  // {
  //   m_someDataObjects = FindAllPersistantDataBindingObjects<SomeData>();
  // }

  public void CreateNewGame()
  {
    m_gameData = new();
  }

  public void LoadGame()
  {
    // Load GameData from file
    m_dataService.LoadData("TestGameData.json", false, out m_gameData);

    if (m_gameData == null)
    {
      Debug.LogError("[DataPersistenceManager] No data found. Initializing data to default.");
      CreateNewGame();
    }

    // Pass loaded data to scene scripts
    LoadData<PlayerData, SaveablePlayerData>(m_gameData.playerData);
  }

  public void SaveGame()
  {
    if (m_gameData == null) return;

    // Get saveable data from scene scripts
    SaveData<PlayerData, SaveablePlayerData>(ref m_gameData.playerData);

    // Save GameData to file
    m_dataService.SaveData("TestGameData.json", m_gameData, false);
  }

  // TODO: Do this from somewhere else??
  // public void OnApplicationQuit()
  // {
  //   SaveGame();
  // }

  // TODO: SHITTY!!! Should we just bind the data???
  private void SaveData<T, TData>(ref TData dataObject) where T : MonoBehaviour, IPersistentDataBinding<TData> where TData : IPersistentData
  {
    var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
    if (entity == null)
    {
      Debug.LogError("[DataPersistenceManager] Unable to locate object of type " + typeof(T) + ".");
      return;
    }

    entity.SaveData(ref dataObject);
  }

  private void SaveData<T, TData>(ref List<TData> dataObjects) where T : MonoBehaviour, IPersistentDataBinding<TData> where TData : IPersistentData, new()
  {
    var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

    foreach (var entity in entities)
    {
      dataObjects.Add(new());
      var dataObject = dataObjects.Last();
      entity.SaveData(ref dataObject);
    }
  }

  private void LoadData<T, TData>(TData dataObject) where T : MonoBehaviour, IPersistentDataBinding<TData> where TData : IPersistentData
  {
    var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault(e => e.Id == dataObject.Id);
    if (entity == null)
    {
      Debug.LogError("[DataPersistenceManager] Unable to locate object of type " + typeof(T) + ".");
      return;
    }

    entity.LoadData(dataObject);
  }

  private void LoadData<T, TData>(List<TData> dataObjects) where T : MonoBehaviour, IPersistentDataBinding<TData> where TData : IPersistentData
  {
    var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

    foreach (var entity in entities)
    {
      var data = dataObjects.FirstOrDefault(d => d.Id == entity.Id);
      if (data == null)
      {
        Debug.LogError("[DataPersistenceManager] Unable to load data " + typeof(TData) + " for type " + typeof(T) + ".");
        continue;
      }
      entity.LoadData(data);
    }
  }
}