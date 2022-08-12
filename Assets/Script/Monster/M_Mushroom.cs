using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MushroomType
{
    RED, GREEN, BLUE
}


public class M_Mushroom : Monster
{
    [SerializeField] protected MushroomType myType;
    [SerializeField] GameObject myDivision = null; // ���� �� ������ ����

    protected override void IsAttack(Transform target)
    {
        if (myType != MushroomType.RED)
        {
            StartCoroutine(Attacking(target));
        }
    }

    IEnumerator Attacking(Transform target)
    {
        while (true)
        {
            nav.SetDestination(target.position);
            yield return null;
        }
    }

    override public void OnDamage(float damage)
    {
        if (myState == State.Die) return;

        myAnim.SetTrigger("damage"); // �ǰݽ� �ִϸ��̼� ȣ��

        // ������ ����Ʈ ����
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, myHPBar.transform.parent);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 2.0f, obj.transform.localPosition.z);

        if (damage > 9999.0f)
        {
            // ��弦�� ���
            obj.GetComponentInChildren<TMP_Text>().text = "Head Shot!!!";
            obj.GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0, 1); // ������ �۾��� ���� �� �絵�� ��
            obj.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold; // �۾�ü�� �β��� �ٲ�
            obj.GetComponentInChildren<TMP_Text>().fontSize *= 1.5f; // ��Ʈ ũ�⸦ 1.5��� �ٲ�
        }
        else
        {
            obj.GetComponentInChildren<TMP_Text>().text = "-" + (int)damage;
        }

        if (!UpdateHP(-damage))
        {
            Division(); // ���� ���ͷ� ������
            ChangeState(State.Die);
        }

        Invoke("HPEffect", 0.5f); // 0.5���Ŀ� hp�� �پ��� ȿ�� �߻�
    }

    protected void Division()
    {
        // Red->Green->Blue ������ ������        

        if (myType == MushroomType.BLUE) return;

        // �п� ���� ����
        GameObject mon1 = Instantiate(myDivision, this.transform.parent.parent); 
        GameObject mon2 = Instantiate(myDivision, this.transform.parent.parent);

        mon1.transform.position = mon2.transform.position = this.transform.position;
    }


}