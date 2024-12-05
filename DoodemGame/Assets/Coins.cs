using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coins : MonoBehaviour
{
    public TMP_Text textomonedas;
    
    public int numCoins;
        
    // Start is called before the first frame update
    void Start()
    {
            
    }
    
    // Update is called once per frame
    void Update()
    {
            
    }
    
    public void sumarCoins(int coins)
    {
        numCoins += coins;
        textomonedas.text = numCoins.ToString();
    }
    
    public void restarCoins(int coins)
    {
        numCoins -= coins;
    }
}
