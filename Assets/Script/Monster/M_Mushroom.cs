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
    [SerializeField] GameObject myDivision = null; // 죽은 후 생성할 몬스터

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

        myAnim.SetTrigger("damage"); // 피격시 애니메이션 호출

        // 데미지 이펙트 생성
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, myHPBar.transform.parent);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 2.0f, obj.transform.localPosition.z);

        if (damage > 9999.0f)
        {
            // 헤드샷인 경우
            obj.GetComponentInChildren<TMP_Text>().text = "Head Shot!!!";
            obj.GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0, 1); // 빨간색 글씨로 눈에 잘 띄도록 함
            obj.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold; // 글씨체를 두껍게 바꿈
            obj.GetComponentInChildren<TMP_Text>().fontSize *= 1.5f; // 폰트 크기를 1.5배로 바꿈
        }
        else
        {
            obj.GetComponentInChildren<TMP_Text>().text = "-" + (int)damage;
        }

        if (!UpdateHP(-damage))
        {
            Division(); // 하위 몬스터로 나눠짐
            ChangeState(State.Die);
        }

        Invoke("HPEffect", 0.5f); // 0.5초후에 hp가 줄어드는 효과 발생
    }

    protected void Division()
    {
        // Red->Green->Blue 순으로 나눠짐        

        if (myType == MushroomType.BLUE) return;

        // 분열 몬스터 생성
        GameObject mon1 = Instantiate(myDivision, this.transform.parent.parent); 
        GameObject mon2 = Instantiate(myDivision, this.transform.parent.parent);

        mon1.transform.position = mon2.transform.position = this.transform.position;
    }


}