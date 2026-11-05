using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSo itemSo;
    public SpriteRenderer spriteRender;

    public bool canBePickedUp = true;
    public int quantity;
    public static event Action<ItemSo, int> OnItemLooted;

    private void OnValidate()
    {
        if (itemSo == null)
        {
            return;
        }
    }

    public void Initialize(ItemSo itemSo, int quantity)
    {
        this.itemSo = itemSo;
        this.quantity = quantity;

        canBePickedUp = false;
        UpdateAppearance();

    }

    private void UpdateAppearance()
    {
        spriteRender.sprite = itemSo.icon;
        this.name = itemSo.itemName;
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.CompareTag("Player") && canBePickedUp == true)
        {
            OnItemLooted?.Invoke(itemSo, quantity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            canBePickedUp = true;
        }
    }
}
