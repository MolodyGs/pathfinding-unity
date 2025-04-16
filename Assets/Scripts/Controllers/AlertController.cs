using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Collections;

namespace Controllers
{
  public class AlertController : MonoBehaviour
  {
    GameObject text;
    TextMeshProUGUI textMeshPro;
    bool waitingForClose = false;

    public void Awake()
    {
      GameObject canvas = GameObject.Find("Canvas");
      text = canvas.transform.Find("AlertText").gameObject;
      text.SetActive(false);
      textMeshPro = text.GetComponent<TextMeshProUGUI>();
    }

    public void ShowLoadingMessage()
    {
      text.SetActive(true);
      textMeshPro.text = "Loading...";
    }

    public void ShowNoPathMessage()
    {
      waitingForClose = true;
      text.SetActive(true);
      textMeshPro.text = "Path not found!";
      StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
      yield return new WaitForSeconds(2);
      text.SetActive(false);
      waitingForClose = false;
    }

    public void Hide()
    {
      if (waitingForClose) return;
      text.SetActive(false);
    }
  }
}