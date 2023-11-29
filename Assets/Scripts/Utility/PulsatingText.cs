using UnityEngine;
using TMPro;

public class PulsatingText : MonoBehaviour
{
    public float minAlpha = 0.5f;
    public float maxAlpha = 1.0f;
    public float pulseSpeed = 1.5f;

    private TMP_Text tmpText;
    private Color originalColor;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        originalColor = tmpText.color; // Store the original color
    }

    void Update()
    {
        // Calculate the alpha value
        float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2 * (maxAlpha - minAlpha) + minAlpha;

        // Apply the alpha value
        tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }
}
