using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// Movimiento del jugador a través de los tiles.
/// </summary>
public static class MoveController
{
  /// <summary>
  /// Controla el movimiento del personaje a través de los tiles.
  /// Se espera obtener la lista de tiles a partir de la variable "path" en PathfindingController.
  /// </summary>
  public static async Task Move()
  {
    /// Verifica que exista un origen y un destino.
    if (InputController.origin == null || InputController.destination == null) return;

    // Busca al jugador
    GameObject Player = GameObject.Find("Player");

    // Obtiene el origen y lo establece como el punto inicial para el jugador
    Tile origin = InputController.origin.GetComponent<Tile>();
    Player.transform.position = new(origin.transform.position.x, 1.0f, origin.transform.position.z);

    // Itera entre los tiles del camino y mueve al jugador a través de ellos.
    for (int i = PathfindingController.path.Count - 1; i > 0; i--)
    {
      // Obtiene el tile
      GameObject tile = PathfindingController.path[i];

      // Realiza un movimiento suave desde el jugador hasta el tile siguiente
      await SmoothMovement(Player, new(tile.transform.position.x, 1.0f, tile.transform.position.z));
    }

    return;
  }

  /// <summary>
  /// Realiza un movimiento suave entre 2 posiciones
  /// <param name="Player">GameObject del Jugador</param>
  /// <param name="destination">Vector3 del destino</param>
  /// </summary>
  static async Task SmoothMovement(GameObject Player, Vector3 destination)
  {
    float distance = Vector3.Distance(Player.transform.position, destination);
    float step = 0.6f;

    // Mueve al jugador hacia el destino hasta que la distancia entre los puntos sea menor a 0.1
    while (distance > 0.1f)
    {
      Player.transform.position = Vector3.MoveTowards(Player.transform.position, destination, step);
      distance = Vector3.Distance(Player.transform.position, destination);

      // Espera 10 milisegundos antes de continuar para dar un efecto de movimiento suave
      await Task.Delay(10);
    }
  }
}