using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;

    private static float itemPickUpDistance = 1.0f;

    private void Update()
    {
        CheckIfPlayerPickup();
    }

    void CheckIfPlayerPickup()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, itemPickUpDistance, Vector3.up, itemPickUpDistance, 1 << LayerMask.NameToLayer("Player"));
        if (hits.Length > 0)
        {
            hits[0].transform.gameObject.GetComponent<PlayerController>().inventory.AddItemToInventory(item.GetType());
            Destroy(gameObject);
        }
    }
}
