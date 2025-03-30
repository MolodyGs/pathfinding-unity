using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

/// <summary>
/// Controla la creación y lectura de los tiles del juego.
/// </summary>
public static class TilesController
{
  // Lista de tiles en formato DTO
  static TileDTO[] tiles;

  // Prefab de tile
  static readonly GameObject tilePrefab = Resources.Load<GameObject>("Tile");

  /// <summary>
  /// Lee los datos de los tiles desde el archivo tiles.json.
  /// </summary>
  public static async Task<GameObject[]> ReadTilesData()
  {
    string path = Application.dataPath + "/Resources";
    object data = await JsonController.ReadJson<TileListDTO>(path, "tiles.json");

    // Si no existe el archivo, se crea uno nuevo
    if (data == null) { return await CreateJson(); }

    // Si existen tiles en la escena, se redondean sus posiciones y se crea un nuevo archivo tiles.json
    if (GameObject.Find("Tiles").transform.childCount > 0)
    {
      // Obtiene el contenedor de tiles en la escena
      GameObject parent = GameObject.Find("Tiles");

      // Redondea las posiciones de los tiles
      List<GameObject> children = new();
      foreach (Transform child in parent.transform)
      {
        children.Add(child.gameObject);
        RoundPosition(child.gameObject);
      }
      // Crear un nuevo archivo tiles.json con los datos de los tiles redondeados
      await CreateJson();
      return children.ToArray();
    }

    // Si el archivo existe, se crean los tiles a partir de los datos leídos
    try
    {
      // La data debe ser un objeto TileListDTO
      TileListDTO tileList = (TileListDTO)data;
      tiles = tileList.tiles;

      // Se generan los tiles a partir de los datos leídos
      return CreateTiles(tiles);
    }
    catch (Exception e)
    {
      Debug.LogError("Error al castear el objeto a TileList: " + e.Message);
      return null;
    }
  }

  /// <summary>
  /// Crea los tiles a partir de un arreglo de TileDTO.
  /// </summary>
  static GameObject[] CreateTiles(TileDTO[] tiles)
  {
    Debug.Log("Creando tiles...");
    try
    {
      // Se obtiene el contenedor de los tiles en la escena
      GameObject tilesContainer = GameObject.Find("Tiles");

      // Se crea una lista de GameObjects a partir de los datos de los tiles con el mismo tamaño que tiles
      GameObject[] tilesObj = new GameObject[tiles.Length];

      // Variables Auxiliares
      Tile tileComponent;
      GameObject tileObj;

      // Se itera entre los tiles y se crean los GameObjects
      for (int i = 0; i < tiles.Length; i++)
      {
        // Se instancia un nuevo tile a partir del prefab
        tileObj = UnityEngine.Object.Instantiate(tilePrefab);
        tilesObj[i] = tileObj;
        tileObj.transform.position = new Vector3(tiles[i].x, 0, tiles[i].z);

        // Se añade el tile como hijo del contenedor de tiles
        tileObj.transform.parent = tilesContainer.transform;

        // Se cambia el estado del tile a bloqueado si es necesario
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


  /// <summary>
  /// Crea un archivo tiles.json con los datos de los tiles.
  /// </summary>
  static async Task<GameObject[]> CreateJson()
  {
    // Se obtiene el contenedor de los tiles en la escena
    GameObject tilesContainer = GameObject.Find("Tiles");

    // Se crea un arreglo de GameObjects con el tamaño del contenedor de tiles
    GameObject[] tilesObj = new GameObject[tilesContainer.transform.childCount];

    // Variables auxiliares
    GameObject tileObj;
    int count = tilesContainer.transform.childCount;

    // Se crea un arreglo de TileDTO con el tamaño del contenedor de tiles
    tiles = new TileDTO[count];

    // Se itera entre los tiles y se obtienen los datos de cada uno
    for (int i = 0; i < count; i++)
    {
      // Se obtiene el GameObject y el componente Tile
      tileObj = tilesContainer.transform.GetChild(i).gameObject;
      Tile tile = tileObj.GetComponent<Tile>();

      // Se añade el tile al arreglo de tiles
      tilesObj[i] = tileObj;

      // Se crea un nuevo tile con la información del tile actual
      tiles[i] = new TileDTO
      {
        x = tile.x,
        z = tile.z,
        blocked = tile.blocked
      };

      // Se cambia la posición de los tiles bloqueados
      if (tile.blocked)
      {
        tileObj.transform.position = new Vector3(tileObj.transform.position.x, .5f, tileObj.transform.position.z);
      }
    }

    // Se genera una lista de tiles en formato DTO
    TileListDTO tileList = new()
    {
      tiles = tiles
    };

    // Se crea el archivo tiles.json
    await JsonController.CreateJson("Resources", "tiles.json", tileList);

    // Retorna los tiles en formato GameObject
    return tilesObj;
  }

  /// <summary>
  /// Redondea la posición de un tile a un número entero.
  /// </summary>
  public static void RoundPosition(GameObject tile)
  {
    tile.transform.position = new Vector3((float)Math.Round(tile.transform.position.x), 0, (float)Math.Round(tile.transform.position.z));
  }
}