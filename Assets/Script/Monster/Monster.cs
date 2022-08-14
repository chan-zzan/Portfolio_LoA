using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public enum MonsterType
{ 
    BEE, MUSHROOM, RABBIT, TURTLE, Dragon
}

public abstract class Monster : AIProperty
{
    [SerializeField] MonsterType monsterType;
    [SerializeField] bool isBoss = false; // �ش� ���Ͱ� �������� üũ

    public enum State
    {
        Create, Roaming, Attacking, Die
    }

    [SerializeField]
    protected State myState = State.Create;

    protected Slider myHPBar;
    Slider myHPEffectBar;
    bool OnHPEffect = false;

    private Transform Target;

    private Vector3 RandomPos = Vector3.zero; // ���� ������

    // �ι� ���� ����(�� �������� �̵�)
    private Vector2 AreaRange_X =  new Vector2(-4.5f, 4.5f);
    private Vector2 AreaRange_Z = new Vector2(-5.0f, 9.0f);

    protected abstract void IsAttack(Transform target); // �߻�Ŭ������ �̿��� �θ𿡼��� �������� �ʰ� �ڽĿ����� �ش� �Լ��� ������ �����ϵ��� ����

    float? curHP = null;

    protected bool UpdateHP(float data = 0)
    {
        if (curHP == null)
        {
            // HP�� ó���� ���� ���ٸ� �ʱ� HP�� ����
            curHP = myData.GetMaxHP();
        }

        curHP += data; // data��ŭ HP ����

        if (curHP <= 0.0f)
        {
            curHP = 0.0f;
            myHPBar.value = 0.0f;
            return false; // �׾��� ��� false ��ȯ
        }

        return true;
    }

    public virtual void OnDamage(float damage)
    {
        if (myState == State.Die) return;

        myAnim.SetTrigger("damage"); // �ǰݽ� �ִϸ��̼� ȣ��
        
        // ������ ����Ʈ ����
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, myHPBar.transform.parent);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 2.0f, obj.transform.localPosition.z);

        if (damage > 9999.0f)
        {
            // ��弦�� ���
            obj.GetComponentInChildren<TMP_Text>().text = "Head Shot!!!";
            obj.GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0, 1); // ������ �۾��� ���� �� �絵�� ��
            obj.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold; // �۾�ü�� �β��� �ٲ�
            obj.GetComponentInChildren<TMP_Text>().fontSize *= 1.5f; // ��Ʈ ũ�⸦ 1.5��� �ٲ�
        }
        else
        {
            obj.GetComponentInChildren<TMP_Text>().text = "-" + (int)damage;
        }

        if (!UpdateHP(-damage))
        {
            
            ChangeState(State.Die);
        }

        Invoke("HPEffect", 0.5f); // 0.5���Ŀ� hp�� �پ��� ȿ�� �߻�
    }

    protected void HPEffect()
    {
        OnHPEffect = true;
    }

    private void Awake()
    {
        // �������� ǥ��
        if (isBoss)
        {
            myHPBar = UIManager.Instance.BossHPBar;
            myHPEffectBar = UIManager.Instance.BossHPEffectBar;
        }
        else
        {
            myHPEffectBar = this.transform.parent.GetComponentInChildren<MonsterHPBar>().GetComponentsInChildren<Slider>()[0];
            myHPBar = this.transform.parent.GetComponentInChildren<MonsterHPBar>().GetComponentsInChildren<Slider>()[1];
        }
    }

    private void Start()
    {
        UpdateHP(); // �ʱ� ���� hp ����

        if (isBoss)
        {
            FindTarget(Player.Instance.transform);
        }
        else
        {
            ChangeState(State.Roaming);
        }
    }

    private void FixedUpdate()
    {
        StateProcess();

        // UI�� ü�¹� ǥ��
        myHPBar.value = Mathf.Lerp(myHPBar.value, curHP.Value / myData.GetMaxHP(), Time.deltaTime * 5.0f); // hp�� �ε巴�� �پ��ų� �þ���� ����

        if (OnHPEffect)
        {
            myHPEffectBar.value = Mathf.Lerp(myHPEffectBar.value, myHPBar.value, Time.deltaTime * 5.0f); // hp�� �ε巴�� �پ��ų� �þ���� ����

            if (myHPEffectBar.value - 0.01f <= myHPBar.value)
            {
                OnHPEffect = false;
                myHPEffectBar.value = myHPBar.value;
            }
        }
    }

    protected void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case State.Create:
                break;
            case State.Roaming:
                myAnim.SetBool("move", true);
                myAnim.SetBool("attack", false);
                nav.speed = 1.0f;
                nav.acceleration = 3.0f;
                StopAllCoroutines();
                Roaming();
                break;
            case State.Attacking:
                StopAllCoroutines();
                IsAttack(Target); // ���� ���� �ٸ��� ����
                break;
            case State.Die:
                {
                    print("die");
                    StopAllCoroutines();

                    // ����ġ ���
                    int num = 2 + StageManager.Instance.GetStageIndex() / 5 + Random.Range(0, 3); // ��� ����
                    Vector3 curPos = new Vector3(this.transform.position.x, 1.0f, this.transform.position.z);

                    for (int i = 0; i < num; i++)
                    {
                        // ���� ���������� �ڽ����� ����ġ ������ ���
                        GameObject ExpObj = Instantiate(StageManager.Instance.DropExp, curPos, this.transform.rotation, StageManager.Instance.GetStage());
                    }

                    Destroy(this.GetComponent<CapsuleCollider>()); // �ݶ��̴��� ���ּ� �÷��̾ Ÿ���� ���� �ʵ��� ��

                    // ���� ���͸� �÷��̾��� Ÿ�� ����Ʈ���� ����
                    Player.Instance.CharacterMove.Detect.monsters.Remove(this.transform);

                    // ���� ���������� ���Ͱ� ���̻� ���� ���
                    if (Player.Instance.CharacterMove.Detect.monsters.Count == 0)
                    {
                        StageManager.Instance.StageClear(); // ���� �������� Ŭ����
                    }

                    nav.speed = 0.0f;
                    myAnim.SetBool("die", true);

                    if (monsterType == MonsterType.Dragon)
                    {
                        // ������ ������ ���� ���
                        UIManager.Instance.EndingPanel.SetActive(true);
                        UIManager.Instance.stageNum.text = "" + StageManager.Instance.GetStageIndex();
                        GameManager.Instance.PauseScene();
                    }

                    Destroy(this.transform.parent.gameObject, 1.0f);
                }            
                break;
        }
    }

    void StateProcess()
    {
        switch (myState)
        {
            case State.Create:
                break;
            case State.Roaming:
                nav.SetDestination(RandomPos); // ���� ���������� �̵�
                break;
            case State.Attacking:
                if (Player.Instance.myState == global::State.GameOver)
                {
                    ChangeState(State.Roaming);
                }
                break;
            case State.Die:
                break;
        }
    }

    public void StartRoaming()
    {
        if (myState == State.Die) return;

        ChangeState(State.Roaming);
    }


    public void FindTarget(Transform target)
    {
        if (myState != State.Attacking)
        {
            // Ÿ���� ã����� Attcking ���·� ����
            this.Target = target;
            ChangeState(State.Attacking);
            //print("Find : " + Target.name);
        }
    }

    protected void SetRandomPosition(Vector3 roamingStartPos, ref Vector3 randomPos, Vector2 randomRange_X, Vector2 randomRange_Z)
    {
        // �̵��� ������ �������� ����
        randomPos.x = roamingStartPos.x + Random.Range(randomRange_X.x, randomRange_X.y);
        randomPos.z = roamingStartPos.z + Random.Range(randomRange_Z.x, randomRange_Z.y);

        // -4.5 < x < 4.5 : AreaRange_X
        // -5.0 < z < 9.0 : AreaRange_Z

        Vector3 basePos = this.transform.parent.position;
        //print(this.name + " basePos: " + basePos);

        Vector2 curAreaRange_X = new Vector2(basePos.x, basePos.x) + AreaRange_X;
        Vector2 curAreaRange_Z = new Vector2(basePos.z, basePos.z) + AreaRange_Z;

        //print(this.name + " AreaRange_X: " + curAreaRange_X);
        //print(this.name + " AreaRange_Z: " + curAreaRange_Z);

        if (randomPos.x < curAreaRange_X.x)
        {
            // x�� ���� �������� ���� ������ �Ѿ ��� 
            randomPos.x = curAreaRange_X.x;
        }
        else if (randomPos.x > curAreaRange_X.y)
        {
            // x�� ���� �������� ���� ������ �Ѿ ���
            randomPos.x = curAreaRange_X.y;
        }
        else if (randomPos.z < curAreaRange_Z.x)
        {
            // z�� ���� �������� ���� ������ �Ѿ ���
            randomPos.z = curAreaRange_Z.x;
        }
        else if (randomPos.z > curAreaRange_Z.y)
        {
            // z�� ���� �������� ���� ������ �Ѿ ���
            randomPos.z = curAreaRange_Z.y;
        }
    }

    void Roaming()
    {
        if (myState == State.Roaming)
        {
            // �̵��� ������ �������� ����
            SetRandomPosition(this.transform.position, ref RandomPos, new Vector2(-2.0f, 2.0f), new Vector2(-2.0f, 2.0f));

            // �ι� ��� �ð�
            StartCoroutine(RoamingWait(2.0f, Roaming)); 
        }     
    }

    IEnumerator RoamingWait(float t, UnityAction done)
    {
        yield return new WaitForSeconds(t); // t�ʸ�ŭ ��ٸ�
        
        done?.Invoke(); // delegate�� ����� �Լ� ����
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            print("hit");
            Player.Instance.OnDamage(myData.GetATK());
        }
    }
}
