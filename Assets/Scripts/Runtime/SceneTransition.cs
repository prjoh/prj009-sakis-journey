using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SceneTransitionID
{
  public string Prefix;
  public SceneTransitionTag Tag;

  public void Reset()
  {
    Prefix = string.Empty;
    Tag = null;
  }

  public string GetID()
  {
    var tagString = Tag ? Tag.name : "None";
    return Prefix + tagString;
  }
}

public class SceneTransition : MonoBehaviour
{
  [SerializeField] private SceneTransitionID sceneTransitionID;

  [Header("References")]
  [SerializeField] private GameSceneObject nextScene;
  [SerializeField] private Transform playerSpawnTransform;

  [Header("Settings")]
  [SerializeField] private Vector2 size;
  [SerializeField] private UnityTag playerTag;
  [SerializeField] private UnityTag sceneTransitionTag;

  private GameSceneObject m_fromScene;
  private CameraController m_cameraController;

  private BoxCollider2D m_collider;

  private eSpriteDirection m_lastPlayerDirection = eSpriteDirection.Right;

  void OnValidate()
  {
    //If scene is not valid, the gameobject is most likely not instantiated (ex. prefabs)
    if (!gameObject.scene.IsValid())
    {
      sceneTransitionID.Reset();
      return;
    }

    sceneTransitionID.Prefix = $"ST-{gameObject.scene.name}-";
    gameObject.name = sceneTransitionID.GetID();
  }

  private void Awake()
  {
    m_fromScene = GameManager.Instance.CurrentGameScene;

    Debug.Assert(m_fromScene, "[SceneTransition] No GameSceneObject could be obtained from GameManager!");
    Debug.Assert(nextScene, "[SceneTransition] No GameSceneObject set as next scene in `" + m_fromScene.name + "`!");

    var camera = Camera.main;
    m_cameraController = camera.GetComponent<CameraController>();
    Debug.Assert(m_cameraController, "[SceneTransition] Unable to retrieve CameraController from Camera.main!");

    m_collider = gameObject.AddComponent<BoxCollider2D>();
  }

  private void Start()
  {
    m_collider.size = size;
    m_collider.isTrigger = true;
  }

  private void DoSceneTransition()
  {
    m_cameraController.DoFadeOut(m_fromScene.fadeOutColor, FadeOutCallback);
  }

  private void FadeOutCallback()
  {
    GameManager.Instance.LoadGameScene(nextScene, GetNextSceneTransitionID(), m_lastPlayerDirection);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.CompareTag(playerTag.TagName))
    {
      var graphics = other.gameObject.GetComponentInChildren<GraphicsController>();
      m_lastPlayerDirection = graphics.SpriteDirection;
      DoSceneTransition();
    }
  }

  private SceneTransitionID GetNextSceneTransitionID()
  {
    return new SceneTransitionID
    {
      Prefix = $"ST-{nextScene.scene.name}-",
      Tag = sceneTransitionID.Tag
    };
  }

  #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SceneTransitionID))]
    private class UniqueIdDrawer : PropertyDrawer
  {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty PrefixProp = property.FindPropertyRelative("Prefix");
            SerializedProperty TagProp = property.FindPropertyRelative("Tag");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUI.enabled = false;

            Rect prefixPropRect = position;
            prefixPropRect.width *= 0.5f;
            EditorGUI.PropertyField(prefixPropRect, PrefixProp, GUIContent.none);

            GUI.enabled = true;

            Rect postfixPropRect = position;
            postfixPropRect.x += prefixPropRect.width;
            postfixPropRect.width = prefixPropRect.width;
            EditorGUI.PropertyField(postfixPropRect, TagProp, GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
#endif
}
