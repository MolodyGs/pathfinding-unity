using Unity.VisualScripting;
using UnityEngine;

namespace Components
{
  /// <summary>
  /// Clase utilizada para representar un tile en el mapa como un componente para una GameObject.
  /// </summary>
  public class Tile : MonoBehaviour
  {
    public bool blocked = false;
    public Tile() { }

    /// <summary>
    /// Asigna el tile seleccionado por el usuario como origen o destino.
    /// </summary>
    public async void OnMouseDown() { await Controllers.InputController.SetInput(gameObject); }
    public void OnMouseEnter() { Controllers.InputController.SetInputWhenMouseEnter(gameObject); }

    public void Reset()
    {
      GetComponent<Renderer>().material.color = Global.WHITE;
    }
  }
}

