# TP 1

- Pour visualiser les différents éléménts, il faut créer des Empty Objects dans sa scène et affecter chacun des Scripts à la forme associée.
- Chaque fonction renvoie un tableau de Vector3 pour les vertices et une Liste d'int pour les triangles.

## Plan

- Script : Rectangle.cs
- Fonction RectanglesMesh(int width, int height, int nb_rect_w, int nb_rect_h)
	+ Le nombre de vertices générés est égal : (nb_rect_w + 1) * (nb_rect_h + 1)
	+ Le nombre de triangles générés est égale à nb_rect_h * nb_rect_h * 2

<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/plan.png" width="600">

## Cylinder

- Script : Cylinder.cs
- Fonction CylinderMesh(float r, float h, int m) où r est le rayon des bases, h la hauteur et m le nombre méridians. Cette fonction effectue :
	+ La génération des vertices des bases du cylindre
	+ La génération des vertices centraux des bases (pour bien les fermer)
	+ La construction des triangles des facets selon la formule adéquate
	+ La génération des triangles des bases

<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/cylinder.png" width="600">

## Sphere

- Script : SpherePoles.cs et SphereComplete.cs
- Le premier script me sert uniquement à visualiser les triangles des poles de la sphère pour faciliter ma compréhension
- Le deuxième script quand à lui contient la fonction SphereMesh(int meridians, int parallels), qui génère en fonction d'un nombre de méridians et de parallèles défini les vertices et triangles adéquats, en suivant l'équation vu en cours.

<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/sphere.png" width="600">

## Cone

- Script : Cone.cs
- Fonction ConeMesh(float radius, float height, int segments) où radius est le rayon de la base, height la hauteur et segments le nombre de méridians
	+ D'abord est généré le vertice de la pointe du cone
	+ Puis les vertices de la base ainsi que son centre en suivant l'équation d'un cercle et en fonction du nombre de segments
	+ Ensuite on construit les triangles reliant le vertice de la pointe et les vertices du cercle de la base
	+ Et enfin on construit les triangles de la base

<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/cone.png" width="600">

## Truncated Cone

- Script : TruncatedCone.cs
- La fonction TruncatedConeMesh(float bottomRadius, float topRadius, float height, int segments) fonctionne globalement pareil que ConeMesh() sauf qu'on a un paramètre en plus qui correspond au radius de la plus petite base (de tronquage).
- La génération inclut donc en plus celle des vertices de la base secondaire et son centre.
- Les triangles quand à eux relient les points de la base supérieure à ceux de la base inférieure (on se retrouve avec deux fois plus de triangles que pour Cone.cs)

<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/truncated_cone.png" width="600">

## Pacman

- TODO, incomplete

# TP 2

## Fonction de lecture d'un fichier .off

- La fonction LoadPath(string path) prend en paramètre un nom de mesh sous format .off déposé dans "Assets/Mesh"
- Elle lit les différents paramètres
- Génère les vertices en parsant les valeur flottantes avec le paramètre CultureInfo.InvariantCulture (très important)
- Elle calcule la plus grande valeur de Vector3 des vertices pour la normalisation de tous les vertices
- Elle calcule également la moyenne de Vector3 des vertices pour définir le centre de gravité de l'objet et décale tous les vertices en conséquence
- En même temps elle intègres les différentes données des triangles, tout en calculant la normale via une accumulation de produits vectoriels.
- A la fin elle normalise les normales de chaque sommet et renvoie le tout sous la forme d'un triplet {vertices, listTriangles, normalizedNormals}

<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/bunny_read.png" width="600">

## Fonction d'écriture d'un fichier .obj à partir des données de la première partie

- Cette fonction WriteOnPath(string path, string name, Vector3[] vertices, int[] triangles, Vector3[] normals) prend en entrée le chemin et le nom de du Mesh à écrire sous format .obj
- Elle transcript les données de vertices du mesh initial sous le format "v {0} {1} {2}"
- Ainsi que les données de normales "vn {0} {1} {2}"
- Puis les triangles sous le format d'indexage "f index_1//index_1 index_2//index_2 index_3//index_3". Le mapping entre les index et le tableau de vertices et de normales initiaux se fait naturellement.

<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/bunny_write.png" width="600">
<img src="https://github.com/NajibXY/gamagora_modgeo/blob/main/img/buddha_write.png" width="600">


