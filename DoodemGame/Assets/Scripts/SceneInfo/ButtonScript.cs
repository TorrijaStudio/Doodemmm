using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public item totem;

    private CanvasManager canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = transform.root.GetComponent<CanvasManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickButton()
    {
        canvas.OcClickButton(totem);
    }
}
