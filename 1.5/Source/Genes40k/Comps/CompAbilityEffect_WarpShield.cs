﻿using RimWorld;
using UnityEngine;
using Verse;


namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class CompAbilityEffect_WarpShield : CompAbilityEffect
    {
        public new CompProperties_AbilityWarpShield Props => (CompProperties_AbilityWarpShield)props;

        private static readonly Texture2D MindShieldOffIcon = ContentFinder<Texture2D>.Get("UI/Abilities/BEWH_MindShieldOff");
        private static readonly Texture2D MindShieldOnIcon = ContentFinder<Texture2D>.Get("UI/Abilities/BEWH_MindShieldOn");

        private string tooltipExtra = "BEWH.MankindsFinest.Ability.CurrentToggle".Translate("BEWH.MankindsFinest.CommonKeywords.Off".Translate());

        public override string ExtraTooltipPart()
        {
            return tooltipExtra;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var hediff = parent.pawn.health.hediffSet.hediffs.Find(x => x.def.HasModExtension<DefModExtension_WarpShield>());
            if (hediff == null)
            {
                return;
            }
            if (!(parent is Ability_UpdateGizmo ability_WarpShield))
            {
                return;
            }
            if (hediff.Severity == 2)
            {
                hediff.Severity = 1;

                ability_WarpShield.SetIcon(MindShieldOnIcon);
                tooltipExtra = "BEWH.MankindsFinest.Ability.CurrentToggle".Translate("BEWH.MankindsFinest.CommonKeywords.On".Translate());
            }
            else
            {
                hediff.Severity = 2;
                
                ability_WarpShield.SetIcon(MindShieldOffIcon);
                tooltipExtra = "BEWH.MankindsFinest.Ability.CurrentToggle".Translate("BEWH.MankindsFinest.CommonKeywords.Off".Translate());
            }
        }
    }
}