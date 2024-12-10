using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutoManager : MonoBehaviour
{


    public int index;
    
    
    // DIAPOS
    public GameObject diapo1;
    
    public GameObject diapo2;
    
    public GameObject diapo3;
    
    public GameObject diapo4;
    
    public GameObject diapo5;
    
    public GameObject diapo6;
    
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        Debug.Log("Ahora estas mirando la diapositiva numero "+ index );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // <>

    public void sumarIndice()
    {
        if (index < 5)
        {
            index++;
            cambiarDiapositiva();
        }
    }
    
    
    public void restarIndice()
    {
        if (index > 0)
        {
            index--;
            cambiarDiapositiva();
        }
    }


    public void cambiarDiapositiva()
    {
        Debug.Log("Ahora estas mirando la diapositiva numero " + index);
        if (index == 0)
        {
            diapo1.SetActive(true);
            diapo2.SetActive(false);
            diapo3.SetActive(false);
            diapo4.SetActive(false);
            diapo5.SetActive(false);
            diapo6.SetActive(false);
        }
        else if (index == 1)
        {
            diapo1.SetActive(false);
            diapo2.SetActive(true);
            diapo3.SetActive(false);
            diapo4.SetActive(false);
            diapo5.SetActive(false);
            diapo6.SetActive(false);
        }
        else if (index == 2)
        {
            diapo1.SetActive(false);
            diapo2.SetActive(false);
            diapo3.SetActive(true);
            diapo4.SetActive(false);
            diapo5.SetActive(false);
            diapo6.SetActive(false);
        }
        else if (index == 3)
        {
            diapo1.SetActive(false);
            diapo2.SetActive(false);
            diapo3.SetActive(false);
            diapo4.SetActive(true);
            diapo5.SetActive(false);
            diapo6.SetActive(false);
        }
        else if (index == 4)
        {
            diapo1.SetActive(false);
            diapo2.SetActive(false);
            diapo3.SetActive(false);
            diapo4.SetActive(false);
            diapo5.SetActive(true);
            diapo6.SetActive(false);
        }
        else if (index == 5)
        {
            diapo1.SetActive(false);
            diapo2.SetActive(false);
            diapo3.SetActive(false);
            diapo4.SetActive(false);
            diapo5.SetActive(false);
            diapo6.SetActive(true);
        }
    }
}
