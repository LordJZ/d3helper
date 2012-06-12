using System.Linq;
using D3;

namespace d3helper.Classes
{
    class Barbarian : ClassBase
    {
        private SNOPowerId[] BSkills =
        {
            SNOPowerId.Barbarian_Overpower, // 1
            SNOPowerId.Barbarian_Whirlwind, // 2
            SNOPowerId.Barbarian_Revenge, // 3
            SNOPowerId.Barbarian_WrathOfTheBerserker, // 4
            SNOPowerId.Barbarian_Bash, // left mouse
            SNOPowerId.Barbarian_Rend // right mouse
        };

        public Barbarian()
        {
            Skills = BSkills;
        }

        public override bool Attack(SNOActorId actorId)
        {
            var units = Unit.Get().Where(a => a.ActorId == actorId);

            if (!units.Any())
                return false;

            // getting closest unit
            var u = units.Where(x => x.Life > 0 && x.Mode != UnitMode.Dead).OrderBy(x => BotBase.GetDistance(x)).FirstOrDefault();

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
