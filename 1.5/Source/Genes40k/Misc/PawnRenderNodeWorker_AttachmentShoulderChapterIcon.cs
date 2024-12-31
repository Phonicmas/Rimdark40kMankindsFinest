using Core40k;
using RimWorld;
using UnityEngine;
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
            
            return ModSettings.currentlySelectedPreset != null;
        }

        protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
        {
            var standardPath = "Things/Armor/Imperium/PowerArmor/ChapterIcons/BEWH_ImperialFists";
            
            /*if (ModSettings.currentlySelectedPreset != null)
            {
                standardPath = ModSettings.currentlySelectedPreset?.relatedChapterIconPath;
            }*/
            
            var def = node.apparel.def;
            var apparelColourTwo = (ChapterApparelColourTwo)node.apparel;

            if (apparelColourTwo.CurrentlySelectedChapterIcon != null)
            {
                standardPath = apparelColourTwo.CurrentlySelectedChapterIcon.relatedChapterIconPath;
            }
            
            var shader = ShaderDatabase.CutoutComplex;
                    
            if (def.graphicData.shaderType != null)
            {
                shader = def.graphicData.shaderType.Shader;
            }
            
            //If not colorable, set color.white instead of drawcolors of apparel
            return  GraphicDatabase.Get<Graphic_Multi>(standardPath, shader, node.Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo, def.graphicData);
        }
    }
}