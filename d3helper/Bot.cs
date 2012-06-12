using System;
using System.Linq;
using D3;

namespace d3helper
{
    abstract class Bot
    {
        public const bool Debug = true;

        private const ulong PlayGameButton = 0x51A3923949DC80B7;
        private const ulong CloseConversationButton = 0x942F41B6B5346714;
        private const ulong ReviveAtCheckpointButton = 0xBFAAF48BA9316742;
        private const ulong ExitGameButton = 0x5DB09161C4D6B4C6;
        private const ulong RepairAllButton = 0x80F5D06A035848A5;
        private const ulong DurabilityIndicator = 0xBD8B3C3679D4F4D9;

        // We need to get rid of this somehow...
        protected static int NextTick = Environment.TickCount;

        protected Player Player;

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
            return Player.Attack(actorId);
        }

        public void SetPlayer(Player _player)
        {
            Player = _player;
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

        public static bool Interact(Unit unit)
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

        protected static void SetTimer(int delay)
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
