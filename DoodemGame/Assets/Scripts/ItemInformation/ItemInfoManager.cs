using System;
using tienda;
using Unity.Mathematics;
using UnityEngine;

namespace ItemInformation
{
    public class ItemInfoManager : MonoBehaviour
    {
        [SerializeField] private ItemInfoDisplay displayToInstantiate;

        [SerializeField] private Transform center;
        [SerializeField] private float offset;
        public static ItemInfoManager instance;

        private void Start()
        {
            if(instance)    Destroy(gameObject);
            else
            {
                instance = this;
            }
        }

        public void Display(ScriptableObjectTienda items)
        {
            if (items.objectsToSell.Count == 1)
            {
                var display = Instantiate(displayToInstantiate);
                display.DisplayItem(items);
            }

            var i = 1;
            foreach (var it in items.objectsToSell)
            {
                var display = Instantiate(displayToInstantiate, center.position + Vector3.up*offset*i, quaternion.identity);
                display.DisplayItem(it.scriptableObjectTienda);
                i--;
            }
        }
    }
}
