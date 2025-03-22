using System.IO;
using System;
using UnityEngine;

public class App : MonoBehaviour
{

  async void Start()
  {
    bool response = await TilesController.Instance.ReadTilesData();
    Debug.Log("Tiles data loaded: " + response);
  }

}

