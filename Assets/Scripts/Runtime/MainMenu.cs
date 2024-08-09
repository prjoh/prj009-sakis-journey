using UnityEditor;
using UnityEngine;


public class MainMenu : MonoBehaviour
{
  public void OnContinueClicked()
  {
    GameManager.Instance.LoadGame();
  }

  public void OnNewGameClicked()
  {
    GameManager.Instance.NewGame();
  }

  public void OnSettingsClicked()
  {

  }

  public void OnExitClicked()
  {
    #if UNITY_EDITOR
      EditorApplication.isPlaying = false;
    #else
      Application.Quit();
    #endif
  }
}
