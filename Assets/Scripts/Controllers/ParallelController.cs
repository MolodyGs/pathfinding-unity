using System.Threading.Tasks;
using UnityEngine;

namespace Controllers
{
  public static class ParallelController
  {
    static readonly GameObject pathfindingController = GameObject.Find("PathfindingController");

    public static void Initialize()
    {
      TilesController.AddTileFromScene();
    }

    public static async Task<int> Start()
    {
      return await pathfindingController.GetComponent<PathfindingController>().Path(InputController.origin.transform.position, InputController.destination.transform.position);
    }

    public static async Task<int> Start(Vector3 origin, Vector3 destination)
    {
      int response = await pathfindingController.GetComponent<PathfindingController>().Path(origin, destination);
      TilesController.ResetTiles(true);
      return response;
    }
  }
}
