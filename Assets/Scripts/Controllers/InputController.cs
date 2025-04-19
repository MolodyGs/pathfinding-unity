using System.Threading.Tasks;
using UnityEngine;
using Global;
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
      if (TilesController.Find(destinationTile.x, destinationTile.z).blocked) return 2; // Si el tile está bloqueado, no se puede seleccionar.

      // Si el tile está bloqueado, no se puede seleccionar.
      if (destinationTile.blocked) return 2;

      TileNode player = TilesController.GetPlayerTile(); // Se obtiene el tile del jugador.

      origin = TilesController.Find((int)player.Position().x, (int)player.Position().z);

      // // Si no hay un origen, se asigna el tile seleccionado como origen.
      // if (origin == null)
      // {
      //   Debug.Log("Estableciendo origen: " + tile.Position());
      //   origin = tile;
      //   AuxOrigin = tile;
      //   tile.SetRenderarColorAsGreen();
      //   return 0;
      // }

      // Si no hay un destino, se asigna el tile seleccionado como destino.
      // if (destination == null)
      {
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
          destinationTile.blocked = true;
          origin.blocked = false;
          TilesController.SetPlayerTile(destinationTile);
          await TurnController.FinishPlayerTurn(response);
        }
        return response;
        // // Reinicia los tiles para evitar que se mantengan los caminos anteriores
        // TilesController.ResetTiles();
        // origin.Reset();
        // destination.ResetRendererColor();

        // Debug.Log("Estableciendo origen y destino: " + tile.Position());
        // AuxOrigin = destination;
        // AuxDestination = tile;

        // AuxOrigin.SetRenderarColorAsGreen();
        // AuxDestination.SetRenderarColorAsBlue();

        // Debug.Log("Cargando camino...");
        // response = await ParallelController.Start();
        // if (response == 0)
        // {
        //   destination = AuxDestination;
        //   origin = AuxOrigin;
        // }
        // return response;
      }

      /// <summary>
      /// Asigna el tile por donde el mouse haya pasado por encima
      /// </summary>
      // public static async void SetInputWhenMouseEnter(GameObject tile)
      // {
      //   if (!tile.CompareTag("Tile")) return;
      //   if (origin == null) return;
      //   if (tile.GetComponent<Components.Tile>().blocked) return;

      //   // Comprueba que el tile no sea el mismo que el origen
      //   if (tile.transform.position.x == origin.transform.position.x && tile.transform.position.z == origin.transform.position.z) return;


      //   // Si no existe un destino, se asigna el tile seleccionado como destino.
      //   if (destination == null)
      //   {
      //     destination = tile;
      //     Debug.Log("Estableciendo destino: " + tile.transform.position);

      //     await ParallelController.Start();
      //     return;
      //   }
      //   else
      //   {
      //     // Comprueba que el nuevo tile de destino no sea el mismo que el destino actual
      //     if (destination.transform.position.x == tile.transform.position.x && destination.transform.position.z == tile.transform.position.z) return;

      //     Debug.Log("Estableciendo destino: " + tile.transform.position);
      //     destination = tile;

      //     // Reinicia los tiles para evitar que se mantengan los caminos anteriores
      //     TilesController.ResetTiles();

      //     await ParallelController.Start();
      //   }
      // }
    }
  }
}