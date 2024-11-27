/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

*/

using System.Collections.Generic;
//using Mono.Cecil;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

   /* public List<EventInstance> eventInstances;

    private EventInstance ambienceEventInstance;

    private EventInstance musicEventInstance;

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one audio manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.instance.PajarosAmbience);
        InitializeMusic(FMODEvents.instance.MenuMusic);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetMusicParameter(string parameterName, float paramenterValue)
    {
        musicEventInstance.setParameterByName(parameterName, paramenterValue);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }*/
}
