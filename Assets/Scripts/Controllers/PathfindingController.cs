using System;
using System.Threading.Tasks;
using UnityEngine;
using Components;
using System.Collections;

namespace Controllers
{
  /// <summary>
  /// Controlador de Pathfinding para encontrar el camino más corto entre dos puntos mediante el algoritmo A* y por medio de tiles.
  /// Es necesario que PathfindingController herede de MonoBehaviour para poder utilizar corrutinas y tareas asincrónicas.
  /// </summary>
  public class PathfindingController : MonoBehaviour
  {
    // List<TileNode> openTiles = new List<TileNode>();
    PriorityQueue<TileNode> openTiles = new PriorityQueue<TileNode>();
    private TileNode lastTileReference = null;
    private Vector3 destinationPosition;

    /// <summary>
    /// Inicia el proceso de Pathfinding para encontrar el camino más corto entre dos puntos.
    /// </summary>
    public async Task<int> Path(Vector3 originPosition, Vector3 destinationPosition)
    {
      this.destinationPosition = destinationPosition;

      // Obtiene el tile de origen.
      TileNode origin = TilesController.Find((int)originPosition.x, (int)originPosition.z);
      TileNode destination = TilesController.Find((int)destinationPosition.x, (int)destinationPosition.z);

      if (origin == null || destination == null)
      {
        Debug.Log("No se encontró el tile de origen o destino.");
        return 1;
      }

      // Se establecen los tiles abiertos como vacíos.
      openTiles = new();

      // Calcula el costo de H para el tile de origen.
      int hCost = CalcHCost(origin);
      origin.SetHCost(hCost);

      // Comienza la evaluación del tile de origen y, de la misma manera, la evaluación de los demás tiles.
      await RunCoroutineAsTask(EvaluateTile(origin));

      // Si el último tile evaluado es nulo, entonces no se encontró un camino entre el origen y el destino.
      if (lastTileReference == null)
      {
        Debug.Log("No se encontró un camino entre el origen y el destino.");
        return 1;
      }

      // Si el último tile evaluado corresponde al destino, entonces se obtiene el camino más corto.
      Debug.Log("Estableciendo el camino encontrado: " + lastTileReference.x + " " + lastTileReference.z);
      await lastTileReference.SetPath(new());
      return 0;
    }

    /// <summary>
    /// Comienza una corrutina y la convierte en una tarea.
    /// </summary>
    Task RunCoroutineAsTask(IEnumerator coroutine)
    {
      var tcs = new TaskCompletionSource<bool>();
      StartCoroutine(WaitForCompletion(coroutine, tcs));
      return tcs.Task;
    }

    IEnumerator WaitForCompletion(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
    {
      yield return StartCoroutine(coroutine);
      tcs.SetResult(true);
    }

    /// <summary>
    /// Evalua un nuevo tile activo, buscando el mejor camino entre los vecinos del tile activo.
    /// </summary>
    IEnumerator EvaluateTile(TileNode tile)
    {
      while (true)
      {
        tile.SetClosed(true);
        Debug.Log("Cerrando TILE: " + tile.x + " " + tile.z + " closed: " + tile.GetClosed());
        Debug.Log(" - Evaluando Tile: " + tile.x + " " + tile.z);

        // Lista de vecinos
        TileNode[] neighbors = new TileNode[8];

        // Establece los vecinos
        neighbors = SetNeighbors(neighbors, tile);

        // Itera entre los vecinos y evalua el costo de G y H de cada vecino.
        EvaluateNeighborsCost(neighbors, tile);

        // Busca el mejor tile para continuar el camino.
        TileNode bestTile = openTiles.Dequeue();

        // Si no se encuentra un mejor camino, entonces hemos evaluado todos los caminos sin encontrar el destino.
        if (bestTile == null) { break; }

        // Si el mejor tile es el destino, entonces hemos encontrado el camino más corto.
        if (bestTile.x == destinationPosition.x && bestTile.z == destinationPosition.z)
        {
          Debug.Log("Llegamos al destino");
          lastTileReference = bestTile;
          break;
        }

        // Si aún no hemos llegado al destino, establece el mejor tile como activo y lo agrega a la lista de tiles evaluados.
        // RemoveTileEvaluated(bestTile);

        Debug.Log(" --- Cargando Siguiente Evaluación: " + bestTile.GetPosition() + " closed: " + bestTile.GetClosed());

        // Espera una momento para dar la ilusión de una animación
        yield return new WaitForSeconds(0.01f); // 10ms

        // De forma recursiva, se evalua el siguiente tile.
        tile = bestTile;
      }
    }

    /// <summary>
    /// Itera entre los vecinos de un tile y evalua el costo, comprobando si el tile activo es un mejor padre que el padre actual de los tiles vecino.
    /// </summary>
    void EvaluateNeighborsCost(TileNode[] neighbors, TileNode tile)
    {
      // Itera entre los vecinos y evalua el costo de G y H de cada vecino.
      for (int i = 0; i < neighbors.Length; i++)
      {
        TileNode neighbor = neighbors[i];

        // Si el vecino es nulo o está bloqueado, entonces se omite.
        if (neighbor == null || neighbor.blocked) { continue; }

        // Obtiene la distancia entre los tiles. Generalmente estos tiles están a 1 o raiz de 2 de distancia, esto ya que son tiles adyacentes los evaluados.
        float distance = Vector2.Distance(neighbor.GetPosition(), tile.GetPosition());

        // Calcula el costo de G para el tile vecino.
        int gCost = distance > 1 ? 14 : 10;

        // Si el tile corresponde a un movimiento diagonal, entonces se evalua si tiene tiles adyacentes bloqueados o inexistentes.
        if (gCost == 14)
        {
          try
          {
            // Si al obtener el tile anterior o el siguiente, estos son nulos o están bloqueados, entonces se omite el tile en cuestión.
            if (neighbors[i - 1].blocked && neighbors[i + 1].blocked)
            {
              Debug.Log("Omitiendo tile ya que tiene tiles bloqueados adyacentes " + neighbor.GetPosition() + " " + tile.GetPosition());
              continue;
            }
          }
          catch
          {
            // Cuando los tiles siguiente o anterior son nulos, entonces uno de ellos 2 son un hueco en el mapa.
            Debug.Log("No se pudo obtener los vecinos adyacentes: " + neighbor.GetPosition() + " " + tile.GetPosition());
            continue;
          }
        }

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

        neighbor.SetPlate(true);
        neighbor.SetPlateColor(Global.YELLOW);

        if (!neighbor.GetClosed())
        {
          AddOpenTile(neighbor);
        }
      }
    }

    /// <summary>
    /// Encuentra los vecinos de un tile y los establece en un arreglo.
    /// </summary>
    TileNode[] SetNeighbors(TileNode[] neighbors, TileNode tile)
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
    TileNode FindNeighbor(int x, int z)
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
    int CalcHCost(TileNode tile)
    {
      Debug.Log(destinationPosition);
      Debug.Log(tile);
      float x = Math.Abs(tile.x - destinationPosition.x);
      float z = Math.Abs(tile.z - destinationPosition.z);

      // Retona el costo de H para el tile teniendo en cuenta diagonalidad.
      return 10 * (int)Math.Abs(x - z) + 14 * (int)(x > z ? z : x);
    }

    // Añade un nuevo tile a la lista de tiles abiertos.
    void AddOpenTile(TileNode tile)
    {
      Debug.Log("Añadiendo tile a la lista de tiles abiertos: " + tile.x + " " + tile.z + " closed: " + tile.GetClosed());

      // Si el tile ya fue agregado, se evita volver a agregarlo.
      if (tile.isAdded) return;

      // Se añade el tile a la lista
      openTiles.Enqueue(tile, tile.f);

      // Se establece el tile como agregado para evitar volver a agregarlo.
      tile.isAdded = true;
    }
  }
}