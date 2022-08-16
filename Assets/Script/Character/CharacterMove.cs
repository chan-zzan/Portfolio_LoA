using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : Characteristic
{
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] Transform myCharacter;
    [SerializeField] Camera cam;

    Vector2 moveInput; // 조이스틱 입력 값

    Vector3 moveVec;
    Vector3 moveDir;

    Vector3 lookDir;

    public float MoveSpeed = 5.0f;
    float SmoothRotSpeed = 360.0f;
    bool isMove = false;

    [SerializeField] LayerMask DetectLayer;
    public DetectMonster Detect = null;

    [SerializeField]
    Transform Target;
    Transform NearestTarget;

    float TargetDist = 9999.9f; // 타겟까지의 거리
    float MinDist = 9999.9f; // 최소 거리

    private void Start()
    {
        MoveSpeed = Player.Instance.GetMOVE_SPEED();
    }

    private void FixedUpdate()
    {
        if (Player.Instance.myState == State.GameOver) return;

        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        isMove = moveInput.magnitude != 0; // 입력이 들어왔는지를 판단

        if (isMove)
        {
            // 조이스틱 입력 값이 들어오면 move 상태로 변경
            Player.Instance.ChangeState(State.Move);
        }
    }

    public void PlayerMove()
    {
        JoystickMove();
        myAnim.SetFloat("x", joystick.Horizontal);
        myAnim.SetFloat("y", joystick.Vertical);
    }

    public Transform GetTarget() => Target; // 타겟에 접근

    void JoystickMove()
    {
        if (isMove)
        {
            myAnim.SetBool("Attack", false); // 플레이어가 움직이는 동안은 공격하지 않도록 설정

            moveDir = new Vector3(moveInput.x, 0, moveInput.y);

            if (moveDir != Vector3.zero)
            {
                myCharacter.forward = moveDir; // 캐릭터가 설정한 방향을 바라보게 함
                moveVec = moveDir * MoveSpeed * Time.deltaTime;

                myRigid.MovePosition(myRigid.position + moveVec);
                this.transform.Translate(moveVec, Space.World);
            }
        }
        else
        {
            if (Detect.monsters.Count != 0)
            {
                // 움직이지 않고 몬스터가 감지된 경우 -> attack 상태로 변경 -> 공격할 몬스터를 찾음
                Player.Instance.ChangeState(State.Attack);
            }
            else
            {
                // 움직이지 않고 몬스터도 감지되지 않는 경우 -> idle 상태로 변경
                Player.Instance.ChangeState(State.Idle);
            }
        }

        // 입력이 없을경우 회전x
        if (moveVec.sqrMagnitude == 0) return; // sqrMganitude : 벡터의 제곱크기 반환

        // 회전
        if (moveVec != Vector3.zero)
        {
            Quaternion dirQuat = Quaternion.LookRotation(moveDir); // 회전해야하는 값을 저장
            Quaternion moveQuat = Quaternion.Slerp(myRigid.rotation, dirQuat, SmoothRotSpeed); // 현재 회전값과 바뀔 회전값을 보간
            myRigid.MoveRotation(moveQuat);
        }
    }

    public void FindMonster()
    {
        // 초기화
        TargetDist = 9999.9f;
        MinDist = 9999.9f;
        Target = null;
        NearestTarget = null;

        // 범위내에 감지된 몬스터가 있을 경우
        if (Detect.monsters.Count != 0)
        {
            foreach (Transform mon in Detect.monsters)
            {
                // 장애물과 몬스터만 부딫히도록 설정
                if (Physics.Raycast(myCharacter.position, mon.position - myCharacter.position + new Vector3(0.0f, 0.5f, 0.0f), out RaycastHit hit, 100.0f, DetectLayer.value))
                {
                    Debug.DrawLine(myCharacter.position, hit.point, Color.red);

                    // 플레이어부터 몬스터까지의 거리 저장
                    float curMonsterDist = Vector3.Distance(myCharacter.position, mon.position);

                    // 최소거리 저장
                    if (curMonsterDist < MinDist)
                    {
                        MinDist = curMonsterDist;
                        NearestTarget = mon;
                    }

                    // 현재 몬스터와의 거리가 이전 타겟까지의 거리보다 짧은 경우 타겟으로 설정
                    if (curMonsterDist < TargetDist)
                    {
                        // 몬스터에 부딫힌 경우
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Monster"))
                        {
                            TargetDist = curMonsterDist;
                            Target = hit.transform;
                        }
                    }
                }
            }

            // 장애물에 가려지지 않은 타겟이 없는경우 가장 가까운 적의 위치를 타겟으로 설정
            if (Target == null)
            {
                Target = NearestTarget;
            }

            if (Target == null)
            {
                Player.Instance.ChangeState(State.Idle); // idle 상태로 변경
                return;
            }

            // 타겟을 바라보도록 한 후 공격 시작
            //myCharacter.forward = Target.position - myCharacter.position;

            lookDir = (Target.position - myCharacter.position).normalized;

            Quaternion from = this.transform.rotation;
            Quaternion to = Quaternion.LookRotation(lookDir);

            this.transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 180.0f);
            myRigid.MoveRotation(this.transform.rotation);

            myAnim.SetBool("Attack", true);

            Debug.DrawLine(myCharacter.position, Target.position + new Vector3(0, 0.5f, 0), Color.blue);
        }
        else
        {
            // 리스트에 몬스터가 하나도 없는 경우
            Player.Instance.ChangeState(State.Idle); // idle 상태로 변경
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            // 스테이지를 넘어가는 waypoint를 지나는 경우
            StageManager.Instance.NextStage();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            // 트랩 위를 지난 경우
            Player.Instance.OnDamage(30.0f);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("ExpStar") && Detect.monsters.Count == 0)
        {
            // 경험치 획득
            Destroy(other.transform.parent.gameObject);
            Player.Instance.UpdateExp(5.0f);
        }
    }
}