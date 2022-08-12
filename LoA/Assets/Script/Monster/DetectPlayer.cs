using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public Transform player;

    Monster parentMonster;

    private void Awake()
    {
        parentMonster = this.transform.parent.GetComponent<Monster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player = other.gameObject.transform;
            parentMonster.FindTarget(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            parentMonster.StartRoaming();            
            print("roaming restart");
        }
    }
}
