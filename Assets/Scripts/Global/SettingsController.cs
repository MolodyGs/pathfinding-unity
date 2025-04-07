using Global;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controllador para las opciones de configuración del juego.
/// </summary>
public class SettingsController : MonoBehaviour
{

  public GameObject visualPathfindingButton;
  public GameObject stepsButton;

  public GameObject SpacebarText;

  public void Start()
  {
    SetButtonColor(visualPathfindingButton, Settings.VISUAL_PATHFINDING);
    if (!Settings.VISUAL_PATHFINDING)
    {
      Settings.STEPS = false;
    }
    SetActiveInChildren(visualPathfindingButton, Settings.VISUAL_PATHFINDING);
    SetButtonColor(stepsButton, Settings.STEPS);
  }

  private void SetButtonColor(GameObject button, bool isActive)
  {
    button.GetComponent<Image>().color = isActive ? Colors.SOFT_GREEN : Colors.SOFT_WHITE;
    EventSystem.current.SetSelectedGameObject(null);
  }

  private void SetActiveInChildren(GameObject parent, bool isActive)
  {
    foreach (Transform child in parent.transform)
    {
      child.gameObject.SetActive(isActive);
    }
  }

  public void _SetVisualPathfinding()
  {
    Settings.VISUAL_PATHFINDING = !Settings.VISUAL_PATHFINDING;
    SetActiveInChildren(visualPathfindingButton, Settings.VISUAL_PATHFINDING);
    if (!Settings.VISUAL_PATHFINDING)
    {
      Settings.STEPS = false;
    }
    SetButtonColor(visualPathfindingButton, Settings.VISUAL_PATHFINDING);
    SetButtonColor(stepsButton, Settings.STEPS);
    Debug.Log("Haciendo click! Visual Pathfinding: " + Settings.VISUAL_PATHFINDING);
  }

  public void _SetSteps()
  {
    Settings.STEPS = !Settings.STEPS;
    SetButtonColor(stepsButton, Settings.STEPS);
    SetActiveInChildren(stepsButton, Settings.STEPS);
    SpacebarText.SetActive(Settings.STEPS);
    Debug.Log("Haciendo click! Steps: " + Settings.STEPS);
  }
}