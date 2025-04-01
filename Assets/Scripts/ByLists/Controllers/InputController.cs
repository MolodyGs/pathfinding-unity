using UnityEngine;

namespace Controllers
{
  public static class InputController
  {
    public static GameObject origin { get; set; }
    public static GameObject destination { get; set; }

    /// <summary>
    /// Asigna el tile seleccionado por el usuario como origen o destino.
    /// </summary>
    public static async void SetTile(GameObject tile)
    {

      // Si el tile está bloqueado, no se puede seleccionar.
      if (tile.GetComponent<Components.Tile>().blocked) return;

      // Si no hay un origen, se asigna el tile seleccionado como origen.
      if (origin == null)
      {
        Debug.Log("Estableciendo origen: " + tile.transform.position);
        origin = tile;
        tile.GetComponent<Renderer>().material.color = Global.GREEN;
        return;
      }

      // Si no hay un destino, se asigna el tile seleccionado como destino.
      if (destination == null)
      {
        Debug.Log("Estableciendo destino: " + tile.transform.position);
        if (tile.transform.position.x == origin.transform.position.x && tile.transform.position.z == origin.transform.position.z)
        {
          // Si el destino es el mismo que el origen, no se puede seleccionar.
          Debug.Log("El destino no puede ser el mismo que el origen.");
          return;
        }

        destination = tile;
        tile.GetComponent<Renderer>().material.color = Global.BLUE;

        // Se ejecuta el pathfinding y el movimiento.
        Debug.Log("Cargando camino...");
        await PathfindingController.Path();
        return;
      }

      // Si ya hay un origen y un destino, se reinician los tiles y se asigna el nuevo tile seleccionado como destino.
      PathfindingController.ResetTiles();
      origin.GetComponent<Components.Tile>().Reset();
      destination.GetComponent<Components.Tile>().Reset();
      GameObject[] plates = GameObject.FindGameObjectsWithTag("Plate");
      Debug.Log("Destruyendo el camino de placas...");
      foreach (GameObject plate in plates)
      {
        Debug.Log("Placa destruida! ");
        Object.Destroy(plate);
      }

      Debug.Log("Estableciendo origen y destino: " + tile.transform.position);
      origin = destination;
      destination = tile;
      tile.GetComponent<Renderer>().material.color = Global.GREEN;
      origin.GetComponent<Renderer>().material.color = Global.BLUE;
      await PathfindingController.Path();
    }

    public static async void SetTileWhenMouseEnter(GameObject tile)
    {
      if(!tile.CompareTag("Tile")) return;
      if (origin == null) return;
      if (tile.GetComponent<Components.Tile>().blocked) return;
      if (tile.transform.position.x == origin.transform.position.x && tile.transform.position.z == origin.transform.position.z) return;

      if (destination == null)
      {
        destination = tile;
        Debug.Log("Estableciendo destino: " + tile.transform.position);
      }
      else
      {
        if (destination.transform.position.x == tile.transform.position.x && destination.transform.position.z == tile.transform.position.z) return;
        destination = tile;
        PathfindingController.ResetTiles();
        await PathfindingController.Path();
      }
    }
  }
}

/// <summary> 
/// Lectura de la interacción del usuario con los tiles.
/// </summary>
