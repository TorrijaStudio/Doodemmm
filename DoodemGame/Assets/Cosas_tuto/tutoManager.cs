using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutoManager : MonoBehaviour
{


    public int index;
    
    
    public GameObject[] diapos;
    
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
        if (index < diapos.Length-1)
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
        for (int i = 0; i < diapos.Length; i++)
        {
            diapos[i].SetActive(false);
        }
        diapos[index].SetActive(true);
       
    }
}
