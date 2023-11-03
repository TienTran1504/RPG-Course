using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    private void SetupVisuals()
    {
        if (itemData == null) return;
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }




    public void SetupItem(ItemData _itemdata, Vector2 _velocity){
        itemData = _itemdata;
        rb.velocity = _velocity;
        SetupVisuals();
    }


    public void PickupItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment){
            rb.velocity = new Vector2(0, 7);
            PlayerManager.instance.player.fx.CreatePopUpText("Fulled Inventory");
            return;
        }

        AudioManager.instance.PlaySFX(18,transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
