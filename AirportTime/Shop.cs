// Game simulation class to simulate the purchase of items and running the airport
// Shop class to manage purchase of items like runways
public class Shop
{
    private List<Item> itemsForSale;
    private Treasury treasury;

    public Shop(Treasury treasury)
    {
        this.treasury = treasury;
        itemsForSale = new List<Item>();

        // Initialize predefined items
        InitializeItems();
    }

    // Initialize items available in the shop
    private void InitializeItems()
    {
        itemsForSale.Add(new Item("Tier 1 Runway", "Basic runway for small planes.", 5000, ItemType.Runway, 1));
        itemsForSale.Add(new Item("Tier 2 Runway", "Medium runway suitable for medium planes.", 10000, ItemType.Runway, 2));
        itemsForSale.Add(new Item("Tier 3 Runway", "Large runway for jumbo jets.", 20000, ItemType.Runway, 3));
    }

    // View all available items for sale
    public void ViewItemsForSale()
    {
        Console.WriteLine("Items for Sale:");
        foreach (var item in itemsForSale)
        {
            Console.WriteLine($"{item.Name} (Tier {item.Tier}) - {item.Description} - Price: {item.Price:C}");
        }
    }

    // Buy an item from the shop
    public bool BuyItem(string itemName)
    {
        var item = itemsForSale.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        
        if (item == null)
        {
            Console.WriteLine("Item not found in the shop.");
            return false;
        }

        // Check if the player has enough funds
        if (treasury.GetBalance() >= item.Price)
        {
            treasury.DeductFunds(item.Price, $"Purchased {item.Name}");
            Console.WriteLine($"You have purchased: {item.Name}");
            return true;
        }
        else
        {
            Console.WriteLine("Not enough gold to purchase this item.");
            return false;
        }
    }
}
