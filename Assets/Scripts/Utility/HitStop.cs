using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    private bool waiting;
    public static HitStop instance;

    private Coroutine hitstopRoutine;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Stop(float duration)
    {
        //start the color change coroutine to return to base color
        if (hitstopRoutine != null)
            StopCoroutine(hitstopRoutine);

        Time.timeScale = 0.0f;
        hitstopRoutine = StartCoroutine(Wait(duration));
    }

    private IEnumerator Wait(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        if(!GameOverMenu.justDied && !PauseMenu.isPaused)
            Time.timeScale = 1.0f;
    }
}
