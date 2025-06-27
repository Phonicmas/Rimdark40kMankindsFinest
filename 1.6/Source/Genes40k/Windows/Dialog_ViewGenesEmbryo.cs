using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Dialog_ViewGenesEmbryo : Window
{
    private HumanEmbryo target;

    private Vector2 scrollPosition;

    public override Vector2 InitialSize => new (736f, 700f);

    public Dialog_ViewGenesEmbryo(HumanEmbryo target)
    {
        this.target = target;
        closeOnClickedOutside = true;
    }

    public override void PostOpen()
    {
        if (!ModLister.CheckBiotech("genes viewing"))
        {
            Close(doCloseSound: false);
        }
        else
        {
            base.PostOpen();
        }
    }

    public override void DoWindowContents(Rect inRect)
    {
        inRect.yMax -= CloseButSize.y;
        var rect = inRect;
        rect.xMin += 34f;
        Text.Font = GameFont.Medium;
        Widgets.Label(rect, "ViewGenes".Translate() + ": " + XenotypeIconDefOf.Basic.label);
        Text.Font = GameFont.Small;
        GUI.color = XenotypeDef.IconColor;
        GUI.DrawTexture(new Rect(inRect.x, inRect.y, 30f, 30f), XenotypeIconDefOf.Basic.Icon);
        GUI.color = Color.white;
        inRect.yMin += 34f;
        var size = Vector2.zero;
        GeneUIUtility.DrawGenesInfo(inRect, target, InitialSize.y, ref size, ref scrollPosition);
        if (Widgets.ButtonText(new Rect(inRect.xMax - Window.CloseButSize.x, inRect.yMax, Window.CloseButSize.x, Window.CloseButSize.y), "Close".Translate()))
        {
            Close();
        }
    }
}