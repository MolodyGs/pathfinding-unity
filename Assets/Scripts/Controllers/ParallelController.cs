using System.Threading.Tasks;
using UnityEngine;
using Components;
using System.Collections.Generic;

namespace Controllers
{
  /// <summary>
  /// Controla el proceso de pathfinding en la escena, inicializando el reconocimiento de tiles y ejecutando el algoritmo A*.
  /// Permite iniciar el pathfinding desde el origen y el destino de forma asincrónica. 
  /// </summary>
  public static class ParallelController
  {
    // Se obtiene el objeto PathfindingController de la escena.
    public static PathfindingController pathfindingController = GameObject.Find("PathfindingController").GetComponent<PathfindingController>();
    private static readonly MovementController movementController = GameObject.Find("Controllers").GetComponent<MovementController>();
    public static GameObject entity;
    public static bool isRunning = false;

    /// <summary>
    /// Comienza el pathfinding teniendo como referencia al origen y destino establecido en InputController
    /// </summary>
    // public static async Task<int> Start()
    // {
    //   isRunning = true;
    //   // Siempre que se usa este método lo hacemos desde jugador.
    //   int response = await pathfindingController.Path(InputController.AuxOrigin, InputController.AuxDestination);
    //   if (response == 0)
    //   {
    //     await movementController.Movement(entity, pathfindingController.GetPath());
    //   }
    //   else
    //   {
    //     Debug.Log("No se ha encontrado un camino.");
    //     await TurnController.FinishPlayerTurn(response);
    //     return -1;
    //   }
    //   await TurnController.FinishPlayerTurn(response);
    //   isRunning = false;
    //   return 0;
    // }

    /// <summary>
    /// Comienza el pathfinding teniendo como referencia al origen y destino obtenido como parámetros
    /// </summary>
    public static async Task<int> Start(TileNode origin, TileNode destination)
    {
      isRunning = true; 
      Debug.Log("Desde ParallelController");
      Debug.Log("Origen: " + origin);
      Debug.Log("Destino: " + destination);
      int response = await pathfindingController.Path(origin, destination);
      // isRunning = false;
      return response;
    }
  }
}
