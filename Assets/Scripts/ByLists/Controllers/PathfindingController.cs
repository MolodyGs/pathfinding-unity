using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components;
using UnityEngine;

namespace Controllers
{

  /// <summary>
  /// Controlador de Pathfinding para encontrar el camino más corto entre dos puntos mediante el algoritmo A* y por medio de tiles.
  /// </summary>
  public static class PathfindingController
  {
    public static List<TileNode> path = new();

    /// <summary>
    /// Inicia el proceso de Pathfinding para encontrar el camino más corto entre dos puntos.
    /// </summary>
    public static async Task Path()
    {
      // Obtiene el tile de origen
      Debug.Log("Cargando origen: " + InputController.origin.transform.position.x + " " + InputController.origin.transform.position.z);
      TileNode origin = TilesController.tiles[(int)InputController.origin.transform.position.x, (int)InputController.origin.transform.position.z];
      Debug.Log(InputController.origin.transform.position);
      Debug.Log(origin);
      // Calcula el costo de H para el tile de origen.
      int hCost = CalcHCost(origin);
      origin.SetHCost(hCost);
      origin.closed = true;

      TilesController.AddEvaluatedTile(origin);

      // Obtiene el último tile evaluado. Este corresponde al final o a null si no se encuentra un camino.
      TileNode lastTile = await EvaluateTile(origin);

      // Si el último tile evaluado corresponde al destino, entonces se agrega a la lista de tiles del camino más corto.
      if (lastTile != null)
      {
        // Se establece el camino más corto en los tiles.
        await lastTile.SetPath();

        // Comienza el movimiento del personaje.
        await MoveController.Move();
      }
    }

    /// <summary>
    /// Evalua un nuevo tile activo, buscando el mejor camino entre los vecinos del tile activo.
    /// </summary>
    static async Task<TileNode> EvaluateTile(TileNode tile)
    {
      Debug.Log(" - Evaluando Tile: " + tile.x + " " + tile.z);

      // Lista de vecinos
      TileNode[] neighbors = new TileNode[8];

      // Establece los vecinos
      neighbors = SetNeighbors(neighbors, tile);

      // Itera entre los vecinos y evalua el costo de G y H de cada vecino.
      EvaluateNeighborsCost(neighbors, tile);

      // Busca el mejor tile para continuar el camino.
      TileNode bestTile = FindbestTile();

      // Si no se encuentra un mejor camino, entonces hemos evaluado todos los caminos sin encontrar el destino.
      if (bestTile == null)
      {
        Debug.LogError("No hay camino");
        NoPath();
        return null;
      }

      // Si el mejor tile es el destino, entonces hemos encontrado el camino más corto.
      if (bestTile.x == InputController.destination.transform.position.x && bestTile.z == InputController.destination.transform.position.z)
      {
        Debug.Log("Llegamos al destino");
        return bestTile;
      }

      // Establece el mejor tile como activo y lo agrega a la lista de tiles activos.
      // bestTile.GetComponent<Renderer>().material.color = Global.GREEN;
      bestTile.closed = true;
      RemoveLastTileEvaluated(bestTile);
      // De forma recursiva, se evalua el siguiente tile activo.
      Debug.Log(" --- Cargando Siguiente Evaluación: " + bestTile.GetPosition());

      // await Task.Delay(5);
      return await EvaluateTile(bestTile);
    }


    /// <summary>
    /// Remueve un tile de la lista de tiles evaluados.
    /// </summary>
    static void RemoveLastTileEvaluated(TileNode tileToRemove)
    {
      foreach (TileNode tile in TilesController.evaluatedTiles)
      {
        if (tile.GetPosition() == tileToRemove.GetPosition())
        {
          TilesController.evaluatedTiles.Remove(tile);
          return;
        }
      }
    }

    /// <summary>
    /// Encuentra el mejor tile para continuar el camino.
    /// </summary>
    static TileNode FindbestTile()
    {
      TileNode bestTile = null;
      int bestTileCost = 9999;

      Debug.Log("Buscando mejor tile...");
      Debug.Log("Cantidad de tiles a evaluar: " + TilesController.evaluatedTiles.Count);

      // Itera entre los tiles abiertos y busca el mejor tile para continuar el camino.
      foreach (TileNode tile in TilesController.evaluatedTiles)
      {
        Debug.Log("Tile: ("  + tile.x + ", " + tile.z + ") Closed: " + tile.closed + " g: " + tile.g + " h: " + tile.h + " f: " + tile.f);
        // Si el tile ya fue evaluado, entonces omite.
        if (tile.closed) { continue; }

        // Obtiene el costo F del tile.
        int fcost = tile.f;

        // Si el costo F es mayor a 0 y menor al mejor costo actual, entonces se actualiza el mejor tile.
        if (fcost > 0 && fcost < bestTileCost)
        {
          bestTile = tile;
          bestTileCost = fcost;
        }

        // Si el costo F es igual al mejor costo actual, entonces se compara el costo de H para determinar el mejor tile.
        if (fcost == bestTileCost && tile.h < bestTile.h)
        {
          bestTile = tile;
          bestTileCost = fcost;
        }
      }

      // Retorna el mejor tile encontrado.
      return bestTile;
    }

    /// <summary>
    /// Itera entre los vecinos de un tile y evalua el costo, comprobando si el tile activo es un mejor padre que el padre actual de los tiles vecino.
    /// </summary>
    static void EvaluateNeighborsCost(TileNode[] neighbors, TileNode tile)
    {

      // Itera entre los vecinos y evalua el costo de G y H de cada vecino.
      foreach (TileNode neighbor in neighbors)
      {

        // Si el vecino es nulo o está bloqueado, entonces se omite.
        if (neighbor == null || neighbor.blocked) { continue; }

        // Obtiene la distancia entre los tiles. Generalmente estos tiles están a 1 o raiz de 2 de distancia, esto ya que son tiles adyacentes los evaluados.
        float distance = Vector2.Distance(neighbor.GetPosition(), tile.GetPosition());

        // Calcula el costo de G para el tile vecino.
        int gCost = distance > 1 ? 14 : 10;

        // Suma el costo de G del tile activo al costo de G del tile vecino para obtener el costo G total
        gCost += tile.g;
        Debug.Log("Distance entre tile activo:" + tile.GetPosition() + " y vecino: " + neighbor.GetPosition() + ": " + distance + " gCost: " + gCost + " neighbor gCost: " + neighbor.g);

        // Si el tile tiene un cost F igual a 0, entonces esta es la primera vez que se evalua el tile.
        // Si el nuevo costo es menor al costo actual del tile vecino, entonces se actualiza el costo de G y H.
        if (neighbor.f == 0 || gCost < neighbor.g)
        {
          Debug.Log(" -- Costo actualizado para " + neighbor.GetPosition() + " con el origen: " + tile.GetPosition() + " gcost: " + tile.g);
          neighbor.SetGCost(gCost);
          neighbor.SetHCost(CalcHCost(neighbor));
          neighbor.parent = tile;
        }

        //Establece el color del tile vecino para indicar que ha sido evaluado.
        // neighbor.GetComponent<Renderer>().material.color = neighbor.GetComponent<Tile>().closed ? Global.GREEN : Global.YELLOW;
        Debug.Log(" --- Costo para " + neighbor.GetPosition() + ": gCost: " + neighbor.g + " hCost: " + neighbor.h + " fCost: " + neighbor.f);
      }
    }

    /// <summary>
    /// Encuentra los vecinos de un tile y los establece en un arreglo.
    /// </summary>
    static TileNode[] SetNeighbors(TileNode[] neighbors, TileNode tile)
    {
      // Obtiene la posición de referencia
      int x = tile.x;
      int z = tile.z;

      // Busca los 8 posibles vecinos de un tile, comenzando desde la esquina inferior izquierda y siguiendo el sentido de las manecillas del reloj.
      neighbors[0] = Findneighbor(x - 1, z - 1);
      neighbors[1] = Findneighbor(x - 1, z);
      neighbors[2] = Findneighbor(x - 1, z + 1);
      neighbors[3] = Findneighbor(x, z + 1);
      neighbors[4] = Findneighbor(x + 1, z + 1);
      neighbors[5] = Findneighbor(x + 1, z);
      neighbors[6] = Findneighbor(x + 1, z - 1);
      neighbors[7] = Findneighbor(x, z - 1);
      Debug.Log(" -- Neighbors Establecidos para: " + tile.x + " " + tile.z);

      // Retorna la lista de vecinos
      return neighbors;
    }

    /// <summary>
    /// Encuentra el vecino de un tile
    /// </summary>
    static TileNode Findneighbor(int x, int z)
    {
      Debug.Log(" -- Buscando vecino: " + x + " " + z);
      try
      {
        TileNode node = TilesController.tiles[x, z];

        if (node == null)
        {
          Debug.Log(" -- Vecino no encontrado: " + x + " " + z);
          return null;
        }

        if (!node.closed)
        {
          TilesController.AddEvaluatedTile(node);
        }

        Debug.Log(" -- Vecino encontrado! " + x + " " + z);
        return node;
      }
      catch (Exception e)
      {
        Debug.LogError("Error al buscar vecino: " + e.Message);
      }
      return null;
    }

    /// <summary>
    /// Calcula el costo de H para un tile.
    /// </summary>
    static int CalcHCost(TileNode tile)
    {
      Debug.Log(InputController.destination);
      float x = Math.Abs(tile.x - InputController.destination.transform.position.x);
      float z = Math.Abs(tile.z - InputController.destination.transform.position.z);

      // Retona el costo de H para el tile teniendo en cuenta diagonalidad.
      return 10 * (int)Math.Abs(x - z) + 14 * (int)(x > z ? z : x);
    }

    /// <summary>
    /// Reinicia los tiles del mapa y el camino más corto.
    /// </summary>
    public static void ResetTiles()
    {
      path = new();
      TilesController.ResetTiles();
    }

    /// <summary>
    /// Establece los tiles activos con el color rojo para indicar visualmente que no hay camino.
    /// </summary>
    public static void NoPath()
    {
      // foreach (TileNode tile in TilesController.tiles)
      // {
      //   if (tile.closed)
      //   {
      //     tile.GetComponent<Renderer>().material.color = Global.RED;
      //   }
      // }
    }
  }
}