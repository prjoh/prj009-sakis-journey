using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class DebugTools : MonoBehaviour
{
  const string c_debugRendererPrefabPath = "Assets/Prefabs/DebugRenderer.prefab";

  private static DebugTools m_instance;
  public static DebugTools Instance
  {
      get
      {
        if (m_instance == null)
        {
            m_instance = FindObjectOfType<DebugTools>();
            if (m_instance == null)
            {
              var obj = new GameObject("DebugTools");
              m_instance = obj.AddComponent<DebugTools>();
            }
        }
        return m_instance;
      }
  }

  [SerializeField] private GameObject debugRendererPrefab;

  private readonly Color defaultColor = Color.magenta;

  private List<DebugRenderer> m_debugRenderers = new();

  private void Awake()
  {
    if (!debugRendererPrefab)
    {
      debugRendererPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(c_debugRendererPrefabPath);
    }
    Debug.Assert(debugRendererPrefab, "[DebugTools] No DebugRenderer prefab reference detected! Please make sure to set it in the editor.");
  
    transform.position = Vector3.zero;
  }

  private void Update()
  {
    var destroyObjects = m_debugRenderers
                          .Where(renderer => !renderer.IsActive)
                          .Select(renderer => renderer.gameObject)
                          .ToList();
    m_debugRenderers.RemoveAll(renderer => !renderer.IsActive);
    destroyObjects.ForEach(obj => Destroy(obj));
  }

  private void CreateDebugRenderer(Vector3[] vertexData, Color? color = null, float drawTime = 2.0f, float lineWidth = 0.1f)
  {
    var debugRenderer = Instantiate(debugRendererPrefab, Vector3.zero, Quaternion.identity, transform);
    var debugRendererComponent = debugRenderer.GetComponent<DebugRenderer>();
    Debug.Assert(debugRendererComponent, "[DebugTools] DebugRenderer component not found in prefab! Please make sure it exists.");
    
    var debugRendererColor = (Color)(color != null ? color : defaultColor);
    debugRendererComponent.SetConfig(drawTime, lineWidth, debugRendererColor);
    
    debugRendererComponent.SetVertexData(vertexData);
    m_debugRenderers.Add(debugRendererComponent);
  }

  private Vector3[] CreateBoxCollider2DVertexData(BoxCollider2D collider)
  {
      var colliderPosition = collider.gameObject.transform.position;
      var colliderOffset = (Vector3)collider.offset;
      var colliderSize = (Vector3)collider.size;
      var colliderHalfSize = colliderSize * 0.5f;

      var bottomLeft  = (colliderPosition + colliderOffset - colliderHalfSize).Round(3);
      var bottomRight = (bottomLeft + new Vector3(colliderSize.x, 0.0f, 0.0f)).Round(3);
      var topRight    = (colliderPosition + colliderOffset + colliderHalfSize).Round(3);
      var topLeft     = (topRight - new Vector3(colliderSize.x, 0.0f, 0.0f)).Round(3);

      return new Vector3[8]
      {
        topLeft, topRight,
        topRight, bottomRight,
        bottomRight, bottomLeft,
        bottomLeft, topLeft,
      };
  }

  private Vector3[] CreateNormalVertexData(Vector2 point, Vector2 normal, float length)
  {
    return new Vector3[2]
    {
      point,
      point + normal * length,
    };
  }

  public void DrawBoxCollider2D(BoxCollider2D collider, Color? color = null, float drawTime = 2.0f, float lineWidth = 0.025f)
  {
    CreateDebugRenderer(CreateBoxCollider2DVertexData(collider), color, drawTime, lineWidth);
  }

  public void DrawNormal2D(Vector2 point, Vector2 normal, float length, Color? color = null, float drawTime = 2.0f, float lineWidth = 0.025f)
  {
    CreateDebugRenderer(CreateNormalVertexData(point, normal, length), color, drawTime, lineWidth);
  }
}
