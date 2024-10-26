using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_GeneStorage : Building, IThingHolderEvents<Thing>, IHaulEnroute, IHaulDestination, IHaulSource, ILoadReferenceable, IStorageGroupMember, IStoreSettingsParent, IThingHolder, ISearchableContents
    {
        public int MaximumItems => def.building.maxItemsInCell * def.size.Area;

        public IEnumerable<Thing> GeneAmount => innerContainer.InnerListForReading;

        private ThingOwner<Thing> innerContainer;

        private StorageSettings settings;

        private StorageGroup storageGroup;

        public bool StorageTabVisible => true;

        public ThingOwner SearchableContents => innerContainer;

        StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();

        StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();

        StorageSettings IStorageGroupMember.ThingStoreSettings => settings;

        StorageGroup IStorageGroupMember.Group
        {
            get => storageGroup;
            set => storageGroup = value;
        }

        bool IStorageGroupMember.DrawStorageTab => true;

        bool IStorageGroupMember.ShowRenameButton => Faction == Faction.OfPlayer;

        bool IStorageGroupMember.DrawConnectionOverlay => Spawned;

        string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;



        public Building_GeneStorage()
        {
            innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
        }

        public override void DrawGUIOverlay()
        {
            GenMapUI.DrawThingLabel(this, GeneAmount.Count() + "/" + MaximumItems);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (storageGroup == null || map == storageGroup.Map) return;
            
            var storeSettings = storageGroup.GetStoreSettings();
            storageGroup.RemoveMember(this);
            storageGroup = null;
            settings.CopyFrom(storeSettings);
        }

        public override void PostMake()
        {
            base.PostMake();
            settings = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                settings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (storageGroup != null)
            {
                storageGroup?.RemoveMember(this);
                storageGroup = null;
            }
            innerContainer.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
            base.DeSpawn(mode);
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public void DropThingToReserve(Thing thing)
        {
            if (SearchableContents.Contains(thing))
            {
                GenPlace.TryPlaceThing(thing, this.TrueCenter().ToIntVec3().RandomAdjacentCell8Way(), Map, ThingPlaceMode.Near);
            }
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public void Notify_SettingsChanged()
        {
            if (base.Spawned)
            {
                base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
            }
        }

        public virtual void Notify_ItemAdded(Thing newItem)
        {
            newItem.HitPoints = MaxHitPoints;
        }

        public virtual void Notify_ItemRemoved(Thing newItem)
        {
            newItem.HitPoints = MaxHitPoints;
        }

        public bool Accepts(Thing t)
        {
            if (!GetStoreSettings().AllowedToAccept(t)) return false;

            if (!def.HasModExtension<DefModExtension_SangprimusPortum>()) return innerContainer.CanAcceptAnyOf(t);
            
            return SearchableContents.Where(x => x.def == t.def).EnumerableNullOrEmpty() && innerContainer.CanAcceptAnyOf(t);
        }

        public int SpaceRemainingFor(ThingDef _)
        {
            return MaximumItems - GeneAmount.Count();
        }

        public StorageSettings GetStoreSettings()
        {
            return storageGroup != null ? storageGroup.GetStoreSettings() : settings;
        }

        public StorageSettings GetParentStoreSettings()
        {
            return def.building.fixedStorageSettings;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Deep.Look(ref settings, "settings", this);
            Scribe_References.Look(ref storageGroup, "storageGroup");
        }

    }
}