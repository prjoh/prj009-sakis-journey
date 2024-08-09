using System;


public class Health
{
  public Health(int maxHealth)
  {
    m_maxHealth = maxHealth;
    m_value = m_maxHealth;
  }

  public void SetMaxHealth(int maxHealth)
  {
    m_maxHealth = maxHealth;
  }

  public void Increase(int value)
  {
    m_value = Math.Min(m_value + value, m_maxHealth);
  }

  public void Decrease(int value)
  {
    m_value = Math.Max(m_value - value, 0);
  }

  public bool IsAlive()
  {
    return m_value > 0;
  }

  private int m_maxHealth;
  public int MaxHealth
  {
    get { return m_maxHealth; }
  }
  private int m_value;
  public int Value
  {
    get { return m_value; }
  }
}
