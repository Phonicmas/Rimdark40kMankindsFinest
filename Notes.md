# Display orders for genes
Thunder Warrior: (-100)-(-1)
Firstborn: 1-19
Primaris: 21-23
Legion Specific: 100-199
Custodes: 200-299
Primarch: 300-399
Primarch Specific: 400-499
Living Saint: 500-599
Psyker: 600-699
Pariah: 700-799
Perpetual: 800-899



# NEW ADDITIONS TO MAKE AT SOME POINT (Not for first release most likely)

Make frostblade xml?

Make Fenrisian Wolves (Art also) (Using 1.6 alpha animal stuff)
These can then be found by a quest or by an event where they wander by. 
Should spawn one alpha and a couple of other ones, if alpha is slain, other ones immediately become tamed under the killer. Should also be made compatible with whatever mount riding framework is currently da shit(best), giddy up 2?
https://wh40k.lexicanum.com/wiki/Fenrisian_Wolf 

Make Blood Chalice art and xml, an exotic item that when ingested by pawns with the red thirst, will completely fill essentially all need and restore all wounds. Would be found in a quest to give one. One use, can be replenished somehow - https://wh40k.lexicanum.com/wiki/Blood_Chalice 

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