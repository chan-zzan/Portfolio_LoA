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

            // 회전
            if (rotate && Destination != null)
            {
                lookDir = (detect.player.position - this.transform.position).normalized;

                Quaternion from = this.transform.rotation;
                Quaternion to = Quaternion.LookRotation(lookDir);
                this.transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 7.0f);
            }

            if (following)
            {
                // 몬스터가 플레이어를 따라다니는 상태이면서 일정거리 이상 멀리 떨어진 경우 -> 대쉬
                if (Vector3.Distance(this.transform.position, Destination.position) > DashDist)
                {
                    following = false;
                    print("대쉬");
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
                    // 대쉬 대기중에만 동작
                    if (dashWait)
                    {
                        if (Physics.Raycast(this.transform.position, (Destination.position - this.transform.position).normalized, out RaycastHit hit))
                        {
                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                            {
                                print("장애물 발견 : " + hit.transform.name);

                                // 장애물이 발견된 경우 -> 장애물의 위치로 목표지점 변경
                                Destination = hit.transform;
                            }
                        }
                    }
                }
                break;
            case DetailState.Following:
                {
                    nav.SetDestination(detect.player.position); // 플레이어를 따라다님

                    // 일정거리 이상 다시 멀어지는 경우 대쉬해야함
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
        Destination = target; // 플레이어의 위치를 목표지점으로 설정
        nav.ResetPath();
        rotate = true;

        // 대쉬
        ChangeDetailState(DetailState.Dash);
    }

    IEnumerator Dash(Vector3 targetPos)
    {
        dashWait = true;

        yield return new WaitForSeconds(2.0f); // 공격 전 대기

        dashWait = false;
        targetPos = Destination.position; // 목표지점으로 타겟위치을 세팅

        print("Dash");

        float StopDist; // 멈출거리

        if (Destination.gameObject.layer == LayerMask.NameToLayer("Player")) // 목적지가 플레이어인 경우
        {
            print("플레이어 대쉬");
            // 공격대기를 하는 동안 플레이어가 움직인 위치로 회전 후 마지막 바라보는 방향으로 대쉬            
            StopDist = 0.5f;
        }
        else
        {
            print("장애물 대쉬");
            StopDist = 1.0f;
            targetPos = targetPos - (targetPos - this.transform.position).normalized;
        }

        // 대쉬 속도로 변경
        nav.speed = 5.0f;
        nav.acceleration = 8.0f;

        rotate = false;

        bool dash = true;

        while (dash)
        {            
            // 대쉬
            nav.SetDestination(targetPos);

            // 플레이어에게 일정거리이상 다가오면 대쉬 끝
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
            yield return new WaitForSeconds(2.0f); // 플레이어에 대쉬 후 대기
                        
            // 플레이어에 대쉬한 경우
            if (Vector3.Distance(Destination.position, this.transform.position) < DashDist)
            {
                // 대쉬를 하고나서 거리가 가까워진 경우 ->  Following 상태로 변경
                ChangeDetailState(DetailState.Following);
            }
            else
            {
                rotate = true;

                // 대쉬를 했는데 아직 거리가 먼 경우 -> 다시 대쉬
                ChangeDetailState(DetailState.None);
            }

        }
        else if (Destination.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {            
            // 대쉬를 장애물에 한 경우 -> Following 상태로 변경
            ChangeDetailState(DetailState.Following);
        }

    }

    IEnumerator ObstacleWait()
    {
        yield return new WaitForSeconds(2.0f);

        if (Vector3.Distance(detect.player.position, this.transform.position) > DashDist)
        {
            // 장애물에 대쉬 후에 잠시 캐릭터를 따라 이동하다가 아직 캐릭터와의 거리가 멀다면 다시 대쉬
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
    //    // 목표지점이 플레이어의 위치이고 플레이어와 몬스터 사이 거리가 일정거리 이상 떨어져있는 경우 다시 대쉬 공격
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
    //            // 일정거리 내에 있거나 목표지점이 장애물인 경우 -> 플레이어를 따라다니도록 설정                
    //            nav.SetDestination(detect.player.position);

    //            // 일정거리이상 떨어진 경우 더이상 따라다니지 않도록 설정
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
    //            // 대쉬를 하고나서 거리가 가까워진 경우 -> 플레이어를 따라다니다가 다시 일정거리 이상이 되면 대쉬
    //            nav.SetDestination(detect.player.position);
    //        }
    //        else
    //        {
    //            // 대쉬를 했는데 아직 거리가 먼 경우 -> 다시 대쉬
    //            IsAttack(detect.player);
    //        }
    //    }
    //    else if (Destination.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
    //    {
    //        // 대쉬를 장애물에 한 경우 -> 2초동안 플레이어를 향하여 움직이도록 하다가 다시 대쉬
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
