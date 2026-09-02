namespace Genes40k;

public class JobDriver_CarryMaterialToSangprimus : JobDriver_CarryThingToBuilding
{
    private Building_SangprimusPortum SangprimusPortum => Building as Building_SangprimusPortum;

    protected override bool TargetsValid()
    {
        return SangprimusPortum != null && CarriedThing != null;
    }

    protected override void OnArrived()
    {
        SangprimusPortum?.AddMaterial(CarriedThing);
    }
}