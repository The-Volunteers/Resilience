using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [Header("Musics")]
    public AK.Wwise.Event PlayMainMenuMusic;
    public AK.Wwise.Event StopMainMenuMusic;

    [Header("Settings")]
    public AK.Wwise.RTPC MusicVolume;
    [SerializeField] private float musicVolumeLevel;
    // Start is called before the first frame update
    void Start()
    {
        MusicVolume.SetGlobalValue(musicVolumeLevel);
        PlayMainMenuMusic.Post(gameObject);
    }

    public void StopMenuMusic()
    {
        StopMainMenuMusic.Post(gameObject);
    }
}
