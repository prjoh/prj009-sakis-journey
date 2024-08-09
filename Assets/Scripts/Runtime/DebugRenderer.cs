using UnityEngine;


public class DebugRenderer : MonoBehaviour
{
  [SerializeField] private LineRenderer lineRenderer;
  [SerializeField] private Material debugMaterial;

  private float m_lifetime;
  public bool IsActive
  {
    get { return m_lifetime > 0.0f; }
  }

  private void Awake()
  {
    Debug.Assert(lineRenderer, "[DebugRenderer] LineRenderer reference not found! Please set it in the editor.");
    Debug.Assert(debugMaterial, "[DebugRenderer] Material reference not found! Please set it in the editor.");

    transform.position = Vector3.zero;
  }

  private void Update()
  {
    if (m_lifetime > 0.0f)
    {
      m_lifetime -= Time.deltaTime;
    }
  }

  public void SetConfig(float lifetime, float lineWidth, Color color)
  {
    m_lifetime = lifetime;

    lineRenderer.startWidth = lineWidth;
    lineRenderer.endWidth = lineWidth;

    lineRenderer.sharedMaterial = new Material(debugMaterial)
    {
      color = color
    };
  }

  public void SetVertexData(Vector3[] vertices)
  {
    lineRenderer.positionCount = vertices.Length;
    lineRenderer.SetPositions(vertices);
  }
}
