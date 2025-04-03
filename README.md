# Pathfinding - Unity

Proyecto hecho en **Unity versión 2021.3.8f1**.

Mediante la utilización de **GameObjects** se aplica el algoritmo de búsqueda A* (A estrella). Esto gracias a la identificación de los elementos de la escena, diferenciando entre lo que es un camino libre y uno bloqueado. Es posible cambiar la forma del camino mediante un archivo `tiles.json`, en el cual se define la posición bajo una perspectiva bidimensional (x, z con "y" como altura), además de un parámetro `blocked` para definir si ese tile corresponde a una sección bloqueada.

## Requisitos

Es necesario contar con la versión de Unity `2021.3.8f1` o una compatible. No es necesario añadir librerías externas.

## Estructura del Proyecto

La estrucutra del proyecto está conformada por estos directorios:
```
Pathfinding/
├─ Assets/
│  ├─ Resources/
│  │  ├─ tiles.json
│  ├─ Scenes/
│  ├─ Scrips/
│  │  ├─ Components/
│  │  │  ├─ Tile.cs
│  │  ├─ Controllers/
│  │  │  ├─ InputController.cs
│  │  │  ├─ JsonController.cs
│  │  │  ├─ PathfindingController.cs
│  │  │  ├─ TilesController.cs
│  │  ├─ Serializables/
│  │  │  ├─ TileDTO.cs
│  │  │  ├─ TileListDTO.cs
│  │  ├─ App.cs
├─ Docs/
│  ├─ pathfinding diagram.drawio
```

### Estado Inicial - App.cs

En `App.cs` es donde, mediante la clase estática `TilesController.cs`, el proyecto comienza a realizar una lectura del archivo `tiles.json`. Si este archivo no existe, se creará según los **GameObjects** que ya estén en la escena y que sean hijos del **GameObject Tiles**, esto a través de la clase estática `Json.cs`.

### Algoritmo de Búsqueda A* - pathfinding.cs

...

## Galería

![](/Assets/Resources/media/example1.gif)
