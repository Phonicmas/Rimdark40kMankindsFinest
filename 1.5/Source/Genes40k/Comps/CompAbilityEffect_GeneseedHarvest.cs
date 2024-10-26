using RimWorld;
using Verse;


namespace Genes40k
{
    public class CompAbilityEffect_GeneseedHarvest : CompAbilityEffect
    {
        public new CompProperties_AbilityGeneseedHarvest Props => (CompProperties_AbilityGeneseedHarvest)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var corpse = target.Thing as Corpse;
            var pawn = corpse.InnerPawn;

            Genes40kUtils.MakeGeneseedVial(pawn, Genes40kUtils.IsPrimaris(pawn));

            pawn.health.AddHediff(Genes40kDefOf.BEWH_SecondGeneseedHarvest, null);

            base.Apply(target, dest);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            base.Valid(target, throwMessages);
            if (throwMessages)
            {

            }
            if (!(target.Thing is Corpse))
            {
                return false;
            }
            var corpse = target.Thing as Corpse;

            if (corpse.InnerPawn.genes == null || !corpse.InnerPawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ProgenoidGlands) || corpse.InnerPawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_SecondGeneseedHarvest) || !Genes40kDefOf.BEWH_GeneseedExtractionFirstborn.IsFinished)
            {
                return false;
            }
            return !corpse.InnerPawn.genes.HasActiveGene(Genes40kDefOf.BEWH_BelisarianFurnace) || Genes40kDefOf.BEWH_GeneseedExtractionPrimaris.IsFinished;
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            if (target.Thing != null && target.Thing is Corpse corpse && corpse.InnerPawn != null)
            {
                if (corpse.InnerPawn.genes == null)
                {
                    return null;
                }
                if (!Genes40kDefOf.BEWH_GeneseedExtractionFirstborn.IsFinished)
                {
                    return "BEWH.SMGeneseedExtractionNotResearched".Translate();
                }
                if (!corpse.InnerPawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ProgenoidGlands))
                {
                    return "BEWH.NoProgenoidGlands".Translate();
                }
                if (corpse.InnerPawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_SecondGeneseedHarvest))
                {
                    return "BEWH.SecondaryGlandsAlreadyHarvested".Translate();
                }
                if (corpse.InnerPawn.genes.HasActiveGene(Genes40kDefOf.BEWH_BelisarianFurnace) && !Genes40kDefOf.BEWH_GeneseedExtractionPrimaris.IsFinished)
                {
                    return "BEWH.PMGeneseedExtractionNotResearched".Translate();
                }

            }
            return null;
        }

    }
}