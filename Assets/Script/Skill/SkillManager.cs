using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillName
{
    MultiShot, // 0
    FrontShot,
    BackShot,
    SideShot,
    DiagonalShot,
    HeadShot, // 5
    PierceShot,
    PowerUp,
    MoveSpeedUp,
    AttackSpeedUp,
    ExpUp, // 10
    HPBoost,
    Rage,
    Fury,
    Revival, // 14
    Healing
}

public class SkillManager : MonoBehaviour
{    
    int frontShotNum = 1;
    int multiShotNum = 0;

    public void ActiveSkill(SkillName skillName)
    {
        switch (skillName)
        {
            case SkillName.MultiShot:
                MultiShot();
                break;
            case SkillName.FrontShot:
                FrontShot();
                break;
            case SkillName.BackShot:
                BackShot();
                break;
            case SkillName.SideShot:
                SideShot();
                break;
            case SkillName.DiagonalShot:
                DiagonalShot();
                break;
            case SkillName.HeadShot:
                HeadShot();
                break;
            case SkillName.PierceShot:
                PierceShot();
                break;
            case SkillName.PowerUp:
                PowerUp();
                break;
            case SkillName.MoveSpeedUp:
                MoveSpeedUp();
                break;
            case SkillName.AttackSpeedUp:
                AttackSpeedUp();
                break;
            case SkillName.ExpUp:
                ExpUP();
                break;
            case SkillName.HPBoost:
                HPBoost();
                break;
            case SkillName.Rage:
                Rage();
                break;
            case SkillName.Fury:
                Fury();
                break;
            case SkillName.Revival:
                Revival();
                break;
            case SkillName.Healing:
                Healing();
                break;

        }
    }

    public void ActiveSkill(int skillNum)
    {
        switch (skillNum)
        {
            case 0:
                MultiShot();
                break;
            case 1:
                FrontShot();
                break;
            case 2:
                BackShot();
                break;
            case 3:
                SideShot();
                break;
            case 4:
                DiagonalShot();
                break;
            case 5:
                HeadShot();
                break;
            case 6:
                PierceShot();
                break;
            case 7:
                PowerUp();
                break;
            case 8:
                MoveSpeedUp();
                break;
            case 9:
                AttackSpeedUp();
                break;
            case 10:
                ExpUP();
                break;
            case 11:
                HPBoost();
                break;
            case 12:
                Rage();
                break;
            case 13:
                Fury();
                break;
            case 14:
                Revival();
                break;
            case 15:
                Healing();
                break;
        }
    }

    void MultiShot()
    {
        if (multiShotNum == 0)
        {
            // ?????? ???? ?? ????, ?????? 10??, ???? 15?? ????
            Player.Instance.DoubleShot();
            multiShotNum++;
        }
        else if (multiShotNum == 1)
        {
            Player.Instance.TripleShot();
            multiShotNum++;
        }

        Player.Instance.UpdateATK(-Player.Instance.GetATK() * 0.1f);
        Player.Instance.UpdateATK_SPEED(-Player.Instance.GetATK_SPEED() * 0.15f);
    }

    void FrontShot()
    {
        if (frontShotNum == 1)
        {
            // ?????? ???? ????
            Player.Instance.ActiveLauncher(LauncherType.Front);
            frontShotNum++;
        }
        else if (frontShotNum == 2)
        {
            // ?????? ???? ????
            Player.Instance.ActiveLauncher(LauncherType.Front2);
            frontShotNum++;
        }

        // ?????? 25?? ????
        Player.Instance.UpdateATK(-Player.Instance.GetATK() * 0.25f);
    }

    void BackShot()
    {
        // ???? ???? ???? ???? ????
        Player.Instance.ActiveLauncher(LauncherType.Back);
    }

    void SideShot()
    {
        // ?? ?????? ???? ???? ?????? ????
        Player.Instance.ActiveLauncher(LauncherType.Left);
        Player.Instance.ActiveLauncher(LauncherType.Right);
    }

    void DiagonalShot() // ?????? ??
    {
        // ???????? 2?? ???? ????
        Player.Instance.ActiveLauncher(LauncherType.UpLeft);
        Player.Instance.ActiveLauncher(LauncherType.UpRight);
    }

    void HeadShot()
    {
        // 10?????? ?????? ?????? ????
        Player.Instance.onHeadShot = true;
    }

    void PierceShot() // ??????
    {
        // ???????? ?? ????, ?????? 33?? ????
        Player.Instance.onPierceShot = true;
        Player.Instance.UpdateATK(-Player.Instance.GetATK() * 0.33f);
    }

    void PowerUp()
    {
        // ?????? 30?? ????
        Player.Instance.UpdateATK(Player.Instance.GetATK() * 0.3f);
    }

    void MoveSpeedUp()
    {
        // ???? 15?? ????
        Player.Instance.UpdateMOVE_SPEED(Player.Instance.GetMOVE_SPEED() * 0.15f);
    }

    void AttackSpeedUp()
    {
        // ???? 25?? ????
        Player.Instance.UpdateATK_SPEED(Player.Instance.GetATK_SPEED() * 0.25f);
    }

    void ExpUP()
    {
        // ?????? ?????? 30?? ????, ???????? 2????
        Player.Instance.MaxLvUp(2);
        Player.Instance.UpdateExp(-Player.Instance.GetExp() * 0.3f);
    }

    void HPBoost()
    {
        // ???????? 20?? ????
        Player.Instance.UpdateMaxHP(Player.Instance.GetMaxHP() * 0.2f);
    }

    void Rage() // ????
    {
        // ???? ???? 1???? ?????? 1?? ????
        Player.Instance.onRage = true;
        Player.Instance.SaveATK();
        Player.Instance.UpdateHP();
    }

    void Fury() // ????
    {
        // ???? ???? 1???? ???? 0.4?? ????
        Player.Instance.onFury = true;        
        Player.Instance.SaveATK_SPEED();
        Player.Instance.UpdateHP();
    }

    void Revival() // ????
    {
        // ???? +1
        Player.Instance.UpdateLife(1);        
    }

    void Healing() // ??
    {
        // ???? ?????? 40?? ????
        Player.Instance.UpdateHP(Player.Instance.GetMaxHP() * 0.4f);
    }
}
