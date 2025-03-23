using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public static class TilesController
{
  static TileDTO[] tiles;

  public static void RoundPosition(GameObject tile)
  {
    tile.transform.position = new Vector3((float)Math.Round(tile.transform.position.x), 0, (float)Math.Round(tile.transform.position.z));
  }

  public static async Task<GameObject[]> ReadTilesData()
  {
    string path = Application.dataPath + "/Resources";
    object data = await JsonController.ReadJson<TileListDTO>(path, "tiles.json");

    if (data == null)
    {
      return await CreateJson();
    }

    if (data != null && GameObject.Find("Tiles").transform.childCount > 0)
    {
      GameObject parent = GameObject.Find("Tiles");

      List<GameObject> children = new();
      foreach (Transform child in parent.transform)
      {
        children.Add(child.gameObject);
        RoundPosition(child.gameObject);
      }
        await CreateJson();
      return children.ToArray();
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
      return null;
    }
  }

  static GameObject[] CreateTiles(TileDTO[] tiles)
  {

    Debug.Log("Creando tiles...");

    try
    {
      GameObject tilePrefab = Resources.Load<GameObject>("Tile");
      GameObject tilesContainer = GameObject.Find("Tiles");
      GameObject tileObj;
      GameObject[] tilesObj = new GameObject[tiles.Length];
      Tile tileComponent;

      for (int i = 0; i < tiles.Length; i++)
      {
        tileObj = UnityEngine.Object.Instantiate(tilePrefab);
        tilesObj[i] = tileObj;
        tileObj.transform.position = new Vector3(tiles[i].x, 0, tiles[i].z);
        tileObj.transform.parent = tilesContainer.transform;

        tileComponent = tileObj.GetComponent<Tile>();
        tileComponent.ChangeState(tiles[i].blocked);
      }

      return tilesObj;
    }
    catch (Exception e)
    {
      Debug.LogError("Error al crear los tiles: " + e.Message);
      return null;
    }
  }


  static async Task<GameObject[]> CreateJson()
  {
    GameObject tilesContainer = GameObject.Find("Tiles");
    GameObject tileObj;
    GameObject[] tilesObj = new GameObject[tilesContainer.transform.childCount];
    int count = tilesContainer.transform.childCount;
    tiles = new TileDTO[count];

    for (int i = 0; i < count; i++)
    {
      tileObj = tilesContainer.transform.GetChild(i).gameObject;
      tilesObj[i] = tileObj;
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
    await JsonController.CreateJson("Resources", "tiles.json", tileList);
    return tilesObj;
  }
}