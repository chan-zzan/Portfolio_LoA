using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region �̱���
    private static UIManager instance = null;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "UIManager";
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    // �÷��̾� ���� ����
    [Header("Player")]

    // ����ġ��
    public Slider PlayerExpBar;
    public TMP_Text PlayerLevelText;

    // ü�¹�
    public Slider PlayerHPBar;
    public Slider PlayerHPEffectBar;

    // ����ü��
    public TMP_Text CurHPText;


    // ���� ���� ����
    [Space(10.0f)]
    [Header("Monster")]

    // ���� ü�¹�
    public Slider BossHPBar;
    public Slider BossHPEffectBar;


    // ��ų ���� ����
    [Space(10.0f)]
    [Header("Skills")]

    // ��ų ���� â
    public GameObject SkillSelect;

    // ������ �ִ� ��ų ����� ���� �̹��� �迭
    public Image[] GainedSkills;


    // �˾�â
    [Space(10.0f)]
    [Header("Panels")]

    // �Ͻ����� �г�
    public GameObject PausePanel;

    // ���� ���� �˾�
    public GameObject EndingPanel;    

    // õ�� �˾�
    public GameObject AngelPanel;

    // �׿�
    [Space(10.0f)]
    [Header("etc")]

    // �������� ��ȣ
    public TMP_Text stageNum;


    // ����Ʈ ���
    [Space(10.0f)]
    [Header("Effects")]

    // ���� ������ ȿ��
    public GameObject DmgEffect;

    // ���� �Ѿ�� ���̵� ��/�ƿ� ȿ��
    public Image FadeEffect;


}
