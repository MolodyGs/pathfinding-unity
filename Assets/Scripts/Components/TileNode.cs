using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using Global;

namespace Components
{
  public class TileNode
  {
    // public
    public int x;
    public int z;
    public int g = 0;
    public int h = 0;
    public int f = 0;
    public bool blocked = false;
    public bool isOpen = false;
    public TileNode parent = null;

    // private
    bool closed = false;
    bool isPartOfThePath = false;
    readonly GameObject plate;

    public TileNode(int x, int z)
    {
      this.x = x;
      this.z = z;
      plate = Object.Instantiate(Resources.Load<GameObject>("Plane"));
      plate.transform.position = new Vector3(x, 0.5f, z);
      plate.SetActive(false);
      if (!Settings.VISUAL_PATHFINDING.value)
      {
        plate.GetComponentInChildren<TMP_Text>().text = string.Empty;
      }
    }

    public async Task<int> SetPath(List<TileNode> path)
    {
      Debug.Log("SetPath: " + x + ", " + z + " - Path Count: " + path.Count);
      try
      {
        if (isPartOfThePath)
        {
          Debug.LogError("Error al intentar establecer el camino. Se ha evitado un posible Stack Overflow.");
          return 1;
        };

        isPartOfThePath = true;
        // Activa la placa asociada al nodo
        plate.SetActive(true);

        // Añade el nodo a la lista de nodos para el camino
        path.Add(this);

        SetPlateColor(Colors.GREEN);

        // Verifica que el padre exista
        if (parent == null) return 0;

        // Si el padre no es nulo, entonces llama a su método SetPath de forma recursiva
        return await parent.SetPath(path);
      }
      catch (System.Exception e)
      {
        Debug.LogError("Error al intentar recuperar al padre SetPath: " + e.Message);
        return -1;
      }
    }

    public void Reset(bool softReset = false)
    {
      closed = false;
      isOpen = false;
      parent = null;
      g = 0;
      h = 0;
      f = 0;
      isPartOfThePath = false;
      if (!softReset) plate.SetActive(false);
      CleanText();
    }

    public void SetGCost(int gCost)
    {
      g = gCost;
      f = g + h;
      SetText();
    }

    public void SetHCost(int hCost)
    {
      h = hCost;
      f = g + h;
      SetText();
    }


    public override string ToString()
    {
      return $"TileNode: ({x}, {z}) - G: {g}, H: {h}, F: {f}, Blocked: {blocked}, Closed: {closed}";
    }

    public void SetPlateColor(Color color)
    {
      Debug.Log("Cambiando Color del nodo: " + x + ", " + z + " a: " + color);
      plate.GetComponent<Renderer>().material.color = color;
    }

    public void SetText(bool force = false)
    {
      if (!Settings.VISUAL_PATHFINDING.value && !force) return;
      plate.GetComponentInChildren<TMP_Text>().text =
        "g: " + g + "\n" +
        "h: " + h + "\n" +
        "f: " + f + "\n"
      ;
    }

    public void CleanText()
    {
      plate.GetComponentInChildren<TMP_Text>().text = string.Empty;
    }

    public void SetClosed(bool closed) { this.closed = closed; }
    public bool GetClosed() { return closed; }
    public void SetPlate(bool state) { plate.SetActive(state); }
    public Vector2 GetPosition() { return new Vector2(x, z); }
  }
}
