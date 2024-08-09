using System.Collections;
using UnityEngine;


public enum ePlayerState
{
  Normal,
  Dialogue,
  Chest,
  SceneTransition,
  Dead,
}

// public struct DebugCollision
// {
//   public float drawTime;
//   public BoxCollider2D colliderA;
//   public BoxCollider2D colliderB;
//   public Vector3 normal;
// }

public class PlayerBehaviour : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private CharacterController2D characterController;
  [SerializeField] private InteractableTrigger interactableTrigger;
  [SerializeField] private GraphicsController graphicsController;
  
  [Header("Settings")]
  [SerializeField] private UnityTag enemyTag;
  [SerializeField] private UnityTag sceneTransitionTag;

  // [Header("Debug Settings")]
  // [SerializeField] private float lineWidth;
  // [SerializeField] private Color color;

  private ePlayerState m_state = ePlayerState.Normal;
  // private List<DebugCollision> m_debugCollisions = new();
  private Health m_health = null;

  // private bool m_debugCollisionFlag = false;

  private void Awake()
  {
    m_health = PlayerData.Instance.Health;
  }

  private void Start()
  {
    var camera = Camera.main;
    var cameraController = camera.GetComponent<CameraController>();

    SetPlayerState(ePlayerState.SceneTransition);
    StartCoroutine(EnablePlayerMovementCoroutine(cameraController.FadeInTime * 0.7f));
  }

  private void FixedUpdate()
  {
    graphicsController.SetAnimationParameterBool("IsGrounded", characterController.IsGrounded);
    graphicsController.SetAnimationParameterBool("IsMoving",   characterController.IsMoving);
    graphicsController.SetAnimationParameterBool("IsJumping",  characterController.IsJumping);
  }

  private void Update()
  {
    // if (m_debugCollisionFlag)
    // {
    //   m_debugCollisionFlag = false;
    // }

    // m_debugCollisions.ForEach(col => col.drawTime -= Time.deltaTime);
    // m_debugCollisions.RemoveAll(col => col.drawTime <= 0.0f);

    var horinzontalInput = Input.GetAxis("Horizontal");

    if (horinzontalInput > 0.0f)
    {
      graphicsController.SetSpriteDirection(eSpriteDirection.Right);
    }
    else if (horinzontalInput < 0.0f)
    {
      graphicsController.SetSpriteDirection(eSpriteDirection.Left);
    }
  }

  private IEnumerator EnablePlayerMovementCoroutine(float enableDelay)
  {
    float elapsedTime = 0.0f;
    while (elapsedTime < enableDelay)
    {
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    SetPlayerState(ePlayerState.Normal);
  }

  // TODO: This is bad...
  public void SetPlayerState(ePlayerState state)
  {
    switch (state)
    {
      case ePlayerState.Normal:
      {
        characterController.SetEnabled(true);
        interactableTrigger.enabled = true;
        graphicsController.enabled = true;
        break;
      }
      case ePlayerState.Chest:
      case ePlayerState.Dialogue:
      {
        characterController.SetEnabled(false);
        interactableTrigger.enabled = true;
        graphicsController.enabled = false;
        break;
      }
      case ePlayerState.SceneTransition:
      {
        characterController.SetEnabled(false);
        interactableTrigger.enabled = false;
        graphicsController.enabled = false;
        break;
      }
    }
  }

  // private void DrawBoxCollider2D(BoxCollider2D collider)
  // {
  //   var colliderPosition = collider.gameObject.transform.position;
  //   var colliderOffset = (Vector3)collider.offset;
  //   var colliderSize = (Vector3)collider.size;
  //   var colliderHalfSize = colliderSize * 0.5f;

  //   var bottomLeft = colliderPosition + colliderOffset - colliderHalfSize;
  //   var bottomRight = bottomLeft + new Vector3(colliderSize.x, 0.0f, 0.0f);
  //   var topRight = colliderPosition + colliderOffset + colliderHalfSize;
  //   var topLeft = topRight - new Vector3(colliderSize.x, 0.0f, 0.0f);

  //   Debug.DrawLine(topLeft,     topRight,    Color.red, 2.0f);
  //   Debug.DrawLine(topRight,    bottomRight, Color.red, 2.0f);
  //   Debug.DrawLine(bottomRight, bottomLeft,  Color.red, 2.0f);
  //   Debug.DrawLine(bottomLeft,  topLeft,     Color.red, 2.0f);
  // }

  // private void OnDrawGizmos()
  // {
  //   static void drawCollider(BoxCollider2D collider, Color color)
  //   {
  //     var colliderPosition = collider.gameObject.transform.position;
  //     var colliderOffset = (Vector3)collider.offset;
  //     var colliderSize = (Vector3)collider.size;
  //     var colliderHalfSize = colliderSize * 0.5f;

  //     var bottomLeft = colliderPosition + colliderOffset - colliderHalfSize;
  //     var bottomRight = bottomLeft + new Vector3(colliderSize.x, 0.0f, 0.0f);
  //     var topRight = colliderPosition + colliderOffset + colliderHalfSize;
  //     var topLeft = topRight - new Vector3(colliderSize.x, 0.0f, 0.0f);

  //     Gizmos.color = color;

  //     Gizmos.DrawLine(topLeft, topRight);
  //     Gizmos.DrawLine(topRight, bottomRight);
  //     Gizmos.DrawLine(bottomRight, bottomLeft);
  //     Gizmos.DrawLine(bottomLeft, topLeft);
  //   }

  //   foreach (var debugCollision in m_debugCollisions)
  //   {
  //     drawCollider(debugCollision.colliderA, Color.red);
  //     drawCollider(debugCollision.colliderB, Color.blue);
  //   }
  // }

  private void OnTriggerEnter2D(Collider2D collider) 
  { 
    if (collider.gameObject.CompareTag(enemyTag.TagName)) 
    {
      OnTriggerEnter2D_Enemy(collider);
    }
    else if (collider.gameObject.CompareTag(sceneTransitionTag.TagName))
    {
      OnTriggerEnter2D_SceneTransition(collider);
    }
  }

  private void OnTriggerEnter2D_Enemy(Collider2D collider)
  {
      // TODO: Should we also check for player velocity going down???
      var rb = GetComponent<Rigidbody2D>();
      Debug.Assert(rb, "[PlayerBehaviour] Unable to retrieve Rigidbody2D!");

      var playerPosition = rb.position;
      var collisionPoint = collider.ClosestPoint(playerPosition);

      // If we are already inside collider
      if (MathExtension.Approximately(collisionPoint, playerPosition))
      {
        var boxCollider = (BoxCollider2D)collider;
        playerPosition = rb.position + Vector2.up * boxCollider.size.y;
        collisionPoint = collider.ClosestPoint(playerPosition);
      }

      var collisionNormal = (playerPosition - collisionPoint).normalized;

      DebugTools.Instance.DrawBoxCollider2D((BoxCollider2D)collider, Color.red, 10.0f);
      DebugTools.Instance.DrawBoxCollider2D(GetComponent<BoxCollider2D>(), Color.blue, 10.0f);
      DebugTools.Instance.DrawNormal2D(collisionPoint, collisionNormal, 1.0f, Color.green, 10.0f);

      // // DrawBoxCollider2D((BoxCollider2D)collider);
      // m_debugCollisions.Add(
      //   new DebugCollision{ 
      //     colliderA = (BoxCollider2D)collider, 
      //     colliderB = GetComponent<BoxCollider2D>(), 
      //     drawTime = 2.0f,
      //     normal = collisionNormal,
      //   }
      // );

      var dot = Vector3.Dot(Vector2.up, collisionNormal);
      if (dot > 0.5f)
      {
        var enemyBehavior = collider.gameObject.GetComponent<EnemyBehaviour>();
        Debug.Assert(enemyBehavior, "[PlayerBehaviour] Unable to retrieve EnemyBehaviour!");
        enemyBehavior.Kill();

        characterController.SetKillKnockback();
      }
      else
      {
        m_health.Decrease(1);

        characterController.SetHitKnockback(collisionNormal.x);
        // if(!m_playerData.Health.IsAlive())
        // {
        //   Debug.Log("I GOT KILLED!");
        // }
      }

      // m_debugCollisionFlag = true;
  }

  private void OnTriggerEnter2D_SceneTransition(Collider2D collider)
  {
    SetPlayerState(ePlayerState.SceneTransition);
  }
}
