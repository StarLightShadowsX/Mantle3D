using System.Collections;
using UnityEngine;

public class Northernlight : MonoBehaviour
{
    public AudioSource sound;
    public float waitTime = 15;
    public float maxVolume = .2f;
    public float volumeTime = 3f;

    private bool finished;
    Coroutine coroutine;

    public void Enter()
    {
        if (finished) return;
        Coroutine.Begin(ref coroutine, Routine(), true);
    }
    public void Exit()
    {
        if (finished) return;
        Coroutine.Stop(ref coroutine);
    }

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(waitTime);
        while (sound.volume < maxVolume) sound.volume += Time.unscaledDeltaTime / volumeTime;
        sound.volume = maxVolume;
        finished = true;
    }
}