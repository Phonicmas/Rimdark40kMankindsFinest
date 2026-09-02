using System.Text;
using HarmonyLib;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Pawn), "GetInspectString")]
public class ProgenoidGlandProgress
{
    public static void Postfix(ref string __result, Pawn __instance)
    {
        var line = Genes40kUtils.ProgenoidProgressLine(__instance);

        if (line == null)
        {
            return;
        }

        var stringBuilder = new StringBuilder(__result);

        stringBuilder.AppendLine("\n");
        stringBuilder.AppendLine(line);

        __result = stringBuilder.ToString().TrimEndNewlines();
    }
}