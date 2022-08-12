using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public void ExpEffect()
    {
        // �ڼ�ó�� ����ġ���� �÷��̾� ������ ������
        StartCoroutine(onExpEffect());
    }

    IEnumerator onExpEffect()
    {
        yield return new WaitForSeconds(1.0f);

        float dist = 9999.9f;

        while (dist > 0.0f)
        {
            Vector3 dir = Player.Instance.transform.position - this.transform.position; // �÷��̾������ ����
            dist = dir.magnitude; // �÷��̾������ �Ÿ�

            float delta = Time.deltaTime * 10.0f;

            this.transform.Translate(dir.normalized * delta, Space.World);

            delta = dist - delta > 0 ? delta : dist;

            dist -= delta;
            yield return null;
        }
    }
}
