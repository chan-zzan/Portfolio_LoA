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
    ExpUP, // 10
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
            case SkillName.ExpUP:
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
            // 화살을 한발 더 발사, 공격력 10퍼, 공속 15퍼 감소
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
            // 앞으로 두발 발사
            Player.Instance.ActiveLauncher(LauncherType.Front);
            frontShotNum++;
        }
        else if (frontShotNum == 2)
        {
            // 앞으로 세발 발사
            Player.Instance.ActiveLauncher(LauncherType.Front2);
            frontShotNum++;
        }

        // 공격력 25퍼 감소
        Player.Instance.UpdateATK(-Player.Instance.GetATK() * 0.25f);
    }

    void BackShot()
    {
        // 뒤로 쏘는 화살 한발 추가
        Player.Instance.ActiveLauncher(LauncherType.Back);
    }

    void SideShot()
    {
        // 양 옆으로 쏘는 화살 한발씩 추가
        Player.Instance.ActiveLauncher(LauncherType.Left);
        Player.Instance.ActiveLauncher(LauncherType.Right);
    }

    void DiagonalShot() // 대각선 샷
    {
        // 사선으로 2발 추가 발사
        Player.Instance.ActiveLauncher(LauncherType.UpLeft);
        Player.Instance.ActiveLauncher(LauncherType.UpRight);
    }

    void HeadShot()
    {
        // 10퍼센트 확률로 몬스터 즉사
        Player.Instance.onHeadShot = true;
    }

    void PierceShot() // 관통샷
    {
        // 발사체가 적 관통, 데미지 33퍼 감소
        Player.Instance.onPierceShot = true;
        Player.Instance.UpdateATK(-Player.Instance.GetATK() * 0.33f);
    }

    void PowerUp()
    {
        // 공격력 30퍼 증가
        Player.Instance.UpdateATK(Player.Instance.GetATK() * 0.3f);
    }

    void MoveSpeedUp()
    {
        // 이속 15퍼 증가
        Player.Instance.UpdateMOVE_SPEED(Player.Instance.GetMOVE_SPEED() * 0.15f);
    }

    void AttackSpeedUp()
    {
        // 공속 25퍼 증가
        Player.Instance.UpdateATK_SPEED(Player.Instance.GetATK_SPEED() * 0.25f);
    }

    void ExpUP()
    {
        // 경험치 획득량 30퍼 증가, 최대레벨 2증가
        Player.Instance.UpdateExp(-Player.Instance.GetExp() * 0.3f);
        Player.Instance.MaxLvUp(2);
    }

    void HPBoost()
    {
        // 최대체력 20퍼 증가
        Player.Instance.UpdateMaxHP(Player.Instance.GetMaxHP() * 0.2f);
    }

    void Rage() // 분노
    {
        // 잃은 체력 1퍼당 공격력 1퍼 증가
        Player.Instance.onRage = true;
        Player.Instance.SaveATK();
        Player.Instance.UpdateHP();
    }

    void Fury() // 격노
    {
        // 잃은 체력 1퍼당 공속 0.4퍼 증가
        Player.Instance.onFury = true;        
        Player.Instance.SaveATK_SPEED();
        Player.Instance.UpdateHP();
    }

    void Revival() // 부활
    {
        // 생명 +1
        Player.Instance.UpdateLife(1);        
    }

    void Healing() // 힐
    {
        // 최대 체력의 40퍼 회복
        Player.Instance.UpdateHP(Player.Instance.GetMaxHP() * 0.4f);
    }
}
