using UnityEngine;

public class LinkedList
{
  public LinkedListNode first;
  public LinkedListNode last;

  public LinkedList()
  {
    first = null;
    last = null;
  }

  private void AddFirst(LinkedListNode newNode)
  {
    first = newNode;
    if (last == null)
    {
      first.next = first;
      first.previous = first;
      last = first;
    }
    else
    {
      first.next = last;
      last.previous = first;
    }
  }

  public void Add(GameObject value)
  {
    if (value == null) { 
      Debug.LogError("[LinkedList] No se puede añadir un nodo nulo a la lista enlazada.");
      return;
    }

    LinkedListNode newNode = new(value);

    if (first == null)
    {
      AddFirst(newNode);
      return;
    }

    if (last == null)
    {
      last = newNode;
      first.next = last;
      last.previous = first;
      return;
    }
    last.next = newNode;
    newNode.previous = last;
    newNode.next = first;
    last = newNode;
  }
}

public class LinkedListNode
{
  public LinkedListNode next;
  public LinkedListNode previous;
  public GameObject value;

  public LinkedListNode(GameObject value)
  {
    this.value = value;
  }
}