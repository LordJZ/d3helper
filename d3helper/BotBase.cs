using System;
using System.Linq;
using System.Threading;
using D3;

namespace d3helper
{
    abstract class BotBase
    {
        //Represents the lowest MF value an item can have before being sold,
        //for example, if MIN_MAGIC_FIND = 0.12f, that means if the item in inventory does not have at least 12%, it will be sold.
        protected float MIN_MAGIC_FIND = 0.15f;

        //Represents the lowest GF value an item can have before being sold,
        //for example, if MIN_GOLD_FIND = 0.12f, that means if the item in inventory does not have at least 12%, it will be sold.
        protected float MIN_GOLD_FIND = 0.15f;

        //Represents the lowest hp per hit value an item can have before being sold,
        //for example, if MIN_HP_PER_HIT is 700, that means if the item in inventory does not have at least 700 hit per hit, it will be sold.
        protected float MIN_HP_PER_HIT = 600.0f;

        //Represents the lowest number of sockets an item can have before being sold,
        //for example, if MIN_NUMBER_OF_SOCKETS = 2, that means if the item in inventory does not have at least 2 sockets, it will be sold.
        protected float MIN_NUMBER_OF_SOCKETS = 1.0f;

        //Represents the lowest resistance to all value an item can have before being sold,
        //for example, if MIN_RES_TO_ALL = 50, that means if the item in inventory does not have at least 50 Res to all, it will be sold.
        protected float MIN_RES_TO_ALL = 50.0f;

        //Represents the lowest DPS value an item can have before being sold,
        //for example, if MIN_DPS = 500.0f, that means if the item in inventory does not have at least 500 DPS, it will be sold.
        protected float MIN_DPS = 800.0f;

        public static readonly bool Debug = true;

        private const ulong PlayGameButton = 0x51A3923949DC80B7;
        private const ulong CloseConversationButton = 0x942F41B6B5346714;
        private const ulong ReviveAtCheckpointButton = 0xBFAAF48BA9316742;
        private const ulong ExitGameButton = 0x5DB09161C4D6B4C6;
        private const ulong RepairAllButton = 0x80F5D06A035848A5;
        private const ulong DurabilityIndicator = 0xBD8B3C3679D4F4D9;

        protected int NextTick = Environment.TickCount;
        protected ClassBase Class;

        public abstract void Start();
        public abstract void Stop();

        protected virtual void OnTick(EventArgs e)
        {

        }

        protected virtual void OnDraw(EventArgs e)
        {

        }

        protected virtual void OnGameEnter(EventArgs e)
        {

        }

        protected virtual void OnGameLeave(EventArgs e)
        {

        }

        protected virtual void OnSceneActivate(EventArgs e)
        {

        }

        protected virtual void OnSceneDeactivate(EventArgs e)
        {

        }

        protected bool Attack(SNOActorId actorId)
        {
            SetTimer(100);
            return Class.Attack(actorId);
        }

        public void SetClass(ClassBase _class)
        {
            Class = _class;
        }

        protected bool ExitGame()
        {
            if (Debug)
                Print("ExitGame");
            SetTimer(1500);
            return Click(ExitGameButton, true);
        }

        protected bool GoToTown()
        {
            if (Me.InTown)
                return true;

            if (!Me.InTown && Me.Mode != UnitMode.Casting && Me.Mode != UnitMode.Warping)
            {
                if (Debug)
                    Print("GoToTown");
                SetTimer(7000);
                Me.UsePower(SNOPowerId.UseStoneOfRecall);
            }
            return false;
        }

        protected bool RepairAll()
        {
            if (Debug)
                Print("RepairAll");
            return Click(RepairAllButton, true);
        }

        protected bool Click(ulong hash, bool force = false)
        {
            var ui = UIElement.Get(hash);
            if (ui != null && (force || ui.Visible))
            {
                if (Debug)
                    Print("Clicking {0} (0x{1:X16})", ui.Name, ui.Hash);
                ui.Click();
                return true;
            }
            return false;
        }

        protected bool Interact(SNOActorId actorId)
        {
            var u = Unit.Get().FirstOrDefault(x => x.ActorId == actorId);
            if (u != null)
            {
                Interact(u);
                return true;
            }
            return false;
        }

        protected bool Interact(Unit unit)
        {
            if (unit == null)
                throw new ArgumentNullException("unit");

            if (Debug)
                Print("Interacting with {0} ({1})", unit.Name, unit.ActorId);
            SetTimer(250);
            return Me.UsePower(unit.Type == UnitType.Gizmo || unit.Type == UnitType.Item ? SNOPowerId.Axe_Operate_Gizmo : SNOPowerId.Axe_Operate_NPC, unit);
        }

        protected bool MoveTo(float x, float y)
        {
            if (Debug)
                Print("Moving to {0}, {1}", x, y);
            if (GetDistance(x, y) < 5.0f)
                return true;
            Me.UsePower(SNOPowerId.Walk, x, y, Me.Z);
            SetTimer(500);
            return false;
        }

        protected bool MoveTo(SNOActorId actorId)
        {
            var u = Unit.Get().FirstOrDefault(x => x.ActorId == actorId);
            if (u == null)
                return false;
            return MoveTo(u);
        }

        protected bool MoveTo(Unit u, float distance = 5.0f)
        {
            if (Debug)
                Print("Moving to {0}", u.Name);
            if (GetDistance(u) < distance)
                return true;
            Me.UsePower(SNOPowerId.Walk, u);
            SetTimer(500);
            return false;
        }

        public static float GetDistance(Unit u)
        {
            return GetDistance(u.X, u.Y);
        }

        protected static float GetDistance(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(Me.X - x, 2) + Math.Pow(Me.Y - y, 2));
        }

        protected bool NeedRepair()
        {
            return UIElement.Get(DurabilityIndicator).IsValidAndVisible();
        }

        protected bool IsInventoryFull()
        {
            return Unit.Get().Count(u => u.ItemContainer == Container.Inventory) >= 30;
        }

        protected int GetInventoryFreeSlots()
        {
            return 60 - Unit.Get().Where(i => i.ItemContainer == Container.Inventory).Sum(u => u.ItemSizeX * u.ItemSizeY);
        }

        protected bool SnagItems()
        {
            if (Debug)
                Print("SnagItems");

            Unit item = Unit.Get()
                .Where(u => u.ItemContainer == Container.Unknown && CheckItem(u))
                .OrderBy(u => GetDistance(u)).FirstOrDefault();

            if (item == null)
                return true;

            if (Debug)
                Print("Picking up: {0}", item.Name);

            if (MoveTo(item))
                Interact(item);

            return false;
        }

        protected bool CheckItem(Unit unit)
        {
            if (unit.Type != UnitType.Item)
                return false;

            SNOActorId ActorId = unit.ActorId;

            if (Debug)
                Print("CheckItem: {0} - {1}", unit.Name, ActorId);

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
        protected bool SellItems()
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

        protected static bool CheckCollision(Unit unit)
        {
            return !Game.CheckCollision(unit, NavMeshFlags.ALLOW_PROJECTILE);
        }

        public static void Print(string message)
        {
            if (Debug)
                Logger.Add(message);
            Game.Print(message);
        }

        public static void Print(string format, params object[] args)
        {
            Print(String.Format(format, args));
        }

        protected void SetTimer(int delay)
        {
            NextTick = Environment.TickCount + delay;
        }

        protected bool StartGame()
        {
            SetTimer(2000);
            return !Click(PlayGameButton);
        }

        protected bool CloseConversation()
        {
            SetTimer(200);
            return !Click(CloseConversationButton);
        }

        protected bool ReviveAtCheckpoint()
        {
            SetTimer(1000);
            return !Click(ReviveAtCheckpointButton);
        }
    }
}
