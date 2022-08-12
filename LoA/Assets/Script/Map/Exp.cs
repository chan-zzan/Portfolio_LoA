using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public void ExpEffect()
    {
        // 자석처럼 경험치들을 플레이어 쪽으로 끌어당김
        StartCoroutine(onExpEffect());
    }

    IEnumerator onExpEffect()
    {
        yield return new WaitForSeconds(1.0f);

        float dist = 9999.9f;

        while (dist > 0.0f)
        {
            Vector3 dir = Player.Instance.transform.position - this.transform.position; // 플레이어까지의 방향
            dist = dir.magnitude; // 플레이어까지의 거리

            float delta = Time.deltaTime * 10.0f;

            this.transform.Translate(dir.normalized * delta, Space.World);

            delta = dist - delta > 0 ? delta : dist;

            dist -= delta;
            yield return null;
        }
    }
}
