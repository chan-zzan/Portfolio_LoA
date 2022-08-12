using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region 싱글톤
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

    // 경험치바
    public Slider PlayerExpBar;
    public TMP_Text LevelText;

    // 체력바
    public Slider PlayerHPBar;
    public Slider PlayerHPEffectBar;

    // 현재체력
    public TMP_Text CurHPText;

    // 일시정지 패널
    public GameObject PausePannel;

    // 스킬 선택 창
    public GameObject SkillSelect;

    // 몬스터 데미지 효과
    public GameObject DmgEffect;

    // 던전 넘어갈때 페이드 인/아웃 효과
    public Image FadeEffect;

    // 보스 체력바
    public Slider BossHPBar;
    public Slider BossHPEffectBar;

    // 가지고 있는 스킬 목록을 담을 이미지 배열
    public Image[] GainedSkills;

    // 게임 종료 팝업
    public GameObject EndingPanel;
    
    // 스테이지 번호
    public TMP_Text stageNum;

    // 천사 팝업
    public GameObject AngelPanel;
}
