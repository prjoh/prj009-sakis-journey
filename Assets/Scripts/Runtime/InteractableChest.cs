using UnityEngine;


public class InteractableChest : MonoBehaviour
{
  public class UserData : InteractableComponent.UserData
  {
    public Sprite interactableButtonSprite;
  }

  // GameObject references
  [SerializeField] private InteractableComponent interactableComponent;
  [SerializeField] SpriteRenderer spriteRenderer;
  // Scene object references
  [SerializeField] private PlayerBehaviour playerBehaviour;
  [SerializeField] private ChestUI chestUI;
  // Item data
  [SerializeField] private ItemObject itemData;
  // Settings
  [SerializeField] private bool oneShot = true;
  
  private bool m_open = false;

  private void Awake()
  {
    if (!interactableComponent)
    {
      interactableComponent = GetComponentInChildren<InteractableComponent>();
    }
    Debug.Assert(interactableComponent, "[InteractableDialogue] No InteractableComponent set! Please add a reference in the editor.");
    interactableComponent.InteractableType = eInteractableType.Chest;

    if (!spriteRenderer)
    {
      var renderers = GetComponentsInChildren<SpriteRenderer>();
      spriteRenderer = renderers[1];
    }
    Debug.Assert(spriteRenderer, "[InteractableDialogue] No SpriteRenderer reference found! Please add a reference in the editor.");
  
    if (!playerBehaviour)
    {
      playerBehaviour = FindObjectOfType<PlayerBehaviour>();
    }
    Debug.Assert(playerBehaviour, "[InteractableDialogue] PlayerBehaviour was not found in scene! Please make sure player object exists and has PlayerBehaviour attached.");
  
    if (!chestUI)
    {
      chestUI = FindObjectOfType<ChestUI>();
    }
    Debug.Assert(chestUI, "[InteractableDialogue] ChestUI was not found in scene! Please make sure it exists.");
 
  }

  private void OnEnable()
  {
    interactableComponent.InteractableActive += OnInteractableActive;
    interactableComponent.InteractableInactive += OnInteractableInactive;
    interactableComponent.InteractableButtonEvent += OnInteractableButtonEvent;
  }

  private void OnDisable()
  {
    interactableComponent.InteractableActive -= OnInteractableActive;
    interactableComponent.InteractableInactive -= OnInteractableInactive;
    interactableComponent.InteractableButtonEvent -= OnInteractableButtonEvent;
  }

  private void OnInteractableActive(InteractableComponent.UserData userData)
  {
    Debug.Assert(userData != null, "[InteractableChest] Inavlid UserData passed!");

    var interactableChestData = (InteractableChest.UserData)userData;
    chestUI.SetButtonSprite(ref interactableChestData.interactableButtonSprite);
  }

  private void OnInteractableInactive()
  {
    // TODO
  }

  private void OnInteractableButtonEvent()
  {
    if (!m_open)
    {
    spriteRenderer.color = new Color(0.325f, 1.0f, 0.866f);

      playerBehaviour.SetPlayerState(ePlayerState.Chest);

      chestUI.Show(itemData.itemName, itemData.itemText, ref itemData.sprite);

      m_open = true;
      return;
    }
    else if (chestUI.IsAnimatingText)
    {
      chestUI.ShowAnimatingText();
      return;
    }
    else
    {
      chestUI.Hide();

      playerBehaviour.SetPlayerState(ePlayerState.Normal);

      if (oneShot)
      {
        interactableComponent.SetInteracatbleInactive();
        interactableComponent.gameObject.SetActive(false);
      }
    }
  }
}
