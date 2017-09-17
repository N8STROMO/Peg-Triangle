// TODO Moving pegs within triangle

 using UnityEngine;

public class Space
{
  public bool filled = false;
  public GameObject associatedObject;
  public Vector2 worldPosition;
}

public class PegTriangle : MonoBehaviour {

  private const int boardHeight = 5;
  private const int boardWidth = 5;
  public Space[] board;
  private int turnCount = 0;

  [SerializeField]
  private GameObject pegPrefab;
  [SerializeField]
  private GameObject slot;

  public float desiredMovedDistance;

	// Use this for initialization
	void Start ()
  {
    GenerateBoard();
	}
	
	// Update is called once per frame
	void Update ()
  { 
    if (turnCount == 0)
    {
      if (Input.GetMouseButtonDown(0))
      {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        for (int i = 0; i < board.Length; i++)
        {
          if (Vector2.Distance(board[i].associatedObject.transform.position, mousePosition) <= .3f) 
          {
            board[i].filled = false;
            board[i].associatedObject.SetActive(false);
            turnCount++;
            break;
          }
        }
      }
    }
	}

  void GenerateBoard()
  {
    int slots = 0;
    for(int i = 0; i < boardHeight; i++)
    {
      slots += boardWidth - i;
    }

    board = new Space[slots];

    for (int i = 0; i < slots; i++)
    {
      board[i] = new Space();
      board[i].filled = true;
      GameObject peg = Instantiate(pegPrefab, transform, false);
      peg.transform.localPosition = GetPegWorldPosition(i);
      board[i].associatedObject = peg;
      board[i].worldPosition = GetPegWorldPosition(i);
    }
  }

  private Vector2 GetPegWorldPosition(int peg)
  {
    int count = 0;
    for (int i = 0; i < boardHeight; i++)
    {
      for (int j = 0; j <boardWidth - i; j++)
      {
        Vector2 temp = new Vector2(j - ((boardWidth - (i + 0.5f)) / 2), i);
        if (i == 2 && j == 0)
        {
          desiredMovedDistance = Vector2.Distance(board[0].worldPosition, temp);
        }
        if(count == peg)
        {
          return temp;
        }
        count++;
      }
    }
  return Vector2.zero;
  }
}
