using System;
using UnityEngine;


public class SaveablePlayerData : IPersistentData
{
  public SerializableGuid Id { get; set; }

  // public Vector3 position;
  public int maxHealth;
  public int currentHealth;

  public SaveablePlayerData()
  {
    // position = Vector3.zero;
    maxHealth = 0;
    currentHealth = 0;    
  }
}

public class GameData
{
  public SaveablePlayerData playerData = null;

  public GameData()
  {
    playerData = new();
  }
}
