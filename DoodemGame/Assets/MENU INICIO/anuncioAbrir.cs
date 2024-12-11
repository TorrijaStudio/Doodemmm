using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anuncioAbrir : MonoBehaviour
{
    private string url = "https://astralcatstudio.itch.io/tankito";

    public void abrirAnuncio()
    {
        Application.OpenURL(url);
    }
}
