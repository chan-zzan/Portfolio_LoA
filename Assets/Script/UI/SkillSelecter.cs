using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSelecter : MonoBehaviour
{
    [SerializeField] SkillManager skillManager;

    [SerializeField] List<SkillData> SkillData; // 스킬들의 정보를 담고있는 리스트

    [SerializeField] List<RectTransform> SlotObjects; // 스킬 이미지를 담고 있는 오브젝트
    [SerializeField] List<Sprite> UsingSprites; // 사용되는 sprite

    [SerializeField] TMP_Text[] SkillName; // 스킬의 이름을 표시할 텍스트 배열

    //float slotSpeed = 6000.0f;

    //int playSlotNum = 0;
    //int stopSlotNum = -1;

    //bool play = false;

    int perSlotItemNum = 5; // 슬롯당 아이템 개수

    int skillNum = 0; // 가지고 있는 스킬 개수

    private void OnEnable()
    {
        // 애니메이션 끔
        for (int i = 0; i < SlotObjects.Count; i++)
        {
            SlotObjects[i].GetComponent<Animator>().enabled = false;
        }

        GameManager.Instance.PauseScene(); // 게임 일시정지

        //play = true;
        //playSlotNum = 0;
        //stopSlotNum = -1;

        foreach (Button btn in this.GetComponentsInChildren<Button>())
        {
            // 스킬 버튼 비활성화
            btn.interactable = false;
        }

        for (int i = 0; i < SkillData.Count; i++)
        {
            UsingSprites.Add(SkillData[i].GetSprite()); // 스킬들의 sprite 정보를 가져옴
        }

        SlotSpriteSetting(); // 랜덤으로 sprite 세팅

        // 슬롯을 돌림
        StartCoroutine(SlotRotating(0, 1, 4));
        StartCoroutine(SlotRotating(1, 4, 8));
        StartCoroutine(SlotRotating(2, 3, 12));
    }

    IEnumerator SlotRotating(int slotNum, int stopPos, int rotateNum)
    {
        for (int i = 0; i < (perSlotItemNum * rotateNum * 2) + (stopPos * 2); i++) // (슬롯당 아이템 개수 x 회전수 x 그림이 바뀌는 횟수) + 멈출 위치(2단위)
        {
            SlotObjects[slotNum].anchoredPosition3D -= new Vector3(0, 125.0f, 0);

            if (SlotObjects[slotNum].anchoredPosition3D.y <= 125.0f)
            {
                SlotObjects[slotNum].anchoredPosition3D += new Vector3(0, 1250.0f, 0);
            }

            yield return null;
        }

        // 멈출 때 슬롯 애니메이션 이용
        SlotObjects[slotNum].GetComponent<Animator>().enabled = true;

        if (slotNum == 2)
        {
            foreach (Button btn in this.GetComponentsInChildren<Button>())
            {
                // 스킬 버튼 활성화
                btn.interactable = true;
            }
        }

    }

    /*
    IEnumerator StopControl(int slotNum)
    {
        if (slotNum == 0)
        {
            //처음 시작할때는 조금 더 대기
           yield return new WaitForSecondsRealtime(2.5f);
        }
        else
        {
            //처음 이후에는 빠르게 진행
            yield return new WaitForSecondsRealtime(1.2f);
        }

        playSlotNum++;

        Vector3 slotPos = SlotObjects[slotNum].anchoredPosition3D; // 멈추려는 슬롯
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
            stopSlotNum++; // 더이상 슬롯이 동작 안함
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

        //슬롯을 돌림
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
            //슬롯을 멈춤
            Vector3 curPos = SlotObjects[stopSlotNum].anchoredPosition3D; // 현재위치

            if (curPos.y % 250 != 0)
            {
                //슬롯의 위치 오차 설정
                if (curPos.y % 250 > 125.0f)
                {
                    //slot up -> 슬롯이 위로 이동하면서 칸에 딱 맞춰짐
                    curPos.y += Time.unscaledDeltaTime * 100.0f;

                    if (curPos.y % 250 >= 248)
                    {
                        curPos.y = 250 * ((int)(curPos.y / 250) + 1);
                        ActiveSkillButton(); // 스킬 버튼 활성화
                    }
                }
                else if (curPos.y % 250 <= 125.0f)
                {
                    //slot down -> 슬롯이 아래로 이동하면서 칸에 딱 맞춰짐
                    curPos.y -= Time.unscaledDeltaTime * 100.0f;

                    if (curPos.y % 250 <= 2)
                    {
                        curPos.y = 250 * (int)(curPos.y / 250);
                        ActiveSkillButton(); // 스킬 버튼 활성화
                    }
                }

                SlotObjects[stopSlotNum].anchoredPosition3D = curPos;
            }
        }
    }
    */

    void SlotSpriteSetting()
    {
        // 슬롯의 이미지 선택
        for (int slotNum = 0; slotNum < SlotObjects.Count; slotNum++)
        {
            for (int i = 0; i < SlotObjects[slotNum].childCount; i++)
            {
                if (i == SlotObjects[slotNum].childCount - 1)
                {
                    // 마지막 이미지는 처음 이미지와 같도록해서 그림이 연속되는 것처럼 보이도록 함
                    SlotObjects[slotNum].GetComponentsInChildren<Image>()[i].sprite = SlotObjects[slotNum].GetComponentsInChildren<Image>()[0].sprite;
                    continue;
                }

                // 랜덤 이미지 설정
                int rndNum = Random.Range(0, UsingSprites.Count);
                SlotObjects[slotNum].GetComponentsInChildren<Image>()[i].sprite = UsingSprites[rndNum];

                // 사용된 이미지 삭제
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
                // 스킬 버튼 활성화
                btn.interactable = true;
            }
        }
    }
    */

    public void SelectSkill(int slotNum)
    {
        Vector3 curPos = SlotObjects[slotNum].anchoredPosition3D; // 선택한 슬롯의 위치
        int num = Mathf.RoundToInt(curPos.y / 10.0f) * 10; // 위치를 반올림해서 250단위로 맞춤

        // 선택한 스킬의 sprite를 저장
        Sprite curSprite = SlotObjects[slotNum].GetComponentsInChildren<Image>()[num / 250].sprite;

        // 얻은 스킬을 저장 -> pause시 보여주기 위함
        UIManager.Instance.GainedSkills[skillNum].color = new Color(1, 1, 1, 1); // 알파값을 원래대로 돌려서 눈에 보이도록 함
        UIManager.Instance.GainedSkills[skillNum++].sprite = curSprite; // 이미지 변경

        string skillName = curSprite.name; // 해당 sprite에 접근하여 이름을 저장

        for (int i = 0; i < SkillData.Count; i++)
        {
            // 스킬의 정보에서 같은 스킬의 이름을 찾음
            if (SkillData[i].GetName().ToString() == skillName)
            {
                // 스킬 발동
                //print(SkillData[i].GetName());
                skillManager.ActiveSkill(SkillData[i].GetName());
                break;
            }
        }

        // 스킬 선택 후 창 닫음
        this.transform.parent.gameObject.SetActive(false);
        GameManager.Instance.RePlay(); // 게임 일시정지 해제
    }
}
