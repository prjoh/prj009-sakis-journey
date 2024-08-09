using System;
using UnityEngine;


// TODO: Why is this a singleton / global??? Can we just add it to player???
public class PlayerData : SingletonBehaviour<PlayerData>, IPersistentDataBinding<SaveablePlayerData>
{
  [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
  [SerializeField] private int maxHealth = 6;

  private Transform m_playerTransform = null;

  private Health m_health;
  public Health Health
  {
    get { return m_health; }
  }

  protected override void Awake()
  {
    base.Awake();

    m_health = new(maxHealth);
  }

  // TODO: This is shitty...
  public void BindPlayerTransform(Transform playerTransform)
  {
    m_playerTransform = playerTransform;
  }

  public void LoadData(SaveablePlayerData data)
  {
    m_health.SetMaxHealth(maxHealth);
    m_health.Decrease(data.maxHealth - data.currentHealth);
    // m_playerTransform.position = data.position;
  }

  public void SaveData(ref SaveablePlayerData data)
  {
    // data.position = m_playerTransform.position;
    data.maxHealth = Health.MaxHealth;
    data.currentHealth = Health.Value;
    data.Id = Id;
  }
}
