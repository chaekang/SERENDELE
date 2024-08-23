using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public GameObject purchaseManager;
    public GameObject sellManager;
    public StorageManager storage;

    public TextMeshProUGUI money;

    public GameObject BuyPanel;
    public GameObject SellPanel;

    public bool marketBuy;

    public bool isMarketPanelActive = false;

    private void Update()
    {
        // MarketPanel의 활성화 상태를 확인하여 플래그 설정
        isMarketPanelActive = BuyPanel.activeSelf || SellPanel.activeSelf;

        if (isMarketPanelActive)
        {
            money.gameObject.SetActive(true);
        }
        else
        {
            money.gameObject.SetActive(false);
        }

        if (Inventory.instance.IsOpen())
        {
            ChoiceManager.Instance.choicePanel.gameObject.SetActive(false);
        }

        // 마우스 클릭을 감지하고 처리
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        // 마우스 클릭 위치에서 레이캐스트를 발사하여 오브젝트를 감지
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 클릭된 오브젝트가 "Market" 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag("MarketNPC"))
            {
                // 클릭된 오브젝트에서 Choice 스크립트를 가져옴
                Choice choiceData = hit.collider.GetComponent<Choice>();

                if (choiceData != null)
                {
                    // ChoiceManager를 통해 선택지를 표시하고, 선택 결과에 따른 동작을 처리
                    ChoiceManager.Instance.ShowChoice(choiceData);
                    StartCoroutine(WaitForMarketChoice());
                }
            }
            else if (hit.collider.CompareTag("HotelNPC"))
            {
                // 클릭된 오브젝트에서 Choice 스크립트를 가져옴
                Choice choiceData = hit.collider.GetComponent<Choice>();

                if (choiceData != null)
                {
                    // ChoiceManager를 통해 선택지를 표시하고, 선택 결과에 따른 동작을 처리
                    ChoiceManager.Instance.ShowChoice(choiceData);
                    StartCoroutine(WaitForHotelChoice());
                }
            }
            else if (hit.collider.CompareTag("NPC"))
            {
                GameObject clickedObject = hit.collider.gameObject;
                TalkManager.Instance.Action(clickedObject);
            }
        }
    }

    // 사용자가 선택을 마칠 때까지 대기하는 코루틴
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
                BuyPanel.SetActive(true);
                Debug.Log("Result: " + result);
                break;
            case 2:
                SellPanel.SetActive(true);
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