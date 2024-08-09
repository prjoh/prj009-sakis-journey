// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;


// [CustomEditor(typeof(InteractableTrigger))]
// [CanEditMultipleObjects]
// public class InteractableTrigger_Inspector : Editor
// {
//   private const string propertyName = "triggerButton";
//   private const string propertyPopupLabel = "Trigger Button";

//   private SerializedProperty property;
//   List<string> _choices = new();
//   int _choiceIndex = 0;

//   void OnEnable()
//   {
//     var inputManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
//     if (inputManagerAsset)
//     {
//       SerializedObject inputManagerObject = new(inputManagerAsset);
//       SerializedProperty axisArray = inputManagerObject.FindProperty("m_Axes");

//       for ( int i = 0; i < axisArray.arraySize; ++i )
//       {
//         var axis = axisArray.GetArrayElementAtIndex(i);
//         var name = axis.FindPropertyRelative("m_Name").stringValue;
//         _choices.Add(name);
//       }
//     }

//     // Setup the SerializedProperties.
//     property = serializedObject.FindProperty(propertyName);
//     if (property == null)
//     {
//       return;
//     }

//     // Set the choice index to the previously selected index
//     _choiceIndex = Array.IndexOf(_choices.ToArray(), property.stringValue);
//   }

//   public override void OnInspectorGUI()
//   {
//     if (property == null)
//     {
//       return;
//     }

//     // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
//     serializedObject.Update();

//     //doing the orientation thing
//     _choiceIndex = EditorGUILayout.Popup(propertyPopupLabel, _choiceIndex, _choices.ToArray());
//     if (_choiceIndex < 0)
//         _choiceIndex = 0;
//     property.stringValue = _choices[_choiceIndex];

//     // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
//     serializedObject.ApplyModifiedProperties();
//   }
// }