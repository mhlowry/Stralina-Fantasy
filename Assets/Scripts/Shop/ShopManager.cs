using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShopUIManager))]
public class ShopManager : MonoBehaviour
{
    private BoostManager playerBoostManager;
    private int totalBoosts = 0;

    private List<ShopItem> heavyAttackItems;
    private List<ShopItem> lightAttackItems;
    private List<ShopItem> hpPointsItems;
    private List<ShopItem> willpowerItems;
    private List<ShopItem> speedItems;
    private List<ShopItem> funRandomItems;

    [SerializeField] private List<Sprite> heavyAttackImages;
    [SerializeField] private List<Sprite> lightAttackImages;
    [SerializeField] private List<Sprite> hpPointsImages;
    [SerializeField] private List<Sprite> willpowerImages;
    [SerializeField] private List<Sprite> speedImages;
    [SerializeField] private List<Sprite> funRandomImages;

    [SerializeField] Sprite outOfStock;

    PlayerInput playerInput;
    ShopUIManager uiManager;

    void Awake()
    {
        uiManager = GetComponent<ShopUIManager>();
        playerInput = GameObject.Find("PlayerInput").GetComponent<PlayerInput>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerBoostManager = player.GetComponent<BoostManager>();
        }

        if (playerBoostManager == null)
        {
            Debug.LogError("BoostManager component not found on the player");
        }

        InitializeItems();
        DisplayItems();
        UpdateTotalBoosts();

        //this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    private void OnDisable()
    {
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    void InitializeItems()
    {
        // Heavy Attack Items
        heavyAttackItems = new List<ShopItem>
        {
        new ShopItem("How to Sword 101", ItemType.HeavyAttack, 1, 500, heavyAttackImages[0]),
        new ShopItem("Hero’s Broken Edge", ItemType.HeavyAttack, 2, 1000, heavyAttackImages[1]),
        new ShopItem("Chaos Twin Blade", ItemType.HeavyAttack, 3, 2000, heavyAttackImages[2])
        };

        // Light Attack Items
        lightAttackItems = new List<ShopItem>
        {
        // Assuming lightAttackImages list has corresponding images for these items
        new ShopItem("Light Ale", ItemType.LightAttack, 1, 500, lightAttackImages[0]),
        new ShopItem("Magic Gloves", ItemType.LightAttack, 2, 1000, lightAttackImages[1]),
        new ShopItem("Ritual Claw", ItemType.LightAttack, 3, 2000, lightAttackImages[2])
        };

        // HP Points Items
        hpPointsItems = new List<ShopItem>
        {
        // Assuming hpPointsImages list has corresponding images for these items
        new ShopItem("Comical Meat", ItemType.HPPoints, 1, 500, hpPointsImages[0]),
        new ShopItem("Golden Apple", ItemType.HPPoints, 2, 1000, hpPointsImages[1]),
        new ShopItem("Heart Vessel", ItemType.HPPoints, 3, 2000, hpPointsImages[2])
        };

        // Willpower Items
        willpowerItems = new List<ShopItem>
         {
        // Assuming willpowerImages list has corresponding images for these items
        new ShopItem("Magic Stamp", ItemType.Willpower, 1, 500, willpowerImages[0]),
        new ShopItem("Mythical Medallion", ItemType.Willpower, 2, 1000, willpowerImages[1]),
        new ShopItem("Crest of Ram", ItemType.Willpower, 3, 2000, willpowerImages[2])
        };

        // Speed Items
        speedItems = new List<ShopItem>
        {
        // Assuming speedImages list has corresponding images for these items
        new ShopItem("Normal Cookies", ItemType.Speed, 1, 500, speedImages[0]),
        new ShopItem("Enchanted Feather", ItemType.Speed, 2, 1000, speedImages[1]),
        new ShopItem("Strange Boots", ItemType.Speed, 3, 2000, speedImages[2])
        };

        // Fun Random Items
        funRandomItems = new List<ShopItem>
        {
            new ShopItem("Potion of Whimsical", ItemType.FunRandom, 0, 150, funRandomImages[0]),
            new ShopItem("Purity Charm", ItemType.FunRandom, 0, 200, funRandomImages[1]),
            new ShopItem("Aka’s Oni Mask", ItemType.FunRandom, 0, 250, funRandomImages[2]),
            new ShopItem("Ao’s Oni Mask", ItemType.FunRandom, 0, 250, funRandomImages[3]),
            new ShopItem("Ic’s Divine Wine", ItemType.FunRandom, 0, 150, funRandomImages[4]),
            new ShopItem("Knife of Memory", ItemType.FunRandom, 0, 300, funRandomImages[5]),
            new ShopItem("Moolon Milk", ItemType.FunRandom, 0, 200, funRandomImages[6]),
            new ShopItem("Ness’s Balanced Breakfast", ItemType.FunRandom, 0, 250, funRandomImages[7]),
            new ShopItem("Unknown’s Eye", ItemType.FunRandom, 0, 350, funRandomImages[8]),
            new ShopItem("Weird Machina", ItemType.FunRandom, 0, 300, funRandomImages[9])
        };

    }

    void UpdateTotalBoosts()
    {
        totalBoosts = playerBoostManager.lightAttackBoosts +
                      playerBoostManager.heavyAttackBoosts +
                      playerBoostManager.speedBoosts +
                      playerBoostManager.willpowerBoosts +
                      playerBoostManager.healthPointBoosts;

        uiManager.UpdateTotalBoostedText("+"+totalBoosts);
    }

    void DisplayItems()
    {
        int lightAttackIndex = playerBoostManager.lightAttackBoosts;
        uiManager.UpdateLightAttackStatIncreaseText("+" + lightAttackIndex.ToString());
        // Display the next Light Attack Item based on the specific boost level
        if (lightAttackIndex < 3)
        {
            ShopItem lightAttackItem = lightAttackItems[lightAttackIndex];
            uiManager.UpdateLightAttackNameText(lightAttackItem.itemName);
            uiManager.UpdateLightAttackPriceText(lightAttackItem.purchaseCost.ToString());
            uiManager.UpdateLightAttackImage(lightAttackImages[lightAttackIndex]);
            // Other UI updates for light attack item...
        }
        else
        {
            // Handle cases where 3 specific light attack boosts have been used
            uiManager.UpdateLightAttackNameText("Out of Stock!");
            uiManager.UpdateLightAttackPriceText("N/A");
            uiManager.UpdateLightAttackImage(outOfStock);
            // Update UI to show that no more specific light attack items are available
        }

        // Repeat for other categories using specific boost counters
        // Example for heavy attack:
        // Display the next Heavy Attack Item based on the specific boost level
        int heavyAttackIndex = playerBoostManager.heavyAttackBoosts;
        uiManager.UpdateHeavyAttackStatIncreaseText("+" + heavyAttackIndex.ToString());
        if (heavyAttackIndex < 3)
        {
            ShopItem heavyAttackItem = heavyAttackItems[heavyAttackIndex];
            uiManager.UpdateHeavyAttackNameText(heavyAttackItem.itemName);
            uiManager.UpdateHeavyAttackPriceText(heavyAttackItem.purchaseCost.ToString());
            uiManager.UpdateHeavyAttackImage(heavyAttackImages[heavyAttackIndex]);
            // Other UI updates for heavy attack item...
        }
        else
        {
            // Handle cases where 3 specific heavy attack boosts have been used
            uiManager.UpdateHeavyAttackNameText("Out of Stock!");
            uiManager.UpdateHeavyAttackPriceText("N/A");
            uiManager.UpdateHeavyAttackImage(outOfStock);
            // Update UI to show that no more specific heavy attack items are available
        }


        // Apply similar logic for HP Points, Willpower, Speed
        // Display the next HP Points Item based on the specific boost level
        int hpPointsIndex = playerBoostManager.healthPointBoosts;
        uiManager.UpdateHPPointsStatIncreaseText("+" + hpPointsIndex.ToString());
        if (playerBoostManager.healthPointBoosts < 3)
        {
            ShopItem hpPointsItem = hpPointsItems[hpPointsIndex];
            uiManager.UpdateHPPointsNameText(hpPointsItem.itemName);
            uiManager.UpdateHPPointsPriceText(hpPointsItem.purchaseCost.ToString());
            uiManager.UpdateHPPointsImage(hpPointsImages[hpPointsIndex]);
            // Other UI updates for HP points item...
        }
        else
        {
            // Handle cases where 3 specific HP points boosts have been used
            uiManager.UpdateHPPointsNameText("Out of Stock!");
            uiManager.UpdateHPPointsPriceText("N/A");
            uiManager.UpdateHPPointsImage(outOfStock);
            // Update UI to show that no more specific HP points items are available
        }

        // Display the next Willpower Item based on the specific boost level
        int willpowerIndex = playerBoostManager.willpowerBoosts;
        uiManager.UpdateWillpowerStatIncreaseText("+" + willpowerIndex.ToString());
        if (playerBoostManager.willpowerBoosts < 3)
        {
            ShopItem willpowerItem = willpowerItems[willpowerIndex];
            uiManager.UpdateWillpowerNameText(willpowerItem.itemName);
            uiManager.UpdateWillpowerPriceText(willpowerItem.purchaseCost.ToString());
            uiManager.UpdateWillpowerImage(willpowerImages[willpowerIndex]);
            // Other UI updates for Willpower item...
        }
        else
        {
            // Handle cases where 3 specific Willpower boosts have been used
            uiManager.UpdateWillpowerNameText("Out of Stock!");
            uiManager.UpdateWillpowerPriceText("N/A");
            uiManager.UpdateWillpowerImage(outOfStock);
            // Update UI to show that no more specific Willpower items are available
        }

        // Display the next Speed Item based on the specific boost level
        int speedIndex = playerBoostManager.speedBoosts;
        uiManager.UpdateMoveSpeedStatIncreaseText("+" + speedIndex.ToString());
        if (playerBoostManager.speedBoosts < 3)
        {
            ShopItem speedItem = speedItems[speedIndex];
            uiManager.UpdateMoveSpeedNameText(speedItem.itemName);
            uiManager.UpdateMoveSpeedPriceText(speedItem.purchaseCost.ToString());
            uiManager.UpdateMoveSpeedImage(speedImages[speedIndex]);
            // Other UI updates for Speed item...
        }
        else
        {
            // Handle cases where 3 specific Speed boosts have been used
            uiManager.UpdateMoveSpeedNameText("Out of Stock!");
            uiManager.UpdateMoveSpeedPriceText("N/A");
            uiManager.UpdateMoveSpeedImage(outOfStock);
            // Update UI to show that no more specific Speed items are available
        }



        // Display a random Fun Random Item
        int randomIndex = Random.Range(0, funRandomItems.Count);
        ShopItem randomItem = funRandomItems[randomIndex];
        uiManager.UpdateRandomItemNameText(randomItem.itemName);
        uiManager.UpdateRandomItemPriceText(randomItem.purchaseCost.ToString());
        uiManager.UpdateRandomItemImage(funRandomImages[randomIndex]);
        // Update UI for random item
    }

    public void PurchaseItem(ShopItem item)
    {
        if (totalBoosts >= 8)
        {
            // Handle the case where the total boost limit is reached
            return;
        }

        // Apply the boost from the item to the player
        //ApplyBoost(item);

        // Update the total boosts and refresh the displayed items
        UpdateTotalBoosts();
        DisplayItems();
    }

    public void ApplyBoost(ItemType itemType)
    {
        // Logic to apply the boost from the item to the player's stats
        // Example:
        switch (itemType)
        {
            case ItemType.LightAttack:
                playerBoostManager.lightAttackBoosts = Mathf.Clamp(playerBoostManager.lightAttackBoosts + 1, 0, 5);
                break;
            // Handle other item types similarly
        }
    }

    // Additional logic for Fun Random Items
    // ...

    public enum ItemType
  {
      HeavyAttack,
      LightAttack,
      HPPoints,
      Willpower,
      Speed,
      FunRandom // For special items
  }

  [System.Serializable]
  public class ShopItem
  {
      public string itemName;
      public ItemType itemType;
      public int boostAmount;
      public int purchaseCost;
      public Sprite itemImage;

      // Constructor
      public ShopItem(string name, ItemType type, int boost, int cost, Sprite image)
      {
          itemName = name;
          itemType = type;
          boostAmount = boost;
          purchaseCost = cost;
          itemImage = image;
      }
  }

}
