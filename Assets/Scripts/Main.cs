using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoBehaviour
{
  public void Start()
  {
    // Comienza el proceso inicial de la escena, inicializando el controlador de pathfinding y el controlador de tiles.
    Controllers.ParallelController.Initialize();
    // Controllers.InputController.origin = Controllers.TilesController.tilesObj[18, 0];
    // GameObject destination;

    // while (true)
    // {
    //   int x = Random.Range(18, 33);
    //   int z = Random.Range(0, 15);

    //   if (x != Controllers.InputController.origin.transform.position.x && z != Controllers.InputController.origin.transform.position.z)
    //   {
    //     destination = Controllers.TilesController.tilesObj[x, z];
    //     if (destination == null) continue;
    //     int response = await Controllers.InputController.SetInput(destination);
    //     if (response == -1) 
    //     {
    //       Debug.Log("Tile bloqueado, selecciona otro.");
    //     }
    //   }
    //   await Task.Delay(1000);
    // }
  }
}