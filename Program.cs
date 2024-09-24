using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using TextRPG;

namespace TextRPG
{  
    internal class Program
    {
        static Save save = Save.Instance;
        static PlayerEquippedItems equippedItems = new PlayerEquippedItems();
        static List<Item> myitemList = new List<Item>();

        static Program playerstat = new Program();

        string playername;
        int lv;
        int levelupcount;
        int needlevelupcount;
        int hp;
        int gold;
        int dungeondefense;
        float power;
        float defense;
        bool inventoryEquip;

        static void Main(string[] args)
        {
            save.Load();
            BasicItemsList();
            playerstat.playername = Save.Instance.PlayerName;
            playerstat.lv = Save.Instance.Level;
            playerstat.levelupcount = Save.Instance.LevelUpCount;
            playerstat.needlevelupcount = Save.Instance.NeedLevelUpCount;
            playerstat.hp = Save.Instance.Health;
            playerstat.gold = Save.Instance.Gold;
            playerstat.dungeondefense = Save.Instance.DungeonDefense;
            playerstat.power = Save.Instance.Power;
            playerstat.defense = Save.Instance.Defense;
            playerstat.inventoryEquip = Save.Instance.InventoryEquip;

            Console.WriteLine($"현재 닉네임 : {playerstat.playername}");
            Console.WriteLine("닉네임을 설정해주세요");
            playerstat.playername = Console.ReadLine();
            StartScene();
        }


        public static void ShowPlayerStats()
        {
            float totalPower = playerstat.power;
            float totalDefense = playerstat.defense;

            var equippedWeaponItem = equippedItems.EquippedWeapon != null ? myitemList.Find(i => i.ItemName == equippedItems.EquippedWeapon) : null;
            var equippedArmorItem = equippedItems.EquippedArmor != null ? myitemList.Find(i => i.ItemName == equippedItems.EquippedArmor) : null;

            int weaponPower = equippedWeaponItem != null ? equippedWeaponItem.ItemPower : 0;
            int armorDefense = equippedArmorItem != null ? equippedArmorItem.ItemPower : 0;

            Console.WriteLine($"Lv. {playerstat.lv}");
            Console.WriteLine($"이름 : {playerstat.playername}");
            Console.WriteLine($"체력 : {playerstat.hp}");
            Console.WriteLine($"공격력 : {totalPower} (+{weaponPower})");
            Console.WriteLine($"방어력 : {totalDefense} (+{armorDefense})");
            Console.WriteLine($"Gold : {playerstat.gold} G");
            Console.WriteLine();
            Console.WriteLine("엔터키를 눌러 뒤로 돌아가기");
            Console.ReadLine();

            StartScene();
        }

        public static void Inventory()
        {
            Console.WriteLine("=== 인벤토리 ===");

            foreach (var item in myitemList)
            {
                string equippedIndicator = (item.ItemType == "무기" && item.ItemName == equippedItems.EquippedWeapon) ||
                                           (item.ItemType == "방어구" && item.ItemName == equippedItems.EquippedArmor)
                                           ? "<E> " : "";

                if (item.IsShopItem)
                {
                    Console.WriteLine($"{equippedIndicator}{item.ItemName} | 공격력: {item.ItemPower} | 아이템 타입: {item.ItemType}");
                }
            }

            Console.WriteLine($"무기: {equippedItems.EquippedWeapon ?? "없음"}");
            Console.WriteLine($"방어구: {equippedItems.EquippedArmor ?? "없음"}");

            if (!playerstat.inventoryEquip)
            {
                Console.WriteLine("1. 장착 관리");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    playerstat.inventoryEquip = true;
                    Inventory();
                }
            }
            else
            {
                Console.WriteLine("장착할 아이템을 선택하세요.");
                string inputitem = Console.ReadLine();
                EquipItem(inputitem);
            }

            BackStartScene();
        }

        public static void EquipItem(string inputitem)//아이템 장착
        {
            var item = myitemList.Find(i => i.ItemName == inputitem);
            if (item != null)
            {
                if (item.ItemType == "무기")
                {
                    if (equippedItems.EquippedWeapon != null)
                    {
                        var currentWeapon = myitemList.Find(i => i.ItemName == equippedItems.EquippedWeapon);
                        playerstat.power -= currentWeapon.ItemPower;
                    }
                    playerstat.power += item.ItemPower;
                    equippedItems.EquippedWeapon = item.ItemName;
                }
                else if (item.ItemType == "방어구")
                {
                    if (equippedItems.EquippedArmor != null)
                    {
                        var currentArmor = myitemList.Find(i => i.ItemName == equippedItems.EquippedArmor);
                        playerstat.defense -= currentArmor.ItemPower;
                    }
                    playerstat.defense += item.ItemPower;
                    equippedItems.EquippedArmor = item.ItemName;
                }
                Console.WriteLine($"{item.ItemName}을 장착했습니다.");
            }
        }

        public static void Store()//상점
        {
            Console.WriteLine($"=== 상점 ===  보유한 Gold : {playerstat.gold} G");
            Console.WriteLine();
            Console.WriteLine("1. 구매하기\n2. 판매하기");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out int a) || (a < 1 || a > 2))
            {
                Console.WriteLine("선택지에 없는 입력입니다. 이전으로 돌아갑니다.");
                BackStartScene();
            }

            if (a == 1)
            {
                foreach (var item in myitemList)
                {
                    if (!item.IsShopItem)
                    {
                        Console.WriteLine($"{item.ItemName} | 가격: {item.ShopsellGold} G | 공격력/방어력: {item.ItemPower} | 아이템 타입: {item.ItemType}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("구매할 아이템 이름을 입력하세요:");
                string inputitem = Console.ReadLine();

                var itemToBuy = myitemList.Find(i => i.ItemName == inputitem);
                if (itemToBuy != null && playerstat.gold >= itemToBuy.ShopsellGold)
                {
                    playerstat.gold -= itemToBuy.ShopsellGold;
                    itemToBuy.IsShopItem = true;
                    Console.WriteLine($"{itemToBuy.ItemName}을 구매했습니다.");
                }
                else
                {
                    Console.WriteLine("골드가 부족하거나 아이템이 존재하지 않습니다.");
                }

            }
            else if (a == 2)
            {
                foreach (var item in myitemList)
                {
                    if (item.IsShopItem)
                    {
                        Console.WriteLine($"{item.ItemName} | 가격: {item.ShopsellGold * 0.8} G");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("판매할 아이템 이름을 입력하세요:");
                string inputitem = Console.ReadLine();

                var itemToBuy = myitemList.Find(i => i.ItemName == inputitem);
                if (itemToBuy != null)
                {
                    playerstat.gold += (int)Math.Floor(itemToBuy.ShopsellGold * 0.8f);
                    itemToBuy.IsShopItem = false;
                    Console.WriteLine($"{itemToBuy.ItemName}을 판매했습니다.");
                }
                else
                {
                    Console.WriteLine("아이템이 존재하지 않습니다.");
                }

            }
            else
            {
                Console.WriteLine("1 또는 2를 입력해주세요");
                Store();
            }
            BackStartScene();
        }

        public static void Resting()// 쉼터
        {
            Console.WriteLine($"여기는 쉼터입니다. 500G를 내면 체력을 100까지 회복할 수 있습니다.");

            Console.WriteLine($"1. 휴식하기");
            Console.WriteLine();
            Console.Write(">>>");

            string input = Console.ReadLine();
            if (!int.TryParse(input, out int a) || a != 1)
            {
                Console.WriteLine("선택지에 없는 입력입니다. 이전으로 돌아갑니다.");
                BackStartScene();
            }
            else
            {
                if (playerstat.gold >= 500)
                {
                    Console.WriteLine($"현재 체력 {playerstat.hp}에서 100으로 체력이 회복됩니다.");
                    playerstat.hp = 100;
                    playerstat.gold -= 500;
                    Console.WriteLine($"현재 골드 {playerstat.gold}");
                }
                else
                {
                    Console.WriteLine($"Gold가 부족합니다. 현재 보유한 골드는 {playerstat.gold}입니다.");
                }
            }

            BackStartScene();
        }
        public static void GoDungeon()
        {
            Console.WriteLine($"3가지 난이도의 던전 중 어떤 던전에 들어갈지 선택하세요.");
            Console.WriteLine("1. 쉬운 던전     | 방어력 5 이상 권장");
            Console.WriteLine("2. 보통 던전     | 방어력 7 이상 권장");
            Console.WriteLine("3. 어려운 던전   | 방어력 10 이상 권장");
            Console.WriteLine();
            Console.Write(">>>");
            string input = Console.ReadLine();
            if (playerstat.hp <= 0)
            {
                Console.WriteLine($"현재 체력은 {playerstat.hp}입니다. 던전에 진입 할 수 없습니다.");
                BackStartScene();
            }
            if (!int.TryParse(input, out int a) || a < 1 || a > 3)
            {
                Console.WriteLine("선택지에 없는 입력입니다. 이전으로 돌아갑니다.");
                BackStartScene();
            }
            else
            {
                int dungeoncheck = 4;
                for (int i = 1; i < 4; i++)
                {
                    dungeoncheck += i;
                    if (i == a)
                    {
                        playerstat.dungeondefense = dungeoncheck;
                        DefeatDungeon(playerstat.dungeondefense);
                        //   break;
                    }
                }
            }
            BackStartScene();
        }
        public static void InDungeon(int defensecheck)
        {
            Random rand = new Random();
            int hpdecrease = rand.Next(20, 36);
            playerstat.hp -= hpdecrease + (defensecheck - (int)playerstat.defense);
            if (playerstat.hp < 0)
            {
                playerstat.hp = 0;
                playerstat.gold = (int)(playerstat.gold * 0.5f);
                Console.WriteLine($"당신은 패배하였습니다. 마을로 돌아갑니다. 골드를 절반 잃어버렸습니다. 현재 골드 : {playerstat.gold}");
                DefeatDungeon(defensecheck);
            }
            else
            {
                if (playerstat.dungeondefense == 5)
                {
                    playerstat.gold += 1000;
                    Console.Write("쉬운");
                }
                else if (playerstat.dungeondefense == 7)
                {
                    playerstat.gold += 1700;
                    Console.Write("보통");
                }
                else if (playerstat.dungeondefense == 10)
                {
                    playerstat.gold += 2500;
                    Console.Write("어려운");
                }
                Console.WriteLine($"던전 클리어!. 마을로 돌아갑니다. 현재 체력 : {playerstat.hp} 현재 골드 : {playerstat.gold}");
                LevelUp();

            }
            BackStartScene();
        }
        public static void DefeatDungeon(int defensecheck)
        {
            Random rand = new Random();
            int randnum = rand.Next(1, 101);
            if (randnum <= 40)
            {
                Console.WriteLine($"현재 방어력이 권장 방어력보다 낮아 일정 확률로 패배하였습니다. 현재 체력 : {playerstat.hp} 권장 방어력 : {defensecheck} 현재 방어력 : {playerstat.defense}");
            }
            else
            {
                InDungeon(playerstat.dungeondefense);
                Console.WriteLine();
                Console.Write(">>>");
            }
            BackStartScene();
        }

        public static void LevelUp()// 던전 클리어에 넣기
        {
            playerstat.levelupcount++;
            if (playerstat.levelupcount == playerstat.needlevelupcount)
            {
                playerstat.lv++;
                playerstat.power += 0.5f;
                playerstat.defense++;
                playerstat.levelupcount = 0;
                playerstat.needlevelupcount++;
                Console.WriteLine($"레벨업! 현재 레벨: {playerstat.lv}");
            }

        }

        public static void BackStartScene()// 모든 씬 뒤에 넣기
        {
            Console.WriteLine("엔터키를 눌러 뒤로 돌아가기");
            Console.ReadLine();
            StartScene();
        }

        static void BasicItemsList()
        {
            myitemList.Add(new Item("스파르타의 갑옷", 2, "방어구", true, 50));
            myitemList.Add(new Item("무쇠갑옷", 5, "방어구", true, 70));
            myitemList.Add(new Item("낡은 검", 2, "무기", true, 600));
            myitemList.Add(new Item("원더 완드", 3, "무기", true, 700));
            myitemList.Add(new Item("수련자의 갑옷", 5, "방어구", false, 1000));
            myitemList.Add(new Item("청동 도끼", 2, "무기", false, 1500));
            myitemList.Add(new Item("데몬의 도끼", 10, "무기", false, 7000));
            myitemList.Add(new Item("저주의 가면", -5, "방어구", false, 1));
        }
        public static void StartScene()
        {
            SaveGame();
            Console.WriteLine();
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine($">>> {playerstat.playername}님 원하시는 행동을 입력해주세요");
            Console.WriteLine("1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 휴식\n5. 던전 입장");
            Console.WriteLine();
            Console.Write(">>>");

            int a = int.Parse(Console.ReadLine());

            if (0 <= a && a < 6)
            {
                if (a == 1)
                {
                    ShowPlayerStats();
                }
                else if (a == 2)
                {
                    Inventory();
                }
                else if (a == 3)
                {
                    Store();
                }
                else if (a == 4)
                {
                    Resting();
                }
                else if (a == 5)
                {
                    GoDungeon();
                }
                else if (a == 0)
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                StartScene();
            }
            static void SaveGame()
            {
                save.PlayerName = playerstat.playername;
                save.Level = playerstat.lv;
                save.LevelUpCount = playerstat.levelupcount;
                save.NeedLevelUpCount = playerstat.needlevelupcount;
                save.Health = playerstat.hp;
                save.Gold = playerstat.gold;
                save.DungeonDefense = playerstat.dungeondefense;
                save.Power = playerstat.power;
                save.Defense = playerstat.defense;
                save.InventoryEquip = playerstat.inventoryEquip;
                save.SaveData();
                Console.WriteLine("게임이 저장되었습니다.");
            }
        }
    }
}