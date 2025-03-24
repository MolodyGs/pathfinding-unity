
using UnityEngine;

public static class InputController
{
  public static GameObject origin { get; set; }
  public static GameObject destination { get; set; }

  public static async void SetTile(GameObject tile)
  {

    if (tile.GetComponent<Tile>().blocked) return;

    if (origin == null)
    {
      origin = tile;
      tile.GetComponent<Renderer>().material.color = Global.GREEN;
      return;
    }

    if (destination == null)
    {
      destination = tile;
      tile.GetComponent<Renderer>().material.color = Global.BLUE;
      await PathfindingController.Path();
      await MoveController.Move();
      return;
    }
    PathfindingController.ResetTiles();
    origin.GetComponent<Renderer>().material.color = Global.WHITE;
    destination.GetComponent<Renderer>().material.color = Global.WHITE;
    origin = destination;
    destination = tile;
    await PathfindingController.Path();
    await MoveController.Move();
    tile.GetComponent<Renderer>().material.color = Global.GREEN;
  }
}