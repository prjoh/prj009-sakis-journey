using UnityEngine;


public enum eEnemyState
{
  Walking,
  Dead,
}

public class EnemyBehaviour : MonoBehaviour
{
  [SerializeField] private GraphicsController graphicsController;
  [SerializeField] private EnemyController enemyController;

  [SerializeField] private float despawnTime = 10.0f;

  private eEnemyState m_state = eEnemyState.Walking;
  private bool m_isAlive = true;

  public delegate void OnDied(EnemyBehaviour enemyBehaviour);
  public event OnDied DiedEvent;

  private void Awake()
  {
    if (!graphicsController)
    {
      graphicsController = GetComponentInChildren<GraphicsController>();
    }
    Debug.Assert(graphicsController, "[EnemyBehaviour] No GraphicsController reference found! Please add a reference in the editor.");

    if (!enemyController)
    {
      enemyController = GetComponentInChildren<EnemyController>();
    }
    Debug.Assert(enemyController, "[EnemyBehaviour] No EnemyController reference found! Please add a reference in the editor.");
  }

  private void FixedUpdate()
  {
    UpdateEnemyState();

    switch (m_state)
    {
      case eEnemyState.Walking:
      {
        FixedUpdate_Walking();
        break;
      }
      case eEnemyState.Dead:
        FixedUpdate_Dead();
        break;
    }
  }

  private void Update()
  {
    if (!m_isAlive)
    {
      despawnTime -= Time.deltaTime;

      if (despawnTime <= 0.0f)
      {
        Destroy(gameObject);
      }
    }
  }

  private void UpdateEnemyState()
  {
    if (!m_isAlive)
    {
      m_state = eEnemyState.Dead;
    }
  }

  private void FixedUpdate_Walking()
  {
    if (enemyController.Direction.x > 0.0f)
    {
      graphicsController.SetSpriteDirection(eSpriteDirection.Right);
    }
    else if (enemyController.Direction.x < 0.0f)
    {
      graphicsController.SetSpriteDirection(eSpriteDirection.Left);
    }

    graphicsController.SetAnimationParameterBool("IsIdle",    !enemyController.IsMoving);
    graphicsController.SetAnimationParameterBool("IsWalking", enemyController.IsMoving);
    graphicsController.SetAnimationParameterBool("IsDead",    false);
  }

  // TODO: If we make a StateMachine, we can have thigns like OnEnter / OnExit!!
  private void FixedUpdate_Dead()
  {
    graphicsController.SetAnimationParameterBool("IsIdle",    false);
    graphicsController.SetAnimationParameterBool("IsWalking", false);
    graphicsController.SetAnimationParameterBool("IsDead",    true);

    enemyController.SetEnabled(false);
  }

  public void Kill()
  {
    m_isAlive = false;

    var collider = GetComponent<BoxCollider2D>();
    collider.enabled = false;

    DiedEvent?.Invoke(this);
  }
}
