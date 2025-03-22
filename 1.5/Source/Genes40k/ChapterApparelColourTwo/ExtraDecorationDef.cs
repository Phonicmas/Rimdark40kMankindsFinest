using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class ExtraDecorationDef : DecorationDef
    {
        public bool isHelmetDecoration = false;
        
        public List<Rot4> defaultShowRotation = new() {Rot4.North, Rot4.South, Rot4.East, Rot4.West};

        public ShaderTypeDef shaderType = null;
        
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