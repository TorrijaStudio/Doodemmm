using System;
using System.Collections;
using System.Collections.Generic;
using ItemInformation;
using tienda;
using UnityEngine;

public class item : MonoBehaviour
{
    private bool appear;
    private bool disappear;
    private Quaternion initialRotation;
    public ScriptableObjectTienda[] infos;

    public item activeItemCanvas;
    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up,30*Time.deltaTime,Space.Self);
        if (disappear)
        {
            transform.localScale *= 0.9f;
            if (transform.localScale.x<0.1f)
            {
                disappear = false;
                activeItemCanvas.AnimationAppear();
                gameObject.SetActive(false);
            }
            return;
        }

        if (appear)
        {
            transform.localScale *= 1.1f;
            if (transform.localScale.x > 1f)
            {
                transform.localScale = Vector3.one;
                appear = false;
            }
        }
    }

    public void AnimationDisappear()
    {
        disappear = true;
    }
    public void AnimationAppear()
    {
        gameObject.SetActive(true);
        transform.rotation = initialRotation;
        appear = true;
    }
}
