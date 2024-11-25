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

   public void OnClickPlayButton()
    {
        if(tienda.canOnlyChooseOne && !tienda.SelectedObject) return;
        
        foreach (Transform obTr in storeItems)
        {
            var ob = obTr.GetComponent<objetoTienda>();
            if (ob.selected)
            {
                tienda.boughtObjects.Add(ob.gameObject);
            }
        }
        tienda.botones.SetActive(false); 
        // tienda.DeleteShopItems();
        FindObjectOfType<Inventory>().GetItemsFromShop();
        FindObjectOfType<Inventory>().SpawnTotems();
        // FindObjectOfType<Canvas>().gameObject.SetActive(false);
    }
}
