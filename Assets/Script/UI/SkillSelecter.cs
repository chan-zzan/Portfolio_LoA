using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSelecter : MonoBehaviour
{
    [SerializeField] SkillManager skillManager;

    [SerializeField] List<SkillData> SkillData; // ��ų���� ������ ����ִ� ����Ʈ

    [SerializeField] List<RectTransform> SlotObjects; // ��ų �̹����� ��� �ִ� ������Ʈ
    [SerializeField] List<Sprite> UsingSprites; // ���Ǵ� sprite

    [SerializeField] TMP_Text[] SkillName; // ��ų�� �̸��� ǥ���� �ؽ�Ʈ �迭

    //float slotSpeed = 6000.0f;

    //int playSlotNum = 0;
    //int stopSlotNum = -1;

    //bool play = false;

    int perSlotItemNum = 5; // ���Դ� ������ ����

    int skillNum = 0; // ������ �ִ� ��ų ����

    private void OnEnable()
    {
        // �ִϸ��̼� ��
        for (int i = 0; i < SlotObjects.Count; i++)
        {
            SlotObjects[i].GetComponent<Animator>().enabled = false;
        }

        GameManager.Instance.PauseScene(); // ���� �Ͻ�����

        //play = true;
        //playSlotNum = 0;
        //stopSlotNum = -1;

        foreach (Button btn in this.GetComponentsInChildren<Button>())
        {
            // ��ų ��ư ��Ȱ��ȭ
            btn.interactable = false;
        }

        for (int i = 0; i < SkillData.Count; i++)
        {
            UsingSprites.Add(SkillData[i].GetSprite()); // ��ų���� sprite ������ ������
        }

        SlotSpriteSetting(); // �������� sprite ����

        // ������ ����
        StartCoroutine(SlotRotating(0, 1, 4));
        StartCoroutine(SlotRotating(1, 4, 8));
        StartCoroutine(SlotRotating(2, 3, 12));
    }

    IEnumerator SlotRotating(int slotNum, int stopPos, int rotateNum)
    {
        for (int i = 0; i < (perSlotItemNum * rotateNum * 2) + (stopPos * 2); i++) // (���Դ� ������ ���� x ȸ���� x �׸��� �ٲ�� Ƚ��) + ���� ��ġ(2����)
        {
            SlotObjects[slotNum].anchoredPosition3D -= new Vector3(0, 125.0f, 0);

            if (SlotObjects[slotNum].anchoredPosition3D.y <= 125.0f)
            {
                SlotObjects[slotNum].anchoredPosition3D += new Vector3(0, 1250.0f, 0);
            }

            yield return null;
        }

        // ���� �� ���� �ִϸ��̼� �̿�
        SlotObjects[slotNum].GetComponent<Animator>().enabled = true;

        if (slotNum == 2)
        {
            foreach (Button btn in this.GetComponentsInChildren<Button>())
            {
                // ��ų ��ư Ȱ��ȭ
                btn.interactable = true;
            }
        }

    }

    /*
    IEnumerator StopControl(int slotNum)
    {
        if (slotNum == 0)
        {
            //ó�� �����Ҷ��� ���� �� ���
           yield return new WaitForSecondsRealtime(2.5f);
        }
        else
        {
            //ó�� ���Ŀ��� ������ ����
            yield return new WaitForSecondsRealtime(1.2f);
        }

        playSlotNum++;

        Vector3 slotPos = SlotObjects[slotNum].anchoredPosition3D; // ���߷��� ����
        float stopSpeed = 6000.0f;

        while (stopSpeed > 500.0f)
        {
            float delta = Time.unscaledDeltaTime * stopSpeed;

            slotPos.y -= delta;

            while (slotPos.y <= 100.0f)
            {
                print(slotNum + " : " + "a");
                slotPos.y += 1250.0f;
            }

            SlotObjects[slotNum].anchoredPosition3D = slotPos;

            stopSpeed -= Time.unscaledDeltaTime * 6000;

            yield return null;
        }
        print(slotNum + " : " + SlotObjects[slotNum].anchoredPosition3D.y);

        stopSlotNum++;

        if (playSlotNum < 3)
        {
            yield return StartCoroutine(StopControl(playSlotNum));
        }

        if (stopSlotNum == 2)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            stopSlotNum++; // ���̻� ������ ���� ����
        }
    }
    */

    /*
    void PlaySlot()
    {
        if (playSlotNum > 3)
        {
            play = false;
        }

        //������ ����
        for (int slotNum = playSlotNum; slotNum < SlotObjects.Count; slotNum++)
        {
            Vector3 slotPos = SlotObjects[slotNum].anchoredPosition3D;

            slotPos.y -= Time.unscaledDeltaTime * slotSpeed;

            while (slotPos.y <= 100.0f)
            {
                slotPos.y += 1250.0f;
            }

            SlotObjects[slotNum].anchoredPosition3D = slotPos;
        }

    }
    */

    /*
    void StopSlot()
    {
        if (stopSlotNum < 3)
        {
            //������ ����
            Vector3 curPos = SlotObjects[stopSlotNum].anchoredPosition3D; // ������ġ

            if (curPos.y % 250 != 0)
            {
                //������ ��ġ ���� ����
                if (curPos.y % 250 > 125.0f)
                {
                    //slot up -> ������ ���� �̵��ϸ鼭 ĭ�� �� ������
                    curPos.y += Time.unscaledDeltaTime * 100.0f;

                    if (curPos.y % 250 >= 248)
                    {
                        curPos.y = 250 * ((int)(curPos.y / 250) + 1);
                        ActiveSkillButton(); // ��ų ��ư Ȱ��ȭ
                    }
                }
                else if (curPos.y % 250 <= 125.0f)
                {
                    //slot down -> ������ �Ʒ��� �̵��ϸ鼭 ĭ�� �� ������
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
    */

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

    /*
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
    */

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
