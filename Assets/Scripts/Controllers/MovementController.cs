using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components;
using UnityEngine;

namespace Controllers
{
  public class MovementController : MonoBehaviour
  {
    public async Task Movement(GameObject entity, List<TileNode> tiles)
    {
      Debug.Log("Comenzando el movimiento.");
      await RunCoroutineAsTask(Move(entity, tiles));
      Debug.Log("Movimiento terminado");
    }

    IEnumerator Move(GameObject entity, List<TileNode> tiles)
    {
      for (int i = tiles.Count - 1; i >= 0; i--)
      {
        entity.transform.position = new Vector3(tiles[i].GetPosition().x, 0.5f, tiles[i].GetPosition().y);
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