using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : Characteristic
{
    public Transform myRoot;
    public float MoveSpeed = 1.0f; // �÷��̾� �̵� �ӵ�

    private void OnAnimatorMove()
    {
        myRoot.GetComponent<Rigidbody>().MovePosition(myRoot.position + myAnim.deltaPosition * MoveSpeed); // �������� �̵��� �ϰ� ����
        myRoot.Rotate(myAnim.deltaRotation.eulerAngles);
    }
}
