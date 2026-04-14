using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletePlate : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            SceneManager.LoadScene("Congrats");
        }
    }
}
