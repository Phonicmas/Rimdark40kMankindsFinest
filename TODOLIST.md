# TODO

## Balancing and final touches
Fill nightlord chapter gene with additional gene that might resist its effect

Balance primarch specific genes

Try and balance the more powerful genes. Still having them powerful, but not downright invicible, especially regardig taking damage.

Check cost list for various things - if they're balance and/or hard enough to aquire

Check market price for items as well.

## Text
Check descriptions
Compile list of features

use stuff like {PAWN_nameDef} and {PAWN_pronoun} in description texts instead of "Your" "Pawn" or other ways pawns are reffered too. (Where applicable)

Make sure all type of tags, defs and whatnot are prefixed with BEWH_

## Art

Shoulder pad icons for chapters and ranks (regarding back view, i can either render the backpack on top - or it can be draw to be around it)

Icon for:

Chaplain: Strenghten Faith ability

New turret texture for rogal ability turret (something something, you had some from vehicle mod i believe you said)

Drop pod texture (believe you said you had one of these too) + turret textture for ontop of it.




Maybe new motes for the linking and circle of the living saint aura?


Maybe some different backgrounds (the blue hexagon for xenotype or white circle thingy for endogenes) for the different genes? (Maybe keep the recoloured one psyker and pariah and then make some new type for the different super human, with increasing golden stuff and purity seals n stuff) (im just cooking here, this is not important)


## Code and XML
When chapter icons are made remove the temp setting of chapters in the Genes40kUtils.SetupChapterForPawn() method and the chapterColourDef from Genes40kDefOf



(Maybe) New trait "Serf" that has increase mood and work speed when near a Firstborn, Primaris, Custodes or Primarch (Maybe thunder warrior and living saint?)

(Maybe) Make functionality for jump pack to be installed on power armor. (Maybe some job or recipe, that uses the power armor and a jump pack and some components, which then does stuff and switches the graphic)

## Bugs


## Other

Should the Primarch specific genes be renamed? (currently the name is of the Primarch)


## Notes
To make a fully fledges chapter one should:
1) Make a geneDef for the chapter
2) Make a thingDef for the chapter 
3) Make a recipeDef for the chapter that inherits "BEWH_FirstbornVialInsertion" and "BEWH_PrimarisVialInsertion" and uses "DefModExtension_GeneseedVialRecipe" to define the item used and gene given.
4) Make a shoulderIconDef for your chapter to have a custom icon for the shoulder plates available (Optional)
4) Makes a chapterColourDef featuring the chapters colours, gene and icon. (Optional)


# NEW ADDITIONS TO MAKE AT SOME POINT (Not for first release most likely)

Make frostblade xml?

Make Fenrisian Wolves (Art also, maybe just take vanilla wolf and change it? Maybe ask someone who made a wolf like create for it and then change some of it)
These can then be found by a quest or by an event where they wander by. 
Should spawn one alpha and a couple of other ones, if alpha is slain, other ones immediately become tamed under the killer. Should also be made compatible with whatever mount riding framework is currently da shit(best), giddy up 2?
https://wh40k.lexicanum.com/wiki/Fenrisian_Wolf 

Make Blood Chalice art and xml, an exotic item that when ingested by pawns with the red thirst, will completely fill essentially all need and restore all wounds. Would be found in a quest to give one. One use, can be replenished somehow - https://wh40k.lexicanum.com/wiki/Blood_Chalice 

SOS2 integration?

Living Saint armor?


# Grey Knight

Add Grey Knight genetic material? possibly also a rankCategory for them along with their ranks?? https://wh40k.lexicanum.com/wiki/Grey_Knights

Will most likely be added as addon in seperate mod.

<!-- Chapter DCLXVI - Grey Knights-->
    <ThingDef ParentName="BEWH_ChapterGeneticMaterial">
        <defName>BEWH_ChapterMaterialDCLXVI</defName>
        <label>chapter material DCLXVI - Grey Knights</label>
        <description>This containers holds the genetic mutation material of Chapter DCLXVI - Grey Knights.</description>
        <graphicData>
            <texPath>Things/Item/ChapterMaterial/BEWH_ChapterMaterial_XX</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <modExtensions>
            <li Class="Genes40k.DefModExtension_ChapterMaterial">
                <orderInt>666</orderInt>
                <shownMaterialName>Grey Knight</shownMaterialName>
            </li>
            <li Class="Genes40k.DefModExtension_GeneFromMaterial">
                <addedGene>BEWH_ChapterXXAlphaLegion</addedGene>
            </li>
        </modExtensions>
    </ThingDef>

# Cybernetics

<!-- Angron: Frenzied Berserk -->  <!-- Should instead be transferred to butcher's nail implant (with reduced power on the hediff itself) -->
  <AbilityDef ParentName="BEWH_SelfTargetAbilities">
    <defName>BEWH_AngronBerserk</defName>
    <label>Frenzied Berserk</label>
    <description>Fly into a frenzied rage, gaining increased damage and hit chance, but losing survivability and control.</description>
    <iconPath>UI/Abilities/BEWH_Ability_AngronBerserk</iconPath>
    <cooldownTicksRange>30000</cooldownTicksRange>
    <category>BEWH_Primarch</category>
    <statBases>
      <Ability_Duration>1250</Ability_Duration>
    </statBases>
    <comps>
      <li Class="Core40k.CompProperties_AbilityGiveHediffAndMental">
        <compClass>CompAbilityEffect_GiveHediff</compClass>
        <hediffDef>BEWH_AngronBerserk</hediffDef>
        <mentalStateDef>Berserk</mentalStateDef>
        <onlyApplyToSelf>True</onlyApplyToSelf>
        <replaceExisting>true</replaceExisting>
      </li>
    </comps>
  </AbilityDef>

  <!-- Angron: Berserk -->
	<HediffDef ParentName="BEWH_StatAlteringHediffDef">
		<defName>BEWH_AngronBerserk</defName>
		<label>berserk</label>
		<description>RAAAAGGGEEEEE INCAAAARNATEEEEE</description>
		<stages>
			<li>
				<statFactors>
					<MeleeHitChance>1.5</MeleeHitChance>
					<MeleeDamageFactor>2</MeleeDamageFactor>
					<ArmorRating_Blunt>0.5</ArmorRating_Blunt>
					<ArmorRating_Sharp>0.5</ArmorRating_Sharp>
					<ArmorRating_Heat>0.5</ArmorRating_Heat>
				</statFactors>
				<statOffsets>
					<MeleeDodgeChance>-30</MeleeDodgeChance>
				</statOffsets>
			</li>
		</stages>
		<comps>
			<li Class="Core40k.HediffCompProperties_RemoveMentalStateOnHediffEnd">
				<specificMentalState>Berserk</specificMentalState>
			</li>
		</comps>
	</HediffDef>