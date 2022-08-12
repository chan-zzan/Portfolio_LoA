using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Rabbit : Monster
{    
    protected override void IsAttack(Transform target)
    {
        StartCoroutine(Attacking(target));
    }

    IEnumerator Attacking(Transform target)
    {
        while (true)
        {
            nav.SetDestination(target.position);
            yield return null;
        }
    }
}
