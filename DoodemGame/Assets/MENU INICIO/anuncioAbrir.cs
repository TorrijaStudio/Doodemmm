using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anuncioAbrir : MonoBehaviour
{
    public string url;

   
   
    public void abrirAnuncio()
    {

        Application.OpenURL(url);
    }
}
