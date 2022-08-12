using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : Characteristic
{
    public Transform myRoot;
    public float MoveSpeed = 1.0f; // 플레이어 이동 속도

    private void OnAnimatorMove()
    {
        myRoot.GetComponent<Rigidbody>().MovePosition(myRoot.position + myAnim.deltaPosition * MoveSpeed); // 물리적인 이동을 하게 해줌
        myRoot.Rotate(myAnim.deltaRotation.eulerAngles);
    }
}
