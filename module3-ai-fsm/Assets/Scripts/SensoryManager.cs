using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryManager : MonoBehaviour
{

    public GameObject soundObjects;

    public SimpleStateMachine npc;

    public void OnEnable()
    {
        foreach (Transform transform in soundObjects.transform)
        {
            SoundObject soundObject = transform.GetComponent<SoundObject>();
            if (soundObject != null)
            {
                soundObject.OnSoundEmitted.AddListener(npc.SoundRecieve);
            }
        }
    }

    public void OnDisable()
    {
        foreach (Transform transform in soundObjects.transform)
        {
            SoundObject soundObject = transform.GetComponent<SoundObject>();
            if (soundObject != null)
            {
                soundObject.OnSoundEmitted.RemoveListener(npc.SoundRecieve);
            }
        }
    }
}
