using System.Threading.Tasks;
using UnityEngine;

namespace Controllers
{
  /// <summary>
  /// Controla el proceso de pathfinding en la escena, inicializando el reconocimiento de tiles y ejecutando el algoritmo A*.
  /// Permite iniciar el pathfinding desde el origen y el destino de forma asincrónica. 
  /// </summary>
  public static class ParallelController
  {
    // Se obtiene el objeto PathfindingController de la escena.
    static readonly GameObject pathfindingController = GameObject.Find("PathfindingController");

    /// <summary>
    /// Comienza el pathfinding teniendo como referencia al origen y destino establecido en InputController
    /// </summary>
    public static async Task<int> Start()
    {
      return await pathfindingController.GetComponent<PathfindingController>().Path(InputController.origin.transform.position, InputController.destination.transform.position);
    }

    /// <summary>
    /// Comienza el pathfinding teniendo como referencia al origen y destino obtenido como parámetros
    /// </summary>
    public static async Task<int> Start(Vector3 origin, Vector3 destination)
    {
      int response = await pathfindingController.GetComponent<PathfindingController>().Path(origin, destination);
      TilesController.ResetTiles(true);
      return response;
    }
  }
}
