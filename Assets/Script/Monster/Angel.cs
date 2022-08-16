using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // �÷��̾��� ��� õ�� �˾�â ������ ����
            UIManager.Instance.AngelPanel.SetActive(true);
            this.transform.parent.gameObject.SetActive(false);
        }
    }
}
