using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSlow : MonoBehaviour
{
    public float slow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity e))
        {
            if (!e.isFlying) return;
            e.SpeedModifier -= slow;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Entity e))
        {
            if (!e.isFlying) return;
            e.SpeedModifier += slow;
        }
    }
}
