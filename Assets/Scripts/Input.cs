
using UnityEngine;

public static class Input
{
  public static GameObject origin { get; set; }
  public static GameObject destination { get; set; }

  public static void setTile(GameObject tile)
  {

    if (tile.GetComponent<Tile>().blocked) return;

    if (origin == null)
    {
      origin = tile;
      tile.GetComponent<Renderer>().material.color = Color.green;
      return;
    }

    if (destination == null)
    {
      destination = tile;
      tile.GetComponent<Renderer>().material.color = Color.blue;
      Pathfinding.Path();
      return;
    }
    Pathfinding.ResetTiles();
    origin.GetComponent<Renderer>().material.color = Color.white;
    destination.GetComponent<Renderer>().material.color = Color.white;
    origin = tile;
    destination = null;
    tile.GetComponent<Renderer>().material.color = Color.green;
  }
}