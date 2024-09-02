using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Relic_1 : MonoBehaviour, IInteractable
{
    public Image parchment;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        parchment.gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        parchment.gameObject.SetActive(true);

        Color color = parchment.color;
        color.a = 150f / 255f;  // 초기 투명도 설정
        color.r = 225f / 225f;
        color.g = 225f / 225f;
        color.b = 255f / 255f;  // 초기 b 값 설정
        parchment.color = color;

        // 투명도 150에서 255까지, b 값 160에서 255까지 2초 동안 변경
        float duration = 2f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(150f / 255f, 1f, elapsedTime / duration);
            color.r = Mathf.Lerp(1f, 245f / 255f, elapsedTime / duration);
            color.g = Mathf.Lerp(1f, 244f / 255f, elapsedTime / duration);
            color.b = Mathf.Lerp(1f, 220f / 255f, elapsedTime / duration);
            parchment.color = color;
            yield return null;
        }

        color.a = 1f;
        color.b = 1f;
        parchment.color = color;

        // 3초 동안 유지
        yield return new WaitForSeconds(3.5f);

        // 다시 비활성화
        parchment.gameObject.SetActive(false);
    }

    public string GetInteractPrompt()
    {
        return "유물 확인하기";
    }
}
