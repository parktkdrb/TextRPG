using Newtonsoft.Json.Linq;
using TextRPG;
public class Save
{
    // Singleton �ν��Ͻ�
    private static Save instance;

    public static Save Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Save();
            }
            return instance;
        }
    }

    // �÷��̾� ������ �Ӽ�
    public string PlayerName { get; set; } = "a";
    public int Level { get; set; } = 1;
    public int LevelUpCount { get; set; } = 0;
    public int NeedLevelUpCount { get; set; } = 1;
    public int Health { get; set; } = 100;
    public int Gold { get; set; } = 1500;
    public int DungeonDefense { get; set; } = 0;
    public float Power { get; set; } = 10;
    public float Defense { get; set; } = 5;
    public bool InventoryEquip { get; set; } = false;

    // ������ ��� �� ������ ������
    private static List<Item> myItemList = new List<Item>();
    private static PlayerEquippedItems equippedItems = new PlayerEquippedItems();

    // ������
    private Save()
    {
        Load(); // ��ü ���� �� ������ �ε�
    }
    // ������ �ε� �޼���
    public void Load()
    {
        if (File.Exists(@"D:\SaveData_TextRPG.json"))
        {
            string readData = File.ReadAllText(@"D:\SaveData_TextRPG.json");
            JObject readPlayerStat = JObject.Parse(readData);

            // �÷��̾� ������ �ε�
            PlayerName = (string)readPlayerStat["playername"];
            Level = (int)readPlayerStat["lv"];
            LevelUpCount = (int)readPlayerStat["levelupcount"];
            NeedLevelUpCount = (int)readPlayerStat["needlevelupcount"];
            Health = (int)readPlayerStat["hp"];
            Gold = (int)readPlayerStat["gold"];
            DungeonDefense = (int)readPlayerStat["dungeondefense"];
            Power = (float)readPlayerStat["power"];
            Defense = (float)readPlayerStat["defense"];
            InventoryEquip = (bool)readPlayerStat["inventoryEquip"];

            // ���� ������ ��� �ε�
            myItemList.Clear(); // ���� ������ ��� �ʱ�ȭ
            if (readPlayerStat["myitemList"] != null)
            {
                foreach (var item in readPlayerStat["myitemList"])
                {
                    myItemList.Add(new Item(
                        (string)item["ItemName"],
                        (int)item["ItemPower"],
                        (string)item["ItemType"],
                        (bool)item["IsShopItem"],
                        (int)item["ShopsellGold"]
                    ));
                }
            }

            // ������ ������ �ε�
            if (readPlayerStat["equippedItems"] != null)
            {
                equippedItems.EquippedWeapon = (string)readPlayerStat["equippedItems"]["EquippedWeapon"];
                equippedItems.EquippedArmor = (string)readPlayerStat["equippedItems"]["EquippedArmor"];
            }
        }
    }


    // ������ ���� �޼���
    public void SaveData()
    {
        // ������ ����� JArray�� ��ȯ
        JArray itemsArray = new JArray();
        foreach (var item in myItemList)
        {
            JObject itemObject = new JObject(
                new JProperty("ItemName", item.ItemName),
                new JProperty("ItemPower", item.ItemPower),
                new JProperty("ItemType", item.ItemType),
                new JProperty("IsShopItem", item.IsShopItem),
                new JProperty("ShopsellGold", item.ShopsellGold)
            );
            itemsArray.Add(itemObject);
        }

        // ������ ������ ������ JObject�� ��ȯ
        JObject equippedItemsObject = new JObject(
            new JProperty("EquippedWeapon", equippedItems.EquippedWeapon),
            new JProperty("EquippedArmor", equippedItems.EquippedArmor)
        );

        // ���� ������ JSON ��ü ����
        JObject savePlayerStat = new JObject(
            new JProperty("playername", PlayerName),
            new JProperty("lv", Level),
            new JProperty("levelupcount", LevelUpCount),
            new JProperty("needlevelupcount", NeedLevelUpCount),
            new JProperty("hp", Health),
            new JProperty("gold", Gold),
            new JProperty("dungeondefense", DungeonDefense),
            new JProperty("power", Power),
            new JProperty("defense", Defense),
            new JProperty("inventoryEquip", InventoryEquip),
            new JProperty("myitemList", itemsArray), // ������ ��� �߰�
            new JProperty("equippedItems", equippedItemsObject) // ������ ������ �߰�
        );

        // JSON ���Ϸ� ���� (�����)
        File.WriteAllText(@"D:\SaveData_TextRPG.json", savePlayerStat.ToString());
    }

}


