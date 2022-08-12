using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform myCharacter;

    Vector3 camPos;
    public float offsetY = 10.0f;
    public float offsetZ = -3.0f;

    private void LateUpdate()
    {
        // �������� ī�޶� �̵�x
        if (StageManager.Instance.GetStageIndex() % 10 != 0)
        {
            // ĳ���Ͱ� �յڷ� ������ �� ī�޶� ���� �̵�
            camPos.y = myCharacter.position.y + offsetY;
            camPos.z = myCharacter.position.z + offsetZ;

            this.transform.position = camPos;
        }
    }

    public void CameraMove_X()
    {
        // �÷��̾��� x�� ��ġ�� �̵�(�������� �̵���)
        camPos.x = myCharacter.position.x;

        if (StageManager.Instance.GetStageIndex() % 10 == 0)
        {
            // ������ �̵���
            camPos.y = myCharacter.position.y + offsetY;
            camPos.z = myCharacter.position.z;
            this.transform.position = camPos;
        }
    }
}
