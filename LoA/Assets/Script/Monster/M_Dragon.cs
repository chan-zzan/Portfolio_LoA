using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class M_Dragon : Monster
{
    Transform AttackTarget; // 목표위치

    [SerializeField] Transform TailPoint;
    [SerializeField] GameObject Projectile;

    [SerializeField] ParticleSystem BreathEffect;
    [SerializeField] Transform DamageParent;

    Vector3 lookDir;
    bool rotate = false;

    private void Update()
    {
        if (this.myState == State.Attacking)
        {
            // 회전
            if (rotate && AttackTarget != null)
            {
                lookDir = (Player.Instance.transform.position - this.transform.position).normalized;

                Quaternion from = this.transform.rotation;
                Quaternion to = Quaternion.LookRotation(lookDir);
                this.transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 7.0f);
            }
        }
    }

    override public void OnDamage(float damage)
    {
        // 부모의 함수와는 다르게 호출
        if (myState == State.Die) return;

        // 스킬이 사용중이 아닐 경우에만 애니메이션 호출
        if (!myAnim.GetBool("skillActive")) 
        {
            myAnim.SetTrigger("damage"); // 피격시 애니메이션 호출
        }

        // 데미지 이펙트 생성
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, DamageParent);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 5.0f, obj.transform.localPosition.z);

        if (damage > 9999.0f)
        {
            // 헤드샷인 경우 -> 크리티컬로 처리
            damage = myData.GetATK() * 2; // 공격력의 두배로 데미지를 입힘
            obj.GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0, 1); // 빨간색 글씨로 눈에 잘 띄도록 함
            obj.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold; // 글씨체를 두껍게 바꿈
            obj.GetComponentInChildren<TMP_Text>().fontSize *= 1.5f; // 폰트 크기를 1.5배로 바꿈
        }

        obj.GetComponentInChildren<TMP_Text>().text = "-" + (int)damage;

        if (!UpdateHP(-damage))
        {
            ChangeState(State.Die);

            // 보스 HP를 가리고 캐릭터 경험치바가 보이도록 설정
            UIManager.Instance.PlayerExpBar.transform.parent.gameObject.SetActive(true);
            UIManager.Instance.BossHPBar.transform.parent.gameObject.SetActive(false);
        }

        Invoke("HPEffect", 0.5f); // 0.5초후에 hp가 줄어드는 효과 발생
    }

    protected override void IsAttack(Transform target)
    {
        myAnim.SetBool("attack", true);

        // 목표위치 설정
        AttackTarget = target;

        int skillProb = Random.Range(0, 100);

        if (skillProb < 20)
        {
            // 20퍼 확률로 플레이어에게 돌진
            DashAttack();
        }
        else if (skillProb < 50)
        {
            // 30퍼 확률로 화염발사
            myAnim.SetTrigger("breath");
        }
        else
        {
            // 50퍼 확률로 투사체 발사
            myAnim.SetTrigger("tailshot");
        }
    }

    void BreathAttack()
    {
        print("BreathAttack");

        BreathEffect.Play();
        rotate = false;
        AttackDelay();
    }

    void TailAttack()
    {
        print("TailAttack");

        // TailPoint 위치에서 투사체 생성
        GameObject obj1 = Instantiate(Projectile, TailPoint); 
        GameObject obj2 = Instantiate(Projectile, TailPoint); 
        GameObject obj3 = Instantiate(Projectile, TailPoint);

        // 투사체가 TailPoint의 위치에 영향을 받지 않도록 부모를 변경하여 밖으로 빼줌
        obj1.transform.parent = this.transform.parent; 
        obj2.transform.parent = this.transform.parent; 
        obj3.transform.parent = this.transform.parent;

        // 투사체 발사
        obj1.GetComponent<Rigidbody>().velocity = this.transform.forward * 5.0f;   
        obj2.GetComponent<Rigidbody>().velocity = (this.transform.forward + this.transform.right).normalized * 5.0f;   
        obj3.GetComponent<Rigidbody>().velocity = (this.transform.forward - this.transform.right).normalized * 5.0f;

        AttackDelay();
    }

    void DashAttack()
    {
        print("DashAttack");

        myAnim.SetBool("dash", true);

        StartCoroutine(dash());
    }

    IEnumerator dash()
    {
        yield return new WaitForSeconds(2.0f); // 공격 전 대기

        Vector3 dest = AttackTarget.position; // 대쉬할 목적지

        // 대쉬 속도로 변경
        nav.speed = 1000.0f;
        nav.acceleration = 100.0f;

        rotate = false;

        nav.isStopped = false;

        while (!nav.isStopped)
        {
            // 대쉬
            nav.SetDestination(dest);

            // 플레이어에게 일정거리이상 다가오면 대쉬 끝
            if (Vector3.Distance(this.transform.position, dest) <= 0.3f)
            {
                myAnim.SetBool("dash", false);
                nav.isStopped = true;
            }

            yield return null;
        }

        // 다시 원래 속도로 변경
        nav.speed = 1.0f;
        nav.acceleration = 3.0f;

        yield return new WaitForSeconds(2.0f); // 플레이어에 대쉬 후 대기
        rotate = true;

        yield return new WaitForSeconds(1.0f);
        IsAttack(AttackTarget); // 다음번 스킬 발동
    }

    void AttackDelay()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2.0f);
        rotate = true; 

        IsAttack(AttackTarget);
    }
}
