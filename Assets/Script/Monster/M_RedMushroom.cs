using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class M_RedMushroom : M_Mushroom
{
    Transform AttackTarget; // 목표위치

    [SerializeField] Transform ShotPoint; // 투사체가 발사되는 지점
    [SerializeField] GameObject Projectile; // 투사체로 사용되는 오브젝트

    [SerializeField] Transform DamageParent;

    protected override void IsAttack(Transform target)
    {
        // 목표위치 설정
        AttackTarget = target;

        int skillProb = Random.Range(0, 100);

        if (skillProb < 50)
        {
            print("jump");
            // 50퍼 확률로 점프공격
            JumpAttack();
        }        
        else
        {
            print("follow");
            // 플레이어를 따라다니는 공격
            FollowAttack();
        }
    }

    void JumpAttack()
    {
        StartCoroutine(Jumping(AttackTarget));
    }

    IEnumerator Jumping(Transform target)
    {
        myAnim.SetBool("move", true); // 점프중에는 데미지 애니메이션 작동안함
        float timer = 0.0f;
        nav.isStopped = false;

        // 1초간 회전하면서 플레이어 타겟팅
        while (timer <= 1.0f)
        {
            timer += Time.deltaTime;

            // 회전
            Vector3 lookDir = (Player.Instance.transform.position - this.transform.position).normalized;

            Quaternion from = this.transform.rotation;
            Quaternion to = Quaternion.LookRotation(lookDir);
            this.transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 7.0f);

            yield return null;
        }

        nav.isStopped = true;

        // 이후 점프 하고 착지할때 전방향으로 투사체 발사
        myAnim.SetTrigger("jump");
        print("jump2");

        yield return new WaitForSeconds(2.0f);
        myAnim.SetBool("move", false);
        IsAttack(AttackTarget);
    }

    void JumpAnim()
    {        
        // TailPoint 위치에서 투사체 생성
        GameObject obj1 = Instantiate(Projectile, ShotPoint);
        GameObject obj2 = Instantiate(Projectile, ShotPoint);
        GameObject obj3 = Instantiate(Projectile, ShotPoint);
        GameObject obj4 = Instantiate(Projectile, ShotPoint);
        GameObject obj5 = Instantiate(Projectile, ShotPoint);
        GameObject obj6 = Instantiate(Projectile, ShotPoint);
        GameObject obj7 = Instantiate(Projectile, ShotPoint);
        GameObject obj8 = Instantiate(Projectile, ShotPoint);

        // 투사체가 TailPoint의 위치에 영향을 받지 않도록 부모를 변경하여 밖으로 빼줌
        obj1.transform.parent = this.transform.parent;
        obj2.transform.parent = this.transform.parent;
        obj3.transform.parent = this.transform.parent;
        obj4.transform.parent = this.transform.parent;
        obj5.transform.parent = this.transform.parent;
        obj6.transform.parent = this.transform.parent;
        obj7.transform.parent = this.transform.parent;
        obj8.transform.parent = this.transform.parent;

        // 투사체 발사
        obj1.GetComponent<Rigidbody>().velocity = this.transform.forward * 3.0f;
        obj2.GetComponent<Rigidbody>().velocity = (this.transform.forward + this.transform.right).normalized * 3.0f;
        obj3.GetComponent<Rigidbody>().velocity = (this.transform.forward - this.transform.right).normalized * 3.0f;
        obj4.GetComponent<Rigidbody>().velocity = this.transform.right * 3.0f;
        obj5.GetComponent<Rigidbody>().velocity = -this.transform.right * 3.0f;
        obj6.GetComponent<Rigidbody>().velocity = -this.transform.forward * 3.0f;
        obj7.GetComponent<Rigidbody>().velocity = (-this.transform.forward + this.transform.right).normalized * 3.0f;
        obj8.GetComponent<Rigidbody>().velocity = (-this.transform.forward - this.transform.right).normalized * 3.0f;
    }

    void FollowAttack()
    {
        StartCoroutine(Following(AttackTarget));
    }

    IEnumerator Following(Transform target)
    {
        float timer = 0.0f;
        nav.isStopped = false;

        while (timer <= 2.0f)
        {
            timer += Time.deltaTime;
            nav.SetDestination(target.position);
            yield return null;
        }

        nav.isStopped = true;

        // 다음번 스킬 발동
        yield return new WaitForSeconds(1.0f); 
        IsAttack(AttackTarget); 
    }

    override public void OnDamage(float damage)
    {
        // 부모의 함수와는 다르게 호출
        if (myState == State.Die) return;

        myAnim.SetTrigger("damage"); // 피격시 애니메이션 호출

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
            Division(); // 하위 몬스터로 나눠짐
            ChangeState(State.Die);

            // 보스 HP를 가리고 캐릭터 경험치바가 보이도록 설정
            UIManager.Instance.PlayerExpBar.transform.parent.gameObject.SetActive(true);
            UIManager.Instance.BossHPBar.transform.parent.gameObject.SetActive(false);
        }

        Invoke("HPEffect", 0.5f); // 0.5초후에 hp가 줄어드는 효과 발생
    }
}
