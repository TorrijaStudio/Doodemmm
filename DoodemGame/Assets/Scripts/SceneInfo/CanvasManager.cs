using System;
using System.Collections;
using System.Collections.Generic;
using ItemInformation;
using UnityEditor;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public item activeItem; 
    [SerializeField] private ItemInfoDisplay[] itemInfoDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < itemInfoDisplay.Length; i++)
        {
            itemInfoDisplay[i].DisplayItem(activeItem.infos[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OcClickButton(item o)
    {
        if (activeItem == o) return;
        activeItem.activeItemCanvas = o;
        activeItem.AnimationDisappear();
        activeItem = o;
        for (var index = 0; index < o.infos.Length; index++)
        {
            //itemInfoDisplay[index].DisplayItem(o.infos[index]);
            itemInfoDisplay[index].GetComponent<popUp>().RotateSection(itemInfoDisplay[index],o.infos[index]);
        }
    }
}
