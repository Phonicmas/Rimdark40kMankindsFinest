namespace Genes40k;

public class JobDriver_CarryMatrixToGeneGestator : JobDriver_CarryThingToBuilding
{
    private Building_GeneGestator GeneGestator => Building as Building_GeneGestator;

    protected override bool TargetsValid()
    {
        return GeneGestator != null && CarriedThing != null;
    }

    protected override void OnArrived()
    {
        GeneGestator?.AddGeneMatrix(CarriedThing);
    }
}