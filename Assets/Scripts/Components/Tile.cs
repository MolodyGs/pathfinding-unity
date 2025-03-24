using System;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
  public float x;
  public float z;
  public bool blocked;
  public int gCost = 0;
  public int hCost = 0;
  public int fCost = 0;

  public bool active;

  public GameObject arrow;

  public GameObject parent;

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

  void OnMouseDown()
  {
    Tile tile = GetComponent<Tile>();
    Debug.Log("Haz hecho click en: Tile: " + tile.x + ", " + tile.z + ", " + tile.blocked);
    InputController.SetTile(gameObject);
  }

  public void ChangeState(bool state)
  {
    blocked = state;
    if (blocked)
    {
      Renderer renderer = GetComponent<Renderer>();
      renderer.material.color = Global.RED;
      this.transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);
      this.transform.localScale = new Vector3(transform.localScale.x, 1.4f, transform.localScale.z);
    }
    else
    {
      Renderer renderer = GetComponent<Renderer>();
      renderer.material.color = Global.WHITE;
      this.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
      this.transform.localScale = new Vector3(transform.localScale.x, 0.9f, transform.localScale.z);
    }
  }

  public void SetGCost(int cost)
  {
    gCost = cost;
    fCost = gCost + hCost;
  }

  public void SetHCost(int cost)
  {
    hCost = cost;
    fCost = gCost + hCost;
  }

  public void SetArrow(GameObject arrow)
  {
    Destroy(this.arrow);
    this.arrow = arrow;
  }

  public void Reset()
  {
    gCost = 0;
    hCost = 0;
    fCost = 0;
    active = false;
    parent = null;
    Destroy(arrow);
    arrow = null;
    GetComponent<Renderer>().material.color = blocked ? Global.RED : Global.WHITE;
  }

  public void SetPath()
  {
    PathfindingController.path.Add(this.gameObject);
    GetComponent<Renderer>().material.color = Global.BLUE;
    if (parent == null) return;
    for (int i = 0; i < 3; i++)
    {
      arrow.transform.GetChild(i).GetComponent<Renderer>().material.color = Global.GREEN;
    }
    parent.GetComponent<Tile>().SetPath();
  }
}
