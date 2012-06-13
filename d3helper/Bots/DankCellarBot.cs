using System;
using System.Collections.Generic;
using System.Linq;
using D3;

namespace d3helper.Bots
{
    class DankCellarBot : Bot
    {
        // Max run time per game, milliseconds
        private const int MAXIMUM_RUN_TIME = 1500000;

        private int StartTicks = Environment.TickCount; // current game start ticks
        private int CurrentError = 0;
        private int FrameCounter = 0;
        private int CurrentStep = 0;
        private readonly List<Func<bool>> Steps = new List<Func<bool>>();

        // Used actors (NPC/Objects)
        //readonly SNOActorId DunkCellarClosed = SNOActorId.trOut_OldTristram_CellarDoor_Boarded;
        readonly SNOActorId DunkCellarOpen = SNOActorId.g_Portal_Square_Blue;
        readonly SNOActorId Vendor = SNOActorId.A1_UniqueVendor_Collector_InTown_05;
        int SkipBattle = 0;
        int SkipVendor = 0;

        public override void Start()
        {
            Print("Sword of the Stranger (Act I) Inferno, Old Ruins Checkpoint Required");

            m_waitbeforestart = false;

            Steps.Add(() =>
            {
                if (!Game.Ingame && m_waitbeforestart)
                {
                    m_waitbeforestart = false;
                    SetTimer(10000);
                    return false;
                }

                return Game.Ingame || StartGame();
            });

            Steps.Add(() => Player.UseSkill(0)); // Summon Companion (DH only)

            SetTimer(200);

            // 1991.747 2653.501
            Steps.Add(() => Player.UseSkill(2, 1998.5f, 2622.3f, Me.Z) || true);
            Steps.Add(() => Player.UseSkill(2, 2020f, 2575.5f, Me.Z) || true);
            Steps.Add(() => Player.UseSkill(3) || true); // Preparation (DH only)
            Steps.Add(() => MoveTo(2026f, 2563f));
            Steps.Add(() => Player.UseSkill(2, 2043f, 2533f, Me.Z) || true);

            Steps.Add(() =>
            {
                var u = Unit.Get().FirstOrDefault(x => x.ActorId == DunkCellarOpen);
                if (u == null)
                {
                    CurrentStep = SkipBattle;
                    // jump away
                    if (Player.UseSkill(2, 2022f, 2506f, Me.Z))
                        SetTimer(200);
                    return false;
                }
                else
                    return true;
            });

            Steps.Add(() => Player.UseSkill(2, 2074f, 2503f, Me.Z) || true);
            Steps.Add(() => MoveTo(DunkCellarOpen));
            Steps.Add(() => Interact(DunkCellarOpen));
            Steps.Add(() => Player.UseSkill(2, 118f, 153f, Me.Z) || true);
            Steps.Add(() => MoveTo(124f, 140f));
            Steps.Add(() =>
            {
                var u = Unit.Get().FirstOrDefault(a => a.ActorId.ToString().IndexOf("Quill") >= 0);
                if (u != null)
                    return !Attack(u.ActorId);
                else
                    return true;
            });
            Steps.Add(() => MoveTo(120f, 120f));
            Steps.Add(() =>
            {
                var u = Unit.Get().FirstOrDefault(a => a.ActorId.ToString().IndexOf("Quill") >= 0);
                if (u != null)
                    return !Attack(u.ActorId);
                else
                    return true;
            });
            Steps.Add(() =>
            {
                SetTimer(300);
                return SnagIt.SnagItems();
            });

            SkipBattle = Steps.Count;
            Steps.Add(() => GoToTown());
            Steps.Add(() =>
            {
                // Check if the bot needs to repair or sell items, if so, move to the vendor
                if (NeedRepair() || IsInventoryFull())
                {
                    //foreach (var s in Unit.Get()
                    //        .Where(x => x.ActorId.ToString().IndexOf("Collector") >= 0)
                    //        .Select(x => x.ActorId.ToString() + " at " + x.X + " " + x.Y))
                    //    Print(s);
                    return MoveTo(Vendor);
                }
                else
                {
                    CurrentStep = SkipVendor; // skip remaining steps
                    return false;
                }
            });
            Steps.Add(() => Interact(Vendor));
            Steps.Add(() => RepairAll());
            Steps.Add(() => SnagIt.SellItems());

            SkipVendor = Steps.Count;
            Steps.Add(() => ExitGame());

            Game.OnGameEnterEvent += OnGameEnter;
            Game.OnGameLeaveEvent += OnGameLeave;
            Game.OnTickEvent += OnTick;
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        bool m_waitbeforestart;
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
                m_waitbeforestart = true;
                ExitGame();
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
            StartTicks = Environment.TickCount + (m_waitbeforestart ? 10000 : 0);
        }

        private void Pulse()
        {
            if (Steps[CurrentStep]())
                ++CurrentStep;
        }
    }
}
