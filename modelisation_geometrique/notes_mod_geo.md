# Modélisation Géométrique


- Applications de la thèse entre autre pour le delta de l'état de la ville à travers des modélisations
- Etude qualitative des données d'une année à l'autre

- Pipeline : modélisation -> habillage (couleur, lumière, texture) -> illumination -> visualisation -> rendu

- Modèles surfaciques (polygonique ou paramétrique) et modèles volumiques (voxels/octrees ou surfaces implicites)
- Compromis de taille, scalabilité, multirés, qualité, techniques d'habillage et d'illumination etc...



## Représentations surfaciques, polyèdres et quadriques

- Importer sur un polyèdre de définir le sens des triangles
	- Définir le sens définit la normale et donc l'ordre de calcul lors du lancer de rayon
	- "Recalculate normals" annule cette shit
- Sphère définie en matière de surface continue par x²+y²+z² = r²

- Quadriques contiennent les cylendres, les cônes, les sphères, les --loïdes

- Cylindre 
	- Axe de révolution D
	- Rayon r
	- Ensemble R^3 de points situés à distance r de la droite D
	- Méridiens (droites qui font le tour)
	- Facettes : découpage rectangulaire entre méridiens
- Cônes : ensemble de droite passant par un sommet s'appuyant sur une base (courbe)
	- Facette trapézoidales
	- 2 faces pour les plans limites quand même
- Sphère : centre et rayon
	- Facettisations avec un nombre de méridien >= 3 et des parallèles >= 2


- Facettisation = discrètisation à partir d'une équation de surface.

### TP 1

- Liste de points et liste de triangles

- GameObject : MeshRenderer, MeshFilter, Material

### ... Modélisations surfaciques, maillage ...

### Voxelisation et surface volumique

- Modèles volumiques :
	- Octree régulier : découpage égal
	- Octree adaptatif : découpage irrégulier
		- Plus on approche de la surface/bords plus on peut avoir de découpage, pour l'opti
 		- Hierarchisation et affichage à différentes résolutions vs coût de stockage
 	- Arbres Constructive Solid Geometry 
 		- Utilisation des opérateurs Union, Intersect, Minus pour générer des volumes
 	- Surfaces implicites discrètes
 		- Potentiel par voxel (affichage en fonction du potentiel par exemple)
 	- Marching cube