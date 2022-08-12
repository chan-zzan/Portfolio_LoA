using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMonster : MonoBehaviour
{
    public List<Transform> monsters;

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.NameToLayer("Monster") == other.gameObject.layer)
        {
            foreach (Transform tr in monsters)
            {
                if (other.transform == tr)
                {
                    print("ม฿บน");
                    return;
                }
            }
            monsters.Add(other.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.NameToLayer("Monster") == collision.gameObject.layer)
        {
            monsters.Add(collision.transform);
        }
    }
}
