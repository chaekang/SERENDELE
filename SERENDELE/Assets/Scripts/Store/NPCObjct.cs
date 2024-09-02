using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

public class NPCObjct : MonoBehaviour, INPCInteractable
{
    public NPCData npcData;
    public string GetInteractPrompt()
    {
        Debug.Log(npcData.NPCName);
        return string.Format("{0}과(와) 대화하기", npcData.NPCName);
    }

    public void OnInteract()
    {

    }

    public void HandleNPCInteraction(GameObject curInteractGameobject)
    {
        if (curInteractGameobject.CompareTag("MarketNPC"))
        {
            Choice choiceData = curInteractGameobject.GetComponent<Choice>();

            if (choiceData != null)
            {
                ChoiceManager.Instance.ShowChoice(choiceData);
                StartCoroutine(WaitForMarketChoice());
            }
        }
        else if (curInteractGameobject.CompareTag("HotelNPC"))
        {
            Choice choiceData = curInteractGameobject.GetComponent<Choice>();

            if (choiceData != null)
            {
                ChoiceManager.Instance.ShowChoice(choiceData);
                StartCoroutine(WaitForHotelChoice());
            }
        }
        else if (curInteractGameobject.CompareTag("NPC"))
        {
            TalkManager.Instance.Action(curInteractGameobject);
        }
    }

    private IEnumerator WaitForMarketChoice()
    {
        // 사용자 입력 대기
        while (ChoiceManager.Instance.choiceIng)
        {
            yield return null;
        }

        // 사용자가 H 키를 누를 때까지 대기
        while (!Input.GetKeyDown(KeyCode.H))
        {
            yield return null;
        }

        // 선택 결과에 따른 행동 처리
        int result = ChoiceManager.Instance.GetResult();

        switch (result)
        {
            case 1:
                GameManager.Instance.MarketManager.BuyPanel.SetActive(true);
                Debug.Log("Result: " + result);
                break;
            case 2:
                GameManager.Instance.MarketManager.SellPanel.SetActive(true);
                Debug.Log("Result: " + result);
                break;
            case 3:
                Debug.Log("Result: " + result);
                break;
            default:
                break;
        }

        ChoiceManager.Instance.ExitChoice();
    }

    private IEnumerator WaitForHotelChoice()
    {
        // 사용자 입력 대기
        while (ChoiceManager.Instance.choiceIng)
        {
            yield return null;
        }

        // 사용자가 H 키를 누를 때까지 대기
        while (!Input.GetKeyDown(KeyCode.H))
        {
            yield return null;
        }

        // 선택 결과에 따른 행동 처리
        int result = ChoiceManager.Instance.GetResult();

        switch (result)
        {
            case 1:
                MoneyManager.Spend(50);
                Debug.Log("Result: " + result);
                break;
            case 2:
                Debug.Log("Result: " + result);
                break;
            default:
                break;
        }

        ChoiceManager.Instance.ExitChoice();
    }
}
