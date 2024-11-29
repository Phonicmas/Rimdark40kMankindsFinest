# TODO

## Balancing and final touches
Fill nightlord chapter gene with additional gene that might resist its effect

Balance primarch specific genes

Try and balance the more powerful genes. Still having them powerful, but not downright invicible regardig taking damage.

Check cost list for various things - if they're balance and/or hard enough to aquire

Check market price for items as well.

## Text
Check descriptions
Compile list of features

use stuff like {PAWN_nameDef} and {PAWN_pronoun} in description texts instead of "Your" "Pawn" or other ways pawns are reffered too.

## Art

Primarch Specific Icons (Not sure what excatly to do here) (Maybe flip the helix of the Chapter ones and add some bedazzle? Or just make something that would remind you of them, like red skin and psyker thing i made for Magnus, and the wings for Sanguinius) (The helix should stay i feel) (I think i learn towards just flipping the helix and adding some bedazzle)

primarch material thing icons

Maybe some different backgrounds (the blue hexagon for xenotype or white circle thingy for endogenes) for the different genes? (Maybe keep the recoloured one psyker and pariah and then make some new type for the different super human, with increasing golden stuff and purity seals n stuff) (im just cooking here, this is not important)
    
Insert and Eject primarch Embryo icon

And icon for starting for primarch vat

## Code and XML

add stat offsets and factor and abilities to ranks. possibly add more?

## Bugs

## Other

Should the Primarch specific genes be renamed? (currently the name is of the Primarch)

# NEW ADDITIONS TO MAKE AT SOME POINT

Make frostblade xml?

Make Fenrisian Wolves (Art also, maybe just take vanilla wolf and change it? Maybe ask someone who made a wolf like create for it and then change some of it)
These can then be found by a quest or by an event where they wander by. 
Should spawn one alpha and a couple of other ones, if alpha is slain, other ones immediately become tamed under the killer. Should also be made compatible with whatever mount riding framework is currently da shit(best), giddy up 2?
https://wh40k.lexicanum.com/wiki/Fenrisian_Wolf 

Make Blood Chalice art and xml, an exotic item that when ingested by pawns with the red thirst, will completely fill essentially all need and restore all wounds. - https://wh40k.lexicanum.com/wiki/Blood_Chalice 


https://wh40k.lexicanum.com/wiki/Gene-seed#Chapters_with_known_gene-seed_flaws

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