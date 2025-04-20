using System.Threading.Tasks;
using UnityEngine;
using Components;

namespace Controllers
{
  public static class InputController
  {
    public static TileNode origin { get; set; }
    public static TileNode destination { get; set; }
    public static TileNode AuxOrigin { get; set; }
    public static TileNode AuxDestination { get; set; }

    /// <summary>
    /// Asigna el tile seleccionado por el usuario como origen o destino.
    /// </summary>
    public static async Task<int> SetInput(TileNode destinationTile)
    {
      int response;

      if (ParallelController.isRunning) return 1; // Si el pathfinding está en ejecución, no se puede seleccionar otro tile.
      if (TilesController.Find(destinationTile.x, destinationTile.z) == null) return 1; // Si el tile no existe, no se puede seleccionar.
      if (TilesController.Find(destinationTile.x, destinationTile.z).GetBlockedState()) return 2; // Si el tile está bloqueado, no se puede seleccionar.

      // Si el tile está bloqueado, no se puede seleccionar.
      if (destinationTile.GetBlockedState()) return 2;

      TileNode player = TilesController.GetPlayerTile(); // Se obtiene el tile del jugador.

      origin = TilesController.Find((int)player.Position().x, (int)player.Position().z);

      Debug.Log("Estableciendo destino: " + destinationTile.Position());
      if (destinationTile.x == origin.x && destinationTile.z == origin.z)
      {
        // Si el destino es el mismo que el origen, no se puede seleccionar.
        Debug.Log("El destino no puede ser el mismo que el origen.");
        return 2;
      }

      // Se ejecuta el pathfinding y el movimiento.
      Debug.Log("Cargando camino...");
      response = await ParallelController.Start(origin, destinationTile);
      if (response == 0)
      {
        TilesController.SetPlayerTile(destinationTile);
        await TurnController.FinishPlayerTurn(response);
      }
      return response;
    }

    /// <summary>
    /// Asigna el tile por donde el mouse haya pasado por encima
    /// </summary>
    public static async void SetInputWhenMouseEnter(TileNode tile)
    {
      if (ParallelController.pathfindingController.IsRunning()) return; // Si el pathfinding está en ejecución, no se puede seleccionar otro tile.
      if (!tile.obj.CompareTag("Tile")) return;
      if (origin == null) return;
      if (tile.GetBlockedState()) return;

      // Comprueba que el tile no sea el mismo que el origen
      if (tile.x == origin.x && tile.z == origin.z) return;


      // Si no existe un destino, se asigna el tile seleccionado como destino.
      if (destination == null)
      {
        destination = tile;
        Debug.Log("Estableciendo destino: " + tile.Position());

        await ParallelController.Start(origin, destination);
        return;
      }
      else
      {
        // Comprueba que el nuevo tile de destino no sea el mismo que el destino actual
        if (destination.x == tile.x && destination.z == tile.z) return;

        Debug.Log("Estableciendo destino: " + tile.Position());
        destination = tile;

        await ParallelController.Start(origin, destination);
      }
    }
  }
}