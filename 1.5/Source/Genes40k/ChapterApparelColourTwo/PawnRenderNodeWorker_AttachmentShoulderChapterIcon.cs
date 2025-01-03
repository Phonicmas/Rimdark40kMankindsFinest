using RimWorld;
using Verse;

namespace Genes40k
{
    public class PawnRenderNodeWorker_AttachmentShoulderChapterIcon : PawnRenderNodeWorker_AttachmentBody
    {
        private Genes40kModSettings modSettings = null;

        private Genes40kModSettings ModSettings => modSettings ?? (modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>());
        
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            var pawn = parms.pawn;
            
            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)node.apparel;

            if (apparelColourTwo.LeftShoulderIcon == Genes40kDefOf.BEWH_ShoulderNone)
            {
                return false;
            }
            
            if (parms.Portrait)
            {
                if (parms.facing == Rot4.East)
                {
                    return false;
                }
            }
            else
            {
                if (pawn.Rotation == Rot4.East)
                {
                    return false;
                }
                
                if (parms.posture == PawnPosture.Standing)
                {
                    return true;
                }
            
                var mindState = pawn.mindState;
                if (mindState != null && mindState.duty?.def?.drawBodyOverride.HasValue == true)
                {
                    return pawn.mindState.duty.def.drawBodyOverride.Value;
                }
                if (parms.bed != null && parms.pawn.RaceProps.Humanlike)
                {
                    return parms.bed.def.building.bed_showSleeperBody;
                }
            }
            
            return true;
        }

        protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
        {
            string leftShoulderIcon;
            
            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)node.apparel;

            if (apparelColourTwo.LeftShoulderIcon != null)
            {
                leftShoulderIcon = apparelColourTwo.LeftShoulderIcon.drawnTextureIconPath;
            }
            else if (parms.pawn.Faction != null && parms.pawn.Faction.IsPlayer && ModSettings.currentlySelectedPreset != null)
            {
                leftShoulderIcon = ModSettings.currentlySelectedPreset.relatedChapterIcon.drawnTextureIconPath;
            }
            else
            {
                apparelColourTwo.LeftShoulderIcon = Genes40kDefOf.BEWH_ShoulderNone;
                leftShoulderIcon = apparelColourTwo.LeftShoulderIcon.drawnTextureIconPath;
            }
            
            return GraphicDatabase.Get<Graphic_Multi>(leftShoulderIcon, node.Props.shaderTypeDef.Shader, node.Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo, node.apparel.def.graphicData);
        }
    }
}