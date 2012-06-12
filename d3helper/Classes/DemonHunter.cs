using System.Linq;
using D3;

namespace d3helper.Classes
{
    class DemonHunter : Player
    {
        private SNOPowerId[] DHSkills =
        {
            SNOPowerId.DemonHunter_SmokeScreen, // 1
            SNOPowerId.DemonHunter_Companion, // 2
            SNOPowerId.DemonHunter_Preparation, // 3
            SNOPowerId.DemonHunter_Caltrops, // 4
            SNOPowerId.DemonHunter_HungeringArrow, // left mouse
            SNOPowerId.DemonHunter_ElementalArrow // right mouse
        };

        public DemonHunter()
        {
            Skills = DHSkills;
        }

        public override bool Attack(SNOActorId actorId)
        {
            var units = Unit.Get().Where(a => a.ActorId == actorId);

            if (!units.Any())
                return false;

            // getting closest unit
            var u = units.Where(x => x.Life > 0 && x.Mode != UnitMode.Dead).OrderBy(x => Bot.GetDistance(x)).FirstOrDefault();

            if (u == null)
                return false;

            if (Bot.Debug)
                Bot.Print("Attacking {0} ({1})", u.Name, u.ActorId);

            //look at the current health of the bot, and decide whether it's necessary to use any support moves.
            if (Me.Life / Me.MaxLife < MIN_HEALTH)
            {
                if (!UseSkill(2))   // use preparation if ready
                    UseSkill(0);    // if prep is not ready, use fog
            }

            //makes sure to use caltrops only when needed.
            if (Me.SecondaryResource == Me.MaxSecondaryResource)
                UseSkill(3);

            if (Me.PrimaryResource / Me.MaxPrimaryResource > 0.5f)
                UseSkill(5, u);
            else
                UseSkill(4, u);

            return true;
        }
    }
}
