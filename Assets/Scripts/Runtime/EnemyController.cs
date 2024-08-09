using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private UnityLayer enemyLayer;
    [SerializeField] private UnityTag enemyTag;
    [SerializeField] private Vector2 colliderSize = new(1.5f, 0.9f);
    [SerializeField] private Vector2 colliderOffset = new(0.0f, -0.05f);
    [SerializeField] private float speed = 2.5f;

    private Rigidbody2D m_rigidbody = null;
    private BoxCollider2D m_collider = null;

    private Vector2 m_direction = Vector2.right;

    public Vector2 Direction
    {
      get { return m_direction; }
    }
    public bool IsMoving
    {
      get { return !MathExtension.Approximately(m_rigidbody.velocity.x, 0.0f, 0.01f); }
    }

    private void Awake()
    {
      m_rigidbody = gameObject.AddComponent<Rigidbody2D>();
      m_collider = gameObject.AddComponent<BoxCollider2D>();

      gameObject.layer = enemyLayer.LayerIndex;
      gameObject.tag = enemyTag.TagName;
    }

    private void Start()
    {
      m_collider.size = colliderSize;
      m_collider.offset = colliderOffset;
      m_collider.isTrigger = true;

      m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
      m_rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
      m_rigidbody.freezeRotation = true;
    }

    private void FixedUpdate()
    {
      m_rigidbody.velocity = speed * m_direction;
    }

    public void SetEnabled(bool isEnabled)
    {
      if (!isEnabled)
      {
        m_rigidbody.velocity = Vector2.zero;
      }
      this.enabled = isEnabled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.CompareTag("Player"))
      {
        var playerPosition = other.gameObject.transform.position;
        var position = transform.position;

        // Only change direction if player collision was frontal
        if (Vector2.Dot(position - playerPosition, m_direction) > 0.0f)
        {
          return;
        }
      }

      m_direction *= -1.0f;
    }
}
