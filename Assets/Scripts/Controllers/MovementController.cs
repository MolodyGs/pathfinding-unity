using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components;
using UnityEngine;

namespace Controllers
{
  public class MovementController : MonoBehaviour
  {
    public async Task Movement(GameObject origin, List<TileNode> tiles)
    {
      Debug.Log("Comenzando el movimiento.");
      await RunCoroutineAsTask(Move(origin, tiles));
      Debug.Log("Movimiento terminado");
    }

    IEnumerator Move(GameObject origin, List<TileNode> tiles)
    {
      if (!TurnController.isPlayerTurn)
      {
        tiles[1].SetBlockedState(true);
        tiles[^1].SetBlockedState(false);
      }

      for (int i = tiles.Count - 1; i >= 0; i--)
      {
        if (i == 0 && !TurnController.isPlayerTurn) yield break;
        origin.transform.position = new Vector3(tiles[i].GetPosition().x, 0.5f, tiles[i].GetPosition().y);
        yield return new WaitForSeconds(0.2f);
      }
    }

    Task RunCoroutineAsTask(IEnumerator coroutine)
    {
      var tcs = new TaskCompletionSource<bool>();
      StartCoroutine(WaitForCompletion(coroutine, tcs));
      return tcs.Task;
    }

    IEnumerator WaitForCompletion(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
    {
      yield return StartCoroutine(coroutine);
      tcs.SetResult(true);
    }
  }
}