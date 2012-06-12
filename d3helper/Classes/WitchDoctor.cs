using System.Linq;
using D3;

namespace d3helper.Classes
{
    class WitchDoctor : Player
    {
        private SNOPowerId[] WDSkills =
        {
            SNOPowerId.Witchdoctor_FetishArmy, // 1
            SNOPowerId.Witchdoctor_SoulHarvest, // 2
            SNOPowerId.Witchdoctor_Hex, // 3
            SNOPowerId.Witchdoctor_SpiritWalk, // 4
            SNOPowerId.Witchdoctor_PoisonDart, // left mouse
            SNOPowerId.Witchdoctor_Firebats // right mouse
        };

        public WitchDoctor()
        {
            Skills = WDSkills;
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
