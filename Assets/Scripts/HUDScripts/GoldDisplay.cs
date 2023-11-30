using UnityEngine;
using TMPro;

public class GoldDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText; // Reference to the TextMeshPro UI element

    void Start()
    {
        if (goldText == null)
        {
            Debug.LogError("GoldDisplay: TMP_Text reference is missing.");
        }
    }

    void Update()
    {
        if (goldText != null && GameManager.instance != null)
        {
            UpdateGoldDisplay();
        }
    }

    private void UpdateGoldDisplay()
    {
        int goldAmount = GameManager.instance.GetPlayerGold(); // Get the gold amount from GameManager
        goldText.text = "" + goldAmount; // Update the text with the current gold count
    }
}
