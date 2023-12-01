using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("Price Texts")]
    public TMP_Text lightAttackPriceText;
    public TMP_Text heavyAttackPriceText;
    public TMP_Text moveSpeedPriceText;
    public TMP_Text HPPointsPriceText;
    public TMP_Text willpowerPriceText;
    public TMP_Text randomItemPriceText;

    [Header("Item Name Texts")]
    public TMP_Text lightAttackNameText;
    public TMP_Text heavyAttackNameText;
    public TMP_Text moveSpeedNameText;
    public TMP_Text HPPointsNameText;
    public TMP_Text willpowerNameText;
    public TMP_Text randomItemNameText;

    [Header("Stat Increase Texts")]
    public TMP_Text lightAttackStatIncreaseText;
    public TMP_Text heavyAttackStatIncreaseText;
    public TMP_Text moveSpeedStatIncreaseText;
    public TMP_Text HPPointsStatIncreaseText;
    public TMP_Text willpowerStatIncreaseText;
    public TMP_Text randomItemShuffleText;

    [Header("Total Boosted Text")]
    public TMP_Text boostedNumberText;

    [Header("Item Images")]
    public Image lightAttackImage;
    public Image heavyAttackImage;
    public Image moveSpeedImage;
    public Image HPPointsImage;
    public Image willpowerImage;
    public Image randomItemImage;

    // Individual methods for updating price text
    public void UpdateLightAttackPriceText(string price) { lightAttackPriceText.text = price; }
    public void UpdateHeavyAttackPriceText(string price) { heavyAttackPriceText.text = price; }
    public void UpdateMoveSpeedPriceText(string price) { moveSpeedPriceText.text = price; }
    public void UpdateHPPointsPriceText(string price) { HPPointsPriceText.text = price; }
    public void UpdateWillpowerPriceText(string price) { willpowerPriceText.text = price; }
    public void UpdateRandomItemPriceText(string price) { randomItemPriceText.text = price; }

    // Individual methods for updating item name text
    public void UpdateLightAttackNameText(string name) { lightAttackNameText.text = name; }
    public void UpdateHeavyAttackNameText(string name) { heavyAttackNameText.text = name; }
    public void UpdateMoveSpeedNameText(string name) { moveSpeedNameText.text = name; }
    public void UpdateHPPointsNameText(string name) { HPPointsNameText.text = name; }
    public void UpdateWillpowerNameText(string name) { willpowerNameText.text = name; }
    public void UpdateRandomItemNameText(string name) { randomItemNameText.text = name; }

    // Individual methods for updating stat increase text
    public void UpdateLightAttackStatIncreaseText(string increase) { lightAttackStatIncreaseText.text = increase; }
    public void UpdateHeavyAttackStatIncreaseText(string increase) { heavyAttackStatIncreaseText.text = increase; }
    public void UpdateMoveSpeedStatIncreaseText(string increase) { moveSpeedStatIncreaseText.text = increase; }
    public void UpdateHPPointsStatIncreaseText(string increase) { HPPointsStatIncreaseText.text = increase; }
    public void UpdateWillpowerStatIncreaseText(string increase) { willpowerStatIncreaseText.text = increase; }
    public void UpdateRandomItemShuffleText(string shuffle) { randomItemShuffleText.text = shuffle; }

    // Method for updating total boosted text
    public void UpdateTotalBoostedText(string boostedNumber) { boostedNumberText.text = boostedNumber; }

    // Individual methods for updating item images
    public void UpdateLightAttackImage(Sprite sprite) { lightAttackImage.sprite = sprite; }
    public void UpdateHeavyAttackImage(Sprite sprite) { heavyAttackImage.sprite = sprite; }
    public void UpdateMoveSpeedImage(Sprite sprite) { moveSpeedImage.sprite = sprite; }
    public void UpdateHPPointsImage(Sprite sprite) { HPPointsImage.sprite = sprite; }
    public void UpdateWillpowerImage(Sprite sprite) { willpowerImage.sprite = sprite; }
    public void UpdateRandomItemImage(Sprite sprite) { randomItemImage.sprite = sprite; }
}
