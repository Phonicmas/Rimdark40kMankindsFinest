using System.Collections.Generic;
using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ExtraDecorationDef : Def
    {
        [NoTranslate]
        public string iconPath;
        
        [Unsaved]
        private Texture2D icon;

        public Texture2D Icon
        {
            get
            {
                if (icon != null)
                {
                    return icon;
                }
                
                icon = !iconPath.NullOrEmpty() ? ContentFinder<Texture2D>.Get(iconPath) : ContentFinder<Texture2D>.Get(Genes40kDefOf.BEWH_ShoulderNone.iconPath);
                return icon;
            }
        }

        [NoTranslate]
        public string drawnTextureIconPath;

        public float sortOrder = 0f;
        
        public List<Rot4> defaultShowRotation = new List<Rot4>();

        public ShaderTypeDef shaderType = ShaderTypeDefOf.Cutout;
        
        public Color defaultColor = new Color(1f, 1f, 1f, 1f);
        
        //Should be between 0f-0.99f
        public float layerOrder = 0f;

        public float LayerOrder
        {
            get
            {
                return layerOrder switch
                {
                    > 1 => 0.99f,
                    < 0 => 0,
                    _ => layerOrder
                };
            }
        }
    }
}   