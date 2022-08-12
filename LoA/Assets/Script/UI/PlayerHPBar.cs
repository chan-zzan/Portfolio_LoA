using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform HPLines;

    private void Start()
    {
        ChangeHPLine();
    }

    private void Update()
    {
        this.transform.position = player.position;
    }

    public void ChangeHPLine()
    {
        // 플레이어의 최대체력 증가
        //Player.Instance.UpdateMaxHP(data);

        // 최대체력에 따라 hpline의 scale 값을 정해줌
        float scale_X = 1000 / Player.Instance.GetMaxHP();

        HPLines.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);

        foreach(Transform child in HPLines)
        {
            child.localScale = new Vector3(scale_X, 1, 1);
        }

        HPLines.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);

        // hp바의 현재hp 비율 변경
        //Player.Instance.UpdateHP();
    }
}
