using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backButton : MonoBehaviour
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
        GameManager.Instance.gameCanvas.SetActive(true);
        GameManager.Instance.pauseCanvas.gameObject.SetActive(false);
    }
}
