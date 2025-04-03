using System.Threading.Tasks;
using UnityEngine;

namespace Controllers
{
  public static class ParallelController
  {
    static PathfindingController pathfindingController;

    public static void Initialize()
    {
      TilesController.AddTileFromScene();
      pathfindingController = new();
    }

    public static async Task<int> Start()
    {
      await pathfindingController.Path(InputController.origin.transform.position, InputController.destination.transform.position);
      return 0;
    }

    public static async void Start(Vector3 origin, Vector3 destination)
    {
      await pathfindingController.Path(origin, destination);
      TilesController.ResetTiles(true);
    }
  }
}
