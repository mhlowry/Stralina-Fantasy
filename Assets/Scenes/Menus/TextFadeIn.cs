using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TextAndButtonsFadeIn : MonoBehaviour
{
    public TextMeshProUGUI textToFade; 
    public GameObject buttonsContainer; // Reference to the container with all buttons
    public float fadeInDuration = 3.0f;
    public Color normalColor = Color.white; // Default color
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f); // Off-white color for hover

    private void Start()
    {
        // Initialize title to be transparent
        Color titleColor = textToFade.color;
        titleColor.a = 0;
        textToFade.color = titleColor;

        // Initialize button texts to be transparent
        foreach (Transform child in buttonsContainer.transform)
        {
            // Fade text
            TextMeshProUGUI buttonText = child.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText)
            {
                Color textColor = buttonText.color;
                textColor.a = 0;
                buttonText.color = textColor;
            }
        }

        // Start the coroutine to handle both fade-ins
        StartCoroutine(FadeInSequence());
    }

    IEnumerator FadeInSequence()
    {
        // Fade in the title text first
        yield return FadeTitleIn();

        // Now fade in each button text within the ButtonsContainer
        foreach (Transform child in buttonsContainer.transform)
        {
            yield return FadeButtonIn(child);
        }
    }

    IEnumerator FadeTitleIn()
    {
        float elapsed = 0.0f;
        Color initialColor = textToFade.color;
        Color targetColor = initialColor;
        targetColor.a = 1;  // target alpha is opaque

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            textToFade.color = Color.Lerp(initialColor, targetColor, elapsed / fadeInDuration);
            yield return null;
        }

        textToFade.color = targetColor;  // ensure fully opaque at the end
    }

    IEnumerator FadeButtonIn(Transform buttonTransform)
    {
        float elapsed = 0.0f;

        TextMeshProUGUI buttonText = buttonTransform.GetComponentInChildren<TextMeshProUGUI>();
        Color initialTextColor = buttonText.color;
        Color targetTextColor = initialTextColor;
        targetTextColor.a = 1;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            buttonText.color = Color.Lerp(initialTextColor, targetTextColor, elapsed / fadeInDuration);
            yield return null;
        }

        buttonText.color = targetTextColor;  // ensure fully opaque at the end

        // Add hover functionality after the button text is faded in
        AddButtonHoverEffect(buttonText);
    }

    void AddButtonHoverEffect(TextMeshProUGUI buttonText)
    {
        EventTrigger eventTrigger = buttonText.gameObject.AddComponent<EventTrigger>();
        
        // Pointer Enter event
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((eventData) => { buttonText.color = hoverColor; });
        eventTrigger.triggers.Add(pointerEnterEntry);
        
        // Pointer Exit event
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((eventData) => { buttonText.color = normalColor; });
        eventTrigger.triggers.Add(pointerExitEntry);
    }
}
