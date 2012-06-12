using System;
using D3;

namespace d3helper.Bots
{
    class StatsBot : BotBase
    {
        // Represents whether or not the UI should be display, change this to false if you do not want to see it.
        private readonly bool DISPLAY_UI = true;

        private DateTime StartTime = DateTime.Now; // bot start time

        private int StartingScriptGold = 0;
        private int StartingGameGold = 0;
        private int CurrentGold = 0;
        private int TotalGold = 0;
        private int HighestGold = 0;
        private int TotalRuns = 0;
        private int FailedRuns = 0;
        private int GoodRuns = 0;
        private bool HasDied = false;

        public override void Start()
        {
            StartTime = DateTime.Now;

            Game.OnTickEvent += OnTick;
            Game.OnGameLeaveEvent += OnGameLeave;

            if (DISPLAY_UI)
                Game.OnDrawEvent += OnDraw;
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        protected override void OnGameLeave(EventArgs e)
        {
            TotalRuns++;

            if (HasDied)
                FailedRuns++;
            else
                GoodRuns++;

            HasDied = false;
        }

        protected override void OnTick(EventArgs e)
        {
            if (!Game.Ingame)
                return;

            if (TotalRuns == 0)
                BeginGold();

            if (Me.InTown)
                InitGold();

            UpdateGold();

            if (Me.Life == 0 && Me.Mode == UnitMode.Dead)
                HasDied = true;
        }

        protected override void OnDraw(EventArgs e)
        {
            if (Game.Ingame && Debug)
                Draw.DrawText(10.0f, 10.0f, 0x16A, 0x16, 0xFFFFFFFF, String.Format("Quest: {0}, step {1}, LevelArea {2}, Mode {3}, Position {4} {5}",
                    Me.QuestId, Me.QuestStep, Me.LevelArea, Me.Mode, Me.X, Me.Y));

            if (Game.Ingame)
            {
                TimeSpan ts = DateTime.Now - StartTime;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                string display = "ElapsedTime: " + elapsedTime + "\n" +
                    "Total gold made: " + TotalGold + "\n" +
                    "Average gold per run: " + (TotalGold / (TotalRuns + 1)) + "\n" +
                    "Highest gold per run: " + HighestGold + "\n" +
                    "Total runs: " + TotalRuns + "\n" +
                    "Successful runs: " + GoodRuns + " \n" +
                    "Failed runs: " + FailedRuns + "\n";
                Draw.DrawText(20, 50, 1, 20, 0xFFFFFFFF, display);
            }
        }

        private void BeginGold()
        {
            StartingScriptGold = Me.Gold;
        }

        private void InitGold()
        {
            StartingGameGold = Me.Gold;
            CurrentGold = 0;
        }

        private void UpdateGold()
        {
            CurrentGold = Me.Gold - StartingGameGold;
            TotalGold = Me.Gold - StartingScriptGold;
            if (CurrentGold > HighestGold)
                HighestGold = CurrentGold;
        }
    }
}
