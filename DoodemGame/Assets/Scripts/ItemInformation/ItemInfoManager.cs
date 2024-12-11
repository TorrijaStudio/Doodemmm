using System;
using System.Collections.Generic;
using tienda;
using Tools;
using Totems;
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
        public Dictionary<string, string> keyWords;
        private static string[] _keyWords = new[] { "deathrattle", "poison", "bleed", "burn", "healing", "fly", "slowness", "protected"};
        private static string[] _resourceNames = new[] { "sand", "water", "rock", "tree", "nest", "ice", "grass"};
        private static string _keyWordColor = "#6affb8";
        private static string _resourceColor = "#5ef5ff";

        private void Start()
        {
            if(instance)    Destroy(gameObject);
            else
            {
                instance = this;
            }

            _closeButtonSingle = transform.Find("CloseButton1").gameObject;
            _closeButtonMultiple = transform.Find("CloseButton2").gameObject;
            _closeButtonSingle.SetActive(false);
            _closeButtonMultiple.SetActive(false);
            _openDisplays = new List<GameObject>();
        }

        public void Display(ScriptableObjectTienda items)
        {
            if (items.objectsToSell.Count <= 1)
            {
                var display = Instantiate(displayToInstantiate,center.position, quaternion.identity, transform);
                display.DisplayItem(items);
                _openDisplays.Add(display.gameObject);
                _closeButtonSingle.SetActive(true);
                _closeButtonSingle.transform.SetAsLastSibling();
            }
            else {
                var i = 1;
                var realOffset = Screen.height / 1080f * offset;
                foreach (var it in items.objectsToSell)
                {
                    var display = Instantiate(displayToInstantiate, center.position + Vector3.up * realOffset * i,
                        quaternion.identity, transform);
                    display.DisplayItem(it.scriptableObjectTienda);
                    _openDisplays.Add(display.gameObject);
                    i--;
                }
                _closeButtonMultiple.SetActive(true);
                _closeButtonMultiple.transform.SetAsLastSibling();
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
        
        public static string ProcessText(string text)
        {
            foreach (var resourceName in _resourceNames)
            {
                var indexes = text.AllIndexesOf(resourceName);
                foreach (var index in indexes)
                {
                    text = text.Insert(index + resourceName.Length, "</color>");
                    text = text.Insert(index, $"<color={_resourceColor}>");
                }
            }
            foreach (var resourceName in _keyWords)
            {
                var indexes = text.AllIndexesOf(resourceName);
                foreach (var index in indexes)
                {
                    text = text.Insert(index + resourceName.Length, "</color>");
                    text = text.Insert(index, $"<color={_keyWordColor}>");
                }
            }
            return text;
        }
    }
}
