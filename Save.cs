using Newtonsoft.Json.Linq;
using TextRPG;
public class Save
{
    // Singleton 인스턴스
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

    // 플레이어 데이터 속성
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

    // 아이템 목록 및 장착된 아이템
    private static List<Item> myItemList = new List<Item>();
    private static PlayerEquippedItems equippedItems = new PlayerEquippedItems();

    // 생성자
    private Save()
    {
        Load(); // 객체 생성 시 데이터 로드
    }
    // 데이터 로드 메서드
    public void Load()
    {
        if (File.Exists(@"D:\SaveData_TextRPG.json"))
        {
            string readData = File.ReadAllText(@"D:\SaveData_TextRPG.json");
            JObject readPlayerStat = JObject.Parse(readData);

            // 플레이어 데이터 로드
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

            // 기존 아이템 목록 로드
            myItemList.Clear(); // 기존 아이템 목록 초기화
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

            // 장착된 아이템 로드
            if (readPlayerStat["equippedItems"] != null)
            {
                equippedItems.EquippedWeapon = (string)readPlayerStat["equippedItems"]["EquippedWeapon"];
                equippedItems.EquippedArmor = (string)readPlayerStat["equippedItems"]["EquippedArmor"];
            }
        }
    }


    // 데이터 저장 메서드
    public void SaveData()
    {
        // 아이템 목록을 JArray로 변환
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

        // 장착된 아이템 정보를 JObject로 변환
        JObject equippedItemsObject = new JObject(
            new JProperty("EquippedWeapon", equippedItems.EquippedWeapon),
            new JProperty("EquippedArmor", equippedItems.EquippedArmor)
        );

        // 최종 저장할 JSON 객체 생성
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
            new JProperty("myitemList", itemsArray), // 아이템 목록 추가
            new JProperty("equippedItems", equippedItemsObject) // 장착된 아이템 추가
        );

        // JSON 파일로 저장 (덮어쓰기)
        File.WriteAllText(@"D:\SaveData_TextRPG.json", savePlayerStat.ToString());
    }

}


