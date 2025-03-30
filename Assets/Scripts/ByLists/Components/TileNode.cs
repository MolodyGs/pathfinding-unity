using UnityEngine;
using System.Threading.Tasks;

namespace Components
{
  public class TileNode
  {
    public int x;
    public int z;
    public bool closed = false;
    public bool blocked = false;
    public TileNode parent = null;
    public int g = 0;
    public int h = 0;
    public int f = 0;
    static readonly GameObject platePrefab = Resources.Load<GameObject>("Plane");

    public TileNode(int x, int z)
    {
      this.x = x;
      this.z = z;
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
      // Instancia de un gameobject visual para el camino
      GameObject plate = Object.Instantiate(platePrefab);
      plate.transform.position = new Vector3(x, 1.0f, z);
      plate.GetComponent<Renderer>().material.color = Global.YELLOW;
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
    }
  }
}
