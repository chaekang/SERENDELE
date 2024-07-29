using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength = 1440f; // 하루의 길이 (24시간을 초로 표현, 1시간 = 60초)
    public float startTime = 0.25f; // 0.25f = 6 AM (start at 6 AM)
    private float timeRate;
    public Vector3 noon = new Vector3(90f, 0f, 0f); // 자정의 각도

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        // Increment time percentage
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity, true);
        UpdateLighting(moon, moonColor, moonIntensity, false);

        // Update ambient and reflection intensity
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    private void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve, bool isSun)
    {
        // Update light rotation
        lightSource.transform.eulerAngles = (time - (isSun ? 0.25f : 0.75f)) * noon * 4.0f;

        // Update light color and intensity
        lightSource.color = colorGradient.Evaluate(time);
        lightSource.intensity = intensityCurve.Evaluate(time);

        // Control light activation based on time
        GameObject go = lightSource.gameObject;
        if (isSun)
        {
            // Sun active from 6 AM (0.25) to 8 PM (0.8333)
            if (time >= 0.25f && time <= 0.8333f)
            {
                if (!go.activeInHierarchy) go.SetActive(true);
            }
            else
            {
                if (go.activeInHierarchy) go.SetActive(false);
            }
        }
        else
        {
            // Moon active from 3 PM (0.625) to 8 AM (0.3333)
            if (time >= 0.625f || time <= 0.3333f)
            {
                if (!go.activeInHierarchy) go.SetActive(true);
            }
            else
            {
                if (go.activeInHierarchy) go.SetActive(false);
            }
        }
    }
}
