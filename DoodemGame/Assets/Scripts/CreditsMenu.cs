using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CreditsMenu : MonoBehaviour
{
    public void Back()
    {
        // se va a usar para varias pantallas no solo creditops pero bueno
        SceneManager.LoadScene("newMainMenu"); 
    }

    }

