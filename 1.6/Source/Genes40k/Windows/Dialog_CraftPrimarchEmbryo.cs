using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Dialog_CraftPrimarchEmbryo : Window
{
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
            
        var geneseedNameRect = new Rect(geneseedIconRect);
        geneseedNameRect.height /= 3;
        geneseedNameRect.y = geneseedIconRect.yMin - geneseedNameRect.height;
            
            
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
            
        var embryoNameRect = new Rect(embryoIconRect);
        embryoNameRect.height /= 3;
        embryoNameRect.y = embryoIconRect.yMin - embryoNameRect.height;
            

        var pawnRect = new Rect(rect);
        pawnRect.height /= 2;
        pawnRect.yMin = embryoRect.yMax;
        pawnRect.yMax = rect.yMax;
            
        var pawnIconRect = new Rect(pawnRect);
        pawnIconRect.height /= 2;
        pawnIconRect.width = pawnIconRect.height;
        pawnIconRect.x += pawnRect.width/2 - pawnIconRect.width/2;
        pawnIconRect.y += pawnRect.height/2 - pawnIconRect.height/2;
            
        var pawnNameRect = new Rect(pawnIconRect);
        pawnNameRect.height /= 3;
        pawnNameRect.y = pawnIconRect.yMin - pawnNameRect.height;
            
            
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
            if (list.NullOrEmpty())
            {
                var menuOption = new FloatMenuOption("BEWH.MankindsFinest.GeneManupulationTable.NoneToSelect".Translate(), null, Widgets.PlaceholderIconTex, Color.white)
                {
                    Disabled = true
                };
                list.Add(menuOption);
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
        var toolTipGeneseed = "BEWH.MankindsFinest.GeneManupulationTable.NothingSelected".Translate();
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
            
        Widgets.DrawMenuSection(geneseedNameRect);
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(geneseedNameRect, "BEWH.MankindsFinest.GeneManupulationTable.PrimarchGeneseedVialSelection".Translate());
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
            
            
        GUI.DrawTexture(embryoIconRect, Command.BGTexShrunk);
        var humanEmbryos = map.listerThings.ThingsOfDef(ThingDefOf.HumanEmbryo);
        if (Widgets.ButtonImage(embryoIconRect, ThingDefOf.HumanEmbryo.uiIcon))
        {
            var list = new List<FloatMenuOption>();
            foreach (var thing in humanEmbryos)
            {
                if (thing is not HumanEmbryo humanEmbryo)
                {
                    continue;
                }
                if (humanEmbryo == chosenEmbryo)
                {
                    continue;
                }
                    
                var floatMenuOptionName = ThingDefOf.HumanEmbryo.label.CapitalizeFirst();
                floatMenuOptionName += ": " + (humanEmbryo?.Mother?.NameShortColored ?? "BEWH.MankindsFinest.CommonKeywords.Unknown".Translate()).CapitalizeFirst();
                    
                var menuOption = new FloatMenuOption(floatMenuOptionName, delegate
                {
                    chosenEmbryo = humanEmbryo;
                }, Widgets.PlaceholderIconTex, Color.white);
                list.Add(menuOption);
            }
            if (list.NullOrEmpty())
            {
                var menuOption = new FloatMenuOption("BEWH.MankindsFinest.GeneManupulationTable.NoneToSelect".Translate(), null, Widgets.PlaceholderIconTex, Color.white)
                {
                    Disabled = true
                };
                list.Add(menuOption);
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
        var toolTipEmbryo = "BEWH.MankindsFinest.GeneManupulationTable.NothingSelected".Translate();
        if (humanEmbryos.NullOrEmpty())
        {
            Widgets.DrawRectFast(embryoIconRect, new Color(0f, 0f, 0f, 0.8f));   
        }
        else
        {
            chosenEmbryo ??= (HumanEmbryo)humanEmbryos.First();
            
            toolTipEmbryo = ThingDefOf.HumanEmbryo.label.CapitalizeFirst();
            toolTipEmbryo += ": " + (chosenEmbryo?.Mother?.NameShortColored ?? "BEWH.MankindsFinest.CommonKeywords.Unknown".Translate()).CapitalizeFirst();
            if (Widgets.ButtonText(embryoInspectRect, "InspectGenes".Translate()))
            {
                Find.WindowStack.Add(new Dialog_ViewGenesEmbryo(chosenEmbryo));
            }
        }
        TooltipHandler.TipRegion(embryoIconRect, toolTipEmbryo);
            
        Widgets.DrawMenuSection(embryoNameRect);
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(embryoNameRect, "BEWH.MankindsFinest.GeneManupulationTable.HumanEmbryoSelection".Translate());
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
            
            
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
            foreach (var pawn in playerPawns)
            {
                if (pawn == null)
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
                    menuOption.Label += "\n" + "BEWH.MankindsFinest.GeneManupulationTable.NotSkilledEnoughPrimarchEmbryo".Translate(pawn, text);
                }
                    
                list.Add(menuOption);
            }
            if (!list.NullOrEmpty())
            {
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }
        var toolTipPawn = "BEWH.MankindsFinest.GeneManupulationTable.NothingSelected".Translate();
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
            
        Widgets.DrawMenuSection(pawnNameRect);
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(pawnNameRect, "BEWH.MankindsFinest.GeneManupulationTable.PawnSelection".Translate());
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
            
            
        if (Widgets.ButtonText(new Rect(inRect.xMax - CloseButSize.x, inRect.yMax, CloseButSize.x, CloseButSize.y), "Close".Translate()))
        {
            Close();
        }

        if (chosenPawn == null || chosenEmbryo == null || chosenGeneseedVial == null)
        {
            return;
        }
            
        if (Widgets.ButtonText(new Rect(inRect.xMin, inRect.yMax, CloseButSize.x, CloseButSize.y), "Accept".Translate()))
        {
            var thingCount = new List<ThingCount>
            {
                new (chosenGeneseedVial, 1),
                new (chosenEmbryo, 1)
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