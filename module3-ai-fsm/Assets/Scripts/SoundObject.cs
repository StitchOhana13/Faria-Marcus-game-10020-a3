using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundObject : MonoBehaviour
{

    public UnityEvent<SoundObject> OnSoundTriggered = new UnityEvent<SoundObject>();

    AudioSource audioSource;
    public virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            audioSource.Play();
            OnSoundTriggered.Invoke(this);
        }
    }

}
