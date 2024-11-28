using System;
using TMPro;
using Totems;
using UnityEngine;
using UnityEngine.UI;

namespace ItemInformation
{
    public class ItemInfoDisplay : MonoBehaviour
    {
        //---------------------------------STATS---------------------------------
        [Header("Totem stats")] [SerializeField] private GameObject statsParentObject;
        [SerializeField] private Image damageIcon; 
        [SerializeField] private Image healthIcon; 
        [SerializeField] private Image speedIcon;
        [Space(5)] 
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI speedText;
        
        //---------------------------------TEXTS---------------------------------
        [Space(7)] [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private TextMeshProUGUI flavourText;

        private void Start()
        {
            statsParentObject.SetActive(false);
        }

        public void DisplayTotemPiece(ItemInfoSo itemInfoSo, TotemStats totemStats)
        {
            //Set stats information
            statsParentObject.SetActive(true);
            damageText.SetText(totemStats.damage.ToString());
            healthText.SetText(totemStats.health.ToString());
            speedText.SetText(totemStats.speed.ToString());
            
            //Display common information
            DisplayItem(itemInfoSo);
        }

        public void DisplayItem(ItemInfoSo itemInfoSo)
        {
            itemName.SetText(itemInfoSo.itemName);
            itemDescription.SetText(itemInfoSo.itemDescription);
            flavourText.SetText(itemInfoSo.flavourText);
        }
    }
}
