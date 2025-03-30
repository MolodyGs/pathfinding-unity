using System.IO;
using System;
using UnityEngine;

/// <summary> 
/// Clase inicial la cual ejecuta la lectura del archivo tiles.json a partir de la clase TilesController.
/// </summary>
public class App : MonoBehaviour
{
  GameObject[] tiles;
  public async void Start()
  {
    //Obtiene los tiles a partir de la lectura del archivo tiles.json
    tiles = await TilesController.ReadTilesData();

    // Se asignan los tiles al PathfindingController
    PathfindingController.SetTiles(tiles);

    // Luego de la asignación, se espera la interacción del usuario para iniciar el pathfinding por medio de la clase InputController
  }
}

