using D3;

namespace d3helper
{
    static class Extensions
    {
        public static float GetItemDPS(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return 0.0f;

            float minDamage = u.GetAttributeReal(UnitAttribute.Damage_Weapon_Min_Total_All);
            float maxDamage = u.GetAttributeReal(UnitAttribute.Damage_Weapon_Max_Total_All);
            float speed = u.GetAttributeReal(UnitAttribute.Attacks_Per_Second_Item_Total);

            return ((minDamage + maxDamage) / 2) * speed;
        }

        public static float GetItemGF(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return 0.0f;

            return u.GetAttributeReal(UnitAttribute.Gold_Find);
        }

        public static float GetItemMF(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return 0.0f;

            return u.GetAttributeReal(UnitAttribute.Magic_Find);
        }

        public static bool IsGoldItem(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return false;

            return u.ActorId == SNOActorId.GoldCoin ||
                u.ActorId == SNOActorId.GoldCoins ||
                u.ActorId == SNOActorId.GoldLarge ||
                u.ActorId == SNOActorId.GoldMedium ||
                u.ActorId == SNOActorId.GoldSmall;
        }

        public static bool IsMagicItem(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return false;

            UnitItemQuality ItemQuality = u.ItemQuality;
            return ItemQuality == UnitItemQuality.Magic1 ||
                ItemQuality == UnitItemQuality.Magic2 ||
                ItemQuality == UnitItemQuality.Magic3;
        }

        public static bool IsRareItem(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return false;

            UnitItemQuality ItemQuality = u.ItemQuality;
            return ItemQuality == UnitItemQuality.Rare4 ||
                ItemQuality == UnitItemQuality.Rare5 ||
                ItemQuality == UnitItemQuality.Rare6;
        }

        public static bool IsLegendaryItem(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return false;

            return u.ItemQuality == UnitItemQuality.Legendary;
        }

        public static bool IsArtifactItem(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return false;

            return u.ItemQuality == UnitItemQuality.Artifact;
        }

        public static bool HasAnyStats(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return false;

            float vit = u.GetAttributeReal(UnitAttribute.Vitality_Item);
            float dex = u.GetAttributeReal(UnitAttribute.Dexterity_Item);
            float inte = u.GetAttributeReal(UnitAttribute.Intelligence_Item);
            float str = u.GetAttributeReal(UnitAttribute.Strength_Item);

            return vit > 0 || dex > 0 || inte > 0 || str > 0;
        }

        // locale independent
        public static bool IsGemItem(this Unit u)
        {
            if (u.Type != UnitType.Item)
                return false;

            SNOActorId ActorId = u.ActorId;

            return ActorId == SNOActorId.Amethyst_01 || ActorId == SNOActorId.Amethyst_02 || ActorId == SNOActorId.Amethyst_03 ||
                ActorId == SNOActorId.Amethyst_04 || ActorId == SNOActorId.Amethyst_05 || ActorId == SNOActorId.Amethyst_06 ||
                ActorId == SNOActorId.Amethyst_07 || ActorId == SNOActorId.Amethyst_08 || ActorId == SNOActorId.Amethyst_09 ||
                ActorId == SNOActorId.Amethyst_10 || ActorId == SNOActorId.Amethyst_11 || ActorId == SNOActorId.Amethyst_12 ||
                ActorId == SNOActorId.Topaz_01 || ActorId == SNOActorId.Topaz_02 || ActorId == SNOActorId.Topaz_03 ||
                ActorId == SNOActorId.Topaz_04 || ActorId == SNOActorId.Topaz_05 || ActorId == SNOActorId.Topaz_06 ||
                ActorId == SNOActorId.Topaz_07 || ActorId == SNOActorId.Topaz_08 || ActorId == SNOActorId.Topaz_09 ||
                ActorId == SNOActorId.Topaz_10 || ActorId == SNOActorId.Topaz_11 || ActorId == SNOActorId.Topaz_12 ||
                ActorId == SNOActorId.Emerald_01 || ActorId == SNOActorId.Emerald_02 || ActorId == SNOActorId.Emerald_03 ||
                ActorId == SNOActorId.Emerald_04 || ActorId == SNOActorId.Emerald_05 || ActorId == SNOActorId.Emerald_06 ||
                ActorId == SNOActorId.Emerald_07 || ActorId == SNOActorId.Emerald_08 || ActorId == SNOActorId.Emerald_09 ||
                ActorId == SNOActorId.Emerald_10 || ActorId == SNOActorId.Emerald_11 || ActorId == SNOActorId.Emerald_12 ||
                ActorId == SNOActorId.Ruby_01 || ActorId == SNOActorId.Ruby_02 || ActorId == SNOActorId.Ruby_03 ||
                ActorId == SNOActorId.Ruby_04 || ActorId == SNOActorId.Ruby_05 || ActorId == SNOActorId.Ruby_06 ||
                ActorId == SNOActorId.Ruby_07 || ActorId == SNOActorId.Ruby_08 || ActorId == SNOActorId.Ruby_09 ||
                ActorId == SNOActorId.Ruby_10 || ActorId == SNOActorId.Ruby_11 || ActorId == SNOActorId.Ruby_12;

            // should do the trick as well
            //return ActorId.Contains("Amethyst") || ActorId.Contains("Topaz") || ActorId.Contains("Emerald") || ActorId.Contains("Ruby");
        }

        public static bool IsValidAndVisible(this UIElement ui)
        {
            return ui != null && ui.Visible;
        }
    }
}
