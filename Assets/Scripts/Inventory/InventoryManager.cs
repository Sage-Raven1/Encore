using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Canvas inventoryCanvas;

    public InventorySlotScript[] itemSlots;
    public GameObject lootPrefab;
    public Transform player;

    private void Awake()
    {
        inventoryCanvas.enabled = false;
    }

    private void Start()
    {
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Debug.Log("pressed");
            if (inventoryCanvas.enabled == true)
            {
                inventoryCanvas.enabled = false;
            }
            else
            {
                inventoryCanvas.enabled = true;
            }
        }
    }

    public void AddItem(ItemSo itemSo, int quantity)
    {

        foreach (var slot in itemSlots)
        {
            if(slot.itemSo == itemSo && slot.quantity < itemSo.stackSize)
            {
                int availableSpace = itemSo.stackSize - slot.quantity;
                int amountToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                slot.UpdateUI();

                if (quantity <= 0)
                {
                    return;
                }
            }
        }

        foreach (var slot in itemSlots)
        {
            if (slot.itemSo == null)
            {
                int amountToAdd = Mathf.Min(itemSo.stackSize, quantity);

                slot.itemSo = itemSo;
                slot.quantity = quantity;
                slot.UpdateUI();
                return;
            }
        }

        if (quantity > 0)
        {
            DropLoot(itemSo, quantity);
        }
    }

    private void DropLoot(ItemSo itemSo, int quantity)
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSo, quantity);
    }
}
