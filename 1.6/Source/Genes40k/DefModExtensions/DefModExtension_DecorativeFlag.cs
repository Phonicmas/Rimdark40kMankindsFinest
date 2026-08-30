using UnityEngine;
using Verse;

namespace Genes40k;

/// <summary>
/// Per rotation overrides for the insignia drawn on top of a <see cref="Building_DecorativeFlag"/>.
/// Every field is optional, unset fields fall back to the extension wide defaults.
/// </summary>
public class FlagInsigniaRotationData
{
    public bool draw = true;

    public Vector3? drawOffset;

    public Vector2? drawSize;

    public float? extraRotation;
}

public class DefModExtension_DecorativeFlag : DefModExtension
{
    //Defaults used by every rotation that does not override them.
    public Vector3 insigniaDrawOffset = new Vector3(0f, 0.1f, 0.8f);

    public Vector2 insigniaDrawSize = Vector2.one;

    public float insigniaExtraRotation = 0f;

    //Per rotation overrides, leave a rotation out to use the defaults above.
    public FlagInsigniaRotationData north;

    public FlagInsigniaRotationData east;

    public FlagInsigniaRotationData south;

    public FlagInsigniaRotationData west;

    private FlagInsigniaRotationData DataFor(Rot4 rotation)
    {
        switch (rotation.AsInt)
        {
            case Rot4.NorthInt:
                return north;
            case Rot4.EastInt:
                return east;
            case Rot4.SouthInt:
                return south;
            case Rot4.WestInt:
                return west;
            default:
                return null;
        }
    }

    public bool DrawsInsignia(Rot4 rotation)
    {
        return DataFor(rotation)?.draw ?? true;
    }

    public Vector3 InsigniaDrawOffset(Rot4 rotation)
    {
        return DataFor(rotation)?.drawOffset ?? insigniaDrawOffset;
    }

    public Vector2 InsigniaDrawSize(Rot4 rotation)
    {
        return DataFor(rotation)?.drawSize ?? insigniaDrawSize;
    }

    public float InsigniaExtraRotation(Rot4 rotation)
    {
        return DataFor(rotation)?.extraRotation ?? insigniaExtraRotation;
    }
}
