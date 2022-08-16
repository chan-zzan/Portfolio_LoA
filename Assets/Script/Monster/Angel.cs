using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 플레이어인 경우 천사 팝업창 나오게 설정
            UIManager.Instance.AngelPanel.SetActive(true);
            this.transform.parent.gameObject.SetActive(false);
        }
    }
}
