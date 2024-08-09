using UnityEngine;
using UnityEngine.UI;


public enum eInteractableType
{
  Undefined,
  Dialogue,
  Door,
  Chest,
}

public class InteractableComponent : MonoBehaviour
{
  public class UserData{};

  [SerializeField] RectTransform interactableGraphicTransform;
  [SerializeField] Image interactableGraphicImage;

  public delegate void OnInteractableButtonEvent();
  public event OnInteractableButtonEvent InteractableButtonEvent;
  public void InvokeInteractableEvent() => InteractableButtonEvent?.Invoke();

  public delegate void OnInteractableActive(UserData userData);
  public event OnInteractableActive InteractableActive;

  public delegate void OnInteractableInactive();
  public event OnInteractableInactive InteractableInactive;

  private eInteractableType m_interactableType = eInteractableType.Undefined;
  public eInteractableType InteractableType
  {
      get { return m_interactableType; }
      set { m_interactableType = value; }
  }

  private void Awake()
  {
    Debug.Assert(interactableGraphicTransform, "[InteractableComponent] No RectTransform set! Please add a reference in the editor.");
    Debug.Assert(interactableGraphicImage, "[InteractableComponent] No Image set! Please add a reference in the editor.");
  }

  private void Start()
  {
    SetInteracatbleInactive();
  }

  public void SetInteracatbleActive(ref Sprite interactableButtonSprite, UserData userData = null)
  {
    interactableGraphicImage.sprite = interactableButtonSprite;
    interactableGraphicImage.enabled = true;

    InteractableActive?.Invoke(userData);
  }

  public void SetInteracatbleInactive()
  {
    interactableGraphicImage.sprite = null;
    interactableGraphicImage.enabled = false;

    InteractableInactive?.Invoke();
  }
}
