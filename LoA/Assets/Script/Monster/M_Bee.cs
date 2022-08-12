using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Bee : Monster
{
    public Transform Launcher;
    public GameObject Projectile; // 독침

    Transform Player;
    Vector3 lookDir;

    private void Update()
    {
        if (this.myState == State.Attacking && Player != null)
        {
            // 회전
            lookDir = (Player.position - this.transform.position).normalized;

            Quaternion from = this.transform.rotation;
            Quaternion to = Quaternion.LookRotation(lookDir);
            this.transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 10.0f);
        }
    }

    protected override void IsAttack(Transform target)
    {
        Player = target;
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.5f); // 몬스터가 Attacking 상태가 된후 방향을 잡을 때 까지 조금 대기
        myAnim.SetBool("attack", true);
    }

    void OnAttack() // 애니메이션이 동작할때 호출
    {
        GameObject obj = Instantiate(Projectile, Launcher); // Launcher 위치에서 독침 생성

        obj.transform.parent = this.transform.parent; // Launcher의 위치에 영향을 받지 않도록 부모를 변경하여 밖으로 빼줌

        obj.GetComponent<Rigidbody>().velocity = this.transform.forward * 5.0f; // 독침 발사
    }
}
