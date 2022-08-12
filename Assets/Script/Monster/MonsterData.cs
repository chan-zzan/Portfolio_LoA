using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterData", order = int.MinValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField] float ATK; // ���ݷ�
    [SerializeField] float MaxHP; // �ִ�ü��

    public float GetATK() => ATK;
    public float GetMaxHP() => MaxHP;
}
