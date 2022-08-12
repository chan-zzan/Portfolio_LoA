using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelecter : MonoBehaviour
{
    [SerializeField] SkillManager skillManager;

    [SerializeField] List<SkillData> SkillData; // ��ų���� ������ ����ִ� ����Ʈ

    [SerializeField] List<RectTransform> SlotObjects; // ��ų �̹����� ��� �ִ� ������Ʈ
    [SerializeField] List<Sprite> UsingSprites; // ���Ǵ� sprite

    float slotSpeed = 5000.0f;

    int playSlotNum = 0;
    int stopSlotNum = -1;

    bool play = false;

    int skillNum = 0; // ������ �ִ� ��ų ����

    private void OnEnable()
    {
        GameManager.Instance.PauseScene(); // ���� �Ͻ�����

        play = true;
        playSlotNum = 0;
        stopSlotNum = -1;

        foreach (Button btn in this.GetComponentsInChildren<Button>())
        {
            // ��ų ��ư ��Ȱ��ȭ
            btn.interactable = false;
        }

        for (int i = 0; i < SkillData.Count; i++)
        {
            UsingSprites.Add(SkillData[i].GetSprite()); // ��ų���� sprite ������ ������
        }

        SlotSpriteSetting();
        StartCoroutine(StopControl(playSlotNum));
    }

    private void Update()
    {
        if (play)
        {
            PlaySlot();
        }

        if (stopSlotNum >= 0)
        {
            StopSlot();
        }

    }

    IEnumerator StopControl(int slotNum)
    {
        if (slotNum == 0)
        {
            // ó�� �����Ҷ��� ���� �� ���
            yield return new WaitForSecondsRealtime(2.5f);
        }
        else
        {
            // ó�� ���Ŀ��� ������ ����
            yield return new WaitForSecondsRealtime(1.0f);
        }

        playSlotNum++;

        Vector3 slotPos = SlotObjects[slotNum].anchoredPosition3D; // ���߷��� ����
        float stopSpeed = 5000.0f;

        while (stopSpeed > 500.0f)
        {
            float delta = Time.unscaledDeltaTime * stopSpeed;

            if (slotPos.y <= 100.0f)
            {
                slotPos.y += 1250.0f;
            }

            slotPos.y -= delta;
            SlotObjects[slotNum].anchoredPosition3D = slotPos;

            stopSpeed -= Time.unscaledDeltaTime * 5000;

            yield return null;
        }

        stopSlotNum++;

        if (playSlotNum < 3)
        {
            yield return StartCoroutine(StopControl(playSlotNum));
        }

        if (stopSlotNum == 2)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            stopSlotNum++;          
        }
    }

    void PlaySlot()
    {
        if (playSlotNum > 3)
        {
            play = false;
        }

        // ������ ����
        for (int slotNum = playSlotNum; slotNum < SlotObjects.Count; slotNum++)
        {
            Vector3 slotPos = SlotObjects[slotNum].anchoredPosition3D;

            if (slotPos.y <= 100.0f)
            {
                slotPos.y += 1250.0f;
            }

            slotPos.y -= Time.unscaledDeltaTime * slotSpeed;            

            SlotObjects[slotNum].anchoredPosition3D = slotPos;
        }

    }

    void StopSlot()
    {
        if (stopSlotNum < 3)
        {
            // ������ ����
            Vector3 curPos = SlotObjects[stopSlotNum].anchoredPosition3D; // ������ġ

            if (curPos.y % 250 != 0)
            {
                // ������ ��ġ ���� ����
                if (curPos.y % 250 > 125.0f)
                {
                    // slot up -> ������ ���� �̵��ϸ鼭 ĭ�� �� ������
                    curPos.y += Time.unscaledDeltaTime * 100.0f;

                    if (curPos.y % 250 >= 248)
                    {                        
                        curPos.y = 250 * ((int)(curPos.y / 250) + 1);
                        ActiveSkillButton(); // ��ų ��ư Ȱ��ȭ
                    }
                }
                else if (curPos.y % 250 <= 125.0f)
                {
                    // slot down -> ������ �Ʒ��� �̵��ϸ鼭 ĭ�� �� ������
                    curPos.y -= Time.unscaledDeltaTime * 100.0f;

                    if (curPos.y % 250 <= 2)
                    {
                        curPos.y = 250 * (int)(curPos.y / 250);
                        ActiveSkillButton(); // ��ų ��ư Ȱ��ȭ
                    }
                }

                SlotObjects[stopSlotNum].anchoredPosition3D = curPos;
            }
        }        
    }

    void SlotSpriteSetting()
    {
        // ������ �̹��� ����
        for (int slotNum = 0; slotNum < SlotObjects.Count; slotNum++)
        {
            for (int i = 0; i < SlotObjects[slotNum].childCount; i++)
            {
                if (i == SlotObjects[slotNum].childCount - 1)
                {
                    // ������ �̹����� ó�� �̹����� �������ؼ� �׸��� ���ӵǴ� ��ó�� ���̵��� ��
                    SlotObjects[slotNum].GetComponentsInChildren<Image>()[i].sprite = SlotObjects[slotNum].GetComponentsInChildren<Image>()[0].sprite;
                    continue;
                }

                // ���� �̹��� ����
                int rndNum = Random.Range(0, UsingSprites.Count);
                SlotObjects[slotNum].GetComponentsInChildren<Image>()[i].sprite = UsingSprites[rndNum];

                // ���� �̹��� ����
                UsingSprites.RemoveAt(rndNum);
            }
        }
    }

    void ActiveSkillButton()
    {
        if (stopSlotNum == 2)
        {
            foreach (Button btn in this.GetComponentsInChildren<Button>())
            {
                // ��ų ��ư Ȱ��ȭ
                btn.interactable = true;
            }
        }
    }

    public void SelectSkill(int slotNum)
    {
        Vector3 curPos = SlotObjects[slotNum].anchoredPosition3D; // ������ ������ ��ġ
        int num = Mathf.RoundToInt(curPos.y / 10.0f) * 10; // ��ġ�� �ݿø��ؼ� 250������ ����

        // ������ ��ų�� sprite�� ����
        Sprite curSprite = SlotObjects[slotNum].GetComponentsInChildren<Image>()[num / 250].sprite;

        // ���� ��ų�� ���� -> pause�� �����ֱ� ����
        UIManager.Instance.GainedSkills[skillNum].color = new Color(1, 1, 1, 1); // ���İ��� ������� ������ ���� ���̵��� ��
        UIManager.Instance.GainedSkills[skillNum++].sprite = curSprite; // �̹��� ����

        string skillName = curSprite.name; // �ش� sprite�� �����Ͽ� �̸��� ����

        for (int i = 0; i < SkillData.Count; i++)
        {
            // ��ų�� �������� ���� ��ų�� �̸��� ã��
            if (SkillData[i].GetName().ToString() == skillName)
            {
                // ��ų �ߵ�
                //print(SkillData[i].GetName());
                skillManager.ActiveSkill(SkillData[i].GetName());
                break;
            }
        }

        // ��ų ���� �� â ����
        this.transform.parent.gameObject.SetActive(false);
        GameManager.Instance.RePlay(); // ���� �Ͻ����� ����
    }
}
