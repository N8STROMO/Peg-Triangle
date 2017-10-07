// TODO Moving pegs within triangle

using System;
using UnityEngine;

public class PegTriangle : MonoBehaviour
{
  const float idealDistance = 2.118034f;
  const float acceptableThreshold = .2f;

  private const int boardHeight = 5;
  private const int boardWidth = 5;

  internal Space[] board;

  Peg _selectedPeg;
  public Peg selectedPeg
  {
    get
    {
      return _selectedPeg;
    }
    private set
    {
      if (selectedPeg != null)
      {
        selectedPeg.OnUnselect();
      }
      _selectedPeg = value;
      if (selectedPeg != null)
      {
        selectedPeg.OnSelect();
      }
    }
  }

  public static PegTriangle instance;

  [SerializeField]
  private Peg pegPrefab;

  [SerializeField]
  private Space spacePrefab;

  public float desiredMovedDistance;

  private Space OnMouse;

  protected void Awake()
  {
    instance = this;
  }

  protected void OnDestroy()
  {
    instance = null;
  }

  void Start()
  {
    GenerateBoard();
  }

  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      for (int i = 0; i < board.Length; i++)
      {
        Space selectedSpace = board[i];
        if (Vector2.Distance(selectedSpace.transform.position, mousePosition) <= .3f)
        {
          if (selectedSpace.associatedObject != null)
          { // Changing selected peg
            selectedPeg = selectedSpace.associatedObject;
            break;
          }
          else if (selectedPeg != null)
          { // Attempting a move
            float distance = (selectedSpace.transform.position - selectedPeg.transform.position).magnitude;
            if (Mathf.Abs(distance - idealDistance) < acceptableThreshold)
            { // This is a valid move distance and angle

              Vector3 midpoint = Vector3.Lerp(selectedSpace.transform.position, selectedPeg.transform.position, .5f);
              Space midSpace = FindSelectedSpace(midpoint);
              if (midSpace.associatedObject != null)
              { // This is a valid skip over
                selectedSpace.PlacePeg(selectedPeg);
                selectedPeg = null;
                midSpace.DestroyPeg();
                CheckWinCondition();
              }
              else
              {
                print("You must jump over a peg");
              }
            }
            else
            {
              print("Invalid move distance or angle, try again.");
            }
          }
        }
      }
    }
  }

  private void CheckWinCondition()
  {
    int pegCount = 0;
    for (int i = 0; i < board.Length; i++)
    {
      if(board[i].associatedObject != null)
      {
        pegCount++;
        if (pegCount > 1)
        {
          break;
        }
      }
    }

    if(pegCount == 1)
    {
      print("-------------------");
      print("--------YOU--------");
      print("--------WIN--------");
      print("-------------------");
    }
  }

  void GenerateBoard()
  {
    int slots = 0;
    for (int i = 0; i < boardHeight; i++)
    {
      slots += boardWidth - i;
    }

    board = new Space[slots];

    for (int i = 0; i < slots; i++)
    {
      board[i] = Instantiate(spacePrefab, transform, false);
      board[i].transform.localPosition = GetPegWorldPosition(i);
      Peg peg = Instantiate(pegPrefab, board[i].transform, false);
      peg.transform.localPosition = Vector3.zero;
      board[i].PlacePeg(peg);
    }

    int tileToDrop = SelectRandomTileToDrop();
    board[tileToDrop].DestroyPeg();
  }

  private int SelectRandomTileToDrop()
  {
    int tileToDrop = -1;
    while (true)
    {
      tileToDrop = UnityEngine.Random.Range(0, board.Length);
      Space spaceToDrop = board[tileToDrop];
      // This loop tests each of the possible move directions to see if this is a valid start
      for (int i = 0; i < 6; i++)
      {
        float angle = i * 60 + 30;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector2 testPosition = spaceToDrop.transform.position
          + rotation * new Vector2(0, idealDistance);
        Debug.DrawLine(spaceToDrop.transform.position, testPosition, Color.red, 1);
        Space space = FindSelectedSpace(testPosition);
        if (space != null)
        {
          return tileToDrop;
        }
      }
    }
  }

  private Vector2 GetPegWorldPosition(int peg)
  {
    int count = 0;
    for (int i = 0; i < boardHeight; i++)
    {
      for (int j = 0; j < boardWidth - i; j++)
      {
        Vector2 temp = new Vector2(j - ((boardWidth - (i + 0.5f)) / 2), i);
        if (i == 2 && j == 0)
        {
          desiredMovedDistance = Vector2.Distance(board[0].transform.position, temp);
        }
        if (count == peg)
        {
          return temp;
        }
        count++;
      }
    }
    return Vector2.zero;
  }

  Space FindSelectedSpace(Vector2 pos)
  {
    for (int i = 0; i < board.Length; i++)
    {
      if (Vector2.Distance(board[i].transform.position, pos) <= 0.3f)
      {
        return board[i];
      }
    }
    return null;
  }
  private static Vector2 BoardToRowCol(int pos)
  {
    // Return vector where X is row and Y is column position
    // Figure out where the array starts 
    // Map 


    return Vector2.zero;
  }
}
