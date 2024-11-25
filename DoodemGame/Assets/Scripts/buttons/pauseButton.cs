using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class pauseButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickButton()
    {
        GameManager.Instance.gameCanvas.SetActive(false);
        GameManager.Instance.pauseCanvas.gameObject.SetActive(true);
    }
}
