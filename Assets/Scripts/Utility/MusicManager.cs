using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] string songName;

    void Start()
    {
        AudioManager.instance?.Play(songName);
    }

    void OnDestroy()
    {
        AudioManager.instance?.Stop(songName);
    }
}
