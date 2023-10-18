using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    private bool waiting;
    public static HitStop instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Stop(float duration)
    {
        if (waiting)
            return;
        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }

    private IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);

        if(!GameOverMenu.justDied && !PauseMenu.isPaused)
            Time.timeScale = 1.0f;
        waiting = false;
    }
}
