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
        // �������� Ŭ����� ���� �������鼭 ����
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
        // ó�����·� �ٽ� ��������(������, õ��� �����Ҷ� ���)
        door.transform.localPosition = new Vector3(0, 0, 0);
        wayPoint.SetActive(false);
        clearLight.SetActive(false);
    }
}
