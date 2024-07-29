using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeTxt; // Text component to display time

    private DayNightCycle dayNightCycle; // Reference to DayNightCycle script

    private float lastTime = -1f; // Stores the last time value to detect changes
    private const float updateInterval = 10f / (24f * 60f); // 10 minutes as a fraction of a day

    private void Start()
    {
        // Find the DayNightCycle script in the scene
        dayNightCycle = FindObjectOfType<DayNightCycle>();

        if (dayNightCycle == null)
        {
            Debug.LogError("DayNightCycle script not found in the scene.");
        }
        else
        {
            UpdateTimeDisplay(); // Initial update
        }
    }

    private void Update()
    {
        if (dayNightCycle != null)
        {
            // Check if the time has progressed by at least the update interval (10 in-game minutes)
            if (Mathf.Abs(dayNightCycle.time - lastTime) >= updateInterval)
            {
                lastTime = dayNightCycle.time;
                UpdateTimeDisplay();
            }
        }
    }

    private void UpdateTimeDisplay()
    {
        // Calculate the in-game hours and minutes
        float timePercent = dayNightCycle.time;
        int totalMinutes = Mathf.FloorToInt(timePercent * 24 * 60);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        // Format the time as "HH:MM" and display it
        timeTxt.text = $"{hours:00}:{minutes:00}";
    }
}
