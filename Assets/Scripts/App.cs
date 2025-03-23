using System.IO;
using System;
using UnityEngine;

public class App : MonoBehaviour
{

  GameObject[] tiles;
  public async void Start()
  {
    tiles = await TilesController.ReadTilesData();
    for (int i = 0; i < tiles.Length; i++)
    {
      Debug.Log(tiles[i].transform.position);
    }
    Pathfinding.SetTiles(tiles);
  }
}

