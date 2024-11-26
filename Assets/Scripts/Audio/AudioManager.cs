using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour , ISave
{
    public static AudioManager instance { get; private set; }

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    [Header("Volume")]

    [Range(0, 1)]
    public float masterVolume = 1;

    [Range(0, 1)]
    public float musicVolume = 1;

    [Range(0, 1)]
    public float ambienceVolume = 1;

    [Range(0, 1)]
    public float sfxVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");

        musicBus = RuntimeManager.GetBus("bus:/Music");

        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");

        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(sfxVolume);
    }

    private void Start()
    {
        Debug.Log("Iniciei");
        InitializeAmbienceEventInstance(FMODEvents.instance.ambienteFabrica);
        InitializeMusicEventInstance(FMODEvents.instance.musicaGame);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    private void InitializeAmbienceEventInstance(EventReference ambienceEvent)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEvent);
        ambienceEventInstance.start();
    }

    private void InitializeMusicEventInstance(EventReference musicEvent)
    {
        musicEventInstance = CreateEventInstance(musicEvent);
        musicEventInstance.start();
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void SetMusicParameter(string parameterName, float parameterValue)
    {
        musicEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);

        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emiterGameObject)
    {
        StudioEventEmitter emitter = emiterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return (emitter);
    }

    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach(StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    #region Save&Load

    public void CarregarSave(InfosSave save)
    {
        masterVolume = save.masterVolume;
        musicVolume = save.musicVolume;
        ambienceVolume = save.ambienceVolume;
        sfxVolume = save.sfxVolume;
    }

    public void SalvarSave(ref InfosSave save)
    {
        save.masterVolume = masterVolume;
        save.musicVolume = musicVolume;
        save.ambienceVolume = ambienceVolume;
        save.sfxVolume = sfxVolume;
    }

    #endregion
}
