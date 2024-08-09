using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public enum eCameraMovementState
{
  Focused,
  Follow,
}

public class CameraController : MonoBehaviour
{
  [SerializeField] private GameObject screenOverlayPrefab;
  // [SerializeField] private UnityTag initialCameraGoalTag;
  [SerializeField] private float fadeInDuration = 1.5f;
  [SerializeField] private float fadeInDelay = 1.0f;
  [SerializeField] private float fadeOutDuration = 0.5f;
  [SerializeField] private float fadeOutDelay = 0.0f;

  public float FadeInTime
  {
    get { return fadeInDelay + fadeInDuration; }
  }

  private eCameraMovementState m_movementState = eCameraMovementState.Focused;
  private Transform m_goalTransform = null;

  private Image m_overlayImage = null;

  private void Awake()
  {
    Debug.Assert(screenOverlayPrefab, "[CameraController] No screen overlay prefab reference found. Please make sure to create a player object and tag it in the editor!");

    var screenOverlay = Instantiate(screenOverlayPrefab, Vector3.zero, Quaternion.identity, null);
    var canvas = screenOverlay.GetComponent<Canvas>();
    canvas.worldCamera = Camera.main;
    m_overlayImage = screenOverlay.GetComponentInChildren<Image>();
  }

  // private void Start()
  // {
  //   var currentGameScene = GameManager.Instance.CurrentGameScene;
  //   // var initialGoal = GameObject.FindWithTag(initialCameraGoalTag.TagName);

  //   Debug.Assert(currentGameScene, "[CameraController] Unable to retireve current game scene from GameManager.");
  //   // Debug.Assert(initialGoal, "[CameraController] No initial goal object found in scene. Please make sure to create one!");

  //   // SetMovementState(currentGameScene.initialCameraMovementState, initialGoal.transform);
  //   SetMovementState(currentGameScene.initialCameraMovementState, transform);

  //   DoFadeIn(currentGameScene.fadeInColor);
  // }

  private void FixedUpdate()
  {
    var goalPosition = transform.position;
    goalPosition.x = m_goalTransform.position.x;
    transform.position = Vector3.Lerp(transform.position, goalPosition, Time.fixedDeltaTime * 2.0f);
  }

  public void SetMovementState(eCameraMovementState state, Transform goalTransform)
  {
    m_movementState = state;
    m_goalTransform = goalTransform;

    if (!m_goalTransform)
    {
      m_goalTransform = transform;
    }
  }

  public void DoFadeIn(Color fromColor, Action callback = null)
  {
    Color toColor = new(fromColor.r, fromColor.g, fromColor.b, 0.0f);
    StartCoroutine(FadeOverlayColorCoroutine(fromColor, toColor, fadeInDuration, fadeInDelay, 0.0f, callback));
  }

  public void DoFadeOut(Color toColor, Action callback = null)
  {
    Color fromColor = new(toColor.r, toColor.g, toColor.b, 0.0f);
    StartCoroutine(FadeOverlayColorCoroutine(fromColor, toColor, fadeOutDuration, 0.0f, fadeOutDelay, callback));
  }

  private IEnumerator FadeOverlayColorCoroutine(Color fromColor, Color toColor, float duration, float delayIn, float delayOut, Action callback)
  {
    m_overlayImage.raycastTarget = true;

    m_overlayImage.color = fromColor;

    float elapsedTime = 0f;
    while (elapsedTime < delayIn)
    {
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    elapsedTime = 0.0f;
    while (elapsedTime < duration)
    {
        m_overlayImage.color = Color.Lerp(fromColor, toColor, elapsedTime / duration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    m_overlayImage.color = toColor;

    elapsedTime = 0f;
    while (elapsedTime < delayOut)
    {
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    m_overlayImage.raycastTarget = false;

    callback?.Invoke();
  }
}
