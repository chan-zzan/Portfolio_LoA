using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LauncherType
{ 
    Base, Front, Front2, Back, Left, Right, UpLeft, UpRight
}

public class Launcher : MonoBehaviour
{
    [SerializeField] LauncherType type;

    [SerializeField] PlayerAnimEvent animEvent;
    [SerializeField] GameObject projectile;

    private void Start()
    {
        animEvent.Attack += Shot;
    }

    public void Shot()
    {
        // 타겟 설정
        Transform target = Player.Instance.CharacterMove.GetTarget();

        if (target == null) return;

        // 투사체 생성
        GameObject obj = Instantiate(projectile, this.transform);

        // Launcher의 위치에 영향을 받지 않도록 부모를 변경하여 밖으로 빼줌
        obj.transform.parent = this.transform.parent.parent.parent;

        // 투사체 발사
        obj.GetComponent<Rigidbody>().velocity = this.transform.forward * 5.0f; 
    }

}
