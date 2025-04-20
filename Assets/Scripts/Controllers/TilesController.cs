using System.Collections.Generic;
using Components;
using UnityEngine;
using Global;
using System;

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
        AddTile((int)tile.position.x, (int)tile.position.z, tile.GetComponent<Tile>().blocked, tile.gameObject);
        if (tile.GetComponent<Tile>().blocked)
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
          Debug.Log("Tile: " + tile.x + " " + tile.z + " blocked: " + tile.GetBlockedState());
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
      tiles[x, z] = new TileNode(x, z, blocked, obj);
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
      tile.SetBlockedState(blocked);
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
        if (tile != null && !tile.GetBlockedState())
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
      InputController.origin = player;
    }

    /// <summary>
    /// Establece los tiles vecinos de cada tile en la lista de tiles.
    /// </summary>
    public static void SetNeighbours()
    {
      TileNode tile;
      for (int i = 0; i < tiles.GetLength(0); i++)
      {
        for (int j = 0; j < tiles.GetLength(1); j++)
        {
          tile = tiles[i, j];
          if (tile == null) continue;

          int x = tile.x;
          int z = tile.z;

          tile.neighbors[0] = FindNeighbor(x - 1, z - 1);
          tile.neighbors[1] = FindNeighbor(x - 1, z);
          tile.neighbors[2] = FindNeighbor(x - 1, z + 1);
          tile.neighbors[3] = FindNeighbor(x, z + 1);
          tile.neighbors[4] = FindNeighbor(x + 1, z + 1);
          tile.neighbors[5] = FindNeighbor(x + 1, z);
          tile.neighbors[6] = FindNeighbor(x + 1, z - 1);
          tile.neighbors[7] = FindNeighbor(x, z - 1);
        }
      }
    }

    /// <summary
    /// Encuentra un tile vecino en base a su posición (x, z).
    /// Si no fue posible encontrar el tile, entonces retorna null.
    /// </summary>
    static TileNode FindNeighbor(int x, int z)
    {
      Debug.Log(" -- Buscando vecino: " + x + " " + z);

      // Obtiene el tile vecino de la lista indexada de tiles
      TileNode node = Find(x, z);

      // Si el tile no existe, entonces se retorna null
      if (node == null)
      {
        Debug.Log(" -- Vecino no encontrado: " + x + " " + z);
        return null;
      }

      // Se retorna el tile encontrado
      Debug.Log(" -- Vecino encontrado! " + x + " " + z);
      return node;
    }
  }
}

