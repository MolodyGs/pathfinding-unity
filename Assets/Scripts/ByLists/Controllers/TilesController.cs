using System.Collections.Generic;
using System.Net.Http.Headers;
using Components;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers
{
  /// <summary>
  /// Controla la creación y lectura de los tiles del juego.
  /// </summary>
  public static class TilesController
  {
    // Lista de tiles con acceso indexado (O(1))
    public static TileNode[,] tiles = new TileNode[100, 100];
    public static List<TileNode> evaluatedTiles = new();

    public static void AddTileFromScene()
    {
      GameObject gameObjectTilesParent = GameObject.Find("TilesForLists");
      for (int i = 0; i < gameObjectTilesParent.transform.childCount; i++)
      {
        Transform tile = gameObjectTilesParent.transform.GetChild(i).transform;
        AddTile((int)tile.position.x, (int)tile.position.z, tile.GetComponent<Components.Tile>().blocked);
        if (tile.GetComponent<Components.Tile>().blocked)
        {
          tile.GetComponent<Renderer>().material.color = Global.RED;
        }
      }

      foreach (TileNode tile in tiles)
      {
        if (tile != null)
        {
          Debug.Log("Tile: " + tile.x + " " + tile.z + " blocked: " + tile.blocked);
        }
      }
    }

    public static void AddTile(int x, int z, bool blocked)
    {
      tiles[x, z] = new TileNode(x, z)
      {
        blocked = blocked
      };
    }

    public static void AddTile(TileNode tile)
    {
      tiles[tile.x, tile.z] = tile;
    }

    public static void CleanTiles()
    {
      tiles = new TileNode[16, 16];
    }

    public static bool IsBlocked(int x, int z)
    {
      return tiles[x, z].blocked;
    }

    public static void AddEvaluatedTile(TileNode tile)
    {
      evaluatedTiles.Add(tile);
    }

    public static void ResetTiles()
    {
      evaluatedTiles.Clear();
      foreach (TileNode tile in tiles)
      {
        if (tile == null) continue;
        tile.Reset();
      }
    }
  }
}

