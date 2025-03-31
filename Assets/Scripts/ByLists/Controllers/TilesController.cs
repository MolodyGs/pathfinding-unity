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
    private static TileNode[,] tiles = new TileNode[100, 100];
    public static List<TileNode> openTiles = new();

    public static void AddTileFromScene()
    {
      Debug.Log("Cargando Tiles");
      GameObject gameObjectTilesParent = GameObject.Find("TilesForLists");
      for (int i = 0; i < gameObjectTilesParent.transform.childCount; i++)
      {
        Transform tile = gameObjectTilesParent.transform.GetChild(i).transform;
        AddTile((int)tile.position.x, (int)tile.position.z, tile.GetComponent<Components.Tile>().blocked);
        if (tile.GetComponent<Components.Tile>().blocked)
        {
          tile.GetComponent<Renderer>().material.color = Global.RED;
        }
        Debug.Log("Tile Cargado!");
      }
      Debug.Log("Tiles creados desde la escena.");
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

    public static TileNode Find(int x, int z)
    {
      return tiles[x, z];
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

    public static void AddOpenTile(TileNode tile)
    {
      Debug.Log("Añadiendo tile a la lista de tiles abiertos: " + tile.x + " " + tile.z);
      openTiles.Add(tile);
    }

    /// <summary>
    /// Remueve un tile de la lista de tiles evaluados.
    /// </summary>
    public static void RemoveTileEvaluated(TileNode tileToRemove)
    {
      Debug.Log("Removiendo tile: " + tileToRemove.x + " " + tileToRemove.z);
      // Establece el tile como cerrado y lo elimina de la lista de tiles evaluados.
      foreach (TileNode tile in openTiles)
      {
        if (tile.GetPosition() == tileToRemove.GetPosition())
        {
          Debug.Log("Tile Encontrado: " + tile.x + " " + tile.z);
          bool response = openTiles.Remove(tile);
          Debug.Log("Tile removido?: " + response);
          return;
        }
      }
    }

    public static void ResetTiles()
    {
      openTiles.Clear();
      foreach (TileNode tile in tiles)
      {
        if (tile == null) continue;
        tile.Reset();
      }
    }
  }
}

