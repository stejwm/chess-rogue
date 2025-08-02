using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;


public class AIPlayer : Player
{
    private Board board;
    [SerializeField] protected PlayerAgent agent;


    public AIPlayer(List<GameObject> pieces) : base(pieces)
    {
        this.pieces = pieces;
    }
    public override void Initialize(Board board)
    {
        agent.pieces = pieces;
        agent.StartUp();
        openPositions = new List<Tile>();
        this.board = board;
    }
    public override void InitializeFromLoad(Board board) { }

    public override void CreateMoveCommandDictionary()
    {
        agent.CreateMoveCommandDictionary();
    }

    public void SetSelectedPiece(Chessman piece)
    {
        agent.selectedPiece = piece;
    }
    public void SetSelectedDestination(BoardPosition position)
    {
        agent.destinationPosition = position;
    }




    public void RequestDecision()
    {
        agent.RequestDecision();
    }

    public override void MakeMove(ChessMatch match)
    {
        //StartCoroutine(Move());
        StartCoroutine(DelayedMove());


    }

    public IEnumerator DelayedMove()
    {
        yield return new WaitForSeconds(Settings.Instance.WaitTime);
        Move();
    }
    public async void Move()
    {
        //yield return new WaitForSeconds(Settings.Instance.WaitTime);
        Debug.Log("Requested move from " + color);
        string fen = board.BoardToFEN();
        Debug.Log(fen);
        //string move = await board.StockFishEngine.GetBestMove(fen, 15);
        List<string> moves = await board.StockFishEngine.GetTopMoves(fen, 1);
        if (moves.Count == 0)
        {
            string possibleMoves = "";
            foreach (GameObject piece in pieces)
            {
                if (!(piece || piece.activeSelf))
                    continue;

                Chessman cm = piece.GetComponent<Chessman>();
                string startingPosition = BoardPosition.ConvertToChessNotation(board.GetTileAt(cm.xBoard, cm.yBoard));
                if (cm.isValidForAttack)
                {
                    foreach (Tile tile in cm.GetValidMoves())
                    {
                        string move = startingPosition + BoardPosition.ConvertToChessNotation(tile);
                        possibleMoves += move;
                        possibleMoves += " ";
                    }
                }
            }
            moves = await board.StockFishEngine.GetTopMoves(board.BoardToFEN(), 1, possibleMoves);
        }
        if (moves.Count == 0)
        {
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{PieceColor.Black}{PieceType.King}\" name=\"{PieceColor.Black}{PieceType.King}\"> hope is lost, finish this");
            board.CurrentMatch.NextTurn();
        }
        else
        {
            BoardPosition.ParseUCIMove(PickMoveBasedOnSkill(board.GetValidMovesFromEngineMoves(moves), board.Level), out int fromX, out int fromY, out int toX, out int toY);
            board.CurrentMatch.ExecuteTurn(board.GetChessmanAtPosition(fromX, fromY), toX, toY);
        }



    }
    public override void DestroyPieces()
    {

        base.DestroyPieces(); // Call the base class logic to destroy pieces.
        agent.ShutDown();         // Additional AI-specific logic.

    }
    
    public string PickMoveBasedOnSkill(List<string> topMoves, int skillLevel)
    {
        int moveCount = topMoves.Count;
        int degree = skillLevel * 2;
        // Invert skill level: lower skill = higher maxIndex
        int maxIndex = Mathf.RoundToInt(Mathf.Lerp(moveCount - 1, 0, degree / 10f));

        // Pick randomly between 0 and maxIndex
        int chosenIndex = UnityEngine.Random.Range(0, maxIndex + 1);
        Debug.Log($"Chose index {chosenIndex}: {topMoves[chosenIndex]}");
        return topMoves[chosenIndex];
    }
    
}