using D3;

namespace d3helper
{
    abstract class Player
    {
        //Represents the lowest percentage of health a bot can have before attempting to heal,
        //for example, if MIN_HEALTH = 0.6f, the bot will attempt to heal at 60% HP or lower.
        public readonly float MIN_HEALTH = 0.6f;

        protected SNOPowerId[] Skills;

        public abstract bool Attack(SNOActorId actorId);

        public bool UseSkill(int index, Unit target = null)
        {
            if (Me.IsSkillReady(Skills[index]))
            {
                if (target != null)
                    return Me.UsePower(Skills[index], target);
                else
                    return Me.UsePower(Skills[index]);
            }
            return false;
        }

        public bool UseSkill(int index, float x, float y, float z)
        {
            if (Me.IsSkillReady(Skills[index]))
                return Me.UsePower(Skills[index], x, y, z);
            return false;
        }
    }
}
