using UnityEngine;


public class SingletonBehaviour<T> : MonoBehaviour where T : Component
{
  private static T m_instance;
  public static T Instance
  {
      get
      {
        if (m_instance == null)
        {
            m_instance = FindObjectOfType<T>();
            if (m_instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                m_instance = obj.AddComponent<T>();
            }
        }
        return m_instance;
      }
  }

  protected virtual void Awake()
  {
    Initialize();
  }

  protected virtual void Initialize()
  {
    if (!Application.isPlaying) return;

    if (m_instance == null)
    {
      m_instance = this as T;
    }
    else
    {
      if (m_instance != this)
      {
        Destroy(gameObject);
      }
    }
  }
}
