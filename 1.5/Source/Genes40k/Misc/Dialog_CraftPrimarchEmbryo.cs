using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Dialog_CraftPrimarchEmbryo : Window
    {
        private Vector2 scrollPosition;

        private const float HeaderHeight = 30f;

        private Map map = null;
        private Building_GeneTable geneTable = null;
        
        private GeneseedVial chosenGeneseedVial = null;
        private HumanEmbryo chosenEmbryo = null;
        private Pawn chosenPawn = null;
        private List<Pawn> playerPawnWithSkills = new List<Pawn>();

        private readonly RecipeDef recipe = Genes40kDefOf.BEWH_MakePrimarchEmbryo;

        public Dialog_CraftPrimarchEmbryo(Map map, Building_GeneTable geneTable)
        {
            closeOnClickedOutside = true;
            this.map = map;
            this.geneTable = geneTable;
        }

        public override void DoWindowContents(Rect inRect)
        {
            inRect.yMax -= CloseButSize.y;
            var rect = inRect;

            Text.Font = GameFont.Small;

            var geneseedRect = new Rect(rect);
            geneseedRect.width /= 2;
            geneseedRect.height /= 2;
            
            var geneseedIconRect = new Rect(geneseedRect);
            geneseedIconRect.height /= 2;
            geneseedIconRect.width = geneseedIconRect.height;
            geneseedIconRect.x += geneseedRect.width/2 - geneseedIconRect.width/2;
            geneseedIconRect.y += geneseedRect.height/2 - geneseedIconRect.height/2;
            
            var geneseedInspectRect = new Rect(geneseedIconRect);
            geneseedInspectRect.height /= 4;
            geneseedInspectRect.y = geneseedIconRect.yMax + 10f;
            
            
            var embryoRect = new Rect(geneseedRect)
            {
                xMin = geneseedRect.xMax,
                xMax = rect.xMax
            };
            
            var embryoIconRect = new Rect(embryoRect);
            embryoIconRect.height /= 2;
            embryoIconRect.width = geneseedIconRect.height;
            embryoIconRect.x += embryoRect.width/2 - embryoIconRect.width/2;
            embryoIconRect.y += embryoRect.height/2 - embryoIconRect.height/2;
            
            var embryoInspectRect = new Rect(embryoIconRect);
            embryoInspectRect.height /= 4;
            embryoInspectRect.y = embryoIconRect.yMax + 10f;
            

            var pawnRect = new Rect(rect);
            pawnRect.height /= 2;
            pawnRect.yMin = embryoRect.yMax;
            pawnRect.yMax = rect.yMax;
            
            var pawnIconRect = new Rect(pawnRect);
            pawnIconRect.height /= 2;
            pawnIconRect.width = pawnIconRect.height;
            pawnIconRect.x += pawnRect.width/2 - pawnIconRect.width/2;
            pawnIconRect.y += pawnRect.height/2 - pawnIconRect.height/2;
            
            
            GUI.DrawTexture(geneseedIconRect, Command.BGTexShrunk);
            var primarchGeneseedVials = map.listerThings.ThingsOfDef(Genes40kDefOf.BEWH_GeneseedVialPrimarch);
            if (Widgets.ButtonImage(geneseedIconRect, Genes40kDefOf.BEWH_GeneseedVialPrimarch.uiIcon))
            {
                var list = new List<FloatMenuOption>();
                foreach (var thing in primarchGeneseedVials)
                {
                    if (!(thing is GeneseedVial geneseed))
                    {
                        continue;
                    }
                    if (geneseed == chosenGeneseedVial)
                    {
                        continue;
                    }
                    
                    var floatMenuOptionName = geneseed.def.label.CapitalizeFirst();
                    if (geneseed.extraGeneFromMaterial != null)
                    {
                        floatMenuOptionName += ": " + geneseed.extraGeneFromMaterial.label.CapitalizeFirst();
                    }
                    
                    var menuOption = new FloatMenuOption(floatMenuOptionName, delegate
                    {
                        chosenGeneseedVial = geneseed;
                    }, Widgets.PlaceholderIconTex, Color.white);
                    list.Add(menuOption);
                }
                if (!list.NullOrEmpty())
                {
                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }
            var toolTipGeneseed = "BEWH.NothingSelected".Translate();
            if (primarchGeneseedVials.NullOrEmpty())
            {
                Widgets.DrawRectFast(geneseedIconRect, new Color(0f, 0f, 0f, 0.8f));   
            }
            else
            {
                if (chosenGeneseedVial == null)
                {
                    chosenGeneseedVial = (GeneseedVial)primarchGeneseedVials.First();
                }
                toolTipGeneseed = chosenGeneseedVial.Label.CapitalizeFirst();
                if (chosenGeneseedVial.extraGeneFromMaterial != null)
                {
                    toolTipGeneseed += ": " + chosenGeneseedVial.extraGeneFromMaterial.label.CapitalizeFirst();
                }
                if (Widgets.ButtonText(geneseedInspectRect, "InspectGenes".Translate()))
                {
                    Genes40kUtils.InspectGeneseedVialGenes(chosenGeneseedVial);
                }
            }
            TooltipHandler.TipRegion(geneseedIconRect, toolTipGeneseed);
            
            
            GUI.DrawTexture(embryoIconRect, Command.BGTexShrunk);
            var humanEmbryos = map.listerThings.ThingsOfDef(ThingDefOf.HumanEmbryo);
            if (Widgets.ButtonImage(embryoIconRect, ThingDefOf.HumanEmbryo.uiIcon))
            {
                var list = new List<FloatMenuOption>();
                foreach (var thing in humanEmbryos)
                {
                    if (!(thing is HumanEmbryo humanEmbryo))
                    {
                        continue;
                    }
                    if (humanEmbryo == chosenEmbryo)
                    {
                        continue;
                    }
                    
                    var floatMenuOptionName = humanEmbryo.def.label.CapitalizeFirst();
                    floatMenuOptionName += ": " + humanEmbryo.Mother.NameShortColored.CapitalizeFirst();
                    
                    var menuOption = new FloatMenuOption(floatMenuOptionName, delegate
                    {
                        chosenEmbryo = humanEmbryo;
                    }, Widgets.PlaceholderIconTex, Color.white);
                    list.Add(menuOption);
                }
                if (!list.NullOrEmpty())
                {
                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }
            var toolTipEmbryo = "BEWH.NothingSelected".Translate();
            if (humanEmbryos.NullOrEmpty())
            {
                Widgets.DrawRectFast(embryoIconRect, new Color(0f, 0f, 0f, 0.8f));   
            }
            else
            {
                if (chosenEmbryo == null)
                {
                    chosenEmbryo = (HumanEmbryo)humanEmbryos.First();
                }
                toolTipEmbryo = chosenEmbryo.def.label.CapitalizeFirst();
                toolTipEmbryo += ": " + chosenEmbryo.Mother.NameShortColored.CapitalizeFirst();
                if (Widgets.ButtonText(embryoInspectRect, "InspectGenes".Translate()))
                {
                    Find.WindowStack.Add(new Dialog_ViewGenesEmbryo(chosenEmbryo));
                }
            }
            TooltipHandler.TipRegion(embryoIconRect, toolTipEmbryo);
            
            
            GUI.DrawTexture(pawnIconRect, Command.BGTexShrunk);
            var playerPawns = map.mapPawns.FreeColonistsSpawned;
            playerPawnWithSkills = playerPawns.Where(pawn => recipe.PawnSatisfiesSkillRequirements(pawn)).ToList();
            if (chosenPawn != null)
            {
                Widgets.ThingIcon(pawnIconRect, chosenPawn);
            }
            else
            {
                GUI.DrawTexture(pawnIconRect, XenotypeIconDefOf.Basic.Icon);
            }
            if (Widgets.ButtonInvisible(pawnIconRect))
            {
                var list = new List<FloatMenuOption>();
                foreach (var thing in playerPawns)
                {
                    if (!(thing is Pawn pawn))
                    {
                        continue;
                    }
                    if (pawn == chosenPawn)
                    {
                        continue;
                    }
                    
                    var floatMenuOptionName = pawn.NameFullColored.CapitalizeFirst();

                    var menuOption = new FloatMenuOption(floatMenuOptionName, delegate
                    {
                        chosenPawn = pawn;
                    }, Widgets.PlaceholderIconTex, Color.white);

                    if (!recipe.PawnSatisfiesSkillRequirements(pawn))
                    {
                        menuOption.Disabled = true;
                        var text = recipe.skillRequirements.Aggregate("", (current, skillRequirement) => current + (skillRequirement.skill.label + ": " + skillRequirement.minLevel + " "));
                        text = text.Trim();
                        menuOption.Label += "\n" + "BEWH.NotSkilledEnoughPrimarchEmbryo".Translate(pawn, text);
                    }
                    
                    list.Add(menuOption);
                }
                if (!list.NullOrEmpty())
                {
                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }
            var toolTipPawn = "BEWH.NothingSelected".Translate();
            if (playerPawnWithSkills.NullOrEmpty())
            {
                Widgets.DrawRectFast(pawnIconRect, new Color(0f, 0f, 0f, 0.8f));   
            }
            else
            {
                if (chosenPawn == null)
                {
                    chosenPawn = playerPawnWithSkills.First();
                }
                
                toolTipPawn = chosenPawn.NameFullColored.CapitalizeFirst();
            }
            
            TooltipHandler.TipRegion(pawnIconRect, toolTipPawn);
            
            
            if (Widgets.ButtonText(new Rect(inRect.xMax - CloseButSize.x, inRect.yMax, Window.CloseButSize.x, Window.CloseButSize.y), "Close".Translate()))
            {
                Close();
            }

            if (chosenPawn == null || chosenEmbryo == null || chosenGeneseedVial == null)
            {
                return;
            }
            
            if (Widgets.ButtonText(new Rect(inRect.xMin, inRect.yMax, Window.CloseButSize.x, Window.CloseButSize.y), "Accept".Translate()))
            {
                var thingCount = new List<ThingCount>
                {
                    new ThingCount(chosenGeneseedVial, 1),
                    new ThingCount(chosenEmbryo, 1)
                };
                    
                var bill = recipe.MakeNewBill();
                bill.billStack = new BillStack(geneTable);
                    
                var job = WorkGiver_DoBill.TryStartNewDoBillJob(chosenPawn, bill, geneTable, thingCount, out _);
                chosenPawn.jobs.TryTakeOrderedJob(job);
                geneTable.billStack.AddBill(bill);
                
                Close();
            }
        }
    }
}