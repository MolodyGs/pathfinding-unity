using UnityEngine;

/// <summary> 
/// Lectura de la interacción del usuario con los tiles.
/// </summary>
public static class InputController
{
  public static GameObject origin { get; set; }
  public static GameObject destination { get; set; }

  /// <summary>
  /// Asigna el tile seleccionado por el usuario como origen o destino.
  /// </summary>
  public static async void SetTile(GameObject tile)
  {

    // Si el tile está bloqueado, no se puede seleccionar.
    if (tile.GetComponent<Tile>().blocked) return;

    // Si no hay un origen, se asigna el tile seleccionado como origen.
    if (origin == null)
    {
      origin = tile;
      tile.GetComponent<Renderer>().material.color = Global.GREEN;
      return;
    }

    // Si no hay un destino, se asigna el tile seleccionado como destino.
    if (destination == null)
    {
      destination = tile;
      tile.GetComponent<Renderer>().material.color = Global.BLUE;

      // Se ejecuta el pathfinding y el movimiento.
      await PathfindingController.Path();
      await MoveController.Move();
      return;
    }

    // Si ya hay un origen y un destino, se reinician los tiles y se asigna el nuevo tile seleccionado como destino.
    PathfindingController.ResetTiles();
    origin.GetComponent<Tile>().Reset();
    destination.GetComponent<Tile>().Reset();
    origin = destination;
    destination = tile;
    await PathfindingController.Path();
    await MoveController.Move();
    tile.GetComponent<Renderer>().material.color = Global.GREEN;
  }
}