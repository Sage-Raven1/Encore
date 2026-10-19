using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Collectable")]
public class ItemSo : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite icon;
}
