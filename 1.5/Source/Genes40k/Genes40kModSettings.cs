using Verse;


namespace Genes40k
{
    public class Genes40kModSettings : ModSettings
    {
        public bool psychicPhenomena = true;
        public bool psykerPariahBirth = true;
        public int psykerPariahBirthChance = 10;
        public bool perpetualBirth = true;
        public int perpetualBirthChance = 3;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref psychicPhenomena, "psychicPhenomena", true);
            Scribe_Values.Look(ref psykerPariahBirth, "psykerPariahBirth", true);
            Scribe_Values.Look(ref psykerPariahBirthChance, "psykerPariahBirthChance", 10);
            Scribe_Values.Look(ref perpetualBirthChance, "perpetualBirthChance", 3);
            base.ExposeData();
        }
    }
}