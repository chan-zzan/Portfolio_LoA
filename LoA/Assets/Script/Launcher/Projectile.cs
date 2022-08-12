using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void Start()
    {
        // 3초뒤에도 남아있다면 삭제
        Destroy(this.gameObject, 3.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster") || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // 몬스터에 맞을 경우
            if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                if (Player.Instance.onHeadShot)
                {
                    // 헤드샷 스킬이 활성화 된 경우 -> 10퍼센트 확률로 몬스터 즉사                    
                    Player.Instance.HeadShot(other);
                }
                else
                {
                    // 헤드샷 스킬이 활성화 되지 않은 경우
                    other.GetComponent<Monster>().OnDamage(Player.Instance.GetATK());
                }
            }

            // 관통샷이 활성화 된 경우 -> 몬스터인 경우 오브젝트 삭제x
            if (Player.Instance.onPierceShot)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    return;
                }
            }

            // 오브젝트 멈춤 및 삭제       
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
