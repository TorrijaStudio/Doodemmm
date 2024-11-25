using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationClic : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    private Animator _animator;

    private float timer;
    private bool playing;
    public AnimationClip ac;
    private Vector3 originScale;
    private void Awake()
    {
        originScale = transform.localScale;
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            if (timer < ac.length)
            {
                timer += Time.deltaTime;
            }
            else
            {
                transform.localScale = originScale;
                _animator.enabled = false;
                playing = false;
                timer = 0;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!playing)
        {
            playing = true;
            _animator.enabled = true;
        }
    }
    

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
