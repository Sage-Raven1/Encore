using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSo itemSo;
    public SpriteRenderer sr;

    public int quantity;

    private void OnValidate()
    {
        if (itemSo == null)
        {
            return;
        }
        sr.sprite = itemSo.icon;
    }
}
