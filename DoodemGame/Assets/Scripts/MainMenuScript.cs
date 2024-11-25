using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        
        SceneManager.LoadScene("PrototypeMain"); 
    }

    public void Tienda()
    {
        SceneManager.LoadScene("Menu Tienda"); 

    }

    public void Creditos()
    {
        SceneManager.LoadScene("Menu Creditos");
    }

    public void QuitGame()
    {
        // Salir del juego
        Debug.Log("Quit");
        Application.Quit();
    }
}
