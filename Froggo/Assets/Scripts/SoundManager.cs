using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource effects, music;
    public AudioReverbFilter musicFilter;
    public AudioReverbZone revFilter;

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
    }
    public void EndDistort()
    {
        revFilter.minDistance = 0;
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
        while (t < 0.75f)
        {
            t += Time.deltaTime;

            yield return null;
        }
        //musicFilter.reverbPreset = AudioReverbPreset.Underwater;
        musicFilter.room /= 2;
        revFilter.minDistance = 15;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            revFilter.minDistance += Time.deltaTime * 20;
            music.volume -= Time.deltaTime / 6;
            //musicFilter.room += Time.deltaTime * 35;

            yield return null;
        }
        yield return null;
    }
}
