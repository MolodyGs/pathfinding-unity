# Pathfinding - Unity

![](/Assets/Resources/media/example1.gif)

Proyecto hecho en **Unity versión 2021.3.8f1**.

Mediante la utilización de **GameObjects** se aplica el algoritmo de búsqueda A* (A estrella). Esto gracias a la identificación de los elementos de la escena, diferenciando entre lo que es un camino libre y uno bloqueado. Es posible cambiar la forma del camino mediante un archivo `tiles.json`, en el cual se define la posición bajo una perspectiva bidimensional (x, z con "y" como altura), además de un parámetro `blocked` para definir si ese tile corresponde a una sección bloqueada.

**Es posible utilizar una versión funcional (WebGL) del proyecto mediante el siguiente [enlace](https://molodygs.github.io/pathfinding/).**

## Requisitos

Es necesario contar con la versión de Unity `2021.3.8f1` o una compatible. Además, **se debe añadir la librería `TextMeshPro`**, esto se puede realizar fácilmente mediante el **gestor de paquetes** de Unity.

### Estado Inicial - Main.cs

En `Main.cs` es donde, mediante la clase estática `TilesController.cs`, el proyecto comienza a realizar una identificación de los GameObject en la escena.

### Algoritmo de Búsqueda A* - pathfinding.cs

...

## Galería

![](/Assets/Resources/media/example1.gif)
![](/Assets/Resources/media/example2.gif)
