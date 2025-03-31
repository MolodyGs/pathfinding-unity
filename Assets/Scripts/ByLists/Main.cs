using UnityEngine;

public class Main : MonoBehaviour
{
  public void Start()
  {
    Debug.Log("Cargando tiles...");
    Controllers.TilesController.AddTileFromScene();
  }
}