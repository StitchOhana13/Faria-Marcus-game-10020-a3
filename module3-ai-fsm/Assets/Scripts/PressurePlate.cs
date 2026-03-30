using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : SoundObject
{

    public Material materialNormal;
    public Material materialPressed;

    MeshRenderer meshRenderer;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        meshRenderer.material = materialPressed;
    }

    public void OnTriggerExit(Collider other)
    {
        meshRenderer.material = materialNormal;
    }
}
