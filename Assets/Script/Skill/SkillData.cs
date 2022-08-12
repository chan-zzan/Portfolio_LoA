using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SkillData", menuName = "SkillData", order = int.MinValue + 1)]
public class SkillData : ScriptableObject
{
    [SerializeField] SkillName Name;
    [SerializeField] Sprite Sprite;

    public SkillName GetName() => Name;

    public Sprite GetSprite() => Sprite;
}
