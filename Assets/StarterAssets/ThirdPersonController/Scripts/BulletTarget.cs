using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTarget : MonoBehaviour
{
     
    public TimeTravel timeTravel;
    public GameObject player;
    private void Awake()
    {
        timeTravel = player.GetComponent<TimeTravel>(); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyWorldOne" &&  timeTravel.isWorldOneActive)
        {
            Debug.Log("enemy 1 hit");
            // get component enemy health
            // decrease enemy health
        }
        if (other.tag == "EnemyWorldTwo" && timeTravel.isWorldTwoActive)
        {
            Debug.Log("enemy 2 hit");

            // get component enemy health
            // decrease enemy health
        }

    }
}
