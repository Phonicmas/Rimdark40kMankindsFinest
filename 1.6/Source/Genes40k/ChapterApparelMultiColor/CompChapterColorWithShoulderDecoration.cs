using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class CompChapterColorWithShoulderDecoration : CompChapterColor
{
    public new CompProperties_ChapterColorWithShoulderDecoration Props => (CompProperties_ChapterColorWithShoulderDecoration)props;

    private CompDecorative decorativeComp;
    public CompDecorative DecorativeComp => decorativeComp ??= parent.GetComp<CompDecorative>();

    private bool flipShoulderIcons;
    private bool originalFlipShoulderIcons;

    //The rank slot tracks the wearer's highest Astartes rank.
    private bool followRank = true;
    private bool originalFollowRank = true;

    private bool pendingFlipShoulderIcons;
    private bool pendingFollowRank = true;
    private bool hasPendingShoulderChange;

    private bool syncing;
    private bool suppressSync;

    //Pre-1.1.34 saves stored the two slots here; read once and moved into CompDecorative.
    private ShoulderIconSettings legacyLeft;
    private ShoulderIconSettings legacyRight;

    public bool FlipShoulderIcons => flipShoulderIcons;
    public bool FollowRank => followRank;

    //Slot = the list an icon came from (chapter or rank); side = the shoulder it draws on. They
    //differ while the icons are swapped. Flipped = true is the left shoulder.
    private bool FlippedFor(bool chapterSlot) => chapterSlot ? !flipShoulderIcons : flipShoulderIcons;

    public ShoulderIconDef ChapterIcon => Fitted(chapterSlot: true);
    public ShoulderIconDef RankIcon => Fitted(chapterSlot: false);

    private ShoulderIconDef Fitted(bool chapterSlot)
    {
        var comp = DecorativeComp;
        if (comp == null)
        {
            return null;
        }

        var flipped = FlippedFor(chapterSlot);
        foreach (var pair in comp.Decorations)
        {
            if (pair.Key is ShoulderIconDef def && pair.Value != null && pair.Value.Flipped == flipped)
            {
                return def;
            }
        }

        return null;
    }

    public void SetChapterIcon(ShoulderIconDef def)
    {
        SetSlot(chapterSlot: true, def);
    }

    public void SetRankIcon(ShoulderIconDef def)
    {
        if (def == Genes40kDefOf.BEWH_FollowRankUp)
        {
            followRank = true;
            SyncRankIcon(Wearer, force: true);
            return;
        }

        followRank = false;
        SetSlot(chapterSlot: false, def);
    }

    //null or a setsNull def clears the slot. One icon per slot, so whatever was there goes first.
    private void SetSlot(bool chapterSlot, ShoulderIconDef def)
    {
        var comp = DecorativeComp;
        if (comp == null)
        {
            return;
        }

        var wanted = def is { setsNull: false } ? def : null;
        var current = Fitted(chapterSlot);
        if (current == wanted)
        {
            return;
        }

        if (current != null)
        {
            comp.RemoveDecoration(current);
        }

        if (wanted != null)
        {
            comp.AddDecoration(wanted, new DecorationSettings { Flipped = FlippedFor(chapterSlot) }, setDefaultColors: true);
        }

        comp.Notify_GraphicChanged();
    }

    public Color SlotColour(bool chapterSlot)
    {
        var def = Fitted(chapterSlot);
        if (def != null && DecorativeComp.Decorations.TryGetValue(def, out var settings))
        {
            return settings.Color;
        }

        return Color.white;
    }

    public void SetSlotColour(bool chapterSlot, Color colour)
    {
        var def = Fitted(chapterSlot);
        if (def != null)
        {
            DecorativeComp.SetDecorationColourOne(def, colour);
        }
    }

    public void SwapShoulderIcons()
    {
        var chapter = ChapterIcon;
        var rank = RankIcon;
        flipShoulderIcons = !flipShoulderIcons;

        var comp = DecorativeComp;
        if (comp == null)
        {
            return;
        }

        if (chapter != null)
        {
            comp.SetDecorationFlipped(chapter, FlippedFor(chapterSlot: true));
        }

        if (rank != null)
        {
            comp.SetDecorationFlipped(rank, FlippedFor(chapterSlot: false));
        }
    }

    /// <summary>
    /// Keeps the rank slot on the wearer's highest Astartes rank while it follows rank. A colour the
    /// player picked is carried over to the new icon; otherwise it takes its own default. Does nothing
    /// without a wearer, while a station job is pending, while loading, or while the customization
    /// dialog is open unless forced by the dialog itself.
    /// </summary>
    public void SyncRankIcon(Pawn pawn, bool force = false)
    {
        if (syncing || !followRank || pawn == null || Scribe.mode != LoadSaveMode.Inactive)
        {
            return;
        }

        if (suppressSync && !force)
        {
            return;
        }

        var comp = DecorativeComp;
        if (comp == null || comp.HasPendingChange)
        {
            return;
        }

        var rankInfo = pawn.GetComp<CompRankInfo>();
        var highest = rankInfo?.HighestRankDef(true, Genes40kDefOf.BEWH_AstartesRankCategory)
                      ?? rankInfo?.HighestRankDef(false, Genes40kDefOf.BEWH_AstartesRankCategory);
        var wanted = (highest as ChapterRankDef)?.unlocksRankIcon;
        if (wanted is { setsNull: true })
        {
            wanted = null;
        }

        var current = RankIcon;
        if (current == wanted)
        {
            return;
        }

        syncing = true;
        try
        {
            Color? keep = null;
            if (current != null && comp.Decorations.TryGetValue(current, out var old) && old.Color != (current.defaultColour ?? Color.white))
            {
                keep = old.Color;
            }

            SetSlot(chapterSlot: false, wanted);

            if (keep.HasValue && wanted is { colourable: true })
            {
                comp.SetDecorationColourOne(wanted, keep.Value);
            }
        }
        finally
        {
            syncing = false;
        }
    }

    //The shoulder tab owns the slots while its dialog is open; rank syncs wait until it closes.
    public void BeginEditing()
    {
        suppressSync = true;
    }

    public void EndEditing()
    {
        suppressSync = false;
    }

    public override void InitialColors()
    {
        base.InitialColors();

        flipShoulderIcons = false;
        followRank = true;

        var settings = ModSettings;
        SetChapterIcon(settings?.CurrentlySelectedPreset?.relatedChapterIcon);
        if (settings?.chapterShoulderIconColor != null && ChapterIcon is { colourable: true })
        {
            SetSlotColour(chapterSlot: true, settings.chapterShoulderIconColor.Value);
        }

        DecorativeComp?.SetOriginalDecorations();
    }

    public override void SetOriginals()
    {
        originalFlipShoulderIcons = flipShoulderIcons;
        originalFollowRank = followRank;
        base.SetOriginals();
    }

    public override void Reset()
    {
        flipShoulderIcons = originalFlipShoulderIcons;
        followRank = originalFollowRank;
        base.Reset();
    }

    public override bool HasPendingChange => base.HasPendingChange || hasPendingShoulderChange;

    public override void CapturePending()
    {
        if (flipShoulderIcons != originalFlipShoulderIcons || followRank != originalFollowRank)
        {
            pendingFlipShoulderIcons = flipShoulderIcons;
            pendingFollowRank = followRank;
            hasPendingShoulderChange = true;

            flipShoulderIcons = originalFlipShoulderIcons;
            followRank = originalFollowRank;
        }

        base.CapturePending();
    }

    public override void CommitPending()
    {
        var shoulderChange = hasPendingShoulderChange;
        if (shoulderChange)
        {
            flipShoulderIcons = pendingFlipShoulderIcons;
            followRank = pendingFollowRank;
            hasPendingShoulderChange = false;
        }

        base.CommitPending();

        if (shoulderChange)
        {
            SetOriginals();
        }

        SyncRankIcon(Wearer);
    }

    public override void DiscardPending()
    {
        hasPendingShoulderChange = false;
        base.DiscardPending();
    }

    public override void Notify_Equipped(Pawn pawn)
    {
        base.Notify_Equipped(pawn);
        SyncRankIcon(pawn);
    }

    public override void Notify_ColorChanged()
    {
        base.Notify_ColorChanged();
        SyncRankIcon(Wearer);
    }

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref flipShoulderIcons, "flipShoulderIcons", false);
        Scribe_Values.Look(ref originalFlipShoulderIcons, "originalFlipShoulderIcons", false);
        Scribe_Values.Look(ref followRank, "followRank", true);
        Scribe_Values.Look(ref originalFollowRank, "originalFollowRank", true);
        Scribe_Values.Look(ref hasPendingShoulderChange, "hasPendingShoulderChange", false);
        Scribe_Values.Look(ref pendingFlipShoulderIcons, "pendingFlipShoulderIcons", false);
        Scribe_Values.Look(ref pendingFollowRank, "pendingFollowRank", true);

        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            Scribe_Deep.Look(ref legacyLeft, "leftShoulder");
            Scribe_Deep.Look(ref legacyRight, "rightShoulder");
        }

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            MigrateLegacyShoulderData();
        }

        base.PostExposeData();
    }

    private void MigrateLegacyShoulderData()
    {
        if (legacyLeft == null && legacyRight == null)
        {
            return;
        }

        var comp = DecorativeComp;
        if (comp != null)
        {
            if (ChapterIcon == null && legacyLeft?.ShoulderIcon is { setsNull: false } left)
            {
                comp.AddDecoration(left, new DecorationSettings { Flipped = FlippedFor(chapterSlot: true), Color = legacyLeft.Color, maskDef = left.defaultMask });
            }

            Color? legacyRankColour = null;
            if (RankIcon == null && legacyRight != null)
            {
                if (legacyRight.ShoulderIcon is { setsNull: false } right)
                {
                    followRank = false;
                    comp.AddDecoration(right, new DecorationSettings { Flipped = FlippedFor(chapterSlot: false), Color = legacyRight.Color, maskDef = right.defaultMask });
                }
                else
                {
                    //null meant "use pawn rank"; a stored none icon meant explicitly none.
                    followRank = legacyRight.ShoulderIcon == null;
                    if (followRank)
                    {
                        legacyRankColour = legacyRight.Color;
                    }
                }
            }

            originalFlipShoulderIcons = flipShoulderIcons;
            originalFollowRank = followRank;
            comp.SetOriginalDecorations();

            LongEventHandler.ExecuteWhenFinished(delegate
            {
                SyncRankIcon(Wearer);
                if (legacyRankColour.HasValue && RankIcon is { colourable: true } rankIcon && legacyRankColour.Value != Color.white)
                {
                    comp.SetDecorationColourOne(rankIcon, legacyRankColour.Value);
                }
                comp.SetOriginalDecorations();
            });
        }

        legacyLeft = null;
        legacyRight = null;
    }
}
