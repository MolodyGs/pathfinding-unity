using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers
{
  public static class InputController
  {
    public static GameObject origin { get; set; }
    public static GameObject destination { get; set; }

    /// <summary>
    /// Asigna el tile seleccionado por el usuario como origen o destino.
    /// </summary>
    public static async Task<int> SetInput(GameObject tile)
    {

      // Si el tile está bloqueado, no se puede seleccionar.
      if (tile.GetComponent<Components.Tile>().blocked) return 2;

      // Si no hay un origen, se asigna el tile seleccionado como origen.
      if (origin == null)
      {
        Debug.Log("Estableciendo origen: " + tile.transform.position);
        origin = tile;
        tile.GetComponent<Renderer>().material.color = Global.GREEN;
        return 0;
      }

      // Si no hay un destino, se asigna el tile seleccionado como destino.
      if (destination == null)
      {
        Debug.Log("Estableciendo destino: " + tile.transform.position);
        if (tile.transform.position.x == origin.transform.position.x && tile.transform.position.z == origin.transform.position.z)
        {
          // Si el destino es el mismo que el origen, no se puede seleccionar.
          Debug.Log("El destino no puede ser el mismo que el origen.");
          return 2;
        }
        destination = tile;
        tile.GetComponent<Renderer>().material.color = Global.BLUE;

        // Se ejecuta el pathfinding y el movimiento.
        Debug.Log("Cargando camino...");
        await PathfindingController.Path();
        return 0;
      }

      // Si ya hay un origen y un destino, se reinician los tiles y se asigna el nuevo tile seleccionado como destino.
      TilesController.ResetTiles();
      origin.GetComponent<Components.Tile>().Reset();
      destination.GetComponent<Components.Tile>().Reset();

      Debug.Log("Estableciendo origen y destino: " + tile.transform.position);
      origin = destination;
      destination = tile;
      tile.GetComponent<Renderer>().material.color = Global.GREEN;
      origin.GetComponent<Renderer>().material.color = Global.BLUE;

      Debug.Log("Cargando camino...");
      return await PathfindingController.Path();
    }

    /// <summary>
    /// Asigna el tile por donde el mouse haya pasado por encima
    /// </summary>
    public static async void SetInputWhenMouseEnter(GameObject tile)
    {
      if (!tile.CompareTag("Tile")) return;
      if (origin == null) return;
      if (tile.GetComponent<Components.Tile>().blocked) return;
      if (tile.transform.position.x == origin.transform.position.x && tile.transform.position.z == origin.transform.position.z) return;

      if (destination == null)
      {
        destination = tile;
        Debug.Log("Estableciendo destino: " + tile.transform.position);

        await PathfindingController.Path();
        return;
      }
      else
      {
        if (destination.transform.position.x == tile.transform.position.x && destination.transform.position.z == tile.transform.position.z) return;

        // Vector3 lastDestination = new(destination.transform.position.x, 0, destination.transform.position.z);
        Debug.Log("Estableciendo destino: " + tile.transform.position);
        destination = tile;
        TilesController.ResetTiles();

        Debug.Log("El camino ya ha sido evaludo??");
        bool alreadyEvaluated = TilesController.IsTheDestinationAlreadyEvaluated((int)tile.transform.position.x, (int)tile.transform.position.z);
        
        if (alreadyEvaluated)
        {
          TilesController.SetPath((int)tile.transform.position.x, (int)tile.transform.position.z);
          Debug.Log("[InputController]: El destino ya ha sido evaluado.");
          return;
        }

        await PathfindingController.Path();
      }
    }
  }
}