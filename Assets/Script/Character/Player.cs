using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
struct PlayerStat
{
    public int Life; // ���

    public int Lv; // ����
    public float Exp; // ����ġ

    public float ATK; // ���ݷ�
    public float MaxHP; // �ִ�ü��
    public float ATK_SPEED; // ���ݼӵ�
    public float MOVE_SPEED; // �̵��ӵ�
}

public enum State
{
    Create, Move, Idle, Attack, GameOver
}

public class Player : Characteristic
{
    #region �̱���
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

    [SerializeField] PlayerAnimEvent animEvent; // �ִϸ��̼� ȿ��
    [SerializeField] PlayerStat myStat; // ĳ���� ����
    [SerializeField] GameObject[] Launchers; // �߻�� �迭

    public bool onHeadShot = false; // ��弦 ��ų on/off
    public bool onPierceShot = false; // ���뼦 ��ų on/off
    public bool onRage = false; // �г� ��ų on/off
    public bool onFury = false; // �ݳ� ��ų on/off

    int MaxLv = 5; // �ִ� ����
    float? CurHP = null; // ���� ü��
    float? CurExp = null; // ���� ����ġ��

    float baseATK; // �г� Ȱ��ȭ�� ����Ǵ� �⺻ ���ݷ�
    float baseATK_SPEED; // �ݳ� Ȱ��ȭ�� ����Ǵ� �⺻ ���ݼӵ�

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
        myStat.Lv += data; // ���� ���

        UpdateMaxExp(GetExp() * 0.3f); // ���� �ϴ� ����ġ�� ����
        UpdateMaxHP(GetMaxHP() * 0.2f); // �ִ� hp ����

        if (GetLv() == MaxLv)
        {
            print("�ִ뷹�� ����");
            UIManager.Instance.PlayerLevelText.text = "Lv. Max"; // ���� ǥ��
        }
        else
        {
            UIManager.Instance.PlayerLevelText.text = "Lv. " + GetLv(); // ���� ǥ��
        }

        UIManager.Instance.SkillSelect.SetActive(true); // �������� ��ų ȹ��

        // ������ �ؽ�Ʈ ����Ʈ
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, UIManager.Instance.PlayerHPBar.transform.parent);
        obj.GetComponentInChildren<TMP_Text>().color = new Color(0, 1, 1f, 1); // û�ϻ� �۾��� ǥ��
        obj.GetComponentInChildren<TMP_Text>().fontSize = 150.0f; // ��Ʈ ũ�� ����
        obj.GetComponentInChildren<TMP_Text>().text = "Level UP!!";

        // ������ ����Ʈ
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
        // �ִ뷹���� ��� ����ġ ȹ��x
        if (GetLv() == MaxLv)
        {
            return;
        }

        if (CurExp == null)
        {
            // ó�� �����ϴ� ������ ����
            CurExp = 0.0f;
            UIManager.Instance.PlayerLevelText.text = "Lv. " + GetLv();
            UIManager.Instance.PlayerExpBar.value = CurExp.Value / GetExp();

            UIManager.Instance.SkillSelect.SetActive(true); // ó�� �����Ҷ� ��ų ȹ��
        }

        CurExp += data;

        // ������
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
            // ó�� ������ ���
            CurHP = GetMaxHP();
        }

        if (data != 0)
        {
            // HP ����Ʈ ����
            GameObject obj = Instantiate(UIManager.Instance.DmgEffect, UIManager.Instance.PlayerHPBar.transform.parent);
            obj.GetComponentInChildren<TMP_Text>().text = "";

            if (data > 0)
            {
                // hp ȸ��
                obj.GetComponentInChildren<TMP_Text>().color = new Color(0, 1, 0, 1); // �ʷϻ� �۾��� ȸ�� ǥ��
                obj.GetComponentInChildren<TMP_Text>().text += "+";
            }

            obj.GetComponentInChildren<TMP_Text>().text += (int)data;
        }

        CurHP += data;

        if (CurHP >= GetMaxHP())
        {
            // Ǯ���� ���
            CurHP = GetMaxHP();
        }
        else if (CurHP <= 0.0f)
        {
            // ���� ���
            CurHP = 0.0f;
            UpdateLife(-1);
            UIManager.Instance.PlayerHPBar.value = 0.0f;

            if (myStat.Life <= 0)
            {                
                return false;
            }
            else
            {
                // ��Ȱ
                print("��Ȱ");
                CurHP = null;
                UpdateHP();
            }
        }

        // �г� ��ų�� Ȱ��ȭ �� ���
        if (onRage)
        {
            // ���� ü�� 1�۴� ���ݷ� 1�� ����
            myStat.ATK = baseATK + baseATK * (1.0f - (float)CurHP / GetMaxHP());
        }

        // �ݳ� ��ų�� Ȱ��ȭ �� ���
        if (onFury)
        {
            // ���� ü�� 1�۴� ���� 0.4�� ����
            myStat.ATK_SPEED = baseATK_SPEED + baseATK_SPEED * (1.0f - (float)CurHP / GetMaxHP());
        }

        UIManager.Instance.PlayerHPBar.value = (float)CurHP / GetMaxHP(); // ���� HP�� UI ǥ��
        UIManager.Instance.CurHPText.text = "" + (int)CurHP.Value; // ����HP UI ǥ��

        return true;
    }

    public void UpdateMaxHP(float data)
    {
        float beforeMaxHp = myStat.MaxHP;

        myStat.MaxHP += data;

        if (myStat.MaxHP <= 0.0f)
        {
            // 0���� maxhp�� �۾����� 1�� ����
            myStat.MaxHP = 1.0f;
        }

        if (myStat.MaxHP >= 4000.0f)
        {
            // 40000���� maxhp�� Ŀ���� 4000���� ����
            myStat.MaxHP = 4000.0f;
        }

        // hpline ����
        UIManager.Instance.PlayerHPBar.gameObject.GetComponentInParent<PlayerHPBar>().ChangeHPLine(); 

        if (CurHP != null)
        {
            // �ִ�ü���� �����Ѹ�ŭ ������ �°� ü���� ȸ�� // CurHP = CurHP * (GetMaxHP() / beforeMaxHp) => CurHP = CurHP + [- CurHP + CurHP * (GetMaxHP() / beforeMaxHp)]
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
            print("���ӿ���");
            ChangeState(State.GameOver);
        }

        Invoke("HPEffect", 0.5f); // 0.5���Ŀ� hp�� �پ��� ȿ�� �߻�
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

        // UI�� ü�¹� ǥ��
        UIManager.Instance.PlayerHPBar.value = Mathf.Lerp(UIManager.Instance.PlayerHPBar.value, CurHP.Value / GetMaxHP(), Time.unscaledDeltaTime * 5.0f); // hp�� �ε巴�� �پ��ų� �þ���� ����

        if (OnHPEffect)
        {
            // hpȿ��
            UIManager.Instance.PlayerHPEffectBar.value = Mathf.Lerp(UIManager.Instance.PlayerHPEffectBar.value, UIManager.Instance.PlayerHPBar.value, Time.unscaledDeltaTime * 5.0f);

            if (UIManager.Instance.PlayerHPEffectBar.value - 0.01f <= UIManager.Instance.PlayerHPBar.value)
            {
                OnHPEffect = false;
                UIManager.Instance.PlayerHPEffectBar.value = UIManager.Instance.PlayerHPBar.value;
            }
        }

        if (GetLv() == MaxLv)
        {   
            // �ִ뷹�� ���޽� �ٸ� �� ä��
            UIManager.Instance.PlayerExpBar.value = 1;
        }
        else
        {
            // ���� ����ġ ȹ�淮 UI ǥ��
            UIManager.Instance.PlayerExpBar.value = Mathf.Lerp(UIManager.Instance.PlayerExpBar.value, CurExp.Value / GetExp(), Time.unscaledDeltaTime * 5.0f); // exp�� �ε巴�� �پ��ų� �þ���� ����
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
        // 0.5�� �ڿ� ����ü�� �������� �߻�뿡�� �ѹ� �� �߻�ǵ��� �ϴ� �Լ� �߰�
        animEvent.Attack += () => Invoke("shot", 0.3f);
    }
        
    public void TripleShot()
    {
        print("triple shot");
        animEvent.Attack += () => Invoke("shot", 0.6f);
    }

    void shot()
    {
        // �߻�밡 ������� ��쿡�� �߻�
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

        UIManager.Instance.PlayerLevelText.text = "Lv. " + GetLv(); // ���� ǥ��
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

        // ��弦 -> 10�ۼ�Ʈ Ȯ���� ���� ���
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
