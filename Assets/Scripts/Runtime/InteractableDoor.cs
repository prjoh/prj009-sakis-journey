using UnityEngine;


public class InteractableDoor : MonoBehaviour
{
  // GameObject references
  [SerializeField] private InteractableComponent interactableComponent;
  [SerializeField] private Vector2 size;
  [SerializeField] private bool oneShot = true;

  private Transform m_rendererTransform;
  private BoxCollider2D m_collider;

  private void Awake()
  {
    if (!interactableComponent)
    {
      interactableComponent = GetComponentInChildren<InteractableComponent>();
    }
    Debug.Assert(interactableComponent, "[InteractableDialogue] No InteractableComponent set! Please add a reference in the editor.");
    interactableComponent.InteractableType = eInteractableType.Door;

    var renderer = GetComponentInChildren<SpriteRenderer>();
    m_rendererTransform = renderer.gameObject.transform;

    var colliderObject = new GameObject("Collider");
    colliderObject.transform.parent = transform;
    colliderObject.transform.localPosition = Vector3.zero;
    m_collider = colliderObject.AddComponent<BoxCollider2D>();
    m_collider.size = size;
  }

  private void OnEnable()
  {
    interactableComponent.InteractableActive += OnInteractableActive;
    interactableComponent.InteractableInactive += OnInteractableInactive;
    interactableComponent.InteractableButtonEvent += OpenDoor;
  }

  private void OnDisable()
  {
    interactableComponent.InteractableActive -= OnInteractableActive;
    interactableComponent.InteractableInactive -= OnInteractableInactive;
    interactableComponent.InteractableButtonEvent -= OpenDoor;
  }

  private void OnInteractableActive(InteractableComponent.UserData userData)
  {
    // TODO
  }

  private void OnInteractableInactive()
  {
    // TODO
  }

  private void OpenDoor()
  {
    m_collider.enabled = false;
    var localPosition = m_rendererTransform.localPosition;
    localPosition.y += 4;
    m_rendererTransform.localPosition = localPosition;

    if (oneShot)
    {
      interactableComponent.SetInteracatbleInactive();
      interactableComponent.gameObject.SetActive(false);
    }
  }
}
