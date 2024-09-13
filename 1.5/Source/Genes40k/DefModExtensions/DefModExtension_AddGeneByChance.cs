using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class DefModExtension_AddGeneByChance : DefModExtension
    {
        public Dictionary<GeneDef, float> possibleGenesToGive;
    }
}