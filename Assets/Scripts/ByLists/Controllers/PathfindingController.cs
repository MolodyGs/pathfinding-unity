using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Components;

namespace Controllers
{
  /// <summary>
  /// Controlador de Pathfinding para encontrar el camino más corto entre dos puntos mediante el algoritmo A* y por medio de tiles.
  /// </summary>
  public static class PathfindingController
  {
    /// <summary>
    /// Inicia el proceso de Pathfinding para encontrar el camino más corto entre dos puntos.
    /// </summary>
    public static async Task<int> Path()
    {
      // Obtiene el tile de origen.
      TileNode origin = TilesController.Find((int)InputController.origin.transform.position.x, (int)InputController.origin.transform.position.z);

      // Calcula el costo de H para el tile de origen.
      int hCost = CalcHCost(origin);
      origin.SetHCost(hCost);

      // Como el tile de origen es el primer tile, su costo de G es 0 y se añade a la lista de tiles evaluados.
      origin.SetClosed(true);

      // Obtiene el último tile evaluado de forma recursiva. El resultado corresponde al tile final o, si no se encuentra un camino, a null.
      TileNode lastTile = await EvaluateTile(origin);

      if (lastTile == null)
      {
        Debug.Log("No se encontró un camino entre el origen y el destino.");
        return 1;
      }

      // Si el último tile evaluado corresponde al destino, entonces se obtiene el camino más corto.
      await lastTile.SetPath();
      TilesController.SetNewPath(lastTile.x, lastTile.z);
      return 0;
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
      if (bestTile == null) { return null; }

      // Si el mejor tile es el destino, entonces hemos encontrado el camino más corto.
      if (bestTile.x == InputController.destination.transform.position.x && bestTile.z == InputController.destination.transform.position.z)
      {
        Debug.Log("Llegamos al destino");
        return bestTile;
      }

      // Si aún no hemos llegado al destino, establece el mejor tile como activo y lo agrega a la lista de tiles evaluados.
      TilesController.RemoveTileEvaluated(bestTile);

      Debug.Log(" --- Cargando Siguiente Evaluación: " + bestTile.GetPosition());

      // De forma recursiva, se evalua el siguiente tile.
      return await EvaluateTile(bestTile);
    }

    /// <summary>
    /// Encuentra el mejor tile para continuar el camino.
    /// </summary>
    static TileNode FindbestTile()
    {
      TileNode bestTile = null;
      int bestTileCost = 9999;

      Debug.Log("Buscando mejor tile...");
      Debug.Log("Cantidad de tiles abiertos: " + TilesController.openTiles.Count);

      // Itera entre los tiles abiertos y busca el mejor tile para continuar el camino.
      foreach (TileNode tile in TilesController.openTiles)
      {
        Debug.Log("Tile: (" + tile.x + ", " + tile.z + ") Closed: " + tile.GetClosed() + " g: " + tile.g + " h: " + tile.h + " f: " + tile.f);

        // Si el tile ya fue evaluado, entonces omite.
        if (tile.GetClosed()) { continue; }

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

      bestTile.SetClosed(true);

      Debug.Log("El mejor tile: " + bestTile.x + " " + bestTile.z + " g: " + bestTile.g + " h: " + bestTile.h + " f: " + bestTile.f);
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

      // Busca los 8 posibles vecinos de un tile. Comienza desde la esquina inferior izquierda y sigue el sentido de las manecillas del reloj.
      neighbors[0] = FindNeighbor(x - 1, z - 1);
      neighbors[1] = FindNeighbor(x - 1, z);
      neighbors[2] = FindNeighbor(x - 1, z + 1);
      neighbors[3] = FindNeighbor(x, z + 1);
      neighbors[4] = FindNeighbor(x + 1, z + 1);
      neighbors[5] = FindNeighbor(x + 1, z);
      neighbors[6] = FindNeighbor(x + 1, z - 1);
      neighbors[7] = FindNeighbor(x, z - 1);
      Debug.Log(" -- Neighbors Establecidos para: " + tile.x + " " + tile.z);

      // Retorna la lista de vecinos
      return neighbors;
    }

    /// <summary>
    /// Encuentra el vecino de un tile
    /// </summary>
    static TileNode FindNeighbor(int x, int z)
    {
      Debug.Log(" -- Buscando vecino: " + x + " " + z);
      try
      {
        // Obtiene el tile vecino de la lista indexada de tiles
        TileNode node = TilesController.Find(x, z);

        if (node == null)
        {
          Debug.Log(" -- Vecino no encontrado: " + x + " " + z);
          return null;
        }

        // Si el tile vecino no es un tile cerrado, entonces se agrega a la lista de tiles a evaluar.
        if (!node.GetClosed())
        {
          TilesController.AddOpenTile(node);
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
  }
}