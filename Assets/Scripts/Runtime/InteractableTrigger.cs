using UnityEngine;


public class InteractableTrigger : MonoBehaviour
{
  [SerializeField] private Vector2 triggerSize = new(2.0f, 2.0f);
  [SerializeField] private LayerMask interactableLayer;  // TODO: This is doing nothing? Broken??
  // [HideInInspector] public string triggerButton;  // InteractableTrigger_Inspector
  [SerializeField] public string triggerButton;
  [SerializeField] private Sprite buttonSprite;

  private bool m_isInTriggerZone = false;
  private InteractableComponent m_interactableComponentReference = null;

  private void Awake()
  {
    var col = gameObject.AddComponent<BoxCollider2D>();
    col.isTrigger = true;
    col.size = triggerSize;
  }

  private void Start()
  {
    if (!gameObject.GetComponentInParent<Rigidbody2D>() && 
        !gameObject.GetComponentInChildren<Rigidbody2D>())
    {
      var rb = gameObject.AddComponent<Rigidbody2D>();
      rb.bodyType = RigidbodyType2D.Kinematic;
      rb.includeLayers = interactableLayer;
    }
  }

  private void Update()
  {
    if (!m_isInTriggerZone)
    {
      return;
    }

    if (triggerButton.Length == 0)
    {
      return;
    }

    // TODO: How do we handle that this is only triggered once??? (Subsequent button presses should be handled by InteractableDialogue)
    // TODO: How do we handle that the movement is disabled?
    if (Input.GetButtonDown(triggerButton))
    {
      Debug.Assert(m_interactableComponentReference, "[InteractableTrigger] No InteractableComponent reference set!");
      m_interactableComponentReference.InvokeInteractableEvent();
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    m_isInTriggerZone = true;

    m_interactableComponentReference = other.gameObject.GetComponent<InteractableComponent>();
    Debug.Assert(m_interactableComponentReference, "[InteractableTrigger] Unable to retrieve InteractableComponent!");

    switch (m_interactableComponentReference.InteractableType)
    {
      case eInteractableType.Dialogue:
        m_interactableComponentReference.SetInteracatbleActive(
          ref buttonSprite,
          new InteractableDialogue.UserData() { interactableButtonSprite = buttonSprite }
        );
        break;
      case eInteractableType.Chest:
        m_interactableComponentReference.SetInteracatbleActive(
          ref buttonSprite,
          new InteractableChest.UserData() { interactableButtonSprite = buttonSprite }
        );
        break;
      case eInteractableType.Door:
        m_interactableComponentReference.SetInteracatbleActive(ref buttonSprite);
        break;
      case eInteractableType.Undefined:
        Debug.Assert(false, "[InteractableTrigger] InteractableComponent type has not been set properly!");
        break;
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    m_isInTriggerZone = false;

    m_interactableComponentReference = other.gameObject.GetComponent<InteractableComponent>();
    Debug.Assert(m_interactableComponentReference, "[InteractableTrigger] Unable to retrieve InteractableComponent!");
    m_interactableComponentReference.SetInteracatbleInactive();
  }
}
