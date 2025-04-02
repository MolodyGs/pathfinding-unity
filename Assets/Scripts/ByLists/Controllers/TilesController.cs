using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Components;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;




namespace Controllers
{

  public struct Paths
  {
    public List<TileNode> path;
  }

  /// <summary>
  /// Controla la creación y lectura de los tiles del juego.
  /// </summary>
  public static class TilesController
  {
    // Lista de tiles con acceso indexado (O(1))
    private static TileNode[,] tiles = new TileNode[100, 100];
    public static List<TileNode> path = new();
    public static Paths[,] pathsAlreadyEvaluated = new Paths[100, 100];
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
      Debug.Log("Reiniciando el camino y tiles actualmente evaluados.");
      path = new();
      openTiles.Clear();
      foreach (TileNode tile in tiles)
      {
        if (tile == null) continue;
        tile.Reset();
      }
    }

    public static void SetNewPath(int x, int z)
    {
      Debug.Log("Estableciendo tile para el camino evaluado: " + x + " " + z);
      Debug.Log("Último tile agregado: " + path.Last().x + " " + path.Last().z);
      // Si el tile no ha sido evaluado, se crea una nueva lista de caminos.
      if (pathsAlreadyEvaluated[x, z].path == null)
      {
        pathsAlreadyEvaluated[x, z].path = path;
      }
      else
      {
        Debug.Log("El camino ya ha sido evaluado y agregado a la lista de paths.");
      }
    }

    public static bool IsTheDestinationAlreadyEvaluated(int x, int z)
    {
      Debug.Log("Comprobando si el tile ya ha sido evaluado: " + x + " " + z);
      bool alreadyEvaluated = pathsAlreadyEvaluated[x, z].path != null;
      Debug.Log("Destino evaluado?: " + alreadyEvaluated);
      return alreadyEvaluated;
    }

    public static void SetPath(int x, int z)
    {
      Debug.Log("Estableciendo el camino ya evaluado para el destino: " + x + " " + z);
      ResetTiles();
      
      foreach (TileNode tile in pathsAlreadyEvaluated[x, z].path)
      {
          tile.SetPlate(true);
      }
    }
  }
}

