using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //Particle systems should be simulated in world space
    //so that they can be moved around and played
    public ParticleSystem rockDestroy;
    public ParticleSystem seedCollected;
    public ParticleSystem rootFinished;
    public ParticleSystem waterCollect;
    public ParticleSystem moleDestroy;

    //Instance
    public static EffectManager instance;

    //Private
    List<ParticleSystem> usedParticles = new List<ParticleSystem>();


    private void Start()
    {
        instance = this;
    }

    public void PlayParticle(ParticleSystem particle, Vector3 position)
    {
        //Create a temporal particle system if called multiple times
        //within the same frame. (Destroy after 5sec)
        if (usedParticles.Contains(particle))
        {
            TempParticle(particle, position);
        }
        else
        {
            StartCoroutine(ParticleUsed(particle));
            particle.transform.position = position;
            particle.Play();
        }
    }

    void TempParticle(ParticleSystem particle, Vector3 position)
    {
        ParticleSystem part = Instantiate(particle);
        part.transform.position = position;
        part.Play();
        Destroy(part, 5);

    }

    IEnumerator ParticleUsed(ParticleSystem particle)
    {
        usedParticles.Add(particle);
        yield return new WaitForEndOfFrame();
        usedParticles.Remove(particle);
        yield return null;
    }
}
