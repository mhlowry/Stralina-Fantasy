using UnityEngine;
using TMPro;

public class PulsatingText : MonoBehaviour
{
    public float minAlpha = 0f;
    public float maxAlpha = 1f;
    public float pulseSpeed = 1f;
    public float startDelay = 2.0f; // Delay before starting to pulsate

    public TypewriterEffect typewriterEffect;
    public ResponseHandler responseHandler;

    private TMP_Text tmpText;
    private CanvasGroup canvasGroup;
    private float timeSinceStarted;
    private bool delayCompleted;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;  // Start with transparent text
        timeSinceStarted = 0;
        delayCompleted = false;
    }

    void Update()
    {
        // Check if the typewriter effect is done and there are no response events
        if (!typewriterEffect.IsRunning && !responseHandler.HasActiveResponseEvents())
        {
            if (!delayCompleted)
            {
                timeSinceStarted += Time.deltaTime;
                if (timeSinceStarted >= startDelay)
                {
                    delayCompleted = true;
                    timeSinceStarted = -Mathf.PI / 2; // Set phase so alpha starts at 0
                }
            }
            else
            {
                // Calculate the alpha value
                float alpha = (Mathf.Sin(timeSinceStarted * pulseSpeed) + 1) / 2 * (maxAlpha - minAlpha) + minAlpha;
                timeSinceStarted += Time.deltaTime;

                // Apply the alpha value to the CanvasGroup
                canvasGroup.alpha = alpha;
            }
        }
        else
        {
            // Reset alpha to transparent when not pulsating and reset the timer
            canvasGroup.alpha = 0;
            timeSinceStarted = 0;
            delayCompleted = false;
        }
    }
}
