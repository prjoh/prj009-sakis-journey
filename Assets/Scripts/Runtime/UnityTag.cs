using UnityEngine;
 
 
 [System.Serializable]
 public class UnityTag
 {
     [SerializeField]
     private string m_TagName = "Untagged";
     public string TagName
     {
         get { return m_TagName; }
     }
 
     public void Set(string _tagName)
     {
         if (m_TagName != _tagName)
         {
             m_TagName = _tagName;
         }
     }
 }