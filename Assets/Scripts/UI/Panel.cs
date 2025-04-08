using UnityEngine;

namespace UI
{
  public class Panel : MonoBehaviour
  {

    public GameObject[] Buttons;

    /// <summary>
    /// Se establece el estado de los botones del panel.
    /// </summary>
    public void SetButtonsState(bool state)
    {
      // Itera entre los botones del panel y establece su estado
      foreach (GameObject button in Buttons)
      {
        button.GetComponent<BaseButton>().SetButtonState(state);
      }

      // Activa o desactiva el panel
      gameObject.SetActive(state);
    }
  }
}
