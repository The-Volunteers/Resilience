using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Musics")]
    public AK.Wwise.Event PlayInGameMusic;
    public AK.Wwise.Event StopInGameMusic;
    public AK.Wwise.Event PlaySeaMusic;
    public AK.Wwise.Event PlayEndCredits;
    [Header("SFX")]
    public AK.Wwise.Event PlayOpenDoor;
    public AK.Wwise.Event PlayCloseDoor;
    public AK.Wwise.Event PlayEntity;
    public AK.Wwise.Event PlayGrab;
    public AK.Wwise.Event PlayWalk;
    public AK.Wwise.Event StopWalk;
    public AK.Wwise.Event PlayCarboard;
    public AK.Wwise.Event PlayTrash;
    public AK.Wwise.Event PlayAmbIndoor;
    public AK.Wwise.Event StopAmbIndoor;
    public AK.Wwise.Event PlayAmbOutdoor;
    public AK.Wwise.Event StopAmbOutdoor;
    [Header("Settings")]
    public AK.Wwise.RTPC GlobalVolume;
    public AK.Wwise.RTPC MusicVolume;
    public AK.Wwise.RTPC SfxVolume;
    [SerializeField] private float GlobalVolumeLevel;
    [SerializeField] private float MusicVolumeLevel;
    [SerializeField] private float SfxVolumeLevel;

    // Start is called before the first frame update
    void Start()
    {
        GlobalVolume.SetGlobalValue(GlobalVolumeLevel);
        MusicVolume.SetGlobalValue(MusicVolumeLevel);   
        SfxVolume.SetGlobalValue(SfxVolumeLevel);

        PlayAmbIndoor.Post(gameObject);
        PlayingInGameMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayingInGameMusic()
    {
        PlayInGameMusic.Post(gameObject);
    }
    public void StopingInGameMusic()
    {
        StopInGameMusic.Post(gameObject);
    }
    public void PlayingSeaMusic()
    {
        PlaySeaMusic.Post(gameObject);
    }
    public void PlayingOutDoorMusic()
    {
        PlayAmbOutdoor.Post(gameObject);
    }
    public void StopingOutDoorMusic()
    {
        StopAmbOutdoor.Post(gameObject);
    }

    public void PlayingOpenDoorSound(GameObject door)
    {
        PlayOpenDoor.Post(door);
    }
    public void PlayingCloseDoorSound(GameObject door)
    {
        PlayCloseDoor.Post(door);
    }
    public void PlayingGrabSound()
    {
        PlayGrab.Post(gameObject);
    }
    public void PlayingCardboardSound()
    {
        PlayCarboard.Post(gameObject);
    }
    public void PlayingTrashSound()
    {
        PlayTrash.Post(gameObject);
    }
    public void PlayingFootstepSound()
    {
        PlayWalk.Post(gameObject);
    }
    public void StopingFootstepSound()
    {
        StopWalk.Post(gameObject);
    }
    public void PlayingEntitySound(GameObject entity)
    {
        PlayEntity.Post(entity);
    }
    public void PlayingEndCreditsMusic()
    {
        PlayEndCredits.Post(gameObject);
    }
}
