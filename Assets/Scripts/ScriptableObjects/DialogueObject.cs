using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct DialogueSegment
{
  public Sprite avatarSprite;
  public bool isLeftAvatar;
  public string text;
}

[CreateAssetMenu(fileName = "DialogueObject", menuName = "NPC Dialogue Object", order = 0)]
public class DialogueObject : ScriptableObject
{
  public List<DialogueSegment> dialogue;
}
