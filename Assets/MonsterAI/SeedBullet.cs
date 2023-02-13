using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBullet : MonoBehaviour
{
    [SerializeField]
    PlayerStats PlayerFunc;


    void Start()
    {
        PlayerFunc = GameObject.Find("Player/Main Character").GetComponent<PlayerStats>();
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerFunc.LoseHP(10);
        }
        Destroy(gameObject);
    }
}
