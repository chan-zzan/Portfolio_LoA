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
        // 보스방은 카메라 이동x
        if (StageManager.Instance.GetStageIndex() % 10 != 0)
        {
            // 캐릭터가 앞뒤로 움직일 때 카메라도 같이 이동
            camPos.y = myCharacter.position.y + offsetY;
            camPos.z = myCharacter.position.z + offsetZ;

            this.transform.position = camPos;
        }
    }

    public void CameraMove_X()
    {
        // 플레이어의 x축 위치로 이동(스테이지 이동시)
        camPos.x = myCharacter.position.x;

        if (StageManager.Instance.GetStageIndex() % 10 == 0)
        {
            // 보스방 이동시
            camPos.y = myCharacter.position.y + offsetY;
            camPos.z = myCharacter.position.z;
            this.transform.position = camPos;
        }
    }
}
