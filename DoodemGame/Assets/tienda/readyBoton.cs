using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class readyBoton : MonoBehaviour
{
    // Start is called before the first frame update
    public objetoTienda[] storeObjects;
    public playerInfoStore tienda;
    public Transform storeItems;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuySelected()
    {
        if(tienda.canOnlyChooseOne && !tienda.SelectedObject) return;
        
        foreach (Transform obTr in storeItems)
        {
            var ob = obTr.GetComponent<objetoTienda>();
            if (ob.selected)
            {
                tienda.PlayerMoney -= ob.price;
                tienda.boughtObjects.Add(ob.gameObject);
            }
        }
        FindObjectOfType<Inventory>().GetItemsFromShop();
        // FindObjectOfType<Inventory>().SpawnTotems();
    }
    
   public void OnClickPlayButton()
    {
        if(tienda.canOnlyChooseOne && !tienda.SelectedObject) return;
        
        foreach (Transform obTr in storeItems)
        {
            var ob = obTr.GetComponent<objetoTienda>();
            if (ob.selected)
            {
                tienda.PlayerMoney -= ob.price;
                tienda.boughtObjects.Add(ob.gameObject);
            }
        }
        tienda.ToggleShopOrMixing(false);
        // FindObjectOfType<Inventory>().SpawnTotems();
        // tienda.botones.SetActive(false); 
        tienda.DeleteShopItems();
        // FindObjectOfType<Canvas>().gameObject.SetActive(false);
    }
}
