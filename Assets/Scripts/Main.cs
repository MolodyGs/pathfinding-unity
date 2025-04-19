using System.Collections;
using System.Threading.Tasks;
using Controllers;
using UnityEngine;

public class Main : MonoBehaviour
{
  public async void Start()
  {
    await RunCoroutineAsTask(Reader.ReadTxtFile("tiles.txt"));
    TurnController.Inizialize(false);
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