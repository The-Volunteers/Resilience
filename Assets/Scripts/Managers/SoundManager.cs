using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Musics")]
    public AK.Wwise.Event PlayInGameMusic;
    public AK.Wwise.Event StopInGameMusic;
    public AK.Wwise.Event PlaySeaMusic;
    [Header("Sounds")]
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
    public AK.Wwise.RTPC volume;
    [SerializeField] private float volumeLevel;
    // Start is called before the first frame update
    void Start()
    {
        volume.SetGlobalValue(volumeLevel);
        //AkUnitySoundEngine.SetRTPCValue("GameParameter", volumeLevel, gameObject);
        PlayAmbIndoor.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
