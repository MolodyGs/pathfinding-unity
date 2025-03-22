using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;


public class TilesController : MonoBehaviour
{
  // Instancia estática privada que se inicializa solo cuando se necesita.
  private static TilesController instance = null;

  // Objeto de bloqueo para asegurar la sincronización en entornos multihilo.
  private static readonly object lockObject = new object();

  // Constructor privado para evitar instanciaciones externas.
  private TilesController()
  {
    Console.WriteLine("Instancia de Singleton creada.");
  }

  // Propiedad pública que retorna la instancia única de la clase.
  public static TilesController Instance
  {
    get
    {
      // Bloquear el acceso cuando se está creando la instancia en un entorno multihilo.
      lock (lockObject)
      {
        // Si la instancia no ha sido creada, se crea en este momento.
        if (instance == null)
        {
          instance = new TilesController();
        }
        return instance;
      }
    }
  }
  TileDTO[] tiles;
  public static bool done = false;

  // Start is called before the first frame update
  void Start()
  {
    ReadTilesData();
  }

  void ReadTilesData()
  {
    string path = Application.dataPath + "/Resources";
    object data = Json.ReadJson(path, "tiles.json", typeof(TileListDTO));
    if (data == null)
    {
      Debug.LogError("No se pudo leer el archivo JSON.");
      CreateJson();
      return;
    }

    try
    {
      TileListDTO tileList = (TileListDTO)data;
      tiles = tileList.tiles;
      Debug.Log("Tiles Count: " + tiles.Length);
      foreach (TileDTO tile in tiles)
      {
        Debug.Log($"Tile: {tile.x}, {tile.z}, {tile.blocked}");
      }
      done = true;
    }
    catch (Exception e)
    {
      Debug.LogError("Error al castear el objeto a TileList: " + e.Message);
    }

  }


  void CreateJson()
  {
    GameObject tilesContainer = this.gameObject;
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

    Debug.Log("Tiles Count: " + tiles.Length);
    foreach (TileDTO tile in tiles)
    {
      Debug.Log($"Tile: {tile.x}, {tile.z}, {tile.blocked}");
    }

    // Serializar la lista de tiles en un objeto contenedor TileList
    TileListDTO tileList = new()
    {
      tiles = tiles
    };

    string json = JsonUtility.ToJson(tileList, true);
    string path = Path.Combine(Application.dataPath, "Resources", "tiles.json");

    // Asegurarse de que el directorio existe
    string directory = Path.GetDirectoryName(path);
    if (!Directory.Exists(directory))
    {
      Directory.CreateDirectory(directory);
    }

    // Guardar el archivo JSON
    File.WriteAllText(path, json);

    Debug.Log($"JSON creado en: {path}");
  }
}