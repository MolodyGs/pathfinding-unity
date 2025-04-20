using System;
using System.Threading.Tasks;
using UnityEngine;
using Components;
using System.Collections;
using System.Diagnostics;
using Global;
using System.Collections.Generic;

namespace Controllers
{
  /// <summary>
  /// Controlador de Pathfinding para encontrar el camino más corto entre dos puntos mediante el algoritmo A* y por medio de tiles.
  /// Es necesario que PathfindingController herede de MonoBehaviour para poder utilizar corrutinas y tareas asincrónicas.
  /// </summary>
  public class PathfindingController : MonoBehaviour
  {
    AlertController alertController;
    PriorityQueue<TileNode> openTiles;
    private TileNode lastTileReference;
    private TileNode destinationReference;
    private bool isRunning;
    private List<TileNode> path;

    /// <summary>
    /// Antesala del método StartPath, que inicia el proceso de Pathfinding para encontrar el camino más corto entre dos puntos.
    /// </summary>
    public async Task<int> Path(TileNode origin, TileNode destination)
    {
      
      InitialState();

      // Establece el destino como una variable global.
      this.destinationReference = destination;

      // Comienza el proceso de pathfinding
      isRunning = true;

      await RunCoroutineAsTask(StartPath(origin, destination));

      // Oculta cualquier alterta que se esté mostrando.
      alertController.Hide();

      isRunning = false;

      return 0;
    }

    /// <summary>
    /// Inicia el proceso de Pathfinding para encontrar el camino más corto entre dos puntos.
    /// </summary>
    IEnumerator StartPath(TileNode origin, TileNode destination)
    {

      alertController.ShowLoadingMessage();
      yield return null;

      UnityEngine.Debug.Log("Desde Pathfinding");
      UnityEngine.Debug.Log("origin Tile: " + origin);
      UnityEngine.Debug.Log("destination Tile: " + destination);

      if (origin == null || destination == null)
      {
        UnityEngine.Debug.Log("No se encontró el tile de origen o destino.");
        yield break;
      }

      // El tile de origen es el tile que se va a evaluar primero, por lo que se establece como cerrado.
      origin.SetClosed(true);

      // Calcula el costo de H para el tile de origen.
      int hCost = CalcHCost(origin);
      origin.SetHCost(hCost);

      // Comienza un cronómetro para medir el tiempo de evaluación.
      Stopwatch stopwatch = new();
      stopwatch.Start();

      // Determina si debe realizar pausas entre cada iteración Según el valor de VISUAL_PATHFINDING.
      if (Settings.VISUAL_PATHFINDING.value)
      {
        // Comienza el proceso de evaluación de tiles por intervalos de tiempo.
        yield return StartCoroutine(VisualEvaluateTile(origin));
      }
      else
      {
        // Comienza el proceso de evaluación sin intervalos de tiempo.
        DefaultEvaluateTile(origin);
      }

      // Detiene el cronómetro y muestra el tiempo de evaluación.
      stopwatch.Stop();
      UnityEngine.Debug.Log("[Time] Tiempo de evaluación: " + stopwatch.ElapsedMilliseconds + "ms");

      // Si el último tile evaluado es nulo, entonces no se encontró un camino entre el origen y el destino.
      if (lastTileReference == null)
      {
        UnityEngine.Debug.Log("No se encontró un camino entre el origen y el destino.");
        alertController.ShowNoPathMessage();
        yield break;
      }

      // Si el último tile evaluado corresponde al destino, entonces se obtiene el camino más corto.
      UnityEngine.Debug.Log("Estableciendo el camino encontrado: " + lastTileReference.x + " " + lastTileReference.z);
      lastTileReference.SetPath(path);

      yield break;
    }

    /// <summary>
    /// Evalua un nuevo tile activo, buscando el mejor camino entre los vecinos del tile activo. Realiza pausas entre cada iteración.
    /// </summary>
    IEnumerator VisualEvaluateTile(TileNode tile)
    {
      while (true)
      {
        tile.SetPlateColor(Colors.BLUE);
        TileNode bestTile = EvaluateTile(tile);

        // Si el mejor tile es nulo, entonces no se encontró un camino entre el origen y el destino.
        if (bestTile == null) break;

        // Si el mejor tile es el destino, entonces hemos encontrado el camino más corto.
        if (bestTile.x == destinationReference.x && bestTile.z == destinationReference.z)
        {
          UnityEngine.Debug.Log("Llegamos al destino");
          lastTileReference = bestTile;
          break;
        }

        // Se establece el mejor tile
        tile = bestTile;

        // Evita que al presionar la barra espaciadora se evaluen más de un nodo. Espera 0.05 segundos entre cada iteración.
        yield return new WaitForSeconds(0.05f);

        // Si STEPS está activado, entonces espera a que el usuario presione la barra espaciadora para continuar.
        if (Settings.STEPS.value)
        {
          UnityEngine.Debug.Log("Esperando a que se presione la barra espaciadora...");
          yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
      }

      // Al finalizar la búsqueda, se oculta el mensaje de carga.
      alertController.Hide();
    }

    /// <summary>
    /// Evalua un nuevo tile activo, buscando el mejor camino entre los vecinos del tile activo.
    /// </summary>
    void DefaultEvaluateTile(TileNode tile)
    {
      TileNode bestTile;
      while (true)
      {
        bestTile = EvaluateTile(tile);

        // Si el mejor tile es nulo, entonces no se encontró un camino entre el origen y el destino.
        if (bestTile == null) break;

        // Si el mejor tile es el destino, entonces hemos encontrado el camino más corto.
        if (bestTile.x == destinationReference.x && bestTile.z == destinationReference.z)
        {
          UnityEngine.Debug.Log("Llegamos al destino");
          lastTileReference = bestTile;
          break;
        }

        // Se establece el mejor tile
        tile = bestTile;
      }
    }

    TileNode EvaluateTile(TileNode tile)
    {
      tile.SetClosed(true);
      UnityEngine.Debug.Log("Cerrando TILE: " + tile.x + " " + tile.z + " closed: " + tile.GetClosed());
      UnityEngine.Debug.Log(" - Evaluando Tile: " + tile.x + " " + tile.z);

      // Itera entre los vecinos y evalua el costo de G y H de cada vecino.
      EvaluateNeighborsCost(tile.neighbors, tile);

      // Busca el mejor tile para continuar el camino.
      TileNode bestTile = openTiles.Dequeue();

      // Si no se encuentra un mejor camino, entonces hemos evaluado todos los caminos sin encontrar el destino.
      if (bestTile == null) { return null; }

      UnityEngine.Debug.Log(" --- Cargando Siguiente Evaluación: " + bestTile.GetPosition() + " closed: " + bestTile.GetClosed());

      // De forma recursiva, se evalua el siguiente tile.
      return bestTile;
    }

    /// <summary>
    /// Itera entre los vecinos de un tile y evalua el costo, comprobando si el tile activo es un mejor padre que el padre actual de los tiles vecino.
    /// </summary>
    void EvaluateNeighborsCost(TileNode[] neighbors, TileNode tile)
    {

      // Itera entre los vecinos y evalua el costo de G y H de cada vecino.
      for (int i = 0; i < neighbors.Length; i++)
      {
        // Almacena el vecino actual.
        TileNode neighbor = neighbors[i];

        // Si el vecino es nulo o está bloqueado, entonces se omite.
        if (neighbor == null || neighbor.GetBlockedState()) { continue; }

        // Obtiene la distancia entre los tiles. Generalmente estos tiles están a 1 o raiz de 2 de distancia, esto ya que son tiles adyacentes los evaluados.
        float distance = Vector2.Distance(neighbor.GetPosition(), tile.GetPosition());

        // Calcula el costo de G para el tile vecino.
        int gCost = distance > 1 ? 14 : 10;

        // Si el tile corresponde a un movimiento diagonal, entonces se evalua si tiene tiles adyacentes bloqueados o inexistentes.
        if (gCost == 14)
        {
          if (HaveAdjacentTiles(tile, neighbors, i)) { continue; }
        }

        // Suma el costo de G del tile activo al costo de G del tile vecino para obtener el costo G total
        gCost += tile.g;
        UnityEngine.Debug.Log("Distance entre tile activo:" + tile.GetPosition() + " y vecino: " + neighbor.GetPosition() + ": " + distance + " gCost: " + gCost + " neighbor gCost: " + neighbor.g);

        // Si el tile tiene un cost F igual a 0, entonces esta es la primera vez que se evalua el tile.
        // Si el nuevo costo es menor al costo actual del tile vecino, entonces se actualiza el costo de G y H.
        if (neighbor.f == 0 || gCost < neighbor.g)
        {
          UnityEngine.Debug.Log(" -- Costo actualizado para " + neighbor.GetPosition() + " con el origen: " + tile.GetPosition() + " gcost: " + tile.g);
          neighbor.SetGCost(gCost);
          neighbor.SetHCost(CalcHCost(neighbor));
          neighbor.parent = tile;
        }

        UnityEngine.Debug.Log(" --- Costo para " + neighbor.GetPosition() + ": gCost: " + neighbor.g + " hCost: " + neighbor.h + " fCost: " + neighbor.f);

        if (Settings.VISUAL_PATHFINDING.value)
        {
          neighbor.SetPlate(true);
          if (!neighbor.GetClosed()) neighbor.SetPlateColor(Colors.YELLOW);
        }

        if (!neighbor.GetClosed())
        {
          AddOpenTile(neighbor);
        }
      }
    }

    /// <summary>
    /// Comprueba que tile no tenga tiles adyacentes que bloqueen el movimiento.
    /// Por como está ordenado la lista de vecinos, los tiles i + 1 e i - 1 son los tiles derecho e izquierdo respectivamente.
    /// </summary>
    bool HaveAdjacentTiles(TileNode tile, TileNode[] neighbors, int i)
    {
      TileNode left = null;
      TileNode right = null;

      // Si el tile es un movimiento diagonal, entonces se evalua si tiene tiles adyacentes bloqueados o inexistentes.
      try
      {
        // Caso especial donde el nodo a evaluar es justamente el primero de la lista (izquieda inferior).
        if (i == 0)
        {
          left = neighbors[^1];
          right = neighbors[i + 1];

        }
        else if (i > 1)
        {
          left = neighbors[i - 1];
          right = neighbors[i + 1];
        }

        bool leftBlocked = false;
        bool rightBlocked = false;

        // Si el tile izquierdo o derecho no es nulo, entonces se evalua si está bloqueado. Caso contrario, se establece como bloqueado por ser nulo (es un hueco).
        if (left != null) leftBlocked = left.GetBlockedState();
        else leftBlocked = true;

        if (right != null) rightBlocked = right.GetBlockedState();
        else rightBlocked = true;

        // Si ambos tiles adyacentes están bloqueados, entonces se omite el tile vecino.
        if (rightBlocked && leftBlocked)
        {
          UnityEngine.Debug.Log("Omitiendo tile ya que tiene tiles bloqueados adyacentes " + neighbors[i].GetPosition() + " " + tile.GetPosition());
          return true;
        }
        else
        {
          // Si alguno de los tiles adyacentes no está bloqueado, entonces el movimiento no está bloqueado.
          return false;
        }
      }
      catch
      {
        // Cuando los tiles siguiente o anterior son nulos, entonces uno de ellos 2 son un hueco en el mapa.
        UnityEngine.Debug.Log("No se pudo obtener los vecinos adyacentes: " + neighbors[i].GetPosition() + " " + tile.GetPosition());
        return true;
      }
    }

    /// <summary>
    /// Calcula el costo de H para un tile.
    /// </summary>
    int CalcHCost(TileNode tile)
    {
      UnityEngine.Debug.Log(destinationReference);
      UnityEngine.Debug.Log(tile);
      float x = Math.Abs(tile.x - destinationReference.x);
      float z = Math.Abs(tile.z - destinationReference.z);

      // Retona el costo de H para el tile teniendo en cuenta diagonalidad.
      return 10 * (int)Math.Abs(x - z) + 14 * (int)(x > z ? z : x);
    }

    // Añade un nuevo tile a la lista de tiles abiertos.
    void AddOpenTile(TileNode tile)
    {
      UnityEngine.Debug.Log("Añadiendo tile a la lista de tiles abiertos: " + tile.x + " " + tile.z + " closed: " + tile.GetClosed());

      // Si el tile ya fue agregado, se evita volver a agregarlo.
      if (tile.isOpen) return;

      // Se añade el tile a la lista
      openTiles.Enqueue(tile, new int[] { tile.f, tile.g });

      // Se establece el tile como agregado para evitar volver a agregarlo.
      tile.isOpen = true;
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
    /// Inicializa el estado del controlador de pathfinding, estableciendo los valores iniciales de las variables.
    /// </summary>
    void InitialState()
    {
      openTiles = new();
      lastTileReference = null;
      destinationReference = new TileNode(0, 0, false, null);
      isRunning = false;
      alertController = GameObject.Find("Controllers").GetComponent<AlertController>();
      path = new List<TileNode>();
      TilesController.ResetTiles();
    }

    /// <summary>
    /// Verifica si el controlador de pathfinding está en ejecución. Evita que se pueda volver a ejecutar el pathfinding mientras se está ejecutando.
    /// </summary>
    public bool IsRunning() { return isRunning; }
    public List<TileNode> GetPath() { return path; }
  }
}