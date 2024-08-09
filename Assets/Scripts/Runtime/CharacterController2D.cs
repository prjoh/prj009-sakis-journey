using Unity.VisualScripting;
using UnityEngine;


public class CharacterController2D : MonoBehaviour
{
  const float c_BoxCollider2DBuffer = 0.015f;
  const string c_HorizontalAxisName = "Horizontal";
  const string c_JumpAxisName = "Jump";

  [Header("References")]
  [SerializeField] private GameObject playerRoot;
  [SerializeField] private PhysicsMaterial2D colliderMaterial;


  [Header("Settings")]
  [SerializeField] private Vector2 playerSize = new(0.9f, 1.65f);
  [SerializeField] private LayerMask groundLayer;

  [SerializeField] private float speed = 60.0f;
  [SerializeField] private float groundFriction = 35.0f;
  [SerializeField] private float airFriction = 20.0f;
  [SerializeField] private float maxRunVelocity = 7.0f;

  [SerializeField] private Vector2 distanceToJumpPeak = new(3.5f, 4.5f);
  [SerializeField] private float gravityEarlyReleaseMultiplier = 1.3f;
  [SerializeField] private float gravityFallMultiplier = 1.2f;
  [SerializeField] private Vector2 maxGravity = new(0.0f, -450.0f);


  [Header("Debug Info")]
  [SerializeField] private string debugStateGrounded;
  [SerializeField] private float debugInputHorizontal;
  [SerializeField] private Vector2 debugGravity;
  [SerializeField] private Vector2 debugVelocity;


  private Rigidbody2D m_rigidbody = null;
  private BoxCollider2D m_collider = null;

  private bool m_isControlEnabled = true;

  private float m_inputHorizontal = 0.0f;
  private bool m_inputJump = false;
  private bool m_inputJumpPressed = false;
  private bool m_isJumping = false;

  private bool m_isGrounded = false;
  private bool m_horizontalJump = false;
  private bool m_isKillKnockback = false;
  private bool m_isHitKnockback = false;

  public bool IsGrounded
  {
    get { return m_isGrounded; }
  }
  public bool IsMoving
  {
    get { return !MathExtension.Approximately(m_rigidbody.velocity.x, 0.0f, 0.01f); }
  }
  public bool IsJumping
  {
    get { return m_isJumping; }
  }

  private void Awake()
  {
    Debug.Assert(playerRoot, "[CharacterController2D] No player root GameObject set! Please add a reference in the editor.");
    Debug.Assert(colliderMaterial, "[CharacterController2D] No PhysicsMaterial2D detected! Please add a reference in the editor.");

    m_rigidbody = playerRoot.AddComponent<Rigidbody2D>();
    m_collider = playerRoot.AddComponent<BoxCollider2D>();
  }

  private void Start()
  {
    m_collider.sharedMaterial = colliderMaterial;
    m_collider.size = playerSize;

    m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
    m_rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    m_rigidbody.freezeRotation = true;
  }

  private void FixedUpdate()
  {
    var currentPosition = m_rigidbody.position;
    var currentVelocity = m_rigidbody.velocity;

    var velocity = currentVelocity;

    var isGrounded = GetIsGrounded();
    if (m_isGrounded && !isGrounded && !m_inputJump)
    {
      // TODO: Set m_isGrounded to false in X frames
    }

    m_isGrounded = isGrounded;

    // Movement in air
    if (!m_isGrounded)
    {
      if (m_isHitKnockback) return;

      debugStateGrounded = "In Air";

      UpdateHorizontalVelocity(ref velocity, airFriction);
      velocity.y = currentVelocity.y;

      // Update gravity if we start falling
      var isFalling = velocity.y < 0.0f;
      var isJumpEarlyRelease = (!m_inputJump && !isFalling && !m_isKillKnockback);
      var isHorizontalMovementRelease = m_horizontalJump && MathExtension.Approximately(m_inputHorizontal, 0.0f, 0.01f);
      
      if (isJumpEarlyRelease || isHorizontalMovementRelease)
      {
        Physics2D.gravity *= gravityEarlyReleaseMultiplier;
      }
      else if (isFalling)
      {
        Physics2D.gravity *= gravityFallMultiplier;
      }

      m_isJumping = !isFalling && !m_isKillKnockback;

      Physics2D.gravity = Vector2.Max(Physics2D.gravity, maxGravity);
      // Use minimum jump peak if player releases jump button early
      // else if (!m_inputJump)
      // {
      //   Physics2D.gravity = new Vector2(0.0f, (-2.0f * distanceToMinJumpPeak.y * (maxRunVelocity * maxRunVelocity)) / (distanceToMinJumpPeak.x * distanceToMinJumpPeak.x));
      // }
    }

    // Movement on ground
    else if (m_isGrounded)
    {
      debugStateGrounded = "Grounded";

      m_horizontalJump = false;
      m_isKillKnockback = false;
      m_isHitKnockback = false;

      // Update velocity
      UpdateHorizontalVelocity(ref velocity, groundFriction);

      // Check for jump
      if (m_inputJumpPressed)
      {
        // Consume jump pressed event
        m_inputJumpPressed = false;
        m_isJumping = true;

        m_isGrounded = false;

        m_horizontalJump = !MathExtension.Approximately(m_inputHorizontal, 0.0f, 0.01f);

        // TODO: Calculate during Awake
        var initialJumpVelocity = (2.0f * distanceToJumpPeak.y * maxRunVelocity) / distanceToJumpPeak.x;
        Physics2D.gravity = new Vector2(0.0f, (-2.0f * distanceToJumpPeak.y * (maxRunVelocity * maxRunVelocity)) / (distanceToJumpPeak.x * distanceToJumpPeak.x));
        velocity.y = initialJumpVelocity + Physics2D.gravity.y * Time.fixedDeltaTime;
      }
      else
      {
        velocity.y = 0.0f;
      }
    }

    velocity.x = Mathf.Clamp(velocity.x, -maxRunVelocity, maxRunVelocity);  // TODO: Move

    m_rigidbody.velocity = velocity;
    debugVelocity = m_rigidbody.velocity;
    debugGravity = Physics2D.gravity;
  }

  private void Update()
  {
    if (!m_isControlEnabled)
    {
      return;
    }

    UpdateInputState();
  }

  private void UpdateInputState()
  {
    m_inputHorizontal = Input.GetAxis(c_HorizontalAxisName);
    m_inputJump = Input.GetAxis(c_JumpAxisName) != 0.0f;

    if (Input.GetButtonDown(c_JumpAxisName) && m_isGrounded) // TODO
    {
      m_inputJumpPressed = true;
    }

    debugInputHorizontal = m_inputHorizontal;
  }

  private void ResetInputState()
  {
    m_inputHorizontal = 0.0f;
    m_inputJump = false;
    m_inputJumpPressed = false;
  }

  private void UpdateHorizontalVelocity(ref Vector2 velocity, float friction)
  {
    if (MathExtension.Approximately(m_inputHorizontal, 0.0f, 0.01f))
    {
      var dFriction = Time.fixedDeltaTime * friction;
      if (dFriction < Mathf.Abs(velocity.x))
      {
        velocity.x -= dFriction * Mathf.Sign(velocity.x);
      }
      else
      {
        velocity.x = 0.0f;
      }
    }
    else
    {
      velocity.x += Time.fixedDeltaTime * m_inputHorizontal * speed;
    }
  }

  private bool GetIsGrounded()
  {
    var currentPosition = m_rigidbody.position;
    var origin = currentPosition + 0.5f * m_collider.size.y * Vector2.down;

    if (!Physics2D.BoxCast(origin, m_collider.size, 0.0f, Vector2.down, 0.1f, groundLayer))
    {
      return false;
    }

    var playerHalfSizeX = new Vector2(playerSize.x * 0.5f, 0.0f);
    var rcHits = new RaycastHit2D[3]
    {
      Physics2D.Raycast(origin - playerHalfSizeX, Vector2.down, 0.1f, groundLayer),
      Physics2D.Raycast(origin + playerHalfSizeX, Vector2.down, 0.1f, groundLayer),
      Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer),
    };
    foreach (var hit in rcHits)
    {
      float distance = Mathf.Abs(hit.point.y - origin.y) - c_BoxCollider2DBuffer;
      if (MathExtension.Approximately(distance, 0.0f, 0.01f))
      {
        return true;
      }
    }

    return false;
  }

  public void SetEnabled(bool isEnabled)
  {
    if (!isEnabled)
    {
      m_rigidbody.velocity = Vector2.zero;

      ResetInputState();
    }

    m_isControlEnabled = isEnabled;
  }

  public void SetKillKnockback()
  {
    var velocity = m_rigidbody.velocity;
    var initialJumpVelocity = (2.0f * distanceToJumpPeak.y * maxRunVelocity) / distanceToJumpPeak.x * 0.5f;
    Physics2D.gravity = new Vector2(0.0f, (-2.0f * distanceToJumpPeak.y * (maxRunVelocity * maxRunVelocity)) / (distanceToJumpPeak.x * distanceToJumpPeak.x));
    velocity.y = initialJumpVelocity + Physics2D.gravity.y * Time.fixedDeltaTime;
    m_rigidbody.velocity = velocity;
    m_isGrounded = false;
    m_isKillKnockback = true;
  }

  public void SetHitKnockback(float direction)
  {
    // var velocity = m_rigidbody.velocity;
    // var initialJumpVelocity = (2.0f * distanceToJumpPeak.y * maxRunVelocity) / distanceToJumpPeak.x * 100.0f;
    // Physics2D.gravity = new Vector2(0.0f, (-2.0f * distanceToJumpPeak.y * (maxRunVelocity * maxRunVelocity)) / (distanceToJumpPeak.x * distanceToJumpPeak.x));
    // // velocity.x = direction * 100.0f * Time.fixedDeltaTime;
    // velocity.y = initialJumpVelocity + Physics2D.gravity.y * Time.fixedDeltaTime;
    // m_rigidbody.velocity = velocity;
    var pos = m_rigidbody.position; 
    pos.y += 0.15f;
    m_rigidbody.position = pos;
    // m_rigidbody.velocity = Vector2.zero;

    var velocity = m_rigidbody.velocity;
    var initialJumpVelocity = (2.0f * distanceToJumpPeak.y * maxRunVelocity) / distanceToJumpPeak.x * 0.35f;
    Physics2D.gravity = new Vector2(0.0f, (-2.0f * distanceToJumpPeak.y * (maxRunVelocity * maxRunVelocity)) / (distanceToJumpPeak.x * distanceToJumpPeak.x));
    velocity.x = direction * 250.0f * Time.fixedDeltaTime;
    velocity.y = initialJumpVelocity + Physics2D.gravity.y * Time.fixedDeltaTime;
    m_rigidbody.velocity = velocity;

    m_isGrounded = false;
    m_isHitKnockback = true;
    // m_rigidbody.AddForce(new Vector2(5.0f * direction, 5.0f), ForceMode2D.Impulse);;
  }

    // private void OnCollisionEnter2D(Collision2D collision) 
    // { 
    //     if (collision.gameObject.CompareTag("Level")) 
    //     { 
    //       foreach (var contact in collision.contacts)
    //       {
    //         if (contact.normal.normalized == Vector2.up)
    //         {
    //           Debug.Log("m_isOnGround = true");
    //           m_isOnGround = true;
    //           break;
    //         }
    //       }
    //       // collision.gameObject.SendMessage("ApplyDamage", 10);
    //     } 
    // } 

    // private void OnCollisionExit2D(Collision2D collision) 
    // { 
    //     if (collision.gameObject.CompareTag("Level")) 
    //     { 
    //       foreach (var contact in collision.contacts)
    //       {
    //         if (contact.normal.normalized == Vector2.up)
    //         {
    //           Debug.Log("m_isOnGround = false");
    //           m_isOnGround = false;
    //           break;
    //         }
    //       }
    //     } 
    // } 
}
