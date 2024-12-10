using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class Recipe_MakePrimarchEmbryo : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            var embryo = (PrimarchEmbryo)ThingMaker.MakeThing(Genes40kDefOf.BEWH_PrimarchEmbryo);

            var hEmbryo = (HumanEmbryo)ingredients.First(x => x is HumanEmbryo);
            var geneseedVial = (GeneseedVial)ingredients.First(x => x is GeneseedVial);

            if (geneseedVial.extraGeneFromMaterial != null)
            {
                geneseedVial.GeneSet.AddGene(geneseedVial.extraGeneFromMaterial);
            }
            
            embryo.Initialize(hEmbryo.Mother, hEmbryo.Father, geneseedVial.GeneSet, hEmbryo.GeneSet, geneseedVial.iconDef, geneseedVial.xenotype);

            GenPlace.TryPlaceThing(embryo, billDoer.Position, billDoer.Map, ThingPlaceMode.Direct);
        }
    }
}