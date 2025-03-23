using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public static class Pathfinding
{
  static GameObject[] tiles;

  public static void SetTiles(GameObject[] tiles)
  {
    Pathfinding.tiles = tiles;
  }

  public static async void Path()
  {
    List<GameObject> activeTiles;

    activeTiles = new List<GameObject>
    {
      Input.origin
    };
    Input.origin.GetComponent<Tile>().active = true;

    int hCost = CalcHCost(Input.origin.transform.position);

    Input.origin.GetComponent<Tile>().SetGCost(0);
    Input.origin.GetComponent<Tile>().SetHCost(hCost);

    Debug.Log("hCost: " + hCost);
    Debug.Log("Total" + Input.origin.GetComponent<Tile>().fCost);

    GameObject lastTile = await EvaluateTile(activeTiles, 0);

    lastTile.GetComponent<Tile>().SetPath();
  }

  static async Task<GameObject> EvaluateTile(List<GameObject> activeTiles, int lastActiveTile)
  {
    Debug.Log(" - Evaluando Tile: " + activeTiles[lastActiveTile].transform.position);
    GameObject[] neighbors = new GameObject[8];
    SetNeighbors(neighbors, activeTiles[lastActiveTile]);

    for (int i = 0; i < neighbors.Length; i++)
    {
      if (neighbors[i] != null && !neighbors[i].GetComponent<Tile>().blocked)
      {
        EvaluateCost(neighbors[i], activeTiles[lastActiveTile]);
      }
    }

    GameObject betterTile = null;
    int betterTileCost = 9999;

    foreach (GameObject tile in tiles)
    {

      if (tile.GetComponent<Tile>().active)
      {
        continue;
      }

      int fcost = tile.GetComponent<Tile>().fCost;
      if (fcost > 0 && fcost < betterTileCost)
      {
        betterTile = tile;
        betterTileCost = fcost;
      }
    }

    if (betterTile != null)
    {

      if (betterTile.transform.position == Input.destination.transform.position)
      {
        Debug.Log("Llegamos al destino");
        return betterTile;
      }

      activeTiles.Add(betterTile);
      betterTile.GetComponent<Renderer>().material.color = Color.green;
      betterTile.GetComponent<Tile>().active = true;
      Debug.Log(" --- Cargando Siguiente Evaluación: " + betterTile.transform.position);
      await Wait(1000);
      return await EvaluateTile(activeTiles, lastActiveTile + 1);
    }
    else
    {
      Debug.LogError("No hay camino");
      return null;
    }
  }
  async static Task Wait(int milisegundos)
  {
    await Task.Delay(milisegundos);
  }
  static void EvaluateCost(GameObject neighbor, GameObject origin)
  {
    float distance = Vector3.Distance(neighbor.transform.position, origin.transform.position);
    neighbor.GetComponent<Renderer>().material.color = neighbor.GetComponent<Tile>().active ? Color.green : Color.yellow;
    int gCost = neighbor.GetComponent<Tile>().gCost + distance > 1 ? 14 : 10;
    gCost += origin.GetComponent<Tile>().gCost;
    Debug.Log("Distance: " + distance + " gCost: " + gCost + " neighbor gCost: " + neighbor.GetComponent<Tile>().gCost);

    if (neighbor.GetComponent<Tile>().fCost == 0 || gCost < neighbor.GetComponent<Tile>().gCost)
    {
      neighbor.GetComponent<Tile>().SetGCost(gCost);
      neighbor.GetComponent<Tile>().SetHCost(CalcHCost(neighbor.transform.position));
      neighbor.GetComponent<Tile>().parent = origin;
    }
    Debug.Log(" --- Costo para " + neighbor.transform.position + ": gCost: " + neighbor.GetComponent<Tile>().gCost + " hCost: " + neighbor.GetComponent<Tile>().hCost + " fCost: " + neighbor.GetComponent<Tile>().fCost);
  }

  static void SetNeighbors(GameObject[] neighbors, GameObject tile)
  {
    int x = (int)tile.transform.position.x;
    int z = (int)tile.transform.position.z;

    neighbors[0] = Findneighbor(tiles, new Vector3(x - 1, 0, z - 1));
    neighbors[1] = Findneighbor(tiles, new Vector3(x - 1, 0, z));
    neighbors[2] = Findneighbor(tiles, new Vector3(x - 1, 0, z + 1));
    neighbors[3] = Findneighbor(tiles, new Vector3(x, 0, z + 1));
    neighbors[4] = Findneighbor(tiles, new Vector3(x + 1, 0, z + 1));
    neighbors[5] = Findneighbor(tiles, new Vector3(x + 1, 0, z));
    neighbors[6] = Findneighbor(tiles, new Vector3(x + 1, 0, z - 1));
    neighbors[7] = Findneighbor(tiles, new Vector3(x, 0, z - 1));
    Debug.Log(" -- Neighbors Establecidos para: " + tile.transform.position);
  }

  static GameObject Findneighbor(GameObject[] list, Vector3 tilePosition)
  {
    Debug.Log(" -- Buscando vecino: " + tilePosition);
    try
    {
      for (int i = 0; i < list.Length; i++)
      {
        if (list[i].transform.position.x == tilePosition.x && list[i].transform.position.z == tilePosition.z)
        {
          Debug.Log(" -- Vecino encontrado: " + list[i].transform.position);
          return list[i];
        }
      }
      Debug.Log(" -- Vecino no encontrado: " + tilePosition);
    }
    catch (Exception e)
    {
      foreach (GameObject tile in list)
      {
        Debug.Log(" -- Tile: " + tile);
      }
      Debug.LogError("Error al buscar vecino: " + e.Message);
    }
    return null;
  }

  static int CalcHCost(Vector3 tile)
  {
    int hCost = 10 * (int)(Math.Abs(tile.x - Input.destination.transform.position.x) + Math.Abs(tile.z - Input.destination.transform.position.z));
    return hCost;
  }

  public static void ResetTiles()
  {
    foreach (GameObject tile in tiles)
    {
      tile.GetComponent<Tile>().Reset();
    }
  }

}
