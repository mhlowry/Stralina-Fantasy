using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Linq;

[RequireComponent(typeof(ShopUIManager))]
public class ShopManager : MonoBehaviour
{
    private Player player;
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

    int curRandomItemIndex;

    PlayerInput playerInput;
    ShopUIManager uiManager;
    public DialogueUI dialogueUI;

    void Awake()
    {
        uiManager = GetComponent<ShopUIManager>();
        playerInput = GameObject.Find("PlayerInput").GetComponent<PlayerInput>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            this.player = player.GetComponent<Player>();
        }

        InitializeItems();
        DisplayItems();
        UpdateTotalBoosts();
        RandomizeItem();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("UI");
        DisplayItems();
        UpdateTotalBoosts();
    }

    private void OnDisable()
    {
        if (playerInput != null && playerInput.isActiveAndEnabled)
        {
            playerInput.SwitchCurrentActionMap("Gameplay");
        }
    }

    public void enableShoppeMenu()
    {
        // Close the dialogue box
        if (dialogueUI != null)
        {
            dialogueUI.CloseDialogueBox();
        }

        // Enable the Shoppe Menu
        GameObject shoppeMenu = GameObject.Find("Canvas (base)").transform.Find("Shoppe menu").gameObject;
        shoppeMenu.SetActive(true);
    }

    public void disableShoppeMenu()
    {
        // Disable the Shoppe Menu
        GameObject shoppeMenu = GameObject.Find("Canvas (base)").transform.Find("Shoppe menu").gameObject;
        shoppeMenu.SetActive(false);

        // // Close the dialogue box
        // dialogueUI.CloseDialogueBox(); // I think the dialogue box will always be closed by this point

        // Switch the current action map to "Gameplay"
        playerInput.SwitchCurrentActionMap("Gameplay");

        // Enable player input
        player.EnableInput();
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
            new ShopItem("Potion of Whimsical", ItemType.FunRandom, 0, 1000, funRandomImages[0],
                "This item gives 2 random stat boosts, but 2 random boost decreases"),
            new ShopItem("Purity Charm", ItemType.FunRandom, 0, 200, funRandomImages[1],
                "This item removes all stat boosts and gives currency equal to 300 times number of boosts removed"),
            new ShopItem("Aka’s Oni Mask", ItemType.FunRandom, 0, 500, funRandomImages[2],
                "This item swaps boosts in Heavy Attack and boosts in HP"),
            new ShopItem("Ao’s Oni Mask", ItemType.FunRandom, 0, 500, funRandomImages[3],
                "This item swaps boosts in Light Attack and boosts in Willpower"),
            new ShopItem("Ic’s Divine Wine", ItemType.FunRandom, 0, 1000, funRandomImages[4],
                "This item boosts Willpower by 2, but decreases Speed boosts by 1"),
            new ShopItem("Knife of Memory", ItemType.FunRandom, 0, 1500, funRandomImages[5],
                "This item boosts both attack stats by 1 but decreases HP boosts by 2"),
            new ShopItem("Moolon Milk", ItemType.FunRandom, 0, 1000, funRandomImages[6],
                "This item boosts HP by 2"),
            new ShopItem("Ness’s Balanced Lunch", ItemType.FunRandom, 0, 1500, funRandomImages[7],
                "This item puts all stats at 1 boost"),
            new ShopItem("Unknown’s Eye", ItemType.FunRandom, 0, 1750, funRandomImages[8],
                "This item boosts a random stat by 4, but reduces all other boosts to 0"),
            new ShopItem("Weird Machina", ItemType.FunRandom, 0, 500, funRandomImages[9],
                "This item shuffles all current stat boosts randomly across each stat")
        };

    }

    void UpdateTotalBoosts()
    {
        totalBoosts = GameManager.instance.lightAttackBoosts +
                      GameManager.instance.heavyAttackBoosts +
                      GameManager.instance.speedBoosts +
                      GameManager.instance.willpowerBoosts +
                      GameManager.instance.healthPointBoosts;

        uiManager.UpdateTotalBoostedText("+" + totalBoosts);
    }

    void DisplayItems()
    {
        int lightAttackIndex = GameManager.instance.lightAttackBoosts;
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
        int heavyAttackIndex = GameManager.instance.heavyAttackBoosts;
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
        int hpPointsIndex = GameManager.instance.healthPointBoosts;
        uiManager.UpdateHPPointsStatIncreaseText("+" + hpPointsIndex.ToString());
        if (GameManager.instance.healthPointBoosts < 3)
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
        int willpowerIndex = GameManager.instance.willpowerBoosts;
        uiManager.UpdateWillpowerStatIncreaseText("+" + willpowerIndex.ToString());
        if (GameManager.instance.willpowerBoosts < 3)
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
        int speedIndex = GameManager.instance.speedBoosts;
        uiManager.UpdateMoveSpeedStatIncreaseText("+" + speedIndex.ToString());
        if (GameManager.instance.speedBoosts < 3)
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
    }

    public void RandomizeItem()
    {
        // Display a random Fun Random Item
        curRandomItemIndex = UnityEngine.Random.Range(0, funRandomItems.Count);
        ShopItem randomItem = funRandomItems[curRandomItemIndex];
        uiManager.UpdateRandomItemNameText(randomItem.itemName);
        uiManager.UpdateRandomItemPriceText(randomItem.purchaseCost.ToString());
        uiManager.UpdateRandomItemImage(funRandomImages[curRandomItemIndex]);
        uiManager.UpdateRandomItemShuffleText(randomItem.description);
        // Update UI for random item
    }

    public void PurchaseLightAttack()
    {
        if (GameManager.instance.lightAttackBoosts < 3)
            PurchaseItem(lightAttackItems[GameManager.instance.lightAttackBoosts]);
    }

    public void PurchaseHeavyAttack()
    {
        if (GameManager.instance.heavyAttackBoosts < 3)
            PurchaseItem(heavyAttackItems[GameManager.instance.heavyAttackBoosts]);
    }

    public void PurchaseHPPoints()
    {
        if (GameManager.instance.healthPointBoosts < 3)
            PurchaseItem(hpPointsItems[GameManager.instance.healthPointBoosts]);
    }

    public void PurchaseWillpower()
    {
        if (GameManager.instance.willpowerBoosts < 3)
            PurchaseItem(willpowerItems[GameManager.instance.willpowerBoosts]);
    }

    public void PurchaseSpeed()
    {
        if (GameManager.instance.speedBoosts < 3)
            PurchaseItem(speedItems[GameManager.instance.speedBoosts]);
    }

    public void PurchaseSpecial()
    {
        PurchaseSpecialItem(funRandomItems[curRandomItemIndex]);
    }

    public void ShuffleItemButton()
    {
        if (200 > player.GetCurGold())
        {
            // Handle the case where the player can't afford item
            return;
        }
        player.SpendGold(200);
        RandomizeItem();
    }

    void PurchaseSpecialItem(ShopItem item)
    {
        if (item.purchaseCost > player.GetCurGold())
        {
            // Handle the case where the player can't afford item
            return;
        }

        player.SpendGold(item.purchaseCost);

        switch (curRandomItemIndex)
        {
            case 0: //Potion of Whimsical
                PotionWhimsey();
                break;

            case 1: //Purity charm
                player.GainGold(300 * totalBoosts);
                GameManager.instance.lightAttackBoosts = 0;
                GameManager.instance.heavyAttackBoosts = 0;
                GameManager.instance.speedBoosts = 0;
                GameManager.instance.willpowerBoosts = 0;
                GameManager.instance.healthPointBoosts = 0;
                break;

            case 2: //Aka's oni mask
                //This uses a tuple to swap values!  Neat!
                (GameManager.instance.healthPointBoosts, GameManager.instance.heavyAttackBoosts) = (GameManager.instance.heavyAttackBoosts, GameManager.instance.healthPointBoosts);
                break;

            case 3: //Ao's oni mask
                //This uses a tuple to swap values!  Neat!
                (GameManager.instance.willpowerBoosts, GameManager.instance.lightAttackBoosts) = (GameManager.instance.lightAttackBoosts, GameManager.instance.willpowerBoosts);
                break;

            case 4: //Ic's Divine Wine
                ApplyBoost(ItemType.Willpower);
                ApplyBoost(ItemType.Willpower);
                ApplyDeBoost(ItemType.Speed);
                break;

            case 5: //Knife of Memory
                ApplyBoost(ItemType.HeavyAttack);
                ApplyBoost(ItemType.LightAttack);
                ApplyDeBoost(ItemType.HPPoints);
                ApplyDeBoost(ItemType.HPPoints);
                break;

            case 6: //Moolon Milk
                ApplyBoost(ItemType.HPPoints);
                ApplyBoost(ItemType.HPPoints);
                break;

            case 7: //Ness's Balanced Lunch
                GameManager.instance.lightAttackBoosts = 1;
                GameManager.instance.heavyAttackBoosts = 1;
                GameManager.instance.speedBoosts = 1;
                GameManager.instance.willpowerBoosts = 1;
                GameManager.instance.healthPointBoosts = 1;
                break;

            case 8: //Unknown's Eye
                UnknownEye();
                break;

            case 9: //Weird Machina
                WeirdMachina();
                break;

            default:
                Debug.Log("Please I fucking beg of you if you somehow got this debug do not tell me just leave me in sweet, blissful ignorance");
                break;
        }

        // Update the total boosts and refresh the displayed items
        RandomizeItem();
        UpdateTotalBoosts();
        DisplayItems();
    }

    void PurchaseItem(ShopItem item)
    {
        if (item.purchaseCost > player.GetCurGold())
        {
            // Handle the case where the player can't afford item
            return;
        }
        // Apply the boost from the item to the player
        if (ApplyBoost(item.itemType))
            player.SpendGold(item.purchaseCost);

        // Update the total boosts and refresh the displayed items
        UpdateTotalBoosts();
        DisplayItems();
    }

    bool ApplyBoost(ItemType itemType)
    {
        // Logic to apply the boost from the item to the player's stats
        // Example:
        // Handle the case where the total boost limit is reached
        if (totalBoosts >= 8)
        {
            //return false if boost failed
            return false;
        }

        switch (itemType)
        {
            case ItemType.HeavyAttack:
                if (GameManager.instance.heavyAttackBoosts >= 5)
                { return false; }
                Debug.Log("Boosted Heavy Attack");
                GameManager.instance.SetHeavyAttackBoosts(Mathf.Clamp(GameManager.instance.heavyAttackBoosts + 1, 0, 5));
                break;

            case ItemType.LightAttack:
                if (GameManager.instance.lightAttackBoosts >= 5)
                { return false; }
                Debug.Log("Boosted Light Attack");
                GameManager.instance.lightAttackBoosts = Mathf.Clamp(GameManager.instance.lightAttackBoosts + 1, 0, 5);
                break;

            case ItemType.HPPoints:
                if (GameManager.instance.healthPointBoosts >= 5)
                { return false; }
                Debug.Log("Boosted HP");
                GameManager.instance.healthPointBoosts = Mathf.Clamp(GameManager.instance.healthPointBoosts + 1, 0, 5);
                break;

            case ItemType.Willpower:
                if (GameManager.instance.willpowerBoosts >= 5)
                { return false; }
                Debug.Log("Boosted WillPower");
                GameManager.instance.willpowerBoosts = Mathf.Clamp(GameManager.instance.willpowerBoosts + 1, 0, 5);
                break;

            case ItemType.Speed:
                if (GameManager.instance.speedBoosts >= 5)
                { return false; }
                Debug.Log("Boosted Movement Speed");
                GameManager.instance.speedBoosts = Mathf.Clamp(GameManager.instance.speedBoosts + 1, 0, 5);
                break;

            default:
                { return false; }
        }

        //return true if boost succeeded
        return true;
    }

    bool ApplyDeBoost(ItemType itemType)
    {
        // Logic to apply the boost from the item to the player's stats
        // Example:
        // Handle the case where the total boost limit is reached
        if (totalBoosts <= 0)
        {
            //return false if deboost failed
            return false;
        }

        switch (itemType)
        {
            case ItemType.HeavyAttack:
                if (GameManager.instance.heavyAttackBoosts <= 0)
                { return false; }
                Debug.Log("DeBoosted Heavy Attack");
                GameManager.instance.heavyAttackBoosts = Mathf.Clamp(GameManager.instance.heavyAttackBoosts - 1, 0, 5);
                break;

            case ItemType.LightAttack:
                if (GameManager.instance.lightAttackBoosts <= 0)
                { return false; }
                Debug.Log("DeBoosted Light Attack");
                GameManager.instance.lightAttackBoosts = Mathf.Clamp(GameManager.instance.lightAttackBoosts - 1, 0, 5);
                break;

            case ItemType.HPPoints:
                if (GameManager.instance.healthPointBoosts <= 0)
                { return false; }
                Debug.Log("DeBoosted HPPoints");
                GameManager.instance.healthPointBoosts = Mathf.Clamp(GameManager.instance.healthPointBoosts - 1, 0, 5);
                break;

            case ItemType.Willpower:
                if (GameManager.instance.willpowerBoosts <= 0)
                { return false; }
                Debug.Log("DeBoosted Willpower");
                GameManager.instance.willpowerBoosts = Mathf.Clamp(GameManager.instance.willpowerBoosts - 1, 0, 5);
                break;

            case ItemType.Speed:
                if (GameManager.instance.speedBoosts <= 0)
                { return false; }
                Debug.Log("DeBoosted Move Speed");
                GameManager.instance.speedBoosts = Mathf.Clamp(GameManager.instance.speedBoosts - 1, 0, 5);
                break;

            default:
                { return false; }
        }

        //return true if deboost succeeded
        return true;
    }

    // Additional logic for Fun Random Items
    // ...
    void PotionWhimsey()
    {
        int numOfAttempts = 0; HashSet<int> loggedTypes = new HashSet<int>();
        int decreaseSuccess = 0; //This is probably wildly inefficient but I do not give a fuck rn (5:10 am)
        while (numOfAttempts < 10 && decreaseSuccess < 2)
        {
            int rand = UnityEngine.Random.Range(1, 6);
            ItemType type = ItemType.HeavyAttack;

            if (!loggedTypes.Contains(rand))
            {
                switch (rand)
                {
                    case 1:
                        type = ItemType.HeavyAttack;
                        break;

                    case 2:
                        type = ItemType.LightAttack;
                        break;

                    case 3:
                        type = ItemType.HPPoints;
                        break;

                    case 4:
                        type = ItemType.Willpower;
                        break;

                    case 5:
                        type = ItemType.Speed;
                        break;

                    default:
                        Debug.LogError("Unhandled random value: " + rand);
                        break;
                }

                if (ApplyDeBoost(type))
                    decreaseSuccess++;
                else
                {
                    loggedTypes.Add(rand);
                }
            }
            numOfAttempts++;
        }

        numOfAttempts = 0; loggedTypes = new HashSet<int>();
        int increaseSuccess = 0; //This is probably wildly inefficient but I do not give a fuck rn (5:10 am)
        while (numOfAttempts < 10 && increaseSuccess < 2)
        {
            int rand = UnityEngine.Random.Range(1, 6);
            ItemType type = ItemType.HeavyAttack;

            if (!loggedTypes.Contains(rand))
            {
                switch (rand)
                {
                    case 1:
                        type = ItemType.HeavyAttack;
                        break;

                    case 2:
                        type = ItemType.LightAttack;
                        break;

                    case 3:
                        type = ItemType.HPPoints;
                        break;

                    case 4:
                        type = ItemType.Willpower;
                        break;

                    case 5:
                        type = ItemType.Speed;
                        break;

                    default:
                        Debug.LogError("Unhandled random value: " + rand);
                        break;
                }

                if (ApplyBoost(type))
                    increaseSuccess++;
                else
                {
                    loggedTypes.Add(rand);
                }
            }
            numOfAttempts++;
        }
    }

    //Courtesy of ChatGPT
    void UnknownEye()
    {
        // Create an array of all ItemType values
        ItemType[] allStats = (ItemType[])Enum.GetValues(typeof(ItemType));

        // Choose a random stat from the array
        ItemType randomStat = allStats[UnityEngine.Random.Range(0, allStats.Length - 1)];

        // Set the chosen stat to 4 boosts
        switch (randomStat)
        {
            case ItemType.HeavyAttack:
                GameManager.instance.heavyAttackBoosts = 4;
                break;

            case ItemType.LightAttack:
                GameManager.instance.lightAttackBoosts = 4;
                break;

            case ItemType.HPPoints:
                GameManager.instance.healthPointBoosts = 4;
                break;

            case ItemType.Willpower:
                GameManager.instance.willpowerBoosts = 4;
                break;

            case ItemType.Speed:
                GameManager.instance.speedBoosts = 4;
                break;

            default:
                Debug.LogError("Unhandled ItemType: " + randomStat);
                break;
        }

        // Set all other stats to 0 boosts
        foreach (ItemType stat in allStats)
        {
            if (stat != randomStat)
            {
                switch (stat)
                {
                    case ItemType.HeavyAttack:
                        GameManager.instance.heavyAttackBoosts = 0;
                        break;

                    case ItemType.LightAttack:
                        GameManager.instance.lightAttackBoosts = 0;
                        break;

                    case ItemType.HPPoints:
                        GameManager.instance.healthPointBoosts = 0;
                        break;

                    case ItemType.Willpower:
                        GameManager.instance.willpowerBoosts = 0;
                        break;

                    case ItemType.Speed:
                        GameManager.instance.speedBoosts = 0;
                        break;

                    default:
                        Debug.LogError("Unhandled ItemType: " + stat);
                        break;
                }
            }
        }
    }

    //Also Courtesy of ChatGPT
    void WeirdMachina()
    {
        // Create an array of all ItemType values
        ItemType[] allStats = ((ItemType[])Enum.GetValues(typeof(ItemType))).Take(Enum.GetValues(typeof(ItemType)).Length - 1).ToArray();

        // Create a list to store the number of boosts for each stat
        List<int> boostsPerStat = new List<int>();

        // Randomly distribute the total number of boosts among all stats
        for (int i = 0; i < allStats.Length - 1; i++)
        {
            // Generate a random number of boosts for the current stat
            int randomBoosts = UnityEngine.Random.Range(0, Mathf.Min(totalBoosts + 1, 6));

            // Add the random number of boosts to the list
            boostsPerStat.Add(randomBoosts);

            // Subtract the allocated boosts from the total
            totalBoosts -= randomBoosts;
        }

        // The last stat gets the remaining boosts to ensure the total is unchanged
        boostsPerStat.Add(totalBoosts);

        // Set the boosts for each stat based on the redistributed values
        for (int i = 0; i < allStats.Length; i++)
        {
            switch (allStats[i])
            {
                case ItemType.HeavyAttack:
                    GameManager.instance.heavyAttackBoosts = Mathf.Clamp(boostsPerStat[i], 0, 5);
                    break;

                case ItemType.LightAttack:
                    GameManager.instance.lightAttackBoosts = Mathf.Clamp(boostsPerStat[i], 0, 5);
                    break;

                case ItemType.HPPoints:
                    GameManager.instance.healthPointBoosts = Mathf.Clamp(boostsPerStat[i], 0, 5);
                    break;

                case ItemType.Willpower:
                    GameManager.instance.willpowerBoosts = Mathf.Clamp(boostsPerStat[i], 0, 5);
                    break;

                case ItemType.Speed:
                    GameManager.instance.speedBoosts = Mathf.Clamp(boostsPerStat[i], 0, 5);
                    break;

                default:
                    Debug.LogError("Unhandled ItemType: " + allStats[i]);
                    break;
            }
        }
    }

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

        //Only for use by FunRandom item types
        public string description = "default";

      // Constructor
      public ShopItem(string name, ItemType type, int boost, int cost, Sprite image)
      {
          itemName = name;
          itemType = type;
          boostAmount = boost;
          purchaseCost = cost;
          itemImage = image;
      }

      public ShopItem(string name, ItemType type, int boost, int cost, Sprite image, string description)
        {
            itemName = name;
            itemType = type;
            boostAmount = boost;
            purchaseCost = cost;
            itemImage = image;
            this.description = description;
        }
  }

}
