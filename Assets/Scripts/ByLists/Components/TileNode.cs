using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Components
{
  public class TileNode
  {
    public int x;
    public int z;
    public int g = 0;
    public int h = 0;
    public int f = 0;
    public bool blocked = false;
    private bool closed = false;
    public TileNode parent = null;
    private GameObject plate;

    public TileNode(int x, int z)
    {
      this.x = x;
      this.z = z;
      plate = Object.Instantiate(Resources.Load<GameObject>("Plane"));
      plate.transform.position = new Vector3(x, 1.0f, z);
      plate.SetActive(false);
    }

    public void SetGCost(int gCost)
    {
      g = gCost;
      f = g + h;
    }

    public void SetHCost(int hCost)
    {
      h = hCost;
      f = g + h;
    }

    public async Task SetPath()
    {
      // Activa la placa asociada al nodo
      plate.SetActive(true);
      Controllers.PathfindingController.path.Add(this);
      if (parent == null) return;
      await parent.SetPath();
    }

    public Vector2 GetPosition()
    {
      return new Vector2(x, z);
    }

    public void Reset()
    {
      closed = false;
      parent = null;
      g = 0;
      h = 0;
      f = 0;
      plate.SetActive(false);
    }

    public void SetClosed(bool closed)
    {
      this.closed = closed;
    }

    public bool GetClosed()
    {
      return closed;
    }
  }
}
