using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class DefModExtension_GeneseedVial : DefModExtension
    {
        public XenotypeDef xenotype = null;
        public XenotypeIconDef xenotypeIcon = null;
        public List<GeneDef> extraAddedGenes = new List<GeneDef>();

        public bool overrideXenotypeGenesGiven = false;
        public List<GeneDef> overridenAddedGenes = new List<GeneDef>();

        public RecipeDef recipe = null;

        public HediffDef appliesHediff = null;

        public int minAgeImplant = 0;
        public int maxAgeImplant = 0;
        public int failureChancePerAgePast = 0;
        public int failChanceCap = 0;

        public bool primarch = false;
    }
}