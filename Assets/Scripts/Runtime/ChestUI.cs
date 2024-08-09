using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChestUI : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private GameObject mainPanel;
  [SerializeField] private Image itemImage;
  [SerializeField] private TMP_Text itemName;
  [SerializeField] private TMP_Text itemText;
  [SerializeField] private Image buttonImage;

  [Header("Settings")]
  [SerializeField] private float showAnimationDuration = 0.3f;
  [SerializeField] private float hideAnimationDuration = 0.2f;
  [SerializeField] private float symbolPerSecond = 35.0f;

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

  // private void Update()
  // {
  //   if (!m_isShowing)
  //   {
  //     return;
  //   }

  //   m_timeSinceShow += Time.deltaTime;

  //   if (!buttonPanel.activeSelf && m_timeSinceShow >= showProgressTipThreshold)
  //   {
  //     ShowProgressTip();
  //   } 
  // }

  private void Reset()
  {
    buttonImage.sprite = null;
    itemName.text = "";
    itemText.text = "";
    itemImage.enabled = false;
    mainPanel.SetActive(false);
    m_isShowing = false;
    m_isAnimatingText = false;
    m_animationText = "";
  }

  public void SetButtonSprite(ref Sprite buttonImageSprite)
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

    mainPanel.SetActive(true);
    m_isShowing = true;

    StartCoroutine(GrowMainPanel(Vector3.zero, Vector3.one, showAnimationDuration));
  }

  public void Show(string name, string text)
  {
    itemName.text = name;

    m_animationCoroutine = AnimateText(text, symbolPerSecond);
    StartCoroutine(m_animationCoroutine);
    Show();
  }

  public void Show(string name, string text, ref Sprite itemImageSprite)
  {
    itemName.text = name;

    itemImage.sprite = itemImageSprite;
    itemImage.preserveAspect = true;
    itemImage.enabled = true;

    m_animationCoroutine = AnimateText(text, symbolPerSecond);
    StartCoroutine(m_animationCoroutine);

    Show();
  }

  public void Hide()
  {
    StartCoroutine(GrowMainPanel(Vector3.one, Vector3.zero, hideAnimationDuration, Reset));
  }

  private IEnumerator GrowMainPanel(Vector3 start, Vector3 end, float duration, Action callback = null)
  {
    mainPanel.transform.localScale = start;

    for(float time = 0 ; time < duration * 2 ; time += Time.deltaTime)
    {
        float progress = time / duration;
        mainPanel.transform.localScale = Vector3.Lerp(start, end, progress);
        yield return null;
    }

    mainPanel.transform.localScale = end;

    callback?.Invoke();
  }

  public void ShowAnimatingText()
  {
    if (!m_isAnimatingText)
    {
      return;
    }

    StopCoroutine(m_animationCoroutine);
    itemText.text = m_animationText;

    m_isAnimatingText = false;
    m_animationText = "";
  }

  private IEnumerator AnimateText(string text, float symbolsPerSecond)
  {
    m_animationText = text;
    itemText.text = "";
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
        itemText.text += text[symbolIndex++];
        timer -= timePerSymbol;
        yield return null;
      }

      yield return null;
    }

    itemText.text = text;
    m_isAnimatingText = false;
  }
}
