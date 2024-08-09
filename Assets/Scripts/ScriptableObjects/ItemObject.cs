using UnityEngine;


[CreateAssetMenu(fileName = "ItemObject", menuName = "Item Object", order = 0)]
public class ItemObject : ScriptableObject
{
  public Sprite sprite;
  public string itemName;
  public string itemText;
}
