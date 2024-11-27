/*using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("MoverHojas SFX")]
    [field: SerializeField] public EventReference MoverHojas { get; private set; }

    [field: Header("ClickMadera SFX")]
    [field: SerializeField] public EventReference ClickMadera { get; private set; }

    [field: Header("CaidaTotem SFX")]
    [field: SerializeField] public EventReference CaidaTotem { get; private set; }

    [field: Header("CaidaCadenas SFX")]
    [field: SerializeField] public EventReference CaidaCadenas { get; private set; }


    [field: Header("PajarosAmbience")]
    [field: SerializeField] public EventReference PajarosAmbience { get; private set; }

    [field: Header("MenuMusic")]
    [field: SerializeField] public EventReference MenuMusic { get; private set; }

    public static FMODEvents instance { get; private set; }

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one audio manager in the scene.");
        }
        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

}*/
