using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource effects, music;
    public AudioReverbFilter musicFilter;
    public AudioClip
        stretch,
        jump,
        drift,
        land,
        menuClick,
        bubbleEat,
        bubbleSpawn,
        combo1,
        combo2,
        combo3,
        waterLow,
        planetCollision;


    //Private
    Coroutine distort;
    float musicVolume;
    void Start()
    {
        instance = this;
        musicVolume = music.volume;
    }

    public void PlaySound(AudioClip clip)
    {
        effects.PlayOneShot(clip);
    }

    public void MusicDistort()
    {
        distort = StartCoroutine(Distortion());
        musicFilter.reverbPreset = AudioReverbPreset.Underwater;
    }
    public void EndDistort()
    {
        musicFilter.reverbPreset = AudioReverbPreset.Off;
        music.volume = musicVolume;
        if (distort != null)
        {
            StopCoroutine(distort);
        }
    }

    IEnumerator Distortion()
    {
        float t = 0;
        musicFilter.room /= 2;
        while (t < 0.75f)
        {
            t += Time.deltaTime;
            music.volume -= Time.deltaTime / 5;
            musicFilter.room += Time.deltaTime * 25;

            yield return null;
        }

        yield return null;
    }
}
