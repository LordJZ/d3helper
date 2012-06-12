using System.Linq;
using D3;

namespace d3helper.Classes
{
    class Wizard : ClassBase
    {
        private SNOPowerId[] WSkills =
        {
            SNOPowerId.Wizard_DiamondSkin, // 1
            SNOPowerId.Wizard_Teleport, // 2
            SNOPowerId.Wizard_EnergyArmor, // 3
            SNOPowerId.Wizard_Blizzard, // 4
            SNOPowerId.Wizard_ShockPulse, // left mouse
            SNOPowerId.Wizard_ArcaneOrb // right mouse
        };

        public Wizard()
        {
            Skills = WSkills;
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
