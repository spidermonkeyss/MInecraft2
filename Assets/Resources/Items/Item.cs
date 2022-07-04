using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public uint maxStackSize;
    protected Texture2D _itemTexture;
    protected Texture2D ItemTexture
    {
        get { return _itemTexture; }
        set 
        { 
            _itemTexture = value;
            _itemSprite = Sprite.Create(_itemTexture, new Rect(0, 0, _itemTexture.width, _itemTexture.height), new Vector2(0.5f, 0.5f));
        }
    }
    protected Sprite _itemSprite;

    public Sprite GetItemSprite()
    {
        return _itemSprite;
    }

    //OLD
    public static void SpawnItem<itemType>(Vector3 location) where itemType : Item , new()
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.layer = LayerMask.NameToLayer("Item");
        item.transform.position = location;
        item.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        item.AddComponent<SphereCollider>();
        item.GetComponent<SphereCollider>().isTrigger = true;
        item.AddComponent<Rigidbody>();
        item.AddComponent<ItemObject>();
        item.GetComponent<ItemObject>().item = new itemType();
    }

    public static void SpawnItem(Vector3 location, System.Type itemType)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.layer = LayerMask.NameToLayer("Item");
        item.transform.position = location;
        item.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        item.AddComponent<SphereCollider>();
        item.GetComponent<SphereCollider>().isTrigger = true;
        item.AddComponent<Rigidbody>();
        item.AddComponent<ItemObject>();        
        item.GetComponent<ItemObject>().item = (Item)System.Activator.CreateInstance(itemType);
    }

    public virtual void OnRightClick(Inventory itemOwner, int slotIndex)
    {

    }

    public virtual void OnRightClick(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {

    }

    public virtual void OnLeftClick(Inventory itemOwner, int slotIndex)
    {

    }

    public virtual void OnLeftClick(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {

    }

    public virtual void OnRightClickHeld(Inventory itemOwner, int slotIndex)
    {

    }

    public virtual void OnRightClickHeld(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {

    }

    public virtual void OnLeftClickHeld(Inventory itemOwner, int slotIndex)
    {

    }

    public virtual void OnLeftClickHeld(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {

    }
}
