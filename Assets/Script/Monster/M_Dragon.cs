using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class M_Dragon : Monster
{
    Transform AttackTarget; // ��ǥ��ġ

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
            // ȸ��
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
        // �θ��� �Լ��ʹ� �ٸ��� ȣ��
        if (myState == State.Die) return;

        // ��ų�� ������� �ƴ� ��쿡�� �ִϸ��̼� ȣ��
        if (!myAnim.GetBool("skillActive")) 
        {
            myAnim.SetTrigger("damage"); // �ǰݽ� �ִϸ��̼� ȣ��
        }

        // ������ ����Ʈ ����
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, DamageParent);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 5.0f, obj.transform.localPosition.z);

        if (damage > 9999.0f)
        {
            // ��弦�� ��� -> ũ��Ƽ�÷� ó��
            damage = myData.GetATK() * 2; // ���ݷ��� �ι�� �������� ����
            obj.GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0, 1); // ������ �۾��� ���� �� �絵�� ��
            obj.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold; // �۾�ü�� �β��� �ٲ�
            obj.GetComponentInChildren<TMP_Text>().fontSize *= 1.5f; // ��Ʈ ũ�⸦ 1.5��� �ٲ�
        }

        obj.GetComponentInChildren<TMP_Text>().text = "-" + (int)damage;

        if (!UpdateHP(-damage))
        {
            ChangeState(State.Die);

            // ���� HP�� ������ ĳ���� ����ġ�ٰ� ���̵��� ����
            UIManager.Instance.PlayerExpBar.transform.parent.gameObject.SetActive(true);
            UIManager.Instance.BossHPBar.transform.parent.gameObject.SetActive(false);
        }

        Invoke("HPEffect", 0.5f); // 0.5���Ŀ� hp�� �پ��� ȿ�� �߻�
    }

    protected override void IsAttack(Transform target)
    {
        myAnim.SetBool("attack", true);

        // ��ǥ��ġ ����
        AttackTarget = target;

        int skillProb = Random.Range(0, 100);

        if (skillProb < 20)
        {
            // 20�� Ȯ���� �÷��̾�� ����
            DashAttack();
        }
        else if (skillProb < 50)
        {
            // 30�� Ȯ���� ȭ���߻�
            myAnim.SetTrigger("breath");
        }
        else
        {
            // 50�� Ȯ���� ����ü �߻�
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

        // TailPoint ��ġ���� ����ü ����
        GameObject obj1 = Instantiate(Projectile, TailPoint); 
        GameObject obj2 = Instantiate(Projectile, TailPoint); 
        GameObject obj3 = Instantiate(Projectile, TailPoint);

        // ����ü�� TailPoint�� ��ġ�� ������ ���� �ʵ��� �θ� �����Ͽ� ������ ����
        obj1.transform.parent = this.transform.parent; 
        obj2.transform.parent = this.transform.parent; 
        obj3.transform.parent = this.transform.parent;

        // ����ü �߻�
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
        yield return new WaitForSeconds(2.0f); // ���� �� ���

        Vector3 dest = AttackTarget.position; // �뽬�� ������

        // �뽬 �ӵ��� ����
        nav.speed = 1000.0f;
        nav.acceleration = 100.0f;

        rotate = false;

        nav.isStopped = false;

        while (!nav.isStopped)
        {
            // �뽬
            nav.SetDestination(dest);

            // �÷��̾�� �����Ÿ��̻� �ٰ����� �뽬 ��
            if (Vector3.Distance(this.transform.position, dest) <= 0.3f)
            {
                myAnim.SetBool("dash", false);
                nav.isStopped = true;
            }

            yield return null;
        }

        // �ٽ� ���� �ӵ��� ����
        nav.speed = 1.0f;
        nav.acceleration = 3.0f;

        yield return new WaitForSeconds(2.0f); // �÷��̾ �뽬 �� ���
        rotate = true;

        yield return new WaitForSeconds(1.0f);
        IsAttack(AttackTarget); // ������ ��ų �ߵ�
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
