using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryArrow : MonoBehaviour
{
    public int direction;
    private void OnMouseDown()
    {
        Inventory.Instance.MovePageUpDown(direction);
    }
}
