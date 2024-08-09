using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSceneObject", menuName = "Game Scene", order = 0)]
public class GameSceneObject : ScriptableObject
{
  // Scene data
  public SceneAsset scene;

  public eCameraMovementState initialCameraMovementState;

  public Color fadeInColor = Color.black;
  public Color fadeOutColor = Color.black;
}
