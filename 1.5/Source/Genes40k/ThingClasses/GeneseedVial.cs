using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;


namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class GeneseedVial : GeneSetHolderBase
    {
        private Pawn targetPawn;

        public string xenotypeName;

        public XenotypeDef xenotype;

        public XenotypeIconDef iconDef;

        public GeneDef extraGeneFromMaterial = null;

        private static readonly CachedTexture ImplantTex = new CachedTexture("UI/Gizmos/ImplantGenes");

        private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

        public override string LabelNoCount
        {
            get
            {
                if (xenotypeName.NullOrEmpty())
                {
                    return base.LabelNoCount;
                }
                return "NamedXenogerm".Translate(xenotypeName.Named("NAME"));
            }
        }

        private int RequiredMedicineForImplanting
        {
            get
            {
                DefModExtension_GeneseedVial defMod = def.GetModExtension<DefModExtension_GeneseedVial>();
                int num = 0;
                for (int i = 0; i < defMod.recipe.ingredients.Count; i++)
                {
                    IngredientCount ingredientCount = defMod.recipe.ingredients[i];
                    if (ingredientCount.filter.Allows(ThingDefOf.MedicineUltratech) || ingredientCount.filter.Allows(ThingDefOf.MedicineIndustrial))
                    {
                        num += (int)ingredientCount.GetBaseCount();
                    }
                }
                return num;
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            geneSet = new GeneSet();
            Initialize();
        }

        private bool IsAlreadySuperHuman(Pawn pawn)
        {
            return Genes40kUtils.IsSuperHuman(pawn);
        }

        public void AddExtraGene(GeneDef gene)
        {
            geneSet.AddGene(gene);
        }

        public void AddExtraGenes(List<GeneDef> genes)
        {
            if (!genes.NullOrEmpty())
            {
                foreach (GeneDef gene in genes)
                {
                    geneSet.AddGene(gene);
                }
            }
        }

        public void Initialize()
        {
            DefModExtension_GeneseedVial defMod = def.GetModExtension<DefModExtension_GeneseedVial>();

            if (!defMod.xenotype.genes.NullOrEmpty())
            {
                foreach (GeneDef gene in defMod.xenotype.genes)
                {
                    geneSet.AddGene(gene);
                }
            }

            xenotype = defMod.xenotype;
            xenotypeName = defMod.xenotype.label;
            iconDef = defMod.xenotypeIcon;
        }

        public void SetTargetPawn(Pawn newTarget)
        {
            if (def.GetModExtension<DefModExtension_GeneseedVial>().primarch)
            {
                return;
            }
            TaggedString text = "BEWH.ImplantGeneseedDesc".Translate(newTarget, xenotypeName);
            DefModExtension_GeneseedVial defMod = def.GetModExtension<DefModExtension_GeneseedVial>();
            List<string> failChanceCausedBy = new List<string>();
            var failChance = 0;
            if (!(defMod.minAgeImplant <= newTarget.ageTracker.AgeBiologicalYears && defMod.maxAgeImplant >= newTarget.ageTracker.AgeBiologicalYears))
            {
                var minAgeCheck = newTarget.ageTracker.AgeBiologicalYears - defMod.minAgeImplant;
                var maxAgeCheck = newTarget.ageTracker.AgeBiologicalYears - defMod.maxAgeImplant;
                if (minAgeCheck < maxAgeCheck)
                {
                    minAgeCheck = maxAgeCheck;
                }
                var failChanceAgeOffset = minAgeCheck * defMod.failureChancePerAgePast;
                failChance += failChanceAgeOffset;
                
                failChanceCausedBy.Add("\t- " + "BEWH.FailureChanceCause".Translate(failChanceAgeOffset, "BEWH.OutsideOptimalAgeRange".Translate(newTarget, defMod.minAgeImplant, defMod.maxAgeImplant)));
            }

            var failChanceGeneOffset = 0;

            var failCapChance = defMod.failChanceCap;
            var failChanceCapGeneOffset = 0;

            if (extraGeneFromMaterial != null && extraGeneFromMaterial.HasModExtension<DefModExtension_GeneseedPurity>())
            {
                var geneDefMod = extraGeneFromMaterial.GetModExtension<DefModExtension_GeneseedPurity>();
                failChanceCapGeneOffset += geneDefMod.additionalChanceCapOffset;
                failChanceGeneOffset += geneDefMod.additionalChanceOffset;
                failChanceCausedBy.Add("\t- " + "BEWH.FailureChanceCause".Translate(geneDefMod.additionalChanceOffset, extraGeneFromMaterial.label));
            }

            failCapChance += failChanceCapGeneOffset;
            failChance += failChanceGeneOffset;

            if (failCapChance > 100)
            {
                failCapChance = 100;
            }

            var wasCapped = false;

            if (failChance > failCapChance)
            {
                failChance = failCapChance;
                wasCapped = true;
            }

            if (failChance > 0)
            {
                text += "\n\n" + "BEWH.CurrentFailureChance".Translate(failChance);

                text += "\n\n" + "BEWH.FailureChanceCausedBy".Translate();

                foreach (var failChanceCause in failChanceCausedBy)
                {
                    text += "\n" + failChanceCause;
                }

                if (wasCapped)
                {
                    text += "\n\n" + "BEWH.FailureChanceCapped".Translate(failCapChance);
                }
            }

            text += "\n\n" + "WouldYouLikeToContinue".Translate();
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, delegate
            {
                Bill bill = targetPawn?.BillStack?.Bills?.FirstOrDefault((Bill x) => x.recipe == def.GetModExtension<DefModExtension_GeneseedVial>().recipe);
                if (bill != null)
                {
                    targetPawn.BillStack.Delete(bill);
                }
                HealthCardUtility.CreateSurgeryBill(newTarget, def.GetModExtension<DefModExtension_GeneseedVial>().recipe, null);
                targetPawn = newTarget;
                SendImplantationLetter(newTarget);
            }, destructive: true));
        }

        private void SendImplantationLetter(Pawn targetPawn)
        {
            string arg = string.Empty;
            if (!targetPawn.InBed() && !targetPawn.Map.listerBuildings.allBuildingsColonist.Any((Building x) => x is Building_Bed && RestUtility.CanUseBedEver(targetPawn, x.def) && ((Building_Bed)x).Medical))
            {
                arg = "BEWH.GeneseedOrderedImplantedBedNeeded".Translate(targetPawn.Named("PAWN"));
            }
            int requiredMedicineForImplanting = RequiredMedicineForImplanting;
            string arg2 = string.Empty;
            if (targetPawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine).Sum((Thing x) => x.stackCount) < requiredMedicineForImplanting)
            {
                arg2 = "BEWH.GeneseedOrderedImplantedMedicineNeeded".Translate(requiredMedicineForImplanting.Named("MEDICINENEEDED"));
            }
            Find.LetterStack.ReceiveLetter("BEWH.LetterLabelGeneseedOrderedImplanted".Translate(), "BEWH.LetterGeneseedOrderedImplanted".Translate(targetPawn.Named("PAWN"), requiredMedicineForImplanting.Named("MEDICINENEEDED"), arg.Named("BEDINFO"), arg2.Named("MEDICINEINFO")), LetterDefOf.NeutralEvent);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
            {
                yield return floatMenuOption;
            }
            if (def.GetModExtension<DefModExtension_GeneseedVial>().primarch)
            {
                yield break;
            }
            if (selPawn.genes == null)
            {
                yield break;
            }
            if (IsAlreadySuperHuman(selPawn))
            {
                yield return new FloatMenuOption("BEWH.AlreadySuperHuman".Translate(selPawn.Named("PAWN")), null);
                yield break;
            }
            int num = GeneUtility.MetabolismAfterImplanting(selPawn, geneSet);
            if (num < GeneTuning.BiostatRange.TrueMin)
            {
                yield return new FloatMenuOption("CannotGenericWorkCustom".Translate((string)("OrderImplantationIntoPawn".Translate(selPawn.Named("PAWN")).Resolve().UncapitalizeFirst() + ": " + "ResultingMetTooLow".Translate() + " (") + num + ")"), null);
                yield break;
            }
            yield return new FloatMenuOption("OrderImplantationIntoPawn".Translate(selPawn.Named("PAWN")) + " (" + xenotypeName + ")", delegate
            {
                SetTargetPawn(selPawn);
            });
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (def.GetModExtension<DefModExtension_GeneseedVial>().primarch)
            {
                yield break;
            }
            if (geneSet == null)
            {
                yield break;
            }
            if (targetPawn == null)
            {
                yield return new Command_Action
                {
                    defaultLabel = "BEWH.OrderImplantGeneseed".Translate() + "...",
                    defaultDesc = "BEWH.OrderImplantGeneseedDesc".Translate(RequiredMedicineForImplanting.Named("MEDICINEAMOUNT")),
                    icon = ImplantTex.Texture,
                    action = delegate
                    {
                        List<FloatMenuOption> list = new List<FloatMenuOption>();
                        foreach (Pawn item in base.Map.mapPawns.AllPawnsSpawned)
                        {
                            Pawn pawn = item;
                            if (!pawn.IsQuestLodger() && pawn.genes != null && (pawn.IsColonistPlayerControlled || IsAlreadySuperHuman(pawn) || pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony || (pawn.IsColonyMutant && pawn.IsGhoul)))
                            {
                                int num = GeneUtility.MetabolismAfterImplanting(pawn, geneSet);
                                if (num < GeneTuning.BiostatRange.TrueMin)
                                {
                                    list.Add(new FloatMenuOption((string)(pawn.LabelShortCap + ": " + "ResultingMetTooLow".Translate() + " (") + num + ")", null, pawn, Color.white));
                                }
                                else
                                {
                                    list.Add(new FloatMenuOption(pawn.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap, delegate
                                    {
                                        SetTargetPawn(pawn);
                                    }, pawn, Color.white));
                                }
                            }
                        }
                        if (!list.Any())
                        {
                            list.Add(new FloatMenuOption("NoImplantablePawns".Translate(), null));
                        }
                        Find.WindowStack.Add(new FloatMenu(list));
                    }
                };
                yield break;
            }
            yield return new Command_Action
            {
                defaultLabel = "CancelImplanting".Translate(),
                defaultDesc = "CancelImplantingDesc".Translate(targetPawn.Named("PAWN")),
                icon = CancelIcon,
                action = delegate
                {
                    Log.Message("Cancel implant message");
                    Bill bill = targetPawn?.BillStack?.Bills?.FirstOrDefault((Bill x) => x.recipe == def.GetModExtension<DefModExtension_GeneseedVial>().recipe);
                    if (bill != null)
                    {
                        targetPawn.BillStack.Delete(bill);
                    }
                }
            };
        }

        public void Notify_BillRemoved()
        {
            targetPawn = null;
        }

        public bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!target.IsValid || target.Pawn == null)
            {
                return false;
            }
            if (IsAlreadySuperHuman(target.Pawn))
            {
                if (showMessages)
                {
                    Messages.Message("BEWH.AlreadySuperHuman".Translate(target.Pawn.Named("PAWN")), null);
                }
                return false;
            }
            if (target.Pawn.IsQuestLodger())
            {
                if (showMessages)
                {
                    Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (!target.Pawn.IsColonist && !target.Pawn.IsPrisonerOfColony && !target.Pawn.IsSlaveOfColony)
            {
                if (showMessages)
                {
                    Messages.Message("MessageCanOnlyTargetColonistsPrisonersAndSlaves".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return true;
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            if (targetPawn != null && targetPawn.Map == base.Map)
            {
                GenDraw.DrawLineBetween(this.TrueCenter(), targetPawn.TrueCenter());
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref targetPawn, "targetPawn");
            Scribe_Values.Look(ref xenotypeName, "xenotypeName");
            Scribe_Defs.Look(ref iconDef, "iconDef");
            Scribe_Defs.Look(ref extraGeneFromMaterial, "extraGeneFromMaterial");
            if (iconDef == null && Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                iconDef = XenotypeIconDefOf.Basic;
            }
        }
    }
}