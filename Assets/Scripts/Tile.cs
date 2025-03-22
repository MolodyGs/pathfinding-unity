using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
  public int x;
  public int z;
  public bool blocked;

  public Tile(int x, int z, bool blocked = false)
  {
    this.x = x;
    this.z = z;
    this.blocked = blocked;
  }

  void Start()
  {
    x = (int)transform.position.x;
    z = (int)transform.position.z;

    if (blocked)
    {
      Renderer renderer = GetComponent<Renderer>();
      renderer.material.color = Color.red;
    }
  }
}
