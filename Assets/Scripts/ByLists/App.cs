using UnityEngine;

namespace Assets.Scripts.ByList.App
{
  public class App : MonoBehaviour
  {
    public void Start()
    {
      Debug.Log("Cargando tiles...");
      Controllers.TilesController.AddTileFromScene();
    }
  }
}