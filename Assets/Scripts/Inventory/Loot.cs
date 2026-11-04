using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSo itemSo;
    public SpriteRenderer spriteRender;

    public int quantity;

    private void OnValidate()
    {
        if (itemSo == null)
        {
            return;
        }
        spriteRender.sprite = itemSo.icon;
        this.name = itemSo.itemName;
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            StatsManager.Instance.playerInventory.Add(itemSo.itemName);
            Destroy(gameObject);
        }
    }
}
