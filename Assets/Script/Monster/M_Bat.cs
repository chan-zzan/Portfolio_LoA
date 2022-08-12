using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Bat : Monster
{
    private enum DetailState
    {
        None, Dash, Following
    }

    [SerializeField]
    private DetailState myDetailState = DetailState.None;

    Vector3 lookDir;
    [SerializeField]
    Transform Destination;
    float DashDist = 1.0f;

    bool following = false;
    bool rotate = false;
    bool dashWait = false;

    private void Update()
    {
        if (this.myState == State.Attacking)
        {
            DetailStateProcess();

            // ȸ��
            if (rotate && Destination != null)
            {
                lookDir = (detect.player.position - this.transform.position).normalized;

                Quaternion from = this.transform.rotation;
                Quaternion to = Quaternion.LookRotation(lookDir);
                this.transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 7.0f);
            }

            if (following)
            {
                // ���Ͱ� �÷��̾ ����ٴϴ� �����̸鼭 �����Ÿ� �̻� �ָ� ������ ��� -> �뽬
                if (Vector3.Distance(this.transform.position, Destination.position) > DashDist)
                {
                    following = false;
                    print("�뽬");
                    IsAttack(detect.player);
                }
            }
        }
        else
        {
            myDetailState = DetailState.None;
        }
    }

    private void ChangeDetailState(DetailState s)
    {
        if (myDetailState == s) return;
        myDetailState = s;

        switch (myDetailState)
        {
            case DetailState.None:
                print("None State");
                StopAllCoroutines();
                IsAttack(detect.player);
                break;
            case DetailState.Dash:
                print("Dash State");
                StopAllCoroutines();
                StartCoroutine(Dash(Destination.position));
                break;
            case DetailState.Following:
                {
                    print("Following State");
                    StopAllCoroutines();

                    rotate = true;

                    if (Destination != detect.player)
                    {
                        StartCoroutine(ObstacleWait());
                    }

                    nav.speed = 1.0f;
                    nav.acceleration = 3.0f;
                }
                break;
        }
    }

    void DetailStateProcess()
    {
        switch (myDetailState)
        {
            case DetailState.None:
                break;
            case DetailState.Dash:
                {
                    // �뽬 ����߿��� ����
                    if (dashWait)
                    {
                        if (Physics.Raycast(this.transform.position, (Destination.position - this.transform.position).normalized, out RaycastHit hit))
                        {
                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                            {
                                print("��ֹ� �߰� : " + hit.transform.name);

                                // ��ֹ��� �߰ߵ� ��� -> ��ֹ��� ��ġ�� ��ǥ���� ����
                                Destination = hit.transform;
                            }
                        }
                    }
                }
                break;
            case DetailState.Following:
                {
                    nav.SetDestination(detect.player.position); // �÷��̾ ����ٴ�

                    // �����Ÿ� �̻� �ٽ� �־����� ��� �뽬�ؾ���
                    if (Destination == detect.player)
                    {
                        if (Vector3.Distance(Destination.position, this.transform.position) > DashDist)
                        {
                            ChangeDetailState(DetailState.None);
                        }
                    }                    
                }
                break;
        }
    }

    protected override void IsAttack(Transform target)
    {
        print("attack");
        Destination = target; // �÷��̾��� ��ġ�� ��ǥ�������� ����
        nav.ResetPath();
        rotate = true;

        // �뽬
        ChangeDetailState(DetailState.Dash);
    }

    IEnumerator Dash(Vector3 targetPos)
    {
        dashWait = true;

        yield return new WaitForSeconds(2.0f); // ���� �� ���

        dashWait = false;
        targetPos = Destination.position; // ��ǥ�������� Ÿ����ġ�� ����

        print("Dash");

        float StopDist; // ����Ÿ�

        if (Destination.gameObject.layer == LayerMask.NameToLayer("Player")) // �������� �÷��̾��� ���
        {
            print("�÷��̾� �뽬");
            // ���ݴ�⸦ �ϴ� ���� �÷��̾ ������ ��ġ�� ȸ�� �� ������ �ٶ󺸴� �������� �뽬            
            StopDist = 0.5f;
        }
        else
        {
            print("��ֹ� �뽬");
            StopDist = 1.0f;
            targetPos = targetPos - (targetPos - this.transform.position).normalized;
        }

        // �뽬 �ӵ��� ����
        nav.speed = 5.0f;
        nav.acceleration = 8.0f;

        rotate = false;

        bool dash = true;

        while (dash)
        {            
            // �뽬
            nav.SetDestination(targetPos);

            // �÷��̾�� �����Ÿ��̻� �ٰ����� �뽬 ��
            if (Vector3.Distance(this.transform.position, targetPos) <= StopDist)
            {
                nav.ResetPath();
                dash = false;
            }

            yield return null;
        }        

        print("Dash End");
        

        if (Destination.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            yield return new WaitForSeconds(2.0f); // �÷��̾ �뽬 �� ���
                        
            // �÷��̾ �뽬�� ���
            if (Vector3.Distance(Destination.position, this.transform.position) < DashDist)
            {
                // �뽬�� �ϰ��� �Ÿ��� ������� ��� ->  Following ���·� ����
                ChangeDetailState(DetailState.Following);
            }
            else
            {
                rotate = true;

                // �뽬�� �ߴµ� ���� �Ÿ��� �� ��� -> �ٽ� �뽬
                ChangeDetailState(DetailState.None);
            }

        }
        else if (Destination.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {            
            // �뽬�� ��ֹ��� �� ��� -> Following ���·� ����
            ChangeDetailState(DetailState.Following);
        }

    }

    IEnumerator ObstacleWait()
    {
        yield return new WaitForSeconds(2.0f);

        if (Vector3.Distance(detect.player.position, this.transform.position) > DashDist)
        {
            // ��ֹ��� �뽬 �Ŀ� ��� ĳ���͸� ���� �̵��ϴٰ� ���� ĳ���Ϳ��� �Ÿ��� �ִٸ� �ٽ� �뽬
            ChangeDetailState(DetailState.None);
        }
        else
        {
            print("follow");
            Destination = detect.player;
        }
    }

    //IEnumerator Following()
    //{
    //    // ��ǥ������ �÷��̾��� ��ġ�̰� �÷��̾�� ���� ���� �Ÿ��� �����Ÿ� �̻� �������ִ� ��� �ٽ� �뽬 ����
    //    if (Destination.gameObject.layer == LayerMask.NameToLayer("Player") && Vector3.Distance(Destination.position, this.transform.position) > DashDist)
    //    {
    //        print("1");
    //        this.IsAttack(Destination);
    //    }
    //    else
    //    {
    //        nav.speed = 1.0f;
    //        nav.acceleration = 3.0f;

    //        following = true;

    //        while (following)
    //        {
    //            // �����Ÿ� ���� �ְų� ��ǥ������ ��ֹ��� ��� -> �÷��̾ ����ٴϵ��� ����                
    //            nav.SetDestination(detect.player.position);

    //            // �����Ÿ��̻� ������ ��� ���̻� ����ٴ��� �ʵ��� ����
    //            //if (Vector3.Distance(this.transform.position, Destination.position) > DashDist)
    //            //{
    //            //    following = false;
    //            //}

    //            yield return null;
    //        }

    //    }

    //    if (Destination.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {

    //        if (Vector3.Distance(Destination.position, this.transform.position) < DashDist)
    //        {
    //            // �뽬�� �ϰ��� �Ÿ��� ������� ��� -> �÷��̾ ����ٴϴٰ� �ٽ� �����Ÿ� �̻��� �Ǹ� �뽬
    //            nav.SetDestination(detect.player.position);
    //        }
    //        else
    //        {
    //            // �뽬�� �ߴµ� ���� �Ÿ��� �� ��� -> �ٽ� �뽬
    //            IsAttack(detect.player);
    //        }
    //    }
    //    else if (Destination.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
    //    {
    //        // �뽬�� ��ֹ��� �� ��� -> 2�ʵ��� �÷��̾ ���Ͽ� �����̵��� �ϴٰ� �ٽ� �뽬
    //        nav.speed = 1.0f;
    //        nav.acceleration = 3.0f;

    //        following = true;

    //        while (following)
    //        {
    //            nav.SetDestination(detect.player.position);
    //            yield return null;
    //        }


    //        yield return new WaitForSeconds(2.0f);

    //        IsAttack(detect.player);
    //    }
    //}
}
