using UnityEngine;
using Verse;

namespace Genes40k;

public class FlagIconDef : Def
{
    [NoTranslate]
    public string iconPath;
    [Unsaved(false)]
    private Texture2D icon;
    
    public Texture2D Icon
    {
        get
        {
            if (icon != null)
            {
                return icon;
            }
            icon = !iconPath.NullOrEmpty() ? ContentFinder<Texture2D>.Get(iconPath) : ContentFinder<Texture2D>.Get("NoTex");
            return icon;
        }
    }
    
    public float sortOrder;
    
    public bool setsNull = false;
}