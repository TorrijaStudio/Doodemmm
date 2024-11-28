using System;
using System.Collections.Generic;
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
        private GameObject _closeButtonSingle;
        private GameObject _closeButtonMultiple;
        public static ItemInfoManager instance;
        private List<GameObject> _openDisplays;

        private void Start()
        {
            if(instance)    Destroy(gameObject);
            else
            {
                instance = this;
            }

            _closeButtonSingle = transform.Find("CloseButton1").gameObject;
            _closeButtonSingle = transform.Find("CloseButton2").gameObject;
            _closeButtonSingle.SetActive(false);
            _closeButtonMultiple.SetActive(false);
            _openDisplays = new List<GameObject>();
        }

        public void Display(ScriptableObjectTienda items)
        {
            if (items.objectsToSell.Count == 1)
            {
                var display = Instantiate(displayToInstantiate,center.position, quaternion.identity, transform);
                display.DisplayItem(items);
                _openDisplays.Add(display.gameObject);
                _closeButtonSingle.SetActive(true);
            }
            else {
                var i = 1;
                foreach (var it in items.objectsToSell)
                {
                    var display = Instantiate(displayToInstantiate, center.position + Vector3.up * offset * i,
                        quaternion.identity, transform);
                    display.DisplayItem(it.scriptableObjectTienda);
                    _openDisplays.Add(display.gameObject);
                    i--;
                }
                _closeButtonMultiple.SetActive(true);
            }
        }

        public void CloseDisplays()
        {
            for (int i = _openDisplays.Count - 1; i >= 0; i--)
            {
                Destroy(_openDisplays[i]);
            }
            _openDisplays.Clear();
            
            _closeButtonSingle.SetActive(false);
            _closeButtonMultiple.SetActive(false);
        }
    }
}
