# MorphBuilder
This is an applictaion for building multi stage OBJ morphs.

Many application support the concept of morphs by taking a base OBJ file and morph OBJ file and calculating the delta between the two files. However, many applications do not support multi stage morphs because these applications typically look at the difference between only two states (the base and the morph) without taking into account any other morphs that may already be applied.

Consider a simple example of a cube that has two morphs: a pipe morph which changes the cude into a cylinder and a morph that puts a dent into the top of cylinder. If one creates a morph for turning the cube into a pipe, saves that as morph 1, then puts the dent into the cyilinder and saves that as morph 2, many software will not redner the results correctly when both morphs are applied. This is because the second morph already includes the first morph. This means to apply the second morph we have to first remove the first morph and then apply the second morph. This generally does not look correct if the morphs are applied gradually.

This is where MorphBuilder comes to the rescue. MorphBuilder takes multi state morphs (morphs that are always used in a sequence) and removes the effect of the previous morphs. If the resulting morphs are used instead, the morphs can now be applied in sequence, without needing to de-apply the previous stages, and result in the desired apperance. 

-------------------
Usage Instructions:
-------------------

MorphBuilder Base.OBJ Morph1.OBJ Morph2.OBJ ...

Where "..." can be any number of additional morph file. The files names do not need to be Base.OBJ and Morph?.OBJ but they need to be presented in order (i.e. in the order that they will be applied) and cannot have any spaces in the name. 
