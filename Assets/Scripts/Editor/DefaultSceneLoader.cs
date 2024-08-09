#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


[InitializeOnLoad]
public static class DefaultSceneLoader
{
  static DefaultSceneLoader(){
    var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
    // var pathOfFirstScene = EditorBuildSettings.scenes[1].path;  // DEBUG
    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
    EditorSceneManager.playModeStartScene = sceneAsset;
  }
}
#endif