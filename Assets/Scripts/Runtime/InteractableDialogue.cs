using UnityEngine;


public class InteractableDialogue : MonoBehaviour
{
  public class UserData : InteractableComponent.UserData
  {
    public Sprite interactableButtonSprite;
  }

  // GameObject references
  [SerializeField] private InteractableComponent interactableComponent;

  // Scene object references
  [SerializeField] private PlayerBehaviour playerBehaviour;
  [SerializeField] private DialogueUI dialogueUI;

  // Dialogue data
  [SerializeField] private DialogueObject dialogueData;

  // Settings
  [SerializeField] private bool oneShot = false;

  int m_dialogueDataIndex = 0;

  private void Awake()
  {
    if (!interactableComponent)
    {
      interactableComponent = GetComponentInChildren<InteractableComponent>();
    }
    Debug.Assert(interactableComponent, "[InteractableDialogue] No InteractableComponent set! Please add a reference in the editor.");
    interactableComponent.InteractableType = eInteractableType.Dialogue;

    if (!playerBehaviour)
    {
      playerBehaviour = FindObjectOfType<PlayerBehaviour>();
    }
    Debug.Assert(playerBehaviour, "[InteractableDialogue] PlayerBehaviour was not found in scene! Please make sure player object exists and has PlayerBehaviour attached.");
  
    if (!dialogueUI)
    {
      dialogueUI = FindObjectOfType<DialogueUI>();
    }
    Debug.Assert(dialogueUI, "[InteractableDialogue] DialogueUI was not found in scene! Please make sure it exists.");
  }

  private void OnEnable()
  {
    interactableComponent.InteractableActive += OnInteractableActive;
    interactableComponent.InteractableInactive += OnInteractableInactive;
    interactableComponent.InteractableButtonEvent += ProgressDialogue;
  }

  private void OnDisable()
  {
    interactableComponent.InteractableActive -= OnInteractableActive;
    interactableComponent.InteractableInactive -= OnInteractableInactive;
    interactableComponent.InteractableButtonEvent -= ProgressDialogue;
  }

  private void OnInteractableActive(InteractableComponent.UserData userData)
  {
    Debug.Assert(userData != null, "[InteractableDialogue] Inavlid UserData passed!");

    var interactableDialogueData = (InteractableDialogue.UserData)userData;
    dialogueUI.SetProgressButtonSprite(ref interactableDialogueData.interactableButtonSprite);
  }

  private void OnInteractableInactive()
  {
    // TODO
  }

  private void ProgressDialogue()
  {
    if (dialogueUI.IsAnimatingText)
    {
      dialogueUI.ShowAnimatingText();
      return;
    }

    if (m_dialogueDataIndex == 0)
    {
      playerBehaviour.SetPlayerState(ePlayerState.Dialogue);

      var dialogueSegment = dialogueData.dialogue[m_dialogueDataIndex];
      dialogueUI.Show(dialogueSegment.text, ref dialogueSegment.avatarSprite, dialogueSegment.isLeftAvatar);

      m_dialogueDataIndex++;
    }
    else if (m_dialogueDataIndex == dialogueData.dialogue.Count)
    {
      dialogueUI.Hide();

      playerBehaviour.SetPlayerState(ePlayerState.Normal);

      m_dialogueDataIndex = 0;

      if (oneShot)
      {
        interactableComponent.SetInteracatbleInactive();
        interactableComponent.gameObject.SetActive(false);
      }
    }
    else
    {
      var dialogueSegment = dialogueData.dialogue[m_dialogueDataIndex];
      dialogueUI.Show(dialogueSegment.text, ref dialogueSegment.avatarSprite, dialogueSegment.isLeftAvatar);

      m_dialogueDataIndex++;
    }

  }
}
