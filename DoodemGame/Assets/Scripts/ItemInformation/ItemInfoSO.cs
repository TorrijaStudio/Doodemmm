using System.Collections.Generic;
using Totems;
using UnityEngine;

namespace ItemInformation
{
    [CreateAssetMenu(fileName = "ItemInfoData", menuName = "ScriptableObjects/ScriptableObject item info", order = 1)]
    public class ItemInfoSo : ScriptableObject
    {
        public string itemName;
        public string itemDescription;
        public string flavourText;

        public Sprite icon;

        public bool isTotemPiece;
    }
}