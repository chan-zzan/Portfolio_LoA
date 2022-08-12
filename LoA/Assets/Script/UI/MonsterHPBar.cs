using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHPBar : MonoBehaviour
{
    Transform monster;

    private void Awake()
    {
        monster = this.transform.parent.GetComponentInChildren<Monster>().transform;
    }

    private void Update()
    {
        this.transform.position = monster.position;
    }
}
