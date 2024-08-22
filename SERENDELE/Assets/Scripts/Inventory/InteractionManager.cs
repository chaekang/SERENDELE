using Firebase.Auth;
using TMPro;
using UnityEngine;
using Cinemachine;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract();
}

public class InteractionManager : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    private GameObject curInteractGameobject;
    private IInteractable curInteractable;

    public GameObject promptBg;
    public TextMeshProUGUI promptText;
    public CinemachineVirtualCamera virtualCamera;

    private Camera mainCamera;

    void Update()
    {
        if (!Inventory.instance.inventoryWindow.activeInHierarchy)
        {
            // E 키 입력 시 인벤토리로
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        // 마지막으로 체크한 시간이 checkRate를 넘겼다면
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            CheckForInteractable();
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera가 할당되지 않았습니다.");
            return;
        }
    }

    private void CheckForInteractable()
    {
        // 화면의 정 중앙에 상호작용 가능한 물체가 있는지 확인하기
        Ray centerRay = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));    // 화면의 정 중앙에서 Ray를 쏘겠다.
        Ray upRay = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2 + Mathf.Tan(15 * Mathf.Deg2Rad) * maxCheckDistance));
        Ray downRay = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2 - Mathf.Tan(15 * Mathf.Deg2Rad) * maxCheckDistance));

        bool hitDetected = PerformRaycast(centerRay) || PerformRaycast(upRay) || PerformRaycast(downRay);

        if (!hitDetected)
        {
            curInteractGameobject = null;
            curInteractable = null;
            promptBg.gameObject.SetActive(false);
        }
    }

    private bool PerformRaycast(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
        {
            // 디버그 라인을 사용하여 Ray를 시각화합니다.
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

            // 부딪힌 오브젝트가 우리가 저장해놓은 상호작용이 가능한 오브젝트들인지 확인하기
            if (hit.collider.gameObject != curInteractGameobject)
            {
                // 충돌한 물체 가져오기
                curInteractGameobject = hit.collider.gameObject;
                curInteractable = hit.collider.GetComponent<IInteractable>();
                SetPromptText();
            }
            return true;
        }
        else
        {
            // 디버그 라인을 사용하여 Ray를 시각화합니다.
            Debug.DrawRay(ray.origin, ray.direction * maxCheckDistance, Color.red);
            return false;
        }
    }

    private void SetPromptText()
    {
        promptBg.gameObject.SetActive(true);
        promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt());     // <b></b> : 태그, 마크다운 형식 <b>의 경우 볼드체.
    }

    private void Interact()
    {
        if (curInteractable != null)
        {
            curInteractable.OnInteract();
        }
    }
}