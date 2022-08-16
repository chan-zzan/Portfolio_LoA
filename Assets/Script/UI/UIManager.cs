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

    // 플레이어 관련 정보
    [Header("Player")]

    // 경험치바
    public Slider PlayerExpBar;
    public TMP_Text PlayerLevelText;

    // 체력바
    public Slider PlayerHPBar;
    public Slider PlayerHPEffectBar;

    // 현재체력
    public TMP_Text CurHPText;


    // 몬스터 관련 정보
    [Space(10.0f)]
    [Header("Monster")]

    // 보스 체력바
    public Slider BossHPBar;
    public Slider BossHPEffectBar;


    // 스킬 관련 정보
    [Space(10.0f)]
    [Header("Skills")]

    // 스킬 선택 창
    public GameObject SkillSelect;

    // 가지고 있는 스킬 목록을 담을 이미지 배열
    public Image[] GainedSkills;


    // 팝업창
    [Space(10.0f)]
    [Header("Panels")]

    // 일시정지 패널
    public GameObject PausePanel;

    // 게임 종료 팝업
    public GameObject EndingPanel;    

    // 천사 팝업
    public GameObject AngelPanel;

    // 그외
    [Space(10.0f)]
    [Header("etc")]

    // 스테이지 번호
    public TMP_Text stageNum;


    // 이펙트 헤더
    [Space(10.0f)]
    [Header("Effects")]

    // 몬스터 데미지 효과
    public GameObject DmgEffect;

    // 던전 넘어갈때 페이드 인/아웃 효과
    public Image FadeEffect;


}
