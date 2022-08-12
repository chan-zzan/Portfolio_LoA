using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform door;
    [SerializeField] GameObject wayPoint;
    [SerializeField] GameObject clearLight;

    public void DoorAnimPlay()
    {
        // 스테이지 클리어시 문이 내려가면서 열림
        StartCoroutine(OpenTheDoor());
        clearLight.SetActive(true);
    }

    IEnumerator OpenTheDoor()
    {
        float height = 0.0f;
        door.transform.localPosition = new Vector3(0, height, 0);

        while (height > -2.1)
        {
            door.transform.localPosition = new Vector3(0, height, 0);
            height -= 0.03f;

            yield return null;
        }

        wayPoint.SetActive(true);
    }

    public void ReSetting()
    {
        // 처음상태로 다시 돌려놓음(보스방, 천사방 재사용할때 사용)
        door.transform.localPosition = new Vector3(0, 0, 0);
        wayPoint.SetActive(false);
        clearLight.SetActive(false);
    }
}
