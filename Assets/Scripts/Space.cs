using System;
using UnityEngine;

public class Space : MonoBehaviour
{
  public Peg associatedObject
  {
    get; private set;
  }

  public bool filled
  {
    get
    {
      return associatedObject != null;
    }
  }

  public void PlacePeg(
    Peg peg)
  {
    if(peg.space != null)
    {
      peg.space.associatedObject = null;
    }

    associatedObject = peg;
    peg.space = this;
    peg.transform.SetParent(transform, false);
  }

  public void DestroyPeg()
  {
    Destroy(associatedObject.gameObject);
    associatedObject = null;
  }
}
