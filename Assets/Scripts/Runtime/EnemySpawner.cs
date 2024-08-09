using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private GameObject enemyPrefab;

  [Header("Settings")]
  [SerializeField] private uint maxEnemies = 1;
  [SerializeField] private float spawnTime = 2.0f;

  uint m_enemiesAlive = 0;
  float m_spawnTime = 0.0f;

  private void Awake()
  {
    Debug.Assert(enemyPrefab, "[EnemySpawner] No enemy prefab reference found! Please set it in the editor.");
  }

  private void Start()
  {
    m_spawnTime = spawnTime;
  }

  private void Update()
  {
    if (m_enemiesAlive < maxEnemies)
    {
      m_spawnTime -= Time.deltaTime;

      if (m_spawnTime <= 0.0f)
      {
        var enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        var enemyBehavior = enemy.GetComponent<EnemyBehaviour>();
        Debug.Assert(enemyBehavior, "[EnemySpawner] Unable to retrieve EnemyBehaviour reference!");
        enemyBehavior.DiedEvent += OnEnemyDied;

        m_enemiesAlive++;
        m_spawnTime = spawnTime;
      }
    }
  }

  private void OnEnemyDied(EnemyBehaviour enemyBehaviour)
  {
    enemyBehaviour.DiedEvent -= OnEnemyDied;
    m_enemiesAlive--;
  }
}
