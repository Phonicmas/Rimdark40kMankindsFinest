using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ITab_ContentsGeneStorage : ITab_ContentsBase
    {
        public override IList<Thing> container => GeneStorage.GetDirectlyHeldThings();

        public override bool IsVisible
        {
            get
            {
                if (base.SelThing != null)
                {
                    return base.IsVisible;
                }
                return false;
            }
        }

        public Building_GeneStorage GeneStorage => base.SelThing as Building_GeneStorage;

        public override bool VisibleInBlueprintMode => false;

        public ITab_ContentsGeneStorage()
        {
            labelKey = "TabCasketContents";
            containedItemsKey = "TabCasketContents";
        }

    }
}