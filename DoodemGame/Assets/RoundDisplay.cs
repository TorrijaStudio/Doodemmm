using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundDisplay : MonoBehaviour
{
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite lossSprite;
    [SerializeField] private Sprite tieSprite;
    [Space(15)] [SerializeField] private Transform positions;
    private int _posIndex;

    public enum RoundDisplayInfo
    {
        Win,
        Loss,
        Tie,
    }
    
    public void UpdateRoundDisplay(RoundDisplayInfo info)
    {
        if(_posIndex >= positions.childCount)   return;
        
        var spriteToRender = info switch
        {
            RoundDisplayInfo.Win => winSprite,
            RoundDisplayInfo.Loss => lossSprite,
            RoundDisplayInfo.Tie => tieSprite,
            _ => throw new ArgumentOutOfRangeException(nameof(info), info, null)
        };
        var child = positions.GetChild(_posIndex);
        child.gameObject.SetActive(true);
        child.GetComponent<Image>().sprite = spriteToRender;

        _posIndex++;
    }
}
