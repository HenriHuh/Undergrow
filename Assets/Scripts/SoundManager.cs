using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip musicGarden;
    public AudioClip musicTree;

    public List<AudioClip> harvest;
    public AudioClip sprout;
    public AudioClip selectSeed;
    public AudioClip startRoot;
    public AudioClip buySeed;
    public AudioClip completeTask;
    public AudioClip completeLevel;
    public AudioClip collectSeed;
    public AudioClip collectWater;
    public AudioClip hitRock;
    public AudioClip hitPoison;
    public AudioClip hitMole;
    public AudioClip rootbuildUp;
    public AudioClip rootFinish;
    public AudioClip rootLose;
    public AudioClip waterLowIndicator;
    public AudioClip buttonBasic;
    public AudioClip uiPop;
    public AudioClip chestOpen;
    public AudioClip seedBagOpen;


    AudioSource music;
    [HideInInspector] public AudioSource effect;
    [HideInInspector] public AudioSource poisonSource;


    List<AudioClip> playingClips = new List<AudioClip>();
    Coroutine musicFadeRoutine;
    float musicVolume;

    void Awake()
    {

        music = transform.GetChild(0).GetComponent<AudioSource>();
        effect = transform.GetChild(1).GetComponent<AudioSource>();
        poisonSource = transform.GetChild(2).GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            PlayMusic(music.clip);

            //TODO:
            //  Save and load volumes

        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.position = Camera.main.transform.position;
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
        music.volume = vol;
    }

    public void SetPoisonVolume(float vol)
    {
        poisonSource.volume = vol;
    }

    public void SetEffectVolume(float vol)
    {
        effect.volume = vol;
    }

    public void PlaySound(AudioClip clip)
    {
        if (!playingClips.Contains(clip))
        {
            playingClips.Add(clip);
            effect.PlayOneShot(clip);
            StartCoroutine(RemoveFromPlaying(clip));
        }
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        if (!playingClips.Contains(clip))
        {
            playingClips.Add(clip);
            effect.PlayOneShot(clip, volume);
            StartCoroutine(RemoveFromPlaying(clip));
        }
    }

    public void PlayHarvest()
    {
        AudioClip clip = harvest[Random.Range(0, harvest.Count)];
        if (!playingClips.Contains(clip))
        {
            playingClips.Add(clip);
            effect.PlayOneShot(clip);
            StartCoroutine(RemoveFromPlaying(clip));
        }
    }

    //Prevent same clips stacking by adding them to a list for one frame
    IEnumerator RemoveFromPlaying(AudioClip clip)
    {
        yield return new WaitForEndOfFrame();
        playingClips.Remove(clip);
        yield return null;
    }


    public void PlayMusic(AudioClip clip)
    {
        if (musicFadeRoutine != null)
        {
            StopCoroutine(musicFadeRoutine);
        }
        musicVolume = music.volume;
        musicFadeRoutine = StartCoroutine(MusicChange(clip));
    }

    IEnumerator MusicChange(AudioClip clip)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            music.volume = Mathf.MoveTowards(music.volume, 0, musicVolume * Time.deltaTime);
            yield return null;
        }
        music.clip = clip;
        music.Play();
        t = 0;
        while (t < 4f)
        {
            t += Time.deltaTime;
            music.volume = Mathf.MoveTowards(music.volume, musicVolume, musicVolume * Time.deltaTime / 4);
            yield return null;
        }
        music.volume = musicVolume;
        yield return null;
    }
}
