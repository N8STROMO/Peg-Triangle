using UnityEngine;
public class PegTriangle : MonoBehaviour
{
  #region Data
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
  #endregion

  #region Events
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
      // We need to convert the Input.mousePosition to world space to then compare it to all slot positions.
      Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      for (int i = 0; i < board.Length; i++)
      {
        // Create a variable to assign our current slot in the for loop to.
        Space selectedSpace = board[i];
        if (Vector2.Distance(selectedSpace.transform.position, mousePosition) <= .3f)
        {
          // Successfully found a slot close enough to the mouse.

          // Check if the slot has a peg or not.
          if (selectedSpace.associatedObject != null)
          { // Changing selected peg
            selectedPeg = selectedSpace.associatedObject;
            break;
          }
          // Otherwise we must have clicked an empty space and therefor proceed down branch.
          else if (selectedPeg != null)
          { // Attempting a move
            // .magnitude will return the length of a vector (Example: (1,0,0) = 1).
            float distance = (selectedSpace.transform.position - selectedPeg.transform.position).magnitude; // This is the distance between our initial selection and final move point.
            if (Mathf.Abs(distance - idealDistance) < acceptableThreshold) // Check the distance of our move to ensure it is valid.
            { // This is a valid move distance and ANGLE?

              // TODO Lerp; see documentation
              Vector3 midpoint = Vector3.Lerp(selectedSpace.transform.position, selectedPeg.transform.position, .5f);
              Space midSpace = FindSelectedSpace(midpoint); // Finding the slot associated with our midpoint.
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
  #endregion

  #region Helpers
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

    // One random pegs disappears at start of game.
    int tileToDrop = SelectRandomTileToDrop();
    board[tileToDrop].DestroyPeg();
  }

  // Goal: remove a random tile from the board.
  // Challenge: it can't be just any tile.  remove one from the center and no moves are possible.
  private int SelectRandomTileToDrop()
  {
    // This is the random tile to remove from the board... if it turns out to be a valid selection.
    // This could have been declared below, inside the while loop.
    int tileToDrop = -1;
    // Keep trying until a valid selection is found.
    while (true)
    {
      // Pick any random tile to drop.
      tileToDrop = Random.Range(0, board.Length);
      // This is the space we randomly selected. We know this is a valid space... 
      // but we don't know if removing this space results in a playable game or not yet.
      Space spaceToDrop = board[tileToDrop];
      // This loop tests each of the possible move directions to see if this is a valid start.
      // The board is a hex design...so at best, a space has 6 possible moves to choose from.
      for (int i = 0; i < 6; i++)
      {
        // Instead of doing hex coordinates (see cube coordinates here http://redblobgames.com/grids/hexagons).
        // we simplify by using angles.  A possible move is 30 degrees, 90, 150, 210, 270, 330.
        float angle = i * 60 + 30;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        // We selected a random empty space.  To know if the game has any valid moves, we need to know if there is a move which can land on this empty spot.
        // The starting position which lands here is this position plus...
        // The angle we selected (changes each iteration of this for loop) times the distance a move travels.
        // This multiplication gives us the offset from the current tile's position.
        Vector2 testPosition = spaceToDrop.transform.position
          + rotation * new Vector2(0, idealDistance);
        // This shows the all tests performed in the scene view for 1 second (no impact on the final game).
        Debug.DrawLine(spaceToDrop.transform.position, testPosition, Color.red, 1);
        // All we need to know is if this starting position actually exists.
        // If a center space was selected to remove, test position would never be a valid space and we loop to try another random space.
        Space space = FindSelectedSpace(testPosition);
        if (space != null)
        {
          // The space, representing a possible starting move, exists... we can remove this tile.
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
  #endregion
}
