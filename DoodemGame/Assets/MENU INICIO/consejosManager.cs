using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class consejosManager : MonoBehaviour
{

    public TMP_Text textoConsejos;
    public string[] consejos;
    
    // Start is called before the first frame update
    void Start()
    {
        int num = Random.Range(0, consejos.Length);
        textoConsejos.text = consejos[num];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
