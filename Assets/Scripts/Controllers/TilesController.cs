using System.Collections.Generic;
using Components;
using UnityEngine;
using Global;

namespace Controllers
{
  /// <summary>
  /// Controla la creación y lectura de los tiles del juego.
  /// </summary>
  public static class TilesController
  {
    // Lista de tiles con acceso indexado (O(1))
    public static TileNode[,] tiles = new TileNode[100, 100];
    public static GameObject[,] tilesObj = new GameObject[100, 100];
    private static TileNode player;

    public static void AddTilesFromScene()
    {
      Debug.Log("Cargando Tiles...");
      GameObject gameObjectTilesParent = GameObject.Find("TilesForLists");
      for (int i = 0; i < gameObjectTilesParent.transform.childCount; i++)
      {
        Transform tile = gameObjectTilesParent.transform.GetChild(i).transform;
        tilesObj[(int)tile.position.x, (int)tile.position.z] = tile.gameObject;
        AddTile((int)tile.position.x, (int)tile.position.z, tile.GetComponent<Components.Tile>().blocked, tile.gameObject);
        if (tile.GetComponent<Components.Tile>().blocked)
        {
          tile.GetComponent<Renderer>().material.color = Colors.RED;
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

    /// <summary>
    /// Resetea los tiles y el camino actualmente evaluado.
    /// </summary>
    public static void ResetTiles(bool softReset = false)
    {
      Debug.Log("Reiniciando el camino y tiles actualmente evaluados.");
      foreach (TileNode tile in tiles)
      {
        if (tile == null) continue;
        tile.Reset(softReset);
      }
    }

    /// <summary>
    /// Añade un tile a la lista de tiles.
    /// </summary>
    public static void AddTile(int x, int z, bool blocked, GameObject obj, bool isPlayer = false)
    {
      tiles[x, z] = new TileNode(x, z, obj) { blocked = blocked };
      if (isPlayer)
      {
        player = tiles[x, z];
      }
    }

    /// <summary>
    /// Establece un tile como bloqueado.
    /// </summary>
    public static void SetBlockedState(int x, int z, bool blocked)
    {
      TileNode tile = Find(x, z);
      if (tile == null) return;
      Debug.Log("tile bloqueado");
      tile.blocked = blocked;
    }

    /// <summary>
    /// Retorna un tile en base a su posición (x, z).
    /// </summary>
    public static TileNode Find(int x, int z)
    {
      if (x >= tiles.GetLength(0) || z >= tiles.GetLength(1) || x < 0 || z < 0)
      {
        Debug.Log("Tile fuera de rango: " + x + " " + z);
        return null;
      }
      return tiles[x, z];
    }

    /// <summary>
    /// Retorna un tile en base a su posición (x, z).
    /// </summary>
    public static List<TileNode> FreeTiles()
    {
      List<TileNode> freeTiles = new();
      foreach (TileNode tile in tiles)
      {
        if (tile != null && !tile.blocked)
        {
          freeTiles.Add(tile);
        }
      }
      return freeTiles;
    }

    public static TileNode GetPlayerTile()
    {
      return player;
    }

    public static void SetPlayerTile(TileNode tile)
    {
      player = tile;
    }
  }
}

