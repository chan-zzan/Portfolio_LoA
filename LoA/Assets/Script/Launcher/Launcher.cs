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
        // Ÿ�� ����
        Transform target = Player.Instance.CharacterMove.GetTarget();

        if (target == null) return;

        // ����ü ����
        GameObject obj = Instantiate(projectile, this.transform);

        // Launcher�� ��ġ�� ������ ���� �ʵ��� �θ� �����Ͽ� ������ ����
        obj.transform.parent = this.transform.parent.parent.parent;

        // ����ü �߻�
        obj.GetComponent<Rigidbody>().velocity = this.transform.forward * 5.0f; 
    }

}
