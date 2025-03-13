using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Ability_UpdateGizmo : Ability
    {
        private static readonly Texture2D MindShieldOffIcon = ContentFinder<Texture2D>.Get("UI/Abilities/BEWH_MindShieldOff");
        private static readonly Texture2D MindShieldOnIcon = ContentFinder<Texture2D>.Get("UI/Abilities/BEWH_MindShieldOn");
        
        private void SetIcon(Texture2D texture)
        {
            gizmo.icon = texture;
        }

        public override string Tooltip
        {
            get
            {
                var text = base.Tooltip;

                var warpShield = pawn?.genes?.GetFirstGeneOfType<Gene_WarpShield>();

                if (warpShield == null)
                {
                    return text;
                }
                
                var textBit = warpShield.IsShielded ? "BEWH.MankindsFinest.CommonKeywords.On".Translate() : "BEWH.MankindsFinest.CommonKeywords.Off".Translate();
                
                return text + "\n\n" + "BEWH.MankindsFinest.WarpShield.Tooltip".Translate(textBit);
            }
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

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!base.Activate(target, dest))
            {
                return false;
            }

            var warpShield = pawn?.genes?.GetFirstGeneOfType<Gene_WarpShield>();

            if (warpShield == null)
            {
                return false;
            }
            
            warpShield.SwitchShieldState();

            SetIcon(warpShield.IsShielded ? MindShieldOffIcon : MindShieldOnIcon);

            return true;
        }
    }
}