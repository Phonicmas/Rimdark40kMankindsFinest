using RimWorld;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class RoleRequirement_MustBeSpaceMarine : RoleRequirement
    {
        [NoTranslate]
        private string labelCached;

        public override string GetLabel(Precept_Role role)
        {
            return labelCached ?? (labelCached = "BEWH.MankindsFinest.Ideology.MustBeSpaceMarine".Translate());
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if (p.genes == null)
            {
                return false;
            }
            return p.IsFirstborn();
        }
    }
}