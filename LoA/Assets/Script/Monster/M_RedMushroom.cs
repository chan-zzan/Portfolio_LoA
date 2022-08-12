using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class M_RedMushroom : M_Mushroom
{
    Transform AttackTarget; // ��ǥ��ġ

    [SerializeField] Transform ShotPoint; // ����ü�� �߻�Ǵ� ����
    [SerializeField] GameObject Projectile; // ����ü�� ���Ǵ� ������Ʈ

    [SerializeField] Transform DamageParent;

    protected override void IsAttack(Transform target)
    {
        // ��ǥ��ġ ����
        AttackTarget = target;

        int skillProb = Random.Range(0, 100);

        if (skillProb < 50)
        {
            print("jump");
            // 50�� Ȯ���� ��������
            JumpAttack();
        }        
        else
        {
            print("follow");
            // �÷��̾ ����ٴϴ� ����
            FollowAttack();
        }
    }

    void JumpAttack()
    {
        StartCoroutine(Jumping(AttackTarget));
    }

    IEnumerator Jumping(Transform target)
    {
        myAnim.SetBool("move", true); // �����߿��� ������ �ִϸ��̼� �۵�����
        float timer = 0.0f;
        nav.isStopped = false;

        // 1�ʰ� ȸ���ϸ鼭 �÷��̾� Ÿ����
        while (timer <= 1.0f)
        {
            timer += Time.deltaTime;

            // ȸ��
            Vector3 lookDir = (Player.Instance.transform.position - this.transform.position).normalized;

            Quaternion from = this.transform.rotation;
            Quaternion to = Quaternion.LookRotation(lookDir);
            this.transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 7.0f);

            yield return null;
        }

        nav.isStopped = true;

        // ���� ���� �ϰ� �����Ҷ� ���������� ����ü �߻�
        myAnim.SetTrigger("jump");
        print("jump2");

        yield return new WaitForSeconds(2.0f);
        myAnim.SetBool("move", false);
        IsAttack(AttackTarget);
    }

    void JumpAnim()
    {        
        // TailPoint ��ġ���� ����ü ����
        GameObject obj1 = Instantiate(Projectile, ShotPoint);
        GameObject obj2 = Instantiate(Projectile, ShotPoint);
        GameObject obj3 = Instantiate(Projectile, ShotPoint);
        GameObject obj4 = Instantiate(Projectile, ShotPoint);
        GameObject obj5 = Instantiate(Projectile, ShotPoint);
        GameObject obj6 = Instantiate(Projectile, ShotPoint);
        GameObject obj7 = Instantiate(Projectile, ShotPoint);
        GameObject obj8 = Instantiate(Projectile, ShotPoint);

        // ����ü�� TailPoint�� ��ġ�� ������ ���� �ʵ��� �θ� �����Ͽ� ������ ����
        obj1.transform.parent = this.transform.parent;
        obj2.transform.parent = this.transform.parent;
        obj3.transform.parent = this.transform.parent;
        obj4.transform.parent = this.transform.parent;
        obj5.transform.parent = this.transform.parent;
        obj6.transform.parent = this.transform.parent;
        obj7.transform.parent = this.transform.parent;
        obj8.transform.parent = this.transform.parent;

        // ����ü �߻�
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

        // ������ ��ų �ߵ�
        yield return new WaitForSeconds(1.0f); 
        IsAttack(AttackTarget); 
    }

    override public void OnDamage(float damage)
    {
        // �θ��� �Լ��ʹ� �ٸ��� ȣ��
        if (myState == State.Die) return;

        myAnim.SetTrigger("damage"); // �ǰݽ� �ִϸ��̼� ȣ��

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
            Division(); // ���� ���ͷ� ������
            ChangeState(State.Die);

            // ���� HP�� ������ ĳ���� ����ġ�ٰ� ���̵��� ����
            UIManager.Instance.PlayerExpBar.transform.parent.gameObject.SetActive(true);
            UIManager.Instance.BossHPBar.transform.parent.gameObject.SetActive(false);
        }

        Invoke("HPEffect", 0.5f); // 0.5���Ŀ� hp�� �پ��� ȿ�� �߻�
    }
}
