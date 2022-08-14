using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    #region 싱글톤 // 어디서나 사용가능
    private static StageManager instance = null;

    public static StageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StageManager>();
                if (instance == null)
                {
                    GameObject obj = Instantiate(Resources.Load("StageManager")) as GameObject;
                    obj.name = typeof(StageManager).ToString();
                    instance = obj.GetComponent<StageManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }
    #endregion 

    [SerializeField]
    Transform player;

    [SerializeField]
    Transform StartStage;
    [SerializeField]
    List<Transform> GreenStage; // 1 ~ 10
    [SerializeField]
    List<Transform> BrownStage; // 1 ~ 10
    [SerializeField]
    List<Transform> AngelStage; // 1 ~ 4
    [SerializeField]
    List<Transform> BossStage; // 1 ~ 3

    //[SerializeField]
    int curStageIndex = 0;
    public int GetStageIndex() => curStageIndex;

    int lastStage = 20;

    //[SerializeField]
    int rndIndex; // 랜덤 방 번호

    [SerializeField]
    Transform CurStage;
    public Transform GetStage() => CurStage;

    //[SerializeField] 
    Door UsingDoor;

    public GameObject DropExp; // 드랍되는 경험치 오브젝트

    [SerializeField]
    GameObject LastBoss;

    [SerializeField]
    GameObject MiddleBoss;

    private void Start()
    {
        CurStage = StartStage;
        player.position = CurStage.position;
        StageClear(); // 시작방은 바로 클리어되서 문이 열리도록 함
    }

    public void StageClear()
    { 
        // 스테이지 클리어시 다음 스테이지로 가는 문이 열림
        UsingDoor = CurStage.GetComponentInChildren<Door>();
        UsingDoor.DoorAnimPlay();

        // 경험치 획득 이펙트 발생
        Exp[] allExp = CurStage.GetComponentsInChildren<Exp>();

        foreach (Exp exp in allExp)
        {
            exp.ExpEffect();
        }

    }

    public void NextStage()
    {
        if (curStageIndex % 5 != 0)
        {
            // 스테이지 변경시 사용중이던 문 사용해제(보스,천사방은 재사용하므로 사용해제x)
            UsingDoor.gameObject.SetActive(false);
            UsingDoor = null;
        }
        else
        {
            if (curStageIndex == lastStage)
            {
                // last Stage -> 게임 끝
            }

            UsingDoor.ReSetting();
            UsingDoor = null;
        }

        curStageIndex++;
        StartCoroutine(FadeEffect());

        if (curStageIndex % 5 == 0)
        {
            // 5로 나누어 떨어지는 스테이지 => 천사 or보스
            if (curStageIndex % 10 == 0)
            {
                // Boss Stage
                rndIndex = Random.Range(0, BossStage.Count);
                player.position = BossStage[rndIndex].position;

                // 캐릭터 경험치바를 가리고 보스 HP가 보이도록 설정
                UIManager.Instance.PlayerExpBar.transform.parent.gameObject.SetActive(false);
                UIManager.Instance.BossHPBar.transform.parent.gameObject.SetActive(true);

                if (curStageIndex == lastStage)
                {
                    // last Stage -> 보스생성
                    GameObject boss = Instantiate(LastBoss, BossStage[rndIndex]);
                    boss.transform.position = BossStage[rndIndex].position + new Vector3(0, 0, 5);
                }
                else
                {
                    // middle Boss
                    GameObject boss = Instantiate(MiddleBoss, BossStage[rndIndex]);
                    boss.transform.position = BossStage[rndIndex].position + new Vector3(0, 0, 5);
                }

                // 현재 스테이지 정보 저장
                CurStage = BossStage[rndIndex];
            }
            else
            {
                // Angel Stage
                rndIndex = Random.Range(0, AngelStage.Count);
                player.position = AngelStage[rndIndex].position;

                //// Angel Stage -> 천사생성
                //GameObject boss = Instantiate(Angel, AngelStage[rndIndex]);
                //boss.transform.position = AngelStage[rndIndex].position + new Vector3(0, 0, 0);

                // 현재 스테이지 정보 저장
                CurStage = AngelStage[rndIndex];

                // 천사방인 경우 문이 항상 열려있도록 해야함 -> 들어가자마자 클리어
                StageClear();
            }
        }
        else if (curStageIndex < 10)
        {
            // Green Stage
            rndIndex = Random.Range(0, GreenStage.Count);
            player.position = GreenStage[rndIndex].position;

            // 현재 스테이지 정보 저장
            CurStage = GreenStage[rndIndex];

            // 중복으로 스테이지가 나오지 않도록 리스트에서 삭제
            GreenStage.RemoveAt(rndIndex);
        }
        else if (curStageIndex < 20)
        {
            // Brown Stage
            rndIndex = Random.Range(0, BrownStage.Count);
            player.position = BrownStage[rndIndex].position;

            // 현재 스테이지 정보 저장
            CurStage = BrownStage[rndIndex];

            // 중복으로 스테이지가 나오지 않도록 리스트에서 삭제
            BrownStage.RemoveAt(rndIndex);
        }

        player.position = new Vector3(player.position.x, player.position.y, player.position.z - 4.0f);

        // 카메라 이동
        Camera.main.GetComponent<CameraMove>().CameraMove_X();

        // 현재 방 번호를 표시
        CurStage.GetComponentInChildren<TMP_Text>().text = "" + curStageIndex;

        // 현재 방의 몬스터들을 등장시킴
        CurStage.Find("MonsterSpawn").gameObject.SetActive(true);
        
    }

    IEnumerator FadeEffect()
    {
        float alpha = 1;

        UIManager.Instance.FadeEffect.color = new Color(1, 1, 1, alpha);
        yield return new WaitForSeconds(0.3f);        

        while (alpha >= 0)
        {
            UIManager.Instance.FadeEffect.color = new Color(1, 1, 1, alpha);
            alpha -= 0.02f;
            yield return null;
        }    
    }

}
