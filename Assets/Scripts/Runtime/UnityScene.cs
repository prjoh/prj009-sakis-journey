using UnityEngine;
 
 
 [System.Serializable]
 public class UnityScene
 {
     [SerializeField]
     private string m_SceneName = null;
     public string SceneName
     {
         get { return m_SceneName; }
     }
 
     public void Set(string _sceneName)
     {
         if (m_SceneName != _sceneName)
         {
             m_SceneName = _sceneName;
         }
     }
 }