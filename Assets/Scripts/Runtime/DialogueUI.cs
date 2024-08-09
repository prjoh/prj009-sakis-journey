using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueUI : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private GameObject dialoguePanel;
  [SerializeField] private GameObject buttonPanel;
  [SerializeField] private GameObject avatarLeft;
  [SerializeField] private GameObject avatarRight;
  [SerializeField] private Image leftAvatarImage;
  [SerializeField] private Image rightAvatarImage;
  [SerializeField] private TMP_Text dialogueText;
  [SerializeField] private Image buttonImage;

  [Header("Settings")]
  [SerializeField] private float showAnimationDuration = 0.3f;
  [SerializeField] private float hideAnimationDuration = 0.2f;
  [SerializeField] private float symbolPerSecond = 5.0f;
  [SerializeField] private float showProgressTipThreshold = 4.0f;

  private bool m_isShowing = false;
  public bool IsShowing
  {
    get { return m_isShowing; }
  }
  private float m_timeSinceShow = 0.0f;
  private bool m_isAnimatingText = false;
  public bool IsAnimatingText
  {
    get { return m_isAnimatingText; }
  }
  private string m_animationText = "";
  private IEnumerator m_animationCoroutine = null;

  private void Start()
  {
    Reset();
  }

  private void Update()
  {
    if (!m_isShowing)
    {
      return;
    }

    m_timeSinceShow += Time.deltaTime;

    if (!buttonPanel.activeSelf && m_timeSinceShow >= showProgressTipThreshold)
    {
      ShowProgressTip();
    } 
  }

  private void Reset()
  {
    buttonImage.sprite = null;
    dialogueText.text = "";
    leftAvatarImage.enabled = false;
    rightAvatarImage.enabled = false;
    avatarLeft.SetActive(false);
    avatarRight.SetActive(false);
    buttonPanel.SetActive(false);
    dialoguePanel.SetActive(false);
    m_isShowing = false;
    m_isAnimatingText = false;
    m_animationText = "";
  }

  public void SetProgressButtonSprite(ref Sprite buttonImageSprite)
  {
    buttonImage.sprite = buttonImageSprite;
  }

  public void Show()
  {
    m_timeSinceShow = 0.0f;

    if (m_isShowing)
    {
      return;
    }

    dialoguePanel.SetActive(true);
    m_isShowing = true;

    StartCoroutine(GrowDialoguePanel(Vector3.zero, Vector3.one, showAnimationDuration));
  }

  public void Show(string text)
  {
    m_animationCoroutine = AnimateText(text, symbolPerSecond);
    StartCoroutine(m_animationCoroutine);
    Show();
  }

  public void Show(string text, ref Sprite avatarSprite, bool isLeftAvatar)
  {
    if (isLeftAvatar)
    {
      avatarLeft.SetActive(true);
      avatarRight.SetActive(false);
      leftAvatarImage.sprite = avatarSprite;
      leftAvatarImage.enabled = true;
    }
    else
    {
      avatarLeft.SetActive(false);
      avatarRight.SetActive(true);
      rightAvatarImage.sprite = avatarSprite;
      rightAvatarImage.enabled = true;
    }

    m_animationCoroutine = AnimateText(text, symbolPerSecond);
    StartCoroutine(m_animationCoroutine);

    Show();
  }

  // public void Show(string text, ref Sprite leftAvatarSprite, ref Sprite rightAvatarSprite)
  // {
  //   avatarLeft.SetActive(true);
  //   leftAvatarImage.sprite = leftAvatarSprite;
  //   leftAvatarImage.enabled = true;

  //   avatarRight.SetActive(true);
  //   rightAvatarImage.sprite = rightAvatarSprite;
  //   rightAvatarImage.enabled = true;

  //   dialogueText.text = text;

  //   Show();
  // }

  public void ShowProgressTip()
  {
    buttonPanel.SetActive(true);
  }

  public void Hide()
  {
    StartCoroutine(GrowDialoguePanel(Vector3.one, Vector3.zero, hideAnimationDuration, Reset));
  }

  private IEnumerator GrowDialoguePanel(Vector3 start, Vector3 end, float duration, Action callback = null)
  {
    dialoguePanel.transform.localScale = start;

    for(float time = 0 ; time < duration * 2 ; time += Time.deltaTime)
    {
        float progress = time / duration;
        dialoguePanel.transform.localScale = Vector3.Lerp(start, end, progress);
        yield return null;
    }

    dialoguePanel.transform.localScale = end;

    callback?.Invoke();
  }

  public void ShowAnimatingText()
  {
    if (!m_isAnimatingText)
    {
      return;
    }

    StopCoroutine(m_animationCoroutine);
    dialogueText.text = m_animationText;

    m_isAnimatingText = false;
    m_animationText = "";
  }

  private IEnumerator AnimateText(string text, float symbolsPerSecond)
  {
    m_animationText = text;
    dialogueText.text = "";
    m_isAnimatingText = true;

    var timePerSymbol = 1.0f / symbolsPerSecond;
    var totalSymbols = text.Length;
    float timer = 0.0f;
    int symbolIndex = 0;

    while (symbolIndex != totalSymbols)
    {
      timer += Time.deltaTime;

      while (timer >= timePerSymbol)
      {
        dialogueText.text += text[symbolIndex++];
        timer -= timePerSymbol;
        yield return null;
      }

      yield return null;
    }

    dialogueText.text = text;
    m_isAnimatingText = false;
  }
}
