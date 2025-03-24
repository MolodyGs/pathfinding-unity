using UnityEngine;
using System.Threading.Tasks;

public static class MoveController
{
  public static async Task Move()
  {
    if (InputController.origin == null || InputController.destination == null) return;
    GameObject Player = GameObject.Find("Player");
    Tile origin = InputController.origin.GetComponent<Tile>();
    Tile destination = InputController.destination.GetComponent<Tile>();
    Player.transform.position = new Vector3(origin.transform.position.x, 1.0f, origin.transform.position.z);

    for (int i = PathfindingController.path.Count - 1; i > 0; i--)
    {
      GameObject tile = PathfindingController.path[i];
      Vector3 destinationPosition = new Vector3(tile.transform.position.x, 1.0f, tile.transform.position.z);
      await smoothMovement(Player, destinationPosition);
    }

    return;
  }

  async static Task Wait(int milisegundos)
  {
    await Task.Delay(milisegundos);
  }

  static async Task smoothMovement(GameObject Player, Vector3 destination)
  {
    float distance = Vector3.Distance(Player.transform.position, destination);
    float step = 0.6f;
    while (distance > 0.1f)
    {
      Player.transform.position = Vector3.MoveTowards(Player.transform.position, destination, step);
      distance = Vector3.Distance(Player.transform.position, destination);
      await Wait(10);
    }
  }
}