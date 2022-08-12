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
        // �÷��̾��� �ִ�ü�� ����
        //Player.Instance.UpdateMaxHP(data);

        // �ִ�ü�¿� ���� hpline�� scale ���� ������
        float scale_X = 1000 / Player.Instance.GetMaxHP();

        HPLines.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);

        foreach(Transform child in HPLines)
        {
            child.localScale = new Vector3(scale_X, 1, 1);
        }

        HPLines.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);

        // hp���� ����hp ���� ����
        //Player.Instance.UpdateHP();
    }
}
