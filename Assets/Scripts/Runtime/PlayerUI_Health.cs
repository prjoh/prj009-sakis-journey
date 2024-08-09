using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI_Health : MonoBehaviour
{
  public enum eHealthStatus
  {
    Empty,
    Half,
    Full,
  }

  [SerializeField] private Sprite emptyHealthSprite;
  [SerializeField] private Sprite halfHealthSprite;
  [SerializeField] private Sprite fullHealthSprite;

  [SerializeField] private GameObject healthImagePrefab;

  private Health m_playerHealth = null;
  private List<Tuple<Image, eHealthStatus>> m_healthImages = new();

  private void Awake()
  {
    m_playerHealth = PlayerData.Instance.Health;

    for (int i = 0; i < m_playerHealth.MaxHealth / 2; ++i)
    {
      var gameObject = Instantiate(healthImagePrefab, transform);
      var img = gameObject.GetComponent<Image>();
      m_healthImages.Add(Tuple.Create(img, eHealthStatus.Full));
      img.sprite = fullHealthSprite;
    }
  }

  private void Start()
  {
    UpdateHealthSprites();
  }

  private void Update()
  {
    UpdateHealthSprites();
  }

  private void UpdateHealthSprites()
  {
    var full = m_playerHealth.Value / 2;
    var half = m_playerHealth.Value % 2;

    for (int i = 0; i < m_healthImages.Count; ++i)
    {
      var image = m_healthImages[i].Item1;
      var status = m_healthImages[i].Item2;

      if (full > 0)
      {
        image.sprite = fullHealthSprite;
        m_healthImages[i] = Tuple.Create(image, eHealthStatus.Full);
        full--;
      }
      else if (half > 0)
      {
        if (status == eHealthStatus.Full)
        { 
          // TODO: Flashing animation 
        }
        image.sprite = halfHealthSprite;
        m_healthImages[i] = Tuple.Create(image, eHealthStatus.Half);
        half--;
      }
      else
      {
        if (status != eHealthStatus.Empty)
        { 
          // TODO: Flashing animation 
        }
        image.sprite = emptyHealthSprite;
        m_healthImages[i] = Tuple.Create(image, eHealthStatus.Empty);
      }
    }
  }
}
