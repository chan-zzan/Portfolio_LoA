using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEvent : MonoBehaviour
{
    int count = 0;

    private void OnParticleTrigger()
    {
        count++;

        if (count == 40)
        {
            count = 0;
            Player.Instance.OnDamage(10.0f);
        }
    }
}
