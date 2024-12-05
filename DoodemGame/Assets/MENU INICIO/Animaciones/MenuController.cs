using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject playUI;
    public GameObject shopUI;
    public GameObject optionsUI;

    public GameObject objectsScene;


public void ShowPlayUI(){
    playUI.SetActive(true);
    shopUI.SetActive(false);
    optionsUI.SetActive(false);
}

public void ShowShopUI(){
    playUI.SetActive(false);
    shopUI.SetActive(true);
    optionsUI.SetActive(false);
}

public void ShowOptionsUI(){
    playUI.SetActive(false);
    shopUI.SetActive(false);
    optionsUI.SetActive(true);
}

}