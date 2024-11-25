using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenalHover : MonoBehaviour
{
    public float distancia = 50.0f; // Distancia que se moverá la señal a la derecha
    public float velocidad = 50.0f; // Velocidad del desplazamiento

    private Vector3 posicionOriginal; // Posición inicial de la señal
    private Vector3 posicionDestino;  // Posición hacia donde se moverá
    private bool moverDerecha = false; // Controla si la señal debe moverse a la derecha

    void Start()
    {
        // Guardar la posición original de la señal
        posicionOriginal = transform.position;

        // Calcular la posición destino (a la derecha)
        posicionDestino = posicionOriginal + transform.right * distancia;
    }

    void Update()
    {
        if (moverDerecha)
        {
            // Mover suavemente hacia la posición destino
            Debug.Log("ahora deberia estar moviendose");
            transform.position = Vector3.Lerp(transform.position, posicionDestino, Time.deltaTime * velocidad);
        }
        else
        {
            // Volver suavemente a la posición original
            transform.position = Vector3.Lerp(transform.position, posicionOriginal, Time.deltaTime * velocidad);
        }
    }

    void OnMouseEnter()
    {
        // Activar el movimiento hacia la derecha
        Debug.Log("esta el cursor encima sabes");
        moverDerecha = true;
    }

    void OnMouseExit()
    {
        // Regresar a la posición original
        Debug.Log("YA NO esta el cursor encima sabes");
        moverDerecha = false;
    }
}
