using System;
using System.Collections;
using System.Collections.Generic;
using Totems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour, IPointerDownHandler
{
    private Transform _transform;

    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _transform = transform;
        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wall"), LayerMask.NameToLayer("Pointer"));
    }
    
    // Update is called once per frame
    void Update()
    {
        //If computer (??)
        PressController(MouseToRay());
    }

    private void PressController(Ray ray)
    {
        if (Physics.Raycast(ray, out var hit, 10f, LayerMask.GetMask("Wall")))
        {
            _transform.position = hit.point;
        }
    }
    


    

    private Ray MouseToRay()
    {
        return _camera.ScreenPointToRay(Input.mousePosition);
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        var pos = _camera.ScreenPointToRay(eventData.position);
    }
}
