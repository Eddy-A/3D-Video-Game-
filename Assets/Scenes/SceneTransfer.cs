using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransfer : MonoBehaviour
{
    public GameObject magicRing;
    Scene scene;

    void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    void OnTriggerEnter(Collider other)
    {
        if (scene.name == "Demo1Terrain 1")
        {
            if (other.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(2);
            }
        }
        if (scene.name == "Demo1 Terrain 2")
        {
            if (other.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(3);
            }
        }
    }
}
