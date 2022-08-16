using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : Characteristic
{
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] Transform myCharacter;
    [SerializeField] Camera cam;

    Vector2 moveInput; // ���̽�ƽ �Է� ��

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

    float TargetDist = 9999.9f; // Ÿ�ٱ����� �Ÿ�
    float MinDist = 9999.9f; // �ּ� �Ÿ�

    private void Start()
    {
        MoveSpeed = Player.Instance.GetMOVE_SPEED();
    }

    private void FixedUpdate()
    {
        if (Player.Instance.myState == State.GameOver) return;

        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        isMove = moveInput.magnitude != 0; // �Է��� ���Դ����� �Ǵ�

        if (isMove)
        {
            // ���̽�ƽ �Է� ���� ������ move ���·� ����
            Player.Instance.ChangeState(State.Move);
        }
    }

    public void PlayerMove()
    {
        JoystickMove();
        myAnim.SetFloat("x", joystick.Horizontal);
        myAnim.SetFloat("y", joystick.Vertical);
    }

    public Transform GetTarget() => Target; // Ÿ�ٿ� ����

    void JoystickMove()
    {
        if (isMove)
        {
            myAnim.SetBool("Attack", false); // �÷��̾ �����̴� ������ �������� �ʵ��� ����

            moveDir = new Vector3(moveInput.x, 0, moveInput.y);

            if (moveDir != Vector3.zero)
            {
                myCharacter.forward = moveDir; // ĳ���Ͱ� ������ ������ �ٶ󺸰� ��
                moveVec = moveDir * MoveSpeed * Time.deltaTime;

                myRigid.MovePosition(myRigid.position + moveVec);
                this.transform.Translate(moveVec, Space.World);
            }
        }
        else
        {
            if (Detect.monsters.Count != 0)
            {
                // �������� �ʰ� ���Ͱ� ������ ��� -> attack ���·� ���� -> ������ ���͸� ã��
                Player.Instance.ChangeState(State.Attack);
            }
            else
            {
                // �������� �ʰ� ���͵� �������� �ʴ� ��� -> idle ���·� ����
                Player.Instance.ChangeState(State.Idle);
            }
        }

        // �Է��� ������� ȸ��x
        if (moveVec.sqrMagnitude == 0) return; // sqrMganitude : ������ ����ũ�� ��ȯ

        // ȸ��
        if (moveVec != Vector3.zero)
        {
            Quaternion dirQuat = Quaternion.LookRotation(moveDir); // ȸ���ؾ��ϴ� ���� ����
            Quaternion moveQuat = Quaternion.Slerp(myRigid.rotation, dirQuat, SmoothRotSpeed); // ���� ȸ������ �ٲ� ȸ������ ����
            myRigid.MoveRotation(moveQuat);
        }
    }

    public void FindMonster()
    {
        // �ʱ�ȭ
        TargetDist = 9999.9f;
        MinDist = 9999.9f;
        Target = null;
        NearestTarget = null;

        // �������� ������ ���Ͱ� ���� ���
        if (Detect.monsters.Count != 0)
        {
            foreach (Transform mon in Detect.monsters)
            {
                // ��ֹ��� ���͸� �΋H������ ����
                if (Physics.Raycast(myCharacter.position, mon.position - myCharacter.position + new Vector3(0.0f, 0.5f, 0.0f), out RaycastHit hit, 100.0f, DetectLayer.value))
                {
                    Debug.DrawLine(myCharacter.position, hit.point, Color.red);

                    // �÷��̾���� ���ͱ����� �Ÿ� ����
                    float curMonsterDist = Vector3.Distance(myCharacter.position, mon.position);

                    // �ּҰŸ� ����
                    if (curMonsterDist < MinDist)
                    {
                        MinDist = curMonsterDist;
                        NearestTarget = mon;
                    }

                    // ���� ���Ϳ��� �Ÿ��� ���� Ÿ�ٱ����� �Ÿ����� ª�� ��� Ÿ������ ����
                    if (curMonsterDist < TargetDist)
                    {
                        // ���Ϳ� �΋H�� ���
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Monster"))
                        {
                            TargetDist = curMonsterDist;
                            Target = hit.transform;
                        }
                    }
                }
            }

            // ��ֹ��� �������� ���� Ÿ���� ���°�� ���� ����� ���� ��ġ�� Ÿ������ ����
            if (Target == null)
            {
                Target = NearestTarget;
            }

            if (Target == null)
            {
                Player.Instance.ChangeState(State.Idle); // idle ���·� ����
                return;
            }

            // Ÿ���� �ٶ󺸵��� �� �� ���� ����
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
            // ����Ʈ�� ���Ͱ� �ϳ��� ���� ���
            Player.Instance.ChangeState(State.Idle); // idle ���·� ����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            // ���������� �Ѿ�� waypoint�� ������ ���
            StageManager.Instance.NextStage();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            // Ʈ�� ���� ���� ���
            Player.Instance.OnDamage(30.0f);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("ExpStar") && Detect.monsters.Count == 0)
        {
            // ����ġ ȹ��
            Destroy(other.transform.parent.gameObject);
            Player.Instance.UpdateExp(5.0f);
        }
    }
}