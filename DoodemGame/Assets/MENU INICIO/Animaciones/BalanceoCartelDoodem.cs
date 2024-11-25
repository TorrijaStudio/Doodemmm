using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceoCartelDoodem : MonoBehaviour
{

    public float speed = 1.5f;
    public float range = 10.0f;

    private float anguloInicial;
    // Start is called before the first frame update
    void Start()
    {
        anguloInicial = transform.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        float angulo = anguloInicial+Mathf.Sin(Time.time*speed)*range;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angulo);
    }
}
