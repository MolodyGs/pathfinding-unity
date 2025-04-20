using UnityEngine;
using UnityEngine.UI;
using Global;

namespace UI
{
  public abstract class BaseButton : MonoBehaviour
  {
    public GameObject[] optionalChildren = null; // Parametro opcional
    public GameObject optionalPanel = null; // Parametro opcional
    public SettingReference setting = new();
    Button button = null; // Referencia al componente Button del objeto actual

    /// <summary>
    /// Initializa el estado del botón y su panel hijo. Es posible realizar modificaciones especificas en cada botón.
    /// </summary>
    public abstract void InitialState();

    public void Start()
    {
      button = gameObject.GetComponent<Button>();
      button.onClick.AddListener(_OnClick); // Se añade el evento de clic al botón
    }

    /// <summary>
    /// Acción general al hacer clic en el botón.
    /// </summary>
    public void _OnClick()
    {
      setting.value = !setting.value;

      // Se establece el estado del panel y de los hijos del botón.
      SetChildrenBottonsState(setting.value);

      // Se establece el color del botón
      SetButtonColor();
    }

    /// <summary>
    /// Establece el estado del panel y de los hijos del botón. 
    /// Un botón puede activar la posibilidad de activar otros botones, por lo que si el botón padre se desactiva, entonces los hijos también se desactivan.
    /// </summary>
    protected void SetChildrenBottonsState(bool state)
    {
      // Se desactivan los elementos hijos opcionales del botón
      foreach (GameObject child in optionalChildren) { child.SetActive(state); }

      // Si existe un panel hijo, entonces se establece el estado de los botones del panel.
      if (optionalPanel != null) optionalPanel.GetComponent<Panel>().SetButtonsState(state);
    }

    /// <summary>
    /// Este método se llama cuando un panel tiene a este botón como hijo y el panel a sufrido una modificación.
    /// </summary>
    public void SetButtonState(bool state)
    {
      // Cuando se está ocultando el botón, entonces a su vez se desactiva esta configuración.
      if (state == false) setting.value = false;

      // Tanto al activar como al desactivar el botón, se establece el estado inicial.
      InitialState();

      // Se establece el color del botón
      SetButtonColor();
    }

    /// <summary>
    /// Establece el color del botón dependiendo de su estado.
    /// </summary>
    void SetButtonColor()
    {
      gameObject.GetComponent<Image>().color = setting.value ? Colors.SOFT_GREEN : Colors.SOFT_WHITE;
    }
  }
}

