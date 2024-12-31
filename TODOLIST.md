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

In the FromCode.xml where all text are, sort them properly cause oh boy its not sorted anymore

## Art

Shoulder pad icons for chapters and ranks




Maybe some different backgrounds (the blue hexagon for xenotype or white circle thingy for endogenes) for the different genes? (Maybe keep the recoloured one psyker and pariah and then make some new type for the different super human, with increasing golden stuff and purity seals n stuff) (im just cooking here, this is not important)

Primarch Specific Icons (Not sure what excatly to do here) (Maybe flip the helix of the Chapter ones and add some bedazzle? Or just make something that would remind you of them, like red skin and psyker thing i made for Magnus, and the wings for Sanguinius) (The helix should stay i feel) (I think i learn towards just flipping the helix and adding some bedazzle)

## Code and XML

add stat offsets and factor and abilities to ranks. possibly add more?

Make raid type for imperium, that uses ChapterColourDefs for colour for their armor, and the geneseed for the marine deployed, should all be from same chapter.
^^ use custom PawnGroupKindDef worker, also make them have unlocked the first two ranks of the rank tree, with required days + random between 0-3 additional days.
If player selected chapter colour is not custom, then dont spawn the selected one from them
If player got path to chaos, disable heretic chapters from spawning then.

Increase domitar shooting and melee, either skill or chance to hit. (skill is currently set to 4)

For embryo creation window. Put a box below each current box, with a name of what it is (required-wise, geneseed vial, human embryo, pawn)

For ranks, make it such that if a pawn is recruited, check if their current ranks are inside legal limit, if not, de-rank them.

Raid thingy not working properly.

Make code to allow rank icons and chapter icons to show on shoulder pads. Use render nodes like shoulder pad with maybe custom worker. Get rank icon from rank def, and use highest rank of pawn (or selectable?) For chapter icons make them selectable in the colour tool thingy.

In PawnRenderNodeWorker_AttachmentChapterRankIcon, make standard path use neophyte icon when made.

possible also to give each shoulder pad own colours


Aura ability for living saint, buffing nearby pawn temporarily (Maybe)

And when a rank is given if it is higher or equal to current ones importance then change the pawns rank in their info (the place where you can insert a custome one) (Maybe)

In spect window code, call discard on pawn???

## Bugs


## Other

Should the Primarch specific genes be renamed? (currently the name is of the Primarch)



# NEW ADDITIONS TO MAKE AT SOME POINT (Not for first release most likely)

Make frostblade xml?

Make Fenrisian Wolves (Art also, maybe just take vanilla wolf and change it? Maybe ask someone who made a wolf like create for it and then change some of it)
These can then be found by a quest or by an event where they wander by. 
Should spawn one alpha and a couple of other ones, if alpha is slain, other ones immediately become tamed under the killer. Should also be made compatible with whatever mount riding framework is currently da shit(best), giddy up 2?
https://wh40k.lexicanum.com/wiki/Fenrisian_Wolf 

Make Blood Chalice art and xml, an exotic item that when ingested by pawns with the red thirst, will completely fill essentially all need and restore all wounds. - https://wh40k.lexicanum.com/wiki/Blood_Chalice 

SOS2 integration?




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