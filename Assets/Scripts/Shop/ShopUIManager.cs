using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("Price Texts")]
    public TMP_Text attackPriceText;
    public TMP_Text defensePriceText;
    public TMP_Text moveSpeedPriceText;
    public TMP_Text attackSpeedPriceText;
    public TMP_Text knockbackPriceText;
    public TMP_Text randomItemPriceText;

    [Header("Item Name Texts")]
    public TMP_Text attackNameText;
    public TMP_Text defenseNameText;
    public TMP_Text moveSpeedNameText;
    public TMP_Text attackSpeedNameText;
    public TMP_Text knockbackNameText;
    public TMP_Text randomItemNameText;

    [Header("Stat Increase Texts")]
    public TMP_Text attackStatIncreaseText;
    public TMP_Text defenseStatIncreaseText;
    public TMP_Text moveSpeedStatIncreaseText;
    public TMP_Text attackSpeedStatIncreaseText;
    public TMP_Text knockbackStatIncreaseText;
    public TMP_Text randomItemShuffleText;

    [Header("Total Boosted Text")]
    public TMP_Text boostedNumberText;

    [Header("Item Images")]
    public Image attackImage;
    public Image defenseImage;
    public Image moveSpeedImage;
    public Image attackSpeedImage;
    public Image knockbackImage;
    public Image randomItemImage;

    // Individual methods for updating price text
    public void UpdateAttackPriceText(string price) { attackPriceText.text = price; }
    public void UpdateDefensePriceText(string price) { defensePriceText.text = price; }
    public void UpdateMoveSpeedPriceText(string price) { moveSpeedPriceText.text = price; }
    public void UpdateAttackSpeedPriceText(string price) { attackSpeedPriceText.text = price; }
    public void UpdateKnockbackPriceText(string price) { knockbackPriceText.text = price; }
    public void UpdateRandomItemPriceText(string price) { randomItemPriceText.text = price; }

    // Individual methods for updating item name text
    public void UpdateAttackNameText(string name) { attackNameText.text = name; }
    public void UpdateDefenseNameText(string name) { defenseNameText.text = name; }
    public void UpdateMoveSpeedNameText(string name) { moveSpeedNameText.text = name; }
    public void UpdateAttackSpeedNameText(string name) { attackSpeedNameText.text = name; }
    public void UpdateKnockbackNameText(string name) { knockbackNameText.text = name; }
    public void UpdateRandomItemNameText(string name) { randomItemNameText.text = name; }

    // Individual methods for updating stat increase text
    public void UpdateAttackStatIncreaseText(string increase) { attackStatIncreaseText.text = increase; }
    public void UpdateDefenseStatIncreaseText(string increase) { defenseStatIncreaseText.text = increase; }
    public void UpdateMoveSpeedStatIncreaseText(string increase) { moveSpeedStatIncreaseText.text = increase; }
    public void UpdateAttackSpeedStatIncreaseText(string increase) { attackSpeedStatIncreaseText.text = increase; }
    public void UpdateKnockbackStatIncreaseText(string increase) { knockbackStatIncreaseText.text = increase; }
    public void UpdateRandomItemShuffleText(string shuffle) { randomItemShuffleText.text = shuffle; }

    // Method for updating total boosted text
    public void UpdateTotalBoostedText(string boostedNumber) { boostedNumberText.text = boostedNumber; }

    // Individual methods for updating item images
    public void UpdateAttackImage(Sprite sprite) { attackImage.sprite = sprite; }
    public void UpdateDefenseImage(Sprite sprite) { defenseImage.sprite = sprite; }
    public void UpdateMoveSpeedImage(Sprite sprite) { moveSpeedImage.sprite = sprite; }
    public void UpdateAttackSpeedImage(Sprite sprite) { attackSpeedImage.sprite = sprite; }
    public void UpdateKnockbackImage(Sprite sprite) { knockbackImage.sprite = sprite; }
    public void UpdateRandomItemImage(Sprite sprite) { randomItemImage.sprite = sprite; }
}
