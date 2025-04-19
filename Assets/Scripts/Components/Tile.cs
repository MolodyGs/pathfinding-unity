using UnityEngine;
using Global;

namespace Components
{
  /// <summary>
  /// Clase utilizada para representar un tile en el mapa como un componente para una GameObject.
  /// </summary>
  public class Tile : MonoBehaviour
  {
    public bool blocked = false;
    public TileNode tile;
    public Tile() { }

    /// <summary>
    /// Asigna el tile seleccionado por el usuario como origen o destino.
    /// </summary>
    public async void OnMouseDown() { await Controllers.InputController.SetInput(tile); }
    // public void OnMouseEnter() { Controllers.InputController.SetInputWhenMouseEnter(gameObject); }

    public void Reset()
    {
      GetComponent<Renderer>().material.color = Colors.WHITE;
    }
  }
}

