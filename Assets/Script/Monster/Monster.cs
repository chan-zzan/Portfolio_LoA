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
    [SerializeField] bool isBoss = false; // 해당 몬스터가 보스인지 체크

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

    private Vector3 RandomPos = Vector3.zero; // 랜덤 포지션

    // 로밍 제한 범위(맵 내에서만 이동)
    private Vector2 AreaRange_X =  new Vector2(-4.5f, 4.5f);
    private Vector2 AreaRange_Z = new Vector2(-5.0f, 9.0f);

    protected abstract void IsAttack(Transform target); // 추상클래스를 이용해 부모에서는 선언하지 않고 자식에서만 해당 함수를 무조건 선언하도록 만듦

    float? curHP = null;

    protected bool UpdateHP(float data = 0)
    {
        if (curHP == null)
        {
            // HP가 처리된 적이 없다면 초기 HP로 세팅
            curHP = myData.GetMaxHP();
        }

        curHP += data; // data만큼 HP 증가

        if (curHP <= 0.0f)
        {
            curHP = 0.0f;
            myHPBar.value = 0.0f;
            return false; // 죽었을 경우 false 반환
        }

        return true;
    }

    public virtual void OnDamage(float damage)
    {
        if (myState == State.Die) return;

        myAnim.SetTrigger("damage"); // 피격시 애니메이션 호출
        
        // 데미지 이펙트 생성
        GameObject obj = Instantiate(UIManager.Instance.DmgEffect, myHPBar.transform.parent);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 2.0f, obj.transform.localPosition.z);

        if (damage > 9999.0f)
        {
            // 헤드샷인 경우
            obj.GetComponentInChildren<TMP_Text>().text = "Head Shot!!!";
            obj.GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0, 1); // 빨간색 글씨로 눈에 잘 띄도록 함
            obj.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold; // 글씨체를 두껍게 바꿈
            obj.GetComponentInChildren<TMP_Text>().fontSize *= 1.5f; // 폰트 크기를 1.5배로 바꿈
        }
        else
        {
            obj.GetComponentInChildren<TMP_Text>().text = "-" + (int)damage;
        }

        if (!UpdateHP(-damage))
        {
            
            ChangeState(State.Die);
        }

        Invoke("HPEffect", 0.5f); // 0.5초후에 hp가 줄어드는 효과 발생
    }

    protected void HPEffect()
    {
        OnHPEffect = true;
    }

    private void Awake()
    {
        // 보스임을 표시
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
        UpdateHP(); // 초기 몬스터 hp 세팅

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

        // UI에 체력바 표시
        myHPBar.value = Mathf.Lerp(myHPBar.value, curHP.Value / myData.GetMaxHP(), Time.deltaTime * 5.0f); // hp가 부드럽게 줄어들거나 늘어나도록 변경

        if (OnHPEffect)
        {
            myHPEffectBar.value = Mathf.Lerp(myHPEffectBar.value, myHPBar.value, Time.deltaTime * 5.0f); // hp가 부드럽게 줄어들거나 늘어나도록 변경

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
                IsAttack(Target); // 몬스터 마다 다르게 공격
                break;
            case State.Die:
                {
                    print("die");
                    StopAllCoroutines();

                    // 경험치 드랍
                    int num = 2 + StageManager.Instance.GetStageIndex() / 5 + Random.Range(0, 3); // 드랍 갯수
                    Vector3 curPos = new Vector3(this.transform.position.x, 1.0f, this.transform.position.z);

                    for (int i = 0; i < num; i++)
                    {
                        // 현재 스테이지에 자식으로 경험치 프리팹 드랍
                        GameObject ExpObj = Instantiate(StageManager.Instance.DropExp, curPos, this.transform.rotation, StageManager.Instance.GetStage());
                    }

                    Destroy(this.GetComponent<CapsuleCollider>()); // 콜라이더를 없애서 플레이어에 타겟이 되지 않도록 함

                    // 죽은 몬스터를 플레이어의 타겟 리스트에서 삭제
                    Player.Instance.CharacterMove.Detect.monsters.Remove(this.transform);

                    // 현재 스테이지에 몬스터가 더이상 없는 경우
                    if (Player.Instance.CharacterMove.Detect.monsters.Count == 0)
                    {
                        StageManager.Instance.StageClear(); // 현재 스테이지 클리어
                    }

                    nav.speed = 0.0f;
                    myAnim.SetBool("die", true);

                    if (monsterType == MonsterType.Dragon)
                    {
                        // 마지막 보스가 죽은 경우
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
                nav.SetDestination(RandomPos); // 랜덤 포지션으로 이동
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
            // 타겟을 찾은경우 Attcking 상태로 변경
            this.Target = target;
            ChangeState(State.Attacking);
            //print("Find : " + Target.name);
        }
    }

    protected void SetRandomPosition(Vector3 roamingStartPos, ref Vector3 randomPos, Vector2 randomRange_X, Vector2 randomRange_Z)
    {
        // 이동할 지점을 랜덤으로 설정
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
            // x축 음의 방향으로 제한 범위를 넘어간 경우 
            randomPos.x = curAreaRange_X.x;
        }
        else if (randomPos.x > curAreaRange_X.y)
        {
            // x축 양의 방향으로 제한 범위를 넘어간 경우
            randomPos.x = curAreaRange_X.y;
        }
        else if (randomPos.z < curAreaRange_Z.x)
        {
            // z축 음의 방향으로 제한 범위를 넘어간 경우
            randomPos.z = curAreaRange_Z.x;
        }
        else if (randomPos.z > curAreaRange_Z.y)
        {
            // z축 양의 방향으로 제한 범위를 넘어간 경우
            randomPos.z = curAreaRange_Z.y;
        }
    }

    void Roaming()
    {
        if (myState == State.Roaming)
        {
            // 이동할 지점을 랜덤으로 설정
            SetRandomPosition(this.transform.position, ref RandomPos, new Vector2(-2.0f, 2.0f), new Vector2(-2.0f, 2.0f));

            // 로밍 대기 시간
            StartCoroutine(RoamingWait(2.0f, Roaming)); 
        }     
    }

    IEnumerator RoamingWait(float t, UnityAction done)
    {
        yield return new WaitForSeconds(t); // t초만큼 기다림
        
        done?.Invoke(); // delegate에 저장된 함수 실행
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
