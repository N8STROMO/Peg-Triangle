using UnityEngine;

public class Peg : MonoBehaviour
{
  #region Data
  [SerializeField]
  Color selectedColor = new Color(1, 1, 1, 1);

  Color originalColor;

  SpriteRenderer sprite;

  public Space space;
  #endregion


  protected void Awake()
  {
    sprite = GetComponent<SpriteRenderer>();
    originalColor = sprite.color;
  }

  public void OnSelect()
  {
    sprite.color = selectedColor;
  }

  public void OnUnselect()
  {
    sprite.color = originalColor;
  }
}
