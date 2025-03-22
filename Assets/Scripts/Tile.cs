using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
  public float x;
  public float z;
  public bool blocked;

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

  public void ChangeState(bool state)
  {
    blocked = state;
    if (blocked)
    {
      Renderer renderer = GetComponent<Renderer>();
      renderer.material.color = Color.red;
      this.transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);
      this.transform.localScale = new Vector3(transform.localScale.x, 1.4f, transform.localScale.z);
    }
    else
    {
      Renderer renderer = GetComponent<Renderer>();
      renderer.material.color = Color.white;
      this.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
      this.transform.localScale = new Vector3(transform.localScale.x, 0.9f, transform.localScale.z);
    }
  }
}
