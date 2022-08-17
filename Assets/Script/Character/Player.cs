using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
struct PlayerStat
{
    public int Life; // 목숨

    public int Lv; // 레벨
    public float Exp; // 경험치

    public float ATK; // 공격력
    public float MaxHP; // 최대체력
    public float ATK_SPEED; // 공격속도
    public float MOVE_SPEED; // 이동속도
}

public enum State
{
    Create, Move, Idle, Attack, GameOver
}

public class Player : Characteristic
{
    #region 싱글톤
    private static Player instance = null;

    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "Player";
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<Player>();
                }
            }
            return instance;
        }
    }
    #endregion

    public State myState = State.Create;
    public CharacterMove CharacterMove;

    [SerializeField] PlayerAnimEvent animEvent; // 애니메이션 효과
    [SerializeField] PlayerStat myStat; // 캐릭터 스탯
    [SerializeField] GameObject[] Launchers; // 발사대 배열

    public bool onHeadShot = false; // 헤드샷 스킬 on/off
    public bool onPierceShot = false; // 관통샷 스킬 on/off
    public bool onRage = false; // 분노 스킬 on/off
    public bool onFury = false; // 격노 스킬 on/off

    int MaxLv = 5; // 최대 레벨
    float? CurHP = null; // 현재 체력
    float? CurExp = null; // 현재 경험치량

    float baseATK; // 분노 활성화시 저장되는 기본 공격력
    float baseATK_SPEED; // 격노 활성화시 저장되는 기본 공격속도

    bool OnHPEffect = false;

    public int GetLife() => myStat.Life;
    public int GetLv() => myStat.Lv;
    public float GetExp() => myStat.Exp;
    public float GetATK() => myStat.ATK;
    public float GetMaxHP() => myStat.MaxHP;
    public float GetATK_SPEED() => myStat.ATK_SPEED;
    public float GetMOVE_SPEED() => myStat.MOVE_SPEED;


    public void UpdateLife(int data) => myStat.Life += data;

    public void UpdateLv(int data)
    {
        myStat.Lv += data; // 레벨 상승

        UpdateMaxExp(GetExp() * 0.3f); // 얻어야 하는 경험치량 증가
        UpdateMaxHP(GetMaxHP() * 0.2f); // 최대 hp 증가

        if (GetLv() == MaxLv)
        {
            print("최대레벨 도달");
            UIManager.Instance.PlayerLevelText.text = "Lv. Max"; // 레벨 표시
        }
        else
        {
            UIManager.Instance.PlayerLevelText.text = "Lv. " + GetLv(); // 레벨 표시
        }

        UIManager.Instance.SkillSelect.SetActive(true); // 레벨업시 스킬 획득

        // 레벨업 텍스트 이펙트
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, UIManager.Instance.PlayerHPBar.transform.parent);
        obj.GetComponentInChildren<TMP_Text>().color = new Color(0, 1, 1f, 1); // 청록색 글씨로 표현
        obj.GetComponentInChildren<TMP_Text>().fontSize = 150.0f; // 폰트 크기 변경
        obj.GetComponentInChildren<TMP_Text>().text = "Level UP!!";

        // 레벨업 이펙트
        this.GetComponentInChildren<ParticleSystem>().Play();
    }

    public void UpdateATK(float data) => myStat.ATK += data;

    public void UpdateATK_SPEED(float data)
    {
        myStat.ATK_SPEED += data;
        myAnim.SetFloat("AttackSpeed", myStat.ATK_SPEED);
    }

    public void UpdateMOVE_SPEED(float data) 
    { 
        myStat.MOVE_SPEED += data;
        CharacterMove.MoveSpeed = myStat.MOVE_SPEED;
    }

    void UpdateMaxExp(float data)
    {
        myStat.Exp += data;
    }

    public void UpdateExp(float data = 0)
    {
        // 최대레벨인 경우 경험치 획득x
        if (GetLv() == MaxLv)
        {
            return;
        }

        if (CurExp == null)
        {
            // 처음 시작하는 시점에 동작
            CurExp = 0.0f;
            UIManager.Instance.PlayerLevelText.text = "Lv. " + GetLv();
            UIManager.Instance.PlayerExpBar.value = CurExp.Value / GetExp();

            UIManager.Instance.SkillSelect.SetActive(true); // 처음 시작할때 스킬 획득
        }

        CurExp += data;

        // 레벨업
        if (CurExp >= GetExp())
        {          
            CurExp -= GetExp();
            UpdateLv(1);
        }
    }

    public bool UpdateHP(float data = 0)
    {
        if (CurHP == null)
        {
            // 처음 시작한 경우
            CurHP = GetMaxHP();
        }

        if (data != 0)
        {
            // HP 이펙트 생성
            GameObject obj = Instantiate(UIManager.Instance.DmgEffect, UIManager.Instance.PlayerHPBar.transform.parent);
            obj.GetComponentInChildren<TMP_Text>().text = "";

            if (data > 0)
            {
                // hp 회복
                obj.GetComponentInChildren<TMP_Text>().color = new Color(0, 1, 0, 1); // 초록색 글씨로 회복 표현
                obj.GetComponentInChildren<TMP_Text>().text += "+";
            }

            obj.GetComponentInChildren<TMP_Text>().text += (int)data;
        }

        CurHP += data;

        if (CurHP >= GetMaxHP())
        {
            // 풀피인 경우
            CurHP = GetMaxHP();
        }
        else if (CurHP <= 0.0f)
        {
            // 죽은 경우
            CurHP = 0.0f;
            UpdateLife(-1);
            UIManager.Instance.PlayerHPBar.value = 0.0f;

            if (myStat.Life <= 0)
            {                
                return false;
            }
            else
            {
                // 부활
                print("부활");
                CurHP = null;
                UpdateHP();
            }
        }

        // 분노 스킬이 활성화 된 경우
        if (onRage)
        {
            // 잃은 체력 1퍼당 공격력 1퍼 증가
            myStat.ATK = baseATK + baseATK * (1.0f - (float)CurHP / GetMaxHP());
        }

        // 격노 스킬이 활성화 된 경우
        if (onFury)
        {
            // 잃은 체력 1퍼당 공속 0.4퍼 증가
            myStat.ATK_SPEED = baseATK_SPEED + baseATK_SPEED * (1.0f - (float)CurHP / GetMaxHP());
        }

        UIManager.Instance.PlayerHPBar.value = (float)CurHP / GetMaxHP(); // 현재 HP바 UI 표시
        UIManager.Instance.CurHPText.text = "" + (int)CurHP.Value; // 현재HP UI 표시

        return true;
    }

    public void UpdateMaxHP(float data)
    {
        float beforeMaxHp = myStat.MaxHP;

        myStat.MaxHP += data;

        if (myStat.MaxHP <= 0.0f)
        {
            // 0보다 maxhp가 작아지면 1로 고정
            myStat.MaxHP = 1.0f;
        }

        if (myStat.MaxHP >= 4000.0f)
        {
            // 40000보다 maxhp가 커지면 4000으로 고정
            myStat.MaxHP = 4000.0f;
        }

        // hpline 변경
        UIManager.Instance.PlayerHPBar.gameObject.GetComponentInParent<PlayerHPBar>().ChangeHPLine(); 

        if (CurHP != null)
        {
            // 최대체력이 증가한만큼 비율에 맞게 체력을 회복 // CurHP = CurHP * (GetMaxHP() / beforeMaxHp) => CurHP = CurHP + [- CurHP + CurHP * (GetMaxHP() / beforeMaxHp)]
            UpdateHP((float)CurHP * (GetMaxHP() / beforeMaxHp - 1.0f));
        }
        else
        {
            UpdateHP();
        }
    }

    public void OnDamage(float damage)
    {
        myAnim.SetTrigger("damage");        

        if (!UpdateHP(-damage))
        {
            print("게임오버");
            ChangeState(State.GameOver);
        }

        Invoke("HPEffect", 0.5f); // 0.5초후에 hp가 줄어드는 효과 발생
    }

    void HPEffect()
    {
        OnHPEffect = true;
    }

    private void Awake()
    {
        myAnim.SetFloat("AttackSpeed", myStat.ATK_SPEED);
    }

    private void Start()
    {
        UpdateExp();
        UpdateHP();
        ChangeState(State.Move);
    }

    private void FixedUpdate()
    {
        StateProcess();

        // UI에 체력바 표시
        UIManager.Instance.PlayerHPBar.value = Mathf.Lerp(UIManager.Instance.PlayerHPBar.value, CurHP.Value / GetMaxHP(), Time.unscaledDeltaTime * 5.0f); // hp가 부드럽게 줄어들거나 늘어나도록 변경

        if (OnHPEffect)
        {
            // hp효과
            UIManager.Instance.PlayerHPEffectBar.value = Mathf.Lerp(UIManager.Instance.PlayerHPEffectBar.value, UIManager.Instance.PlayerHPBar.value, Time.unscaledDeltaTime * 5.0f);

            if (UIManager.Instance.PlayerHPEffectBar.value - 0.01f <= UIManager.Instance.PlayerHPBar.value)
            {
                OnHPEffect = false;
                UIManager.Instance.PlayerHPEffectBar.value = UIManager.Instance.PlayerHPBar.value;
            }
        }

        if (GetLv() == MaxLv)
        {   
            // 최대레벨 도달시 바를 꽉 채움
            UIManager.Instance.PlayerExpBar.value = 1;
        }
        else
        {
            // 현재 경험치 획득량 UI 표시
            UIManager.Instance.PlayerExpBar.value = Mathf.Lerp(UIManager.Instance.PlayerExpBar.value, CurExp.Value / GetExp(), Time.unscaledDeltaTime * 5.0f); // exp가 부드럽게 줄어들거나 늘어나도록 변경
        }
    }

    public void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case State.Create:
                break;
            case State.Move:
                //print("PlayerState : Move");
                break;
            case State.Idle:
                //print("PlayerState : Idle");
                myAnim.SetBool("Attack", false);
                break;
            case State.Attack:
                //print("PlayerState : Attack");
                break;
            case State.GameOver:
                //print("PlayerState : GameOver");
                myAnim.SetBool("die", true);
                UIManager.Instance.EndingPanel.SetActive(true);
                UIManager.Instance.stageNum.text = "" +  StageManager.Instance.GetStageIndex();
                GameManager.Instance.PauseScene();
                break;
        }
    }

    void StateProcess()
    {
        switch (myState)
        {
            case State.Create:
                break;
            case State.Move:
                CharacterMove.PlayerMove();
                break;
            case State.Idle:
                break;
            case State.Attack:
                CharacterMove.FindMonster();
                break;
            case State.GameOver:
                break;
        }
    }

    public void ActiveLauncher(LauncherType type)
    {
        Launchers[(int)type].SetActive(true);
    }

    public void DoubleShot()
    {
        // 0.5초 뒤에 투사체가 가용중인 발사대에서 한번 더 발사되도록 하는 함수 추가
        animEvent.Attack += () => Invoke("shot", 0.3f);
    }
        
    public void TripleShot()
    {
        print("triple shot");
        animEvent.Attack += () => Invoke("shot", 0.6f);
    }

    void shot()
    {
        // 발사대가 사용중인 경우에만 발사
        foreach (GameObject launcher in Launchers)
        {
            if (launcher.activeSelf)
            {
                launcher.GetComponent<Launcher>().Shot();
            }
        }
    }

    public void MaxLvUp(int data)
    {
        MaxLv += data;

        UIManager.Instance.PlayerLevelText.text = "Lv. " + GetLv(); // 레벨 표시
    }

    public void SaveATK()
    {
        baseATK = GetATK();
    }

    public void SaveATK_SPEED()
    {
        baseATK_SPEED = GetATK_SPEED();
    }

    public void HeadShot(Collider other)
    {
        float damage;

        // 헤드샷 -> 10퍼센트 확률로 몬스터 즉사
        if (UnityEngine.Random.Range(1, 11) % 10 == 4)
        {
            print("head shot!");
            damage = 9999.9f;
        }
        else
        {
            damage = Player.Instance.GetATK();
        }

        other.GetComponent<Monster>().OnDamage(damage);
    }

}
