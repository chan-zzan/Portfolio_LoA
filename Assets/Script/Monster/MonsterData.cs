using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterData", order = int.MinValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField] float ATK; // 공격력
    [SerializeField] float MaxHP; // 최대체력

    public float GetATK() => ATK;
    public float GetMaxHP() => MaxHP;
}
