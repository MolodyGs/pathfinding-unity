using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;

public static class TilesController
{
  static TileDTO[] tiles;

  public static async Task<bool> ReadTilesData()
  {
    string path = Application.dataPath + "/Resources";
    object data = await Json.ReadJson<TileListDTO>(path, "tiles.json");

    if (data == null)
    {
      return await CreateJson();
    }

    try
    {
      TileListDTO tileList = (TileListDTO)data;
      tiles = tileList.tiles;
      return CreateTiles(tiles);
    }
    catch (Exception e)
    {
      Debug.LogError("Error al castear el objeto a TileList: " + e.Message);
      return false;
    }
  }

  static bool CreateTiles(TileDTO[] tiles)
  {

    Debug.Log("Creando tiles...");

    try
    {
      GameObject tilePrefab = Resources.Load<GameObject>("Tile");
      GameObject tilesContainer = GameObject.Find("Tiles");
      GameObject tileObj;
      Tile tileComponent;

      foreach (TileDTO tile in tiles)
      {
        tileObj = UnityEngine.Object.Instantiate(tilePrefab);
        tileObj.transform.position = new Vector3(tile.x, 0, tile.z);
        tileObj.transform.parent = tilesContainer.transform;

        tileComponent = tileObj.GetComponent<Tile>();
        tileComponent.ChangeState(tile.blocked);
      }

      return true;
    }
    catch (Exception e)
    {
      Debug.LogError("Error al crear los tiles: " + e.Message);
      return false;
    }
  }


  static async Task<bool> CreateJson()
  {
    GameObject tilesContainer = GameObject.Find("Tiles");
    GameObject tileObj;
    int count = tilesContainer.transform.childCount;
    tiles = new TileDTO[count];

    for (int i = 0; i < count; i++)
    {
      tileObj = tilesContainer.transform.GetChild(i).gameObject;
      Tile tile = tileObj.GetComponent<Tile>();
      tiles[i] = new TileDTO
      {
        x = tile.x,
        z = tile.z,
        blocked = tile.blocked
      };

      if (tile.blocked)
      {
        tileObj.transform.position = new Vector3(tileObj.transform.position.x, .5f, tileObj.transform.position.z);
      }
    }

    TileListDTO tileList = new()
    {
      tiles = tiles
    };

    return await Json.CreateJson("Resources", "tiles.json", tileList);
  }
}