using Verse;

namespace Genes40k
{
    public class Gene_WarpShield : Gene
    {
        public bool IsShielded = false;

        public void SwitchShieldState()
        {
            IsShielded = !IsShielded;
        }
    }
}