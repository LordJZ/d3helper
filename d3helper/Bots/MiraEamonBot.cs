using System;
using System.Collections.Generic;
using System.Linq;
using D3;

namespace d3helper.Bots
{
    class MiraEamonBot : Bot
    {
        // Max run time per game, milliseconds
        private const int MAXIMUM_RUN_TIME = 150000;

        private int StartTicks = Environment.TickCount; // current game start ticks
        private int CurrentError = 0;
        private int FrameCounter = 0;
        private int CurrentStep = 0;
        private readonly List<Func<bool>> Steps = new List<Func<bool>>();

        // Used actors (NPC/Objects)
        private readonly SNOActorId CellarOfDamned = SNOActorId.g_Portal_Square_Orange;
        private readonly SNOActorId RavenousDead = SNOActorId.ZombieSkinny_B;
        private readonly SNOActorId Cain = SNOActorId.Cain;
        private readonly SNOActorId HaedrigEamon = SNOActorId.PT_Blacksmith_NonVendor;
        private readonly SNOActorId TashunTheMiner = SNOActorId.A1_UniqueVendor_Miner_InTown_03;
        private readonly SNOActorId MiraEamon = SNOActorId.ZombieFemale_A_BlacksmithA;
        private readonly SNOActorId SturdyBoardedDoor = SNOActorId.trDun_Blacksmith_CellarDoor_Breakable;

        public override void Start()
        {
            Print("A Shattered Crown (part 1) now active.");

            if (Game.Ingame)
                ExitGame();

            Steps.Add(() => !Game.Ingame);
            Steps.Add(() => ChangeQuest());
            Steps.Add(() => StartGame());
            Steps.Add(() => Game.Ingame && Me.InTown);

            // Cain
            Steps.Add(() => MoveTo(Cain));
            Steps.Add(() => Interact(Cain));
            Steps.Add(() => CloseConversation());

            Steps.Add(() => MoveTo(3003.098f, 2790.004f) && Player.UseSkill(1)); // Summon Companion (DH only)
            Steps.Add(() => MoveTo(2993.186f, 2718.52f));
            Steps.Add(() => MoveTo(2954.805f, 2711.546f));

            // Haedrig Eamon
            Steps.Add(() => MoveTo(HaedrigEamon));
            Steps.Add(() => Interact(HaedrigEamon));
            Steps.Add(() => CloseConversation());

            Steps.Add(() => MoveTo(2792.586f, 2619.849f));
            Steps.Add(() => Interact(CellarOfDamned));
            Steps.Add(() => MoveTo(175.9097f, 150.4982f));
            Steps.Add(() => !Attack(SturdyBoardedDoor));
            Steps.Add(() => MoveTo(155.9097f, 150.4982f));
            Steps.Add(() => Unit.Get().Count(a => a.ActorId == RavenousDead) >= 2);
            Steps.Add(() => !Attack(RavenousDead));
            Steps.Add(() => MoveTo(110f, 150.4982f));
            Steps.Add(() => Unit.Get().Count(a => a.ActorId == RavenousDead) >= 2);
            Steps.Add(() => !Attack(RavenousDead));
            Steps.Add(() => MoveTo(65.0f, 145.0f) && Player.UseSkill(3)); // Place Caltrops (DH only)
            Steps.Add(() => MoveTo(125.9097f, 145f));
            Steps.Add(() => Me.PrimaryResource == Me.MaxPrimaryResource ? true : Player.UseSkill(4, 40, 143, Me.Z) && false); // Generate resource
            Steps.Add(() => Unit.Get().Any(a => a.ActorId == MiraEamon));
            Steps.Add(() => !Attack(MiraEamon));
            Steps.Add(() => SnagIt.SnagItems());

            // Haedrig Eamon
            Steps.Add(() => MoveTo(HaedrigEamon));
            Steps.Add(() => Interact(HaedrigEamon));
            Steps.Add(() => CloseConversation());

            Steps.Add(() => SnagIt.SnagItems());
            Steps.Add(() => GoToTown());
            Steps.Add(() =>
            {
                // Check if the bot needs to repair or sell items, if so, move to the vendor
                if (NeedRepair() || IsInventoryFull())
                {
                    if (MoveTo(TashunTheMiner)) // Tashun the Miner
                        return true;
                    return false;
                }
                else
                {
                    CurrentStep = 0; // skip remaining steps
                    ExitGame();
                    return true;
                }
            });
            Steps.Add(() => Interact(TashunTheMiner)); // Tashun the Miner
            Steps.Add(() => RepairAll());
            Steps.Add(() => SnagIt.SellItems());
            Steps.Add(() => ExitGame());

            Game.OnGameEnterEvent += OnGameEnter;
            Game.OnGameLeaveEvent += OnGameLeave;
            Game.OnTickEvent += OnTick;
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        protected override void OnTick(EventArgs e)
        {
            if (Environment.TickCount < NextTick)
                return;

            if (++FrameCounter % 10 != 0)
                return;

            if (Environment.TickCount - StartTicks > MAXIMUM_RUN_TIME)
            {
                Print("Run has taken too long, aborting");
                CurrentError = 1;
            }

            if (Game.Ingame && Me.Life == 0 && Me.Mode == UnitMode.Dead)
            {
                if (ReviveAtCheckpoint())
                    CurrentError = 1;
                else
                    return;
            }

            switch (CurrentError)
            {
                case 0:
                    Pulse();
                    break;
                case 1:
                    if (GoToTown())
                        ExitGame();
                    break;
            }
        }

        protected override void OnGameEnter(EventArgs e)
        {
            Print("OnGameEnter: {0}", Me.LevelArea); // it seems that LevelArea is invalid when called from OnGameEnter...
        }

        protected override void OnGameLeave(EventArgs e)
        {
            if (Debug)
                Print("OnGameLeave");

            CurrentError = 0;
            CurrentStep = 0;
            StartTicks = Environment.TickCount;
        }

        private bool ChangeQuest()
        {
            SetTimer(1000);

            return Game.SetQuest(3, SNOQuestId.Blacksmith, 0);
        }

        private void Pulse()
        {
            if (Steps[CurrentStep]())
                CurrentStep++;
        }
    }
}
