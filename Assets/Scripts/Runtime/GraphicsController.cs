using UnityEngine;


public enum eSpriteDirection
{
  Right,
  Left,
}

public class GraphicsController : MonoBehaviour
{
  [SerializeField] private SpriteRenderer spriteRenderer;
  [SerializeField] private Animator animator;

  private eSpriteDirection m_spriteDirection = eSpriteDirection.Right;
  public eSpriteDirection SpriteDirection
  {
    get { return m_spriteDirection; }
  }

  private void Awake()
  {
    if (!spriteRenderer)
    {
      spriteRenderer = GetComponent<SpriteRenderer>();
    }
    Debug.Assert(spriteRenderer, "[GraphicsController] No SpriteRenderer reference found! Please add a reference in the editor.");
  
    if (!animator)
    {
      animator = GetComponent<Animator>();
    }
    Debug.Assert(animator, "[GraphicsController] No Animator reference found! Please add a reference in the editor.");
  }

  public void SetSpriteDirection(eSpriteDirection spriteDirection)
  {
    if (m_spriteDirection == spriteDirection)
    {
      return;
    }

    m_spriteDirection = spriteDirection;

    switch (m_spriteDirection)
    {
      case eSpriteDirection.Right:
        spriteRenderer.flipX = false;
        break;
      case eSpriteDirection.Left:
        spriteRenderer.flipX = true;
        break;
    }
  }

  public void SetAnimationParameterBool(string name, bool value)
  {
    animator.SetBool(name, value);
  }
}
