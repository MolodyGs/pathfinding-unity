using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Controlador de Pathfinding para encontrar el camino más corto entre dos puntos mediante el algoritmo A* y por medio de GameObjects.
/// </summary>
public static class PathfindingController
{
  // Lista de GameObjects que representan los tiles del mapa.
  static GameObject[] tiles;

  // Lista de GameObjects que representan el camino más corto.
  public static List<GameObject> path = new();

  // Prefab de la flecha que indica la dirección del camino.

  public static async Task Path()
  {
    List<GameObject> activeTiles;

    activeTiles = new List<GameObject>
    {
      InputController.origin
    };
    InputController.origin.GetComponent<Tile>().active = true;

    int hCost = CalcHCost(InputController.origin.transform.position);

    InputController.origin.GetComponent<Tile>().SetGCost(0);
    InputController.origin.GetComponent<Tile>().SetHCost(hCost);

    Debug.Log("hCost: " + hCost);
    Debug.Log("Total" + InputController.origin.GetComponent<Tile>().fCost);

    GameObject lastTile = await EvaluateTile(InputController.origin);

    if (lastTile != null)
    {
      path.Add(lastTile);
      lastTile.GetComponent<Tile>().SetPath();
    }
  }

  /// <summary>
  /// Evalua un nuevo tile activo, buscando el mejor camino entre los vecinos del tile activo.
  /// </summary>
  static async Task<GameObject> EvaluateTile(GameObject activeTiles)
  {
    Debug.Log(" - Evaluando Tile: " + activeTiles.transform.position);

    // Lista de vecinos
    GameObject[] neighbors = new GameObject[8];

    // Establece los vecinos
    neighbors = SetNeighbors(neighbors, activeTiles);

    // Itera entre los vecinos y evalua el costo de G y H de cada vecino.
    EvaluateNeighborsCost(neighbors, activeTiles);

    // Busca el mejor tile para continuar el camino.
    GameObject betterTile = FindBetterTile();

    // Si no se encuentra un mejor camino, entonces hemos evaluado todos los caminos sin encontrar el destino.
    if (betterTile == null)
    {
      Debug.LogError("No hay camino");
      NoPath();
      return null;
    }

    // Si el mejor tile es el destino, entonces hemos encontrado el camino más corto.
    if (betterTile.transform.position == InputController.destination.transform.position)
    {
      Debug.Log("Llegamos al destino");
      return betterTile;
    }

    // Establece el mejor tile como activo y lo agrega a la lista de tiles activos.
    // activeTiles.Add(betterTile);
    betterTile.GetComponent<Renderer>().material.color = Global.GREEN;
    betterTile.GetComponent<Tile>().active = true;

    // De forma recursiva, se evalua el siguiente tile activo.
    Debug.Log(" --- Cargando Siguiente Evaluación: " + betterTile.transform.position);

    await Task.Delay(5);
    return await EvaluateTile(betterTile);

  }

  static GameObject FindBetterTile()
  {
    GameObject betterTile = null;
    int betterTileCost = 9999;
    foreach (GameObject tile in tiles)
    {

      if (tile.GetComponent<Tile>().active)
      {
        continue;
      }

      int fcost = tile.GetComponent<Tile>().fCost;
      if (fcost > 0 && fcost < betterTileCost)
      {
        betterTile = tile;
        betterTileCost = fcost;
      }
    }
    return betterTile;
  }

  /// <summary>
  /// Itera entre los vecinos de un tile y evalua el costo, comprobando si el tile activo es un mejor padre que el padre actual de los tiles vecino.
  /// </summary>
  static void EvaluateNeighborsCost(GameObject[] neighbors, GameObject activeTile)
  {
    foreach (GameObject neighbor in neighbors)
    {
      if (neighbor != null && !neighbor.GetComponent<Tile>().blocked)
      {
        // Obtiene la distancia entre los tiles. Generalmente estos tiles están a 1 o raiz de 2 de distancia, esto ya que son tiles adyacentes los evaluados.
        float distance = Vector3.Distance(neighbor.transform.position, activeTile.transform.position);

        // Calcula el costo de G para el tile vecino.
        int gCost = distance > 1 ? 14 : 10;

        // Suma el costo de G del tile activo al costo de G del tile vecino para obtener el costo G total
        gCost += activeTile.GetComponent<Tile>().gCost;
        Debug.Log("Distance entre tile activo:" + activeTile.transform.position + " y vecino: " + neighbor.transform.position + ": " + distance + " gCost: " + gCost + " neighbor gCost: " + neighbor.GetComponent<Tile>().gCost);

        // Si el tile tiene un cost F igual a 0, entonces esta es la primera vez que se evalua el tile.
        // Si el nuevo costo es menor al costo actual del tile vecino, entonces se actualiza el costo de G y H.
        if (neighbor.GetComponent<Tile>().fCost == 0 || gCost < neighbor.GetComponent<Tile>().gCost)
        {
          Debug.Log(" -- Costo actualizado para " + neighbor.transform.position + " con el origen: " + activeTile.transform.position + " gcost: " + gCost);
          neighbor.GetComponent<Tile>().SetGCost(gCost);
          neighbor.GetComponent<Tile>().SetHCost(CalcHCost(neighbor.transform.position));
          neighbor.GetComponent<Tile>().SetParent(activeTile);
        }

        //Establece el color del tile vecino para indicar que ha sido evaluado.
        neighbor.GetComponent<Renderer>().material.color = neighbor.GetComponent<Tile>().active ? Global.GREEN : Global.YELLOW;
        Debug.Log(" --- Costo para " + neighbor.transform.position + ": gCost: " + neighbor.GetComponent<Tile>().gCost + " hCost: " + neighbor.GetComponent<Tile>().hCost + " fCost: " + neighbor.GetComponent<Tile>().fCost);
      }
    }
  }

  /// <summary>
  /// Encuentra los vecinos de un tile y los establece en un arreglo.
  /// </summary>
  static GameObject[] SetNeighbors(GameObject[] neighbors, GameObject tile)
  {
    // Obtiene la posición de referencia
    int x = (int)tile.transform.position.x;
    int z = (int)tile.transform.position.z;

    // Busca los 8 posibles vecinos de un tile, comenzando desde la esquina inferior izquierda y siguiendo el sentido de las manecillas del reloj.
    neighbors[0] = Findneighbor(new Vector3(x - 1, 0, z - 1));
    neighbors[1] = Findneighbor(new Vector3(x - 1, 0, z));
    neighbors[2] = Findneighbor(new Vector3(x - 1, 0, z + 1));
    neighbors[3] = Findneighbor(new Vector3(x, 0, z + 1));
    neighbors[4] = Findneighbor(new Vector3(x + 1, 0, z + 1));
    neighbors[5] = Findneighbor(new Vector3(x + 1, 0, z));
    neighbors[6] = Findneighbor(new Vector3(x + 1, 0, z - 1));
    neighbors[7] = Findneighbor(new Vector3(x, 0, z - 1));
    Debug.Log(" -- Neighbors Establecidos para: " + tile.transform.position);
    return neighbors;
  }

  /// <summary>
  /// Encuentra el vecino de un tile
  /// </summary>
  static GameObject Findneighbor(Vector3 tilePosition)
  {
    Debug.Log(" -- Buscando vecino: " + tilePosition);
    try
    {
      for (int i = 0; i < tiles.Length; i++)
      {
        if (tiles[i].transform.position.x == tilePosition.x && tiles[i].transform.position.z == tilePosition.z)
        {
          Debug.Log(" -- Vecino encontrado: " + tiles[i].transform.position);
          return tiles[i];
        }
      }
      Debug.Log(" -- Vecino no encontrado: " + tilePosition);
    }
    catch (Exception e)
    {
      foreach (GameObject tile in tiles)
      {
        Debug.Log(" -- Tile: " + tile);
      }
      Debug.LogError("Error al buscar vecino: " + e.Message);
    }
    return null;
  }

  /// <summary>
  /// Calcula el costo de H para un tile.
  /// </summary>
  static int CalcHCost(Vector3 tile)
  {
    float x = Math.Abs(tile.x - InputController.destination.transform.position.x);
    float z = Math.Abs(tile.z - InputController.destination.transform.position.z);
    return 10 * (int)Math.Abs(x - z) + 14 * (int)(x > z ? z : x);
  }

  /// <summary>
  /// Reinicia los tiles del mapa y el camino más corto.
  /// </summary>
  public static void ResetTiles()
  {
    path = new();
    foreach (GameObject tile in tiles)
    {
      tile.GetComponent<Tile>().Reset();
    }
  }

  /// <summary>
  /// Establece los tiles activos con el color rojo para indicar visualmente que no hay camino.
  /// </summary>
  public static void NoPath()
  {
    foreach (GameObject tile in tiles)
    {
      if (tile.GetComponent<Tile>().active)
      {
        tile.GetComponent<Renderer>().material.color = Global.RED;
      }
    }
  }

  /// <summary>
  /// Establece los tiles del mapa.
  /// </summary>
  public static void SetTiles(GameObject[] tiles)
  {
    PathfindingController.tiles = tiles;
  }
}
