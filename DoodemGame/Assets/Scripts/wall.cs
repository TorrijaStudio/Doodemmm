using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class wall : NetworkBehaviour
{
    public int contadorInicial; 
    public bool cuentaRegresiva; 
    public float intervalo;
  

    private TextMeshProUGUI _text;
    private int contadorActual;
    private float tiempoTranscurrido;
    public bool startTimer = true;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        contadorActual = contadorInicial;
        ActualizarTexto();
    }

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;

        if (startTimer && tiempoTranscurrido >= intervalo)
        {
            if (cuentaRegresiva)
            {
                contadorActual--;
                if (contadorActual <= 0)
                {
                    startTimer = false;
                    if (GameManager.Instance.clientId==0)
                    {
                        GameManager.Instance.StartRoundClientRpc();
                    }
                    //transform.position +=Vector3.up*100;
                }
            }
            else
            {
                contadorActual++;
            }

            ActualizarTexto();
            tiempoTranscurrido = 0f;
        }
    }

    void ActualizarTexto()
    {
        int minutos =contadorActual / 60;
        int segundos = contadorActual % 60;
        _text.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        _text.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    public void StartTimer(int time)
    {
        contadorActual = time;
        tiempoTranscurrido = 0;
        startTimer = true;
    }

    public void StopTimer()
    {
        startTimer = false;
        tiempoTranscurrido = 0;
    }
}
