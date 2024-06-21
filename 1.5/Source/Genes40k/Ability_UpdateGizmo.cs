using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Ability_UpdateGizmo : Ability
    {
        public void SetIcon(Texture2D texture)
        {
            gizmo.icon = texture;
        }

        public Ability_UpdateGizmo()
        {
        }

        public Ability_UpdateGizmo(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public Ability_UpdateGizmo(Pawn pawn, Precept sourcePrecept)
        {
            this.pawn = pawn;
            this.sourcePrecept = sourcePrecept;
        }

        public Ability_UpdateGizmo(Pawn pawn, AbilityDef def)
        {
            this.pawn = pawn;
            this.def = def;
            Initialize();
        }

        public Ability_UpdateGizmo(Pawn pawn, Precept sourcePrecept, AbilityDef def)
        {
            this.pawn = pawn;
            this.def = def;
            this.sourcePrecept = sourcePrecept;
            Initialize();
        }
    }
}