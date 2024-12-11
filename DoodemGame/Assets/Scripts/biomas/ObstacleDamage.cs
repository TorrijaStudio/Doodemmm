using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    public float damage;

    public float attackSpeed;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent(out Entity e))
        {
            timer += Time.deltaTime;
            if (timer > 1 / attackSpeed)
            {
                timer = 0;
                e.Attacked(damage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out Entity e))
        {
            timer = 0;
        }
    }
}
