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

    // ����ġ��
    public Slider PlayerExpBar;
    public TMP_Text LevelText;

    // ü�¹�
    public Slider PlayerHPBar;
    public Slider PlayerHPEffectBar;

    // ����ü��
    public TMP_Text CurHPText;

    // �Ͻ����� �г�
    public GameObject PausePannel;

    // ��ų ���� â
    public GameObject SkillSelect;

    // ���� ������ ȿ��
    public GameObject DmgEffect;

    // ���� �Ѿ�� ���̵� ��/�ƿ� ȿ��
    public Image FadeEffect;

    // ���� ü�¹�
    public Slider BossHPBar;
    public Slider BossHPEffectBar;

    // ������ �ִ� ��ų ����� ���� �̹��� �迭
    public Image[] GainedSkills;

    // ���� ���� �˾�
    public GameObject EndingPanel;
    
    // �������� ��ȣ
    public TMP_Text stageNum;

    // õ�� �˾�
    public GameObject AngelPanel;
}
