using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


namespace Controllers
{
  public static class Reader
  {

    public static GameObject tilePrefab = Resources.Load<GameObject>("TileForLists");
    public static GameObject[,] tilesObj = new GameObject[100, 100];

    public static IEnumerator ReadTxtFile(string fileName)
    {

      string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

      UnityWebRequest www = UnityWebRequest.Get(filePath);
      yield return www.SendWebRequest();

      string text;
      if (www.result == UnityWebRequest.Result.Success)
      {
        text = www.downloadHandler.text;
        Debug.Log(text);
        // Aquí puedes llamar a tu lógica para procesar el texto.
      }
      else
      {
        Debug.LogError("Error loading file: " + www.error);
        yield break;
      }

      string[] lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

      int x = 18;
      int z = 15;
      GameObject tile;

      foreach (string line in lines)
      {
        foreach (char c in line.Replace(" ", ""))
        {
          tile = null;
          Debug.Log(c);
          switch (c)
          {
            case '█':
              tile = Object.Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity);
              TilesController.AddTile(x, z, false, tile);
              tile.GetComponent<Components.Tile>().tile = TilesController.Find(x, z);
              tile.transform.SetParent(GameObject.Find("TilesForLists").transform);

              break;
            case 'x':
              tile = Object.Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity);
              tile.GetComponent<Renderer>().material.color = Color.red;
              tile.transform.SetParent(GameObject.Find("TilesForLists").transform);
              TilesController.AddTile(x, z, true, tile);
              tile.GetComponent<Components.Tile>().tile = TilesController.Find(x, z);
              break;
            case 'p':
              tile = Object.Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity);
              tile.transform.SetParent(GameObject.Find("TilesForLists").transform);
              TilesController.AddTile(x, z, false, tile, true);
              tile.GetComponent<Components.Tile>().tile = TilesController.Find(x, z);
              break;
            case 'n':
              Debug.Log("Tile n");
              break;
            default:
              Debug.Log("Tile default");
              break;
          }
          x++;
        }
        z--;
        x = 18;
      }
      TilesController.SetNeighbours();
    }
  }
}