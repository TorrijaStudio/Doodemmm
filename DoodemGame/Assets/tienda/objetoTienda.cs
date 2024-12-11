using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using formulas;
using ItemInformation;
using tienda;
using TMPro;
using Totems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class objetoTienda : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] public ScriptableObjectTienda info;

    public int price;
    public bool selected;
    [SerializeField] private ItemInfoDisplay itemInfoDisplay;

    private playerInfoStore _store;

    private TextMeshProUGUI _proUGUI;
    private PriceBiome _priceBiome = new PriceBiome(20, 1, 2);
    private PricePieceTotem _priceTotemPiece = new PricePieceTotem(3, 5, 5);
    // Start is called before the first frame update
    void Start()
    {
        // var a = keyWordColor.
        // _proUGUI = GetComponentInChildren<TextMeshProUGUI>();
        // _store.OnItemSelected += SetTextColour;
    }

    private void OnDestroy()
    {
        if(_store)
            _store.OnItemSelected -= SetTextColour;
    }

    private void SetTextColour()
    {
        if (selected)
        {
            _proUGUI.color = new Color(0.55f, 0.2f, 0.58f);
            return;
        }

        _proUGUI.color = _store.CanBuyItem(price) ? playerInfoStore.AvailableColor : playerInfoStore.UnavailableColor;
    }
    
    
    public void CreateObject(ScriptableObjectTienda scriptableObjectTienda, bool isFullTotem = false)
    {
        info = scriptableObjectTienda;
        var img = transform.Find("Image").GetComponent<Image>();
        img.sprite = info.image;
        if (!info.isBiome)
        {
            img.transform.localScale = Vector3.one*3.2f;
            img.raycastPadding = Vector4.one * 32f;
            img.GetComponent<BoxCollider2D>().size = Vector2.one * 100 / 3.2f;
        }
        _store = FindObjectOfType<playerInfoStore>();
        _proUGUI = GetComponentInChildren<TextMeshProUGUI>();
        if(isFullTotem)
        {
            price = info.price;
        }
        else
        {
            if (scriptableObjectTienda.isBiome)
            {
                price = _priceBiome.GetPrice(GameManager.Instance.currentRound, GameManager.Instance.playerObjects.Count(aux =>
                {
                    if (aux && aux.TryGetComponent<ABiome>(out var b))
                    {
                        return b.type == info.biomeType;
                    }
                    return false;
                }));
            }
            else
            {
                if (info.objectsToSell.Any(a => a.type == TotemPiece.Type.Head))
                {
                    price = _priceTotemPiece.GetHead();
                }else if (info.objectsToSell.Any(a => a.type == TotemPiece.Type.Feet))
                {
                    price = _priceTotemPiece.GetFeet();
                }else
                {
                    price = _priceTotemPiece.GetBody();
                }
            }
        }
        _proUGUI.SetText(price.ToString());
        _store.OnItemSelected += SetTextColour;
        SetTextColour();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!selected && eventData.button == PointerEventData.InputButton.Left)
        {
            SelectItem();
        }
        else if (selected && eventData.button == PointerEventData.InputButton.Right)
        {
            DeselectItem();
        }
    }

    public void SelectItem()
    {            
        if (!_store.CanBuyItem(price) || (_store.canOnlyChooseTwo  && _store.selectedItemsCount >= 2)) return;
        _store.selectedItemsCount++;
        selected = true;
            
        // if (_store.canOnlyChooseTwo){
        //     _store.SelectedObject = this;
        // }
            
        _store.SelectedItemsCost += price;
    }

    public void DeselectItem()
    {
        if(!selected)   return;
        _store.selectedItemsCount--;
        selected = false;
        _store.SelectedItemsCost -= price;
        // if (_store.canOnlyChooseTwo)
        //     _store.SelectedObject = null;
    }
    
    public void DisplayInfo()
    {
        ItemInfoManager.instance.Display(info);
    }


}

