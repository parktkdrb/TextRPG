using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    public class PlayerEquippedItems
    {
        public string EquippedWeapon { get; set; }
        public string EquippedArmor { get; set; }
        public PlayerEquippedItems() { }

        public PlayerEquippedItems(string weaponName, string armorName)
        {
            EquippedWeapon = weaponName;
            EquippedArmor = armorName;
        }
    }

}
