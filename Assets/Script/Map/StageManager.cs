using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    #region �̱��� // ��𼭳� ��밡��
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
    int rndIndex; // ���� �� ��ȣ

    [SerializeField]
    Transform CurStage;
    public Transform GetStage() => CurStage;

    //[SerializeField] 
    Door UsingDoor;

    public GameObject DropExp; // ����Ǵ� ����ġ ������Ʈ

    [SerializeField]
    GameObject LastBoss;

    [SerializeField]
    GameObject MiddleBoss;

    private void Start()
    {
        CurStage = StartStage;
        player.position = CurStage.position;
        StageClear(); // ���۹��� �ٷ� Ŭ����Ǽ� ���� �������� ��
    }

    public void StageClear()
    { 
        // �������� Ŭ����� ���� ���������� ���� ���� ����
        UsingDoor = CurStage.GetComponentInChildren<Door>();
        UsingDoor.DoorAnimPlay();

        // ����ġ ȹ�� ����Ʈ �߻�
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
            // �������� ����� ������̴� �� �������(����,õ����� �����ϹǷ� �������x)
            UsingDoor.gameObject.SetActive(false);
            UsingDoor = null;
        }
        else
        {
            if (curStageIndex == lastStage)
            {
                // last Stage -> ���� ��
            }

            UsingDoor.ReSetting();
            UsingDoor = null;
        }

        curStageIndex++;
        StartCoroutine(FadeEffect());

        if (curStageIndex % 5 == 0)
        {
            // 5�� ������ �������� �������� => õ�� or����
            if (curStageIndex % 10 == 0)
            {
                // Boss Stage
                rndIndex = Random.Range(0, BossStage.Count);
                player.position = BossStage[rndIndex].position;

                // ĳ���� ����ġ�ٸ� ������ ���� HP�� ���̵��� ����
                UIManager.Instance.PlayerExpBar.transform.parent.gameObject.SetActive(false);
                UIManager.Instance.BossHPBar.transform.parent.gameObject.SetActive(true);

                if (curStageIndex == lastStage)
                {
                    // last Stage -> ��������
                    GameObject boss = Instantiate(LastBoss, BossStage[rndIndex]);
                    boss.transform.position = BossStage[rndIndex].position + new Vector3(0, 0, 5);
                }
                else
                {
                    // middle Boss
                    GameObject boss = Instantiate(MiddleBoss, BossStage[rndIndex]);
                    boss.transform.position = BossStage[rndIndex].position + new Vector3(0, 0, 5);
                }

                // ���� �������� ���� ����
                CurStage = BossStage[rndIndex];
            }
            else
            {
                // Angel Stage
                rndIndex = Random.Range(0, AngelStage.Count);
                player.position = AngelStage[rndIndex].position;

                //// Angel Stage -> õ�����
                //GameObject boss = Instantiate(Angel, AngelStage[rndIndex]);
                //boss.transform.position = AngelStage[rndIndex].position + new Vector3(0, 0, 0);

                // ���� �������� ���� ����
                CurStage = AngelStage[rndIndex];

                // õ����� ��� ���� �׻� �����ֵ��� �ؾ��� -> ���ڸ��� Ŭ����
                StageClear();
            }
        }
        else if (curStageIndex < 10)
        {
            // Green Stage
            rndIndex = Random.Range(0, GreenStage.Count);
            player.position = GreenStage[rndIndex].position;

            // ���� �������� ���� ����
            CurStage = GreenStage[rndIndex];

            // �ߺ����� ���������� ������ �ʵ��� ����Ʈ���� ����
            GreenStage.RemoveAt(rndIndex);
        }
        else if (curStageIndex < 20)
        {
            // Brown Stage
            rndIndex = Random.Range(0, BrownStage.Count);
            player.position = BrownStage[rndIndex].position;

            // ���� �������� ���� ����
            CurStage = BrownStage[rndIndex];

            // �ߺ����� ���������� ������ �ʵ��� ����Ʈ���� ����
            BrownStage.RemoveAt(rndIndex);
        }

        player.position = new Vector3(player.position.x, player.position.y, player.position.z - 4.0f);

        // ī�޶� �̵�
        Camera.main.GetComponent<CameraMove>().CameraMove_X();

        // ���� �� ��ȣ�� ǥ��
        CurStage.GetComponentInChildren<TMP_Text>().text = "" + curStageIndex;

        // ���� ���� ���͵��� �����Ŵ
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
