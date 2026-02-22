using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_SangprimusPortum : Building, IThingHolder
{
    private ThingOwner innerContainer;

    private GameComponent_UnlockedMaterials GameComp => Current.Game?.GetComponent<GameComponent_UnlockedMaterials>();
    private bool gameCompChangeDone = false;
    
    public Building_SangprimusPortum()
    {
        innerContainer = new ThingOwner<Thing>(this);
    }

    public bool CanAcceptMaterial(Thing thing)
    {
        return !GameComp.HasMaterial(thing.def);
    }

    public void AddMaterial(Thing thing)
    {
        GameComp.UnlockMaterial(thing.def);
        thing.Destroy();
    }
    
    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public ThingOwner GetDirectlyHeldThings() => innerContainer;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        Scribe_Values.Look(ref gameCompChangeDone, "gameCompChangeDone");
        
        if (Scribe.mode == LoadSaveMode.PostLoadInit && !gameCompChangeDone)
        {
            if (innerContainer != null)
            {
                foreach (var thing in innerContainer)
                {
                    GameComp.UnlockMaterial(thing.def); 
                }
            }
            innerContainer = new ThingOwner<Thing>(this);
            gameCompChangeDone = true;
        }
    }
}