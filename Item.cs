using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    public class Item
    {
        public string ItemName { get; set; }
        public int ItemPower { get; set; }
        public string ItemType { get; set; }
        public bool IsShopItem { get; set; }
        public int ShopsellGold { get; set; }

        public Item(string itemName, int itemPower, string itemType, bool isShopItem, int shopsellgold)
        {
            ItemName = itemName;
            ItemPower = itemPower;
            ItemType = itemType;
            IsShopItem = isShopItem;
            ShopsellGold = shopsellgold;
        }
    }

}
