using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Projectile : MonoBehaviour
{    
    [SerializeField] MonsterData myMonster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // �÷��̾ ����ü�� ���� ���
                Player.Instance.OnDamage(myMonster.GetATK());
            }

            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // �÷��̾ ����ü�� ���� ���
                Player.Instance.OnDamage(myMonster.GetATK());
            }

            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(this.gameObject);
        }
    }
}
