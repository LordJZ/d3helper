using System.Linq;
using System.Threading;
using D3;

namespace d3helper
{
    class SnagIt
    {
        //Represents the lowest MF value an item can have before being sold,
        //for example, if MIN_MAGIC_FIND = 0.12f, that means if the item in inventory does not have at least 12%, it will be sold.
        private const float MIN_MAGIC_FIND = 0.15f;

        //Represents the lowest GF value an item can have before being sold,
        //for example, if MIN_GOLD_FIND = 0.12f, that means if the item in inventory does not have at least 12%, it will be sold.
        private const float MIN_GOLD_FIND = 0.15f;

        //Represents the lowest hp per hit value an item can have before being sold,
        //for example, if MIN_HP_PER_HIT is 700, that means if the item in inventory does not have at least 700 hit per hit, it will be sold.
        private const float MIN_HP_PER_HIT = 600.0f;

        //Represents the lowest number of sockets an item can have before being sold,
        //for example, if MIN_NUMBER_OF_SOCKETS = 2, that means if the item in inventory does not have at least 2 sockets, it will be sold.
        private const float MIN_NUMBER_OF_SOCKETS = 1.0f;

        //Represents the lowest resistance to all value an item can have before being sold,
        //for example, if MIN_RES_TO_ALL = 50, that means if the item in inventory does not have at least 50 Res to all, it will be sold.
        private const float MIN_RES_TO_ALL = 50.0f;

        //Represents the lowest DPS value an item can have before being sold,
        //for example, if MIN_DPS = 500.0f, that means if the item in inventory does not have at least 500 DPS, it will be sold.
        private const float MIN_DPS = 800.0f;

        public static bool SnagItems()
        {
            if (Bot.Debug)
                Bot.Print("SnagItems");

            Unit item = Unit.Get()
                .Where(u => u.ItemContainer == Container.Unknown && CheckItem(u))
                .OrderBy(u => Bot.GetDistance(u)).FirstOrDefault();

            if (item == null)
                return true;

            if (Bot.Debug)
                Bot.Print("Picking up: {0}", item.Name);

            Bot.Interact(item);

            return false;
        }

        protected static bool CheckItem(Unit unit)
        {
            if (unit.Type != UnitType.Item)
                return false;

            SNOActorId ActorId = unit.ActorId;

            if (Bot.Debug)
                Bot.Print("CheckItem: {0} - {1}", unit.Name, ActorId);

            // Edit to whatever you want to pickup.
            return unit.IsGemItem() // Gems
                || ActorId == SNOActorId.healthPotion_Mythic
                || ActorId == SNOActorId.Crafting_Training_Page_Jeweler
                || ActorId == SNOActorId.Crafting_Training_Page_Jeweler_Hell
                || ActorId == SNOActorId.Crafting_Training_Page_Smith
                || ActorId == SNOActorId.Crafting_Training_Page_Smith_Hell
                || ActorId == SNOActorId.CraftingPlan_Jeweler_Drop
                || ActorId == SNOActorId.CraftingPlan_Smith_Drop
                //|| ActorId == SNOActorId.Crafting_Training_Tome // Tome of Secrets (in inventory)
                || ActorId == SNOActorId.Lore_Book_Flippy // Tome of Secrets
                || unit.IsGoldItem()        // Pickup gold
                || unit.IsMagicItem()       // Pickup magic items
                || unit.IsRareItem()        // Pickup rare items
                || unit.IsLegendaryItem()   // Pickup legendary items
                || unit.IsArtifactItem();   // Pickup artifact items
        }

        // this code causing disconnects sometimes (selling too fast?)
        public static bool SellItems()
        {
            foreach (Unit item in Unit.Get().Where(u => u.ItemContainer == Container.Inventory))
            {
                if (item.ItemQuality >= UnitItemQuality.Magic1 && item.ItemQuality < UnitItemQuality.Rare6)
                {
                    float mf = item.GetItemMF();
                    float gf = item.GetItemGF();
                    float dps = item.GetItemDPS();
                    float sc = item.GetAttributeInteger(UnitAttribute.Sockets);
                    float hp = item.GetAttributeReal(UnitAttribute.Hitpoints_On_Hit);
                    float res = item.GetAttributeReal(UnitAttribute.Resistance_All);

                    //switch (item.ItemType)
                    //{
                    //    case UnitItemType.Axe:
                    //    case UnitItemType.Axe2H:
                    //    case UnitItemType.Bow:
                    //    case UnitItemType.BowClass:
                    //    case UnitItemType.CeremonialDagger:
                    //    case UnitItemType.CombatStaff:
                    //    case UnitItemType.Crossbow:
                    //    case UnitItemType.Dagger:
                    //    case UnitItemType.FistWeapon:
                    //    case UnitItemType.GenericBowWeapon:
                    //    case UnitItemType.GenericRangedWeapon:
                    //    case UnitItemType.GenericSwingWeapon:
                    //    case UnitItemType.GenericThrustWeapon:
                    //    case UnitItemType.HandXbow:
                    //    case UnitItemType.Mace:
                    //    case UnitItemType.Mace2H:
                    //    case UnitItemType.MightyWeapon1H:
                    //    case UnitItemType.MightyWeapon2H:
                    //    case UnitItemType.Polearm:
                    //    case UnitItemType.Spear:
                    //    case UnitItemType.Staff:
                    //    case UnitItemType.Sword:
                    //    case UnitItemType.Sword2H:
                    //    case UnitItemType.Wand:
                    //        break;
                    //}

                    // If the item does not have any of the desired attributes, it will be sold.
                    if ((mf > MIN_MAGIC_FIND && item.HasAnyStats()) ||
                        (gf > MIN_GOLD_FIND && item.HasAnyStats()) ||
                        dps > MIN_DPS ||
                        hp > MIN_HP_PER_HIT ||
                        // Socketed + main stat
                        (sc >= MIN_NUMBER_OF_SOCKETS && item.HasAnyStats()) ||
                        // All Resists + main stat
                        (res >= MIN_RES_TO_ALL && item.HasAnyStats()))
                        continue;

                    item.SellItem();
                    Thread.Sleep(100);
                }
            }

            return true;
        }
    }
}
