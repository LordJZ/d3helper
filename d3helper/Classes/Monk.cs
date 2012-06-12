using System.Linq;
using D3;

namespace d3helper.Classes
{
    class Monk : Player
    {
        private SNOPowerId[] MSkills =
        {
            SNOPowerId.Monk_MysticAlly, // 1
            SNOPowerId.Monk_MantraOfEvasion, // 2
            SNOPowerId.Monk_BlindingFlash, // 3
            SNOPowerId.Monk_Serenity, // 4
            SNOPowerId.Monk_WayOfTheHundredFists, // left mouse
            SNOPowerId.Monk_SevenSidedStrike // right mouse
        };

        public Monk()
        {
            Skills = MSkills;
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

            if (Me.PrimaryResource / Me.MaxPrimaryResource > 0.5f)
                UseSkill(5, u);
            else
                UseSkill(4, u);

            return true;
        }
    }
}
