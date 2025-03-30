using UnityEngine;

namespace Components
{
  /// <summary>
  /// Asigna el tile seleccionado por el usuario como origen o destino.
  /// </summary>
  public class Tile : MonoBehaviour
  {
    public bool blocked = false;
    public Tile() { }

    /// <summary>
    /// Asigna el tile seleccionado por el usuario como origen o destino.
    /// </summary>
    void OnMouseDown() { Controllers.InputController.SetTile(gameObject); }

    public void Reset()
    {
      GetComponent<Renderer>().material.color = Global.WHITE;
    }
  }
}

