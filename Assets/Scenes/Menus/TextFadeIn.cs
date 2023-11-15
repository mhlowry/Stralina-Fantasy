using UnityEngine;
using TMPro;
using System.Collections;

public class TextAndButtonsFadeIn : MonoBehaviour
{
    public TextMeshProUGUI textToFade; 
    public GameObject buttonsContainer; // Reference to the container with all buttons
    public float fadeInDuration = 3.0f;

    private void Start()
    {
        // Initialize title to be transparent
        SetAlpha(textToFade, 0);

        // Initialize button texts to be transparent
        foreach (Transform child in buttonsContainer.transform)
        {
            TextMeshProUGUI buttonText = child.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText)
            {
                SetAlpha(buttonText, 0);
            }
        }

        // Start the coroutine to handle both fade-ins
        StartCoroutine(FadeInSequence());
    }

    private void SetAlpha(TextMeshProUGUI textElement, float alpha)
    {
        if (textElement != null)
        {
            Color color = textElement.color;
            color.a = alpha;
            textElement.color = color;
        }
    }

    IEnumerator FadeInSequence()
    {
        // Fade in the title text first
        yield return StartCoroutine(FadeInText(textToFade));

        // Now fade in each button text within the ButtonsContainer
        foreach (Transform child in buttonsContainer.transform)
        {
            TextMeshProUGUI buttonText = child.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText)
            {
                yield return StartCoroutine(FadeInText(buttonText));
            }
        }
    }

    IEnumerator FadeInText(TextMeshProUGUI textElement)
    {
        float elapsed = 0.0f;
        Color initialColor = textElement.color;
        Color targetColor = initialColor;
        targetColor.a = 1; // target alpha is opaque

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            textElement.color = Color.Lerp(initialColor, targetColor, elapsed / fadeInDuration);
            yield return null;
        }

        textElement.color = targetColor; // ensure fully opaque at the end
    }
}

