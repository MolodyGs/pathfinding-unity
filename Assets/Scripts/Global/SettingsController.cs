using UnityEngine;
using UI;

/// <summary>
/// Controllador para las opciones de configuración del juego.
/// </summary>
public class SettingsController : MonoBehaviour
{
  public GameObject GeneralPanel;
  public GameObject LoadingText;

  public void Start()
  {
    GeneralPanel.GetComponent<Panel>().SetButtonsState(true);
    LoadingText.SetActive(false);
  }
}