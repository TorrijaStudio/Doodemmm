using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IrCreditos : MonoBehaviour
{
    public GameObject credit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void irCreditos()
    {
        AudioManager.PlaySound(SoundType.CLICK);
        //SceneManager.LoadScene("MenuCreditos");
        credit.SetActive(true);
    }

    public void volverCreditos()
    {
        AudioManager.PlaySound(SoundType.CLICK);
        //SceneManager.LoadScene("MenuCreditos");
        credit.SetActive(false);
    }
}
