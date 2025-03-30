using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// Asigna el tile seleccionado por el usuario como origen o destino.
/// </summary>
public class Tile : MonoBehaviour
{
  public float x;
  public float z;
  public bool blocked;
  public int gCost = 0;
  public int hCost = 0;
  public int fCost = 0;
  public bool closed = false;
  public GameObject arrow;
  public GameObject parent;
  static readonly GameObject arrowPrefab = Resources.Load<GameObject>("arrow");

  public Tile(float x, float z, bool blocked = false)
  {
    this.x = x;
    this.z = z;
    this.blocked = blocked;
  }

  void Start()
  {
    x = transform.position.x;
    z = transform.position.z;
    ChangeState(blocked);
  }

  /// <summary>
  /// Asigna el tile seleccionado por el usuario como origen o destino.
  /// </summary>
  void OnMouseDown() { InputController.SetTile(gameObject); }

  /// <summary>
  /// Cambia el estado de la variable "blocked".
  /// </summary>
  public void ChangeState(bool state)
  {
    blocked = state;
    if (blocked)
    {
      Renderer renderer = GetComponent<Renderer>();
      renderer.material.color = Global.BLOCKED;
      transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);
      transform.localScale = new Vector3(transform.localScale.x, 1.4f, transform.localScale.z);
    }
    else
    {
      Renderer renderer = GetComponent<Renderer>();
      renderer.material.color = Global.WHITE;
      transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
      transform.localScale = new Vector3(transform.localScale.x, 0.9f, transform.localScale.z);
    }
  }

  /// <summary>
  /// Establece el costo real acumulado del tile.
  /// </summary>
  public void SetGCost(int cost)
  {
    gCost = cost;
    fCost = gCost + hCost;
  }

  /// <summary>
  /// Establece el padre y la flecha que indica la dirección del camino.
  /// </summary>
  public void SetParent(GameObject parent)
  {
    this.parent = parent;
    Destroy(this.arrow);
    GameObject arrow = Instantiate(arrowPrefab);
    float arrowX = (parent.transform.position.x + gameObject.transform.position.x) / 2;
    float arrowZ = (parent.transform.position.z + gameObject.transform.position.z) / 2;
    arrow.transform.SetPositionAndRotation(new Vector3(arrowX, 0.1f, arrowZ), Quaternion.LookRotation(parent.transform.position - gameObject.transform.position));
    arrow.transform.rotation = Quaternion.Euler(0, arrow.transform.rotation.eulerAngles.y + 90, 0);
    this.arrow = arrow;
  }

  /// <summary>
  /// Establece el costo de la heurística del tile.
  /// </summary>
  public void SetHCost(int cost)
  {
    hCost = cost;
    fCost = gCost + hCost;
  }

  /// <summary>
  /// Resetea las variables del tile.
  /// </summary>
  public void Reset()
  {
    // Reseteo de variables para el pathfinding
    gCost = 0;
    hCost = 0;
    fCost = 0;
    closed = false;
    parent = null;

    // Destrucción de fechas visuales
    Destroy(arrow);
    arrow = null;

    // Establecer el color por default según el valor de "blocked"
    GetComponent<Renderer>().material.color = blocked ? Global.BLOCKED : Global.WHITE;
  }

  /// <summary>
  /// Establece el tile como parte del camino.
  /// </summary>
  public async Task SetPath()
  {
    // Añade el tile a la lista del camino más corto
    PathfindingController.path.Add(gameObject);

    // De forma decorativa, cambia el color del tile
    GetComponent<Renderer>().material.color = Global.BLUE;

    // Si no tiene padre, entonces estamos en el origen
    if (parent == null) return;

    // De forma decorativa, cambia el color de la flecha
    for (int i = 0; i < 3; i++)
    {
      arrow.transform.GetChild(i).GetComponent<Renderer>().material.color = Global.GREEN;
    }

    // Añade el padre del tile como parte del camino más corto (de forma indirecta, utiliza esta misma función pero en otro tile)
    await parent.GetComponent<Tile>().SetPath();
  }
}
