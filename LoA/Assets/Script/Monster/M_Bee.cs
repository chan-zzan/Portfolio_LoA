using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Bee : Monster
{
    public Transform Launcher;
    public GameObject Projectile; // ��ħ

    Transform Player;
    Vector3 lookDir;

    private void Update()
    {
        if (this.myState == State.Attacking && Player != null)
        {
            // ȸ��
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
        yield return new WaitForSeconds(0.5f); // ���Ͱ� Attacking ���°� ���� ������ ���� �� ���� ���� ���
        myAnim.SetBool("attack", true);
    }

    void OnAttack() // �ִϸ��̼��� �����Ҷ� ȣ��
    {
        GameObject obj = Instantiate(Projectile, Launcher); // Launcher ��ġ���� ��ħ ����

        obj.transform.parent = this.transform.parent; // Launcher�� ��ġ�� ������ ���� �ʵ��� �θ� �����Ͽ� ������ ����

        obj.GetComponent<Rigidbody>().velocity = this.transform.forward * 5.0f; // ��ħ �߻�
    }
}
