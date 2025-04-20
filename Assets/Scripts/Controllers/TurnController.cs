using System.Threading.Tasks;
using Components;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Controllers
{
  public static class TurnController
  {
    public static LinkedList entities;
    public static bool isPlayerTurn = true;
    public static LinkedListNode currentEntity;
    private static readonly MovementController movementController = GameObject.Find("Controllers").GetComponent<MovementController>();

    public static async void Inizialize(bool autoPlay = false)
    {

      GameObject player = GameObject.Find("Player");
      GameObject entity1 = GameObject.Find("Entity1");
      GameObject entity2 = GameObject.Find("Entity2");

      entity1.transform.position = new Vector3(20, 0.5f, 0);
      entity2.transform.position = new Vector3(18, 0.5f, 0);

      entities = new LinkedList();
      entities.Add(player);
      entities.Add(entity1);
      entities.Add(entity2);
      ParallelController.entity = entities.first.value;

      // El siguiente turno es el de la primera entidad
      currentEntity = entities.first;

      if (autoPlay)
      {
        ParallelController.isRunning = true;
        await LoadTurn();
      }
    }

    public static async Task<int> LoadTurn()
    {
      List<TileNode> tiles = TilesController.FreeTiles();
      // TileNode destination = tiles[Random.Range(0, tiles.Count - 1)];
      TileNode destination = TilesController.GetPlayerTile();
      TileNode origin = TilesController.Find((int)currentEntity.value.transform.position.x, (int)currentEntity.value.transform.position.z);

      Debug.Log("Es el turno de: " + currentEntity.value.name);
      Debug.Log("Origen: " + currentEntity.value.transform.position);
      Debug.Log("Destino  : " + destination.x + ", " + destination.z);

      int response = await ParallelController.Start(origin, destination);

      if (response != 0)
      {
        Debug.Log("Se ha obtenido un codigo de error distinto de 0: " + response);
        return response;
      }

      await movementController.Movement(currentEntity.value, ParallelController.pathfindingController.GetPath());

      currentEntity = currentEntity.next;
      if (currentEntity.value.name != "Player")
      {
        return await LoadTurn();
      }
      else
      {
        isPlayerTurn = true;
        ParallelController.entity = currentEntity.value;
        ParallelController.isRunning = false;
        return response;
      }
    }

    public static async Task FinishPlayerTurn(int response)
    {
      ParallelController.isRunning = true;
      if (response == 0)
      {
        await movementController.Movement(currentEntity.value, ParallelController.pathfindingController.GetPath());
        isPlayerTurn = false;
        Debug.Log("Finalizando el turno del jugador");
      }
      else
      { 
        Debug.Log("Hubo un codigo de error distinto de 0: " + response);
        ParallelController.isRunning = false;
        return;
      }

      currentEntity = currentEntity.next;
      ParallelController.entity = currentEntity.value;
      Debug.Log("Es el turno de: " + currentEntity.value.name);

      if (currentEntity.value.name != "Player")
      {
        // Es una entidad
        await LoadTurn();
      }
      else
      {
        isPlayerTurn = true;
        ParallelController.isRunning = false;
      }
    }
  }
}