using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
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
        UpdateTotalBoosts();
        DisplayItems();
    }

    void InitializeItems()
  {
      // Heavy Attack Items
      heavyAttackItems = new List<ShopItem>
      {
          new ShopItem("How to Sword 101", ItemType.HeavyAttack, 1, 100),
          new ShopItem("Hero’s Broken Edge", ItemType.HeavyAttack, 2, 200),
          new ShopItem("Chaos Twin Blade", ItemType.HeavyAttack, 3, 300)
      };

      // Light Attack Items
      lightAttackItems = new List<ShopItem>
      {
          new ShopItem("Light Ale", ItemType.LightAttack, 1, 100),
          new ShopItem("Magic Gloves", ItemType.LightAttack, 2, 200),
          new ShopItem("Ritual Claw", ItemType.LightAttack, 3, 300)
      };

      // HP Points Items
      hpPointsItems = new List<ShopItem>
      {
          new ShopItem("Comical Meat", ItemType.HPPoints, 1, 100),
          new ShopItem("Golden Apple", ItemType.HPPoints, 2, 200),
          new ShopItem("Heart Vessel", ItemType.HPPoints, 3, 300)
      };

      // Willpower Items
      willpowerItems = new List<ShopItem>
      {
          new ShopItem("Magic Stamp", ItemType.Willpower, 1, 100),
          new ShopItem("Mythical Medallion", ItemType.Willpower, 2, 200),
          new ShopItem("Crest of Ram", ItemType.Willpower, 3, 300)
      };

      // Speed Items
      speedItems = new List<ShopItem>
      {
          new ShopItem("Normal Cookies", ItemType.Speed, 1, 100),
          new ShopItem("Enchanted Feather", ItemType.Speed, 2, 200),
          new ShopItem("Strange Boots", ItemType.Speed, 3, 300)
      };

      // Fun Random Items
      funRandomItems = new List<ShopItem>
      {
          new ShopItem("Potion of Whimsical", ItemType.FunRandom, 0, 150),
          new ShopItem("Purity Charm", ItemType.FunRandom, 0, 200),
          new ShopItem("Aka’s Oni Mask", ItemType.FunRandom, 0, 250),
          new ShopItem("Ao’s Oni Mask", ItemType.FunRandom, 0, 250),
          new ShopItem("Ic’s Divine Wine", ItemType.FunRandom, 0, 150),
          new ShopItem("Knife of Memory", ItemType.FunRandom, 0, 300),
          new ShopItem("Moolon Milk", ItemType.FunRandom, 0, 200),
          new ShopItem("Ness’s Balanced Breakfast", ItemType.FunRandom, 0, 250),
          new ShopItem("Unknown’s Eye", ItemType.FunRandom, 0, 350),
          new ShopItem("Weird Machina", ItemType.FunRandom, 0, 300)
      };
  }


    void UpdateTotalBoosts()
    {
        totalBoosts = playerBoostManager.lightAttackBoosts +
                      playerBoostManager.heavyAttackBoosts +
                      playerBoostManager.speedBoosts +
                      playerBoostManager.willpowerBoosts +
                      playerBoostManager.healthPointBoosts;
    }

    void DisplayItems()
    {
        ShopUIManager uiManager = GetComponent<ShopUIManager>();
        if (uiManager == null)
        {
            Debug.LogError("ShopUIManager component not found on the GameObject.");
            return;
        }

        // Display the next Light Attack Item based on the specific boost level
        if (playerBoostManager.lightAttackSpecificBoosts < 3)
        {
            int lightAttackIndex = playerBoostManager.lightAttackSpecificBoosts;
            ShopItem lightAttackItem = lightAttackItems[lightAttackIndex];
            uiManager.UpdateAttackNameText(lightAttackItem.itemName);
            uiManager.UpdateAttackPriceText(lightAttackItem.purchaseCost.ToString());
            // Other UI updates for light attack item...
        }
        else
        {
            // Handle cases where 3 specific light attack boosts have been used
            uiManager.UpdateAttackNameText("No more specific boosts available");
            uiManager.UpdateAttackPriceText("N/A");
            // Update UI to show that no more specific light attack items are available
        }

        // Repeat for other categories using specific boost counters
        // Example for heavy attack:
        if (playerBoostManager.heavyAttackSpecificBoosts < 3)
        {
            int heavyAttackIndex = playerBoostManager.heavyAttackSpecificBoosts;
            ShopItem heavyAttackItem = heavyAttackItems[heavyAttackIndex];
            uiManager.UpdateDefenseNameText(heavyAttackItem.itemName);
            uiManager.UpdateDefensePriceText(heavyAttackItem.purchaseCost.ToString());
            // Update UI for heavy attack item
        }
        else
        {
            // Handle cases where 3 specific heavy attack boosts have been used
            uiManager.UpdateDefenseNameText("No more specific boosts available");
            uiManager.UpdateDefensePriceText("N/A");
            // Update UI accordingly
        }

        // Apply similar logic for HP Points, Willpower, Speed
        // ...

        // Display a random Fun Random Item
        int randomIndex = Random.Range(0, funRandomItems.Count);
        ShopItem randomItem = funRandomItems[randomIndex];
        uiManager.UpdateRandomItemNameText(randomItem.itemName);
        uiManager.UpdateRandomItemPriceText(randomItem.purchaseCost.ToString());
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
        ApplyBoost(item);

        // Update the total boosts and refresh the displayed items
        UpdateTotalBoosts();
        DisplayItems();
    }

    void ApplyBoost(ShopItem item)
    {
        // Logic to apply the boost from the item to the player's stats
        // Example:
        switch (item.itemType)
        {
            case ItemType.LightAttack:
                playerBoostManager.lightAttackBoosts++;
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

      // Constructor
      public ShopItem(string name, ItemType type, int boost, int cost)
      {
          itemName = name;
          itemType = type;
          boostAmount = boost;
          purchaseCost = cost;
      }
  }

}
