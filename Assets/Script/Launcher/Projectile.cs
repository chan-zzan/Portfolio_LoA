using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void Start()
    {
        // 3�ʵڿ��� �����ִٸ� ����
        Destroy(this.gameObject, 3.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster") || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // ���Ϳ� ���� ���
            if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                if (Player.Instance.onHeadShot)
                {
                    // ��弦 ��ų�� Ȱ��ȭ �� ��� -> 10�ۼ�Ʈ Ȯ���� ���� ���                    
                    Player.Instance.HeadShot(other);
                }
                else
                {
                    // ��弦 ��ų�� Ȱ��ȭ ���� ���� ���
                    other.GetComponent<Monster>().OnDamage(Player.Instance.GetATK());
                }
            }

            // ���뼦�� Ȱ��ȭ �� ��� -> ������ ��� ������Ʈ ����x
            if (Player.Instance.onPierceShot)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    return;
                }
            }

            // ������Ʈ ���� �� ����       
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(this.gameObject, 0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster") || collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        { 
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(this.gameObject, 0.2f);
        }
    }
}
