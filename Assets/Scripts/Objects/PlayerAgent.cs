using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class PlayerAgent : Agent
{
    public List<GameObject> pieces;
    public PieceColor color;
    //public ChessMatch currentMatch;
    public Dictionary<int,MoveCommand> moveCommands=new Dictionary<int, MoveCommand>();
    public Dictionary<MoveCommand,int> reverseMoveCommands=new Dictionary<MoveCommand, int>();
    private List<MoveCommand> moveHistory = new List<MoveCommand>();
    public Chessman selectedPiece;
    public BoardPosition destinationPosition;
    [SerializeField] private Board board;

    private const int MaxRepetitions = 8;

    public void StartUp(){
        Debug.Log("Starting up agent");
    }

    public void ShutDown(){
        moveCommands.Clear();
    }
    public void CreateMoveCommandDictionary(){
        GenerateMoveCommandDictionary(color==PieceColor.White);
    }
    
    public void GenerateMoveCommandDictionary(bool isPlayerWhite)
    {
        Debug.Log("Generating Move Commands");
        moveCommands.Clear(); // Clear the dictionary before generating new commands
        reverseMoveCommands.Clear();
        int index = 0; // This will go from 0 to 1023

        // Loop through the first 3 rows from the player's perspective
        for (int relativeY = 0; relativeY < 3; relativeY++) // relativeY: 0, 1, 2
        {
            int actualY = isPlayerWhite ? relativeY : 7 - relativeY; // Adjust for color perspective

            for (int relativeX = 0; relativeX < 8; relativeX++) // relativeX: 0 to 7
            {
                int actualX = isPlayerWhite ? relativeX : 7 - relativeX; // Adjust for left-to-right perspective

                Tile tile = board.GetTileAt(actualX, actualY); // Get the tile at this position
                
                Chessman piece = board.GetChessmanAtPosition(tile); // Get the piece on the tile (can be null)
                //Debug.Log($"Generating commands for piece at {actualX}, {actualY}: {(piece==null ? "None" : piece.name)}");
                // Loop through all 64 positions on the board for possible destinations
                for (int destRelativeX = 0; destRelativeX < 8; destRelativeX++)
                {
                    for (int destRelativeY = 0; destRelativeY < 8; destRelativeY++)
                    {
                         // Adjust destination coordinates for perspective
                        int adjustedDestX = isPlayerWhite ? destRelativeX : 7 - destRelativeX;
                        int adjustedDestY = isPlayerWhite ? destRelativeY : 7 - destRelativeY;

                        // Create a MoveCommand for this piece and destination
                        MoveCommand moveCommand = new MoveCommand(piece, adjustedDestX, adjustedDestY);
                        moveCommands.Add(index, moveCommand);
                        if(piece!=null)
                            reverseMoveCommands.Add(moveCommand, index);
                        

                        index++; // Increment the index
                    }
                }
            }
        }

    }
    public MoveCommand GetMoveCommandFromIndex(int index)
    {
        if (moveCommands.ContainsKey(index))
        {
            return moveCommands[index];
        }
        return null;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        
        var selectedMoveCommandIndex = actions.DiscreteActions[0];
        if(selectedMoveCommandIndex == -1)
            return;
        //Debug.Log("branch 0: "+selectedMoveCommandIndex);
        MoveCommand selectedMoveCommand = GetMoveCommandFromIndex(selectedMoveCommandIndex);

        moveHistory.Add(selectedMoveCommand);
        if (moveHistory.Count > MaxRepetitions)
            moveHistory.RemoveAt(0);

        // Check if the last few moves form a back-and-forth pattern
        if (IsRepeatingPattern())
        {
            //GameEnd(PieceColor.None);
        }

        Debug.Log("Action Recieved attempting to execute move from "+ BoardPosition.ConvertToChessNotation(selectedMoveCommand.piece.xBoard, selectedMoveCommand.piece.yBoard)+" to "+BoardPosition.ConvertToChessNotation(selectedMoveCommand.x, selectedMoveCommand.y));
        board.CurrentMatch.ExecuteTurn(selectedMoveCommand.piece, selectedMoveCommand.x, selectedMoveCommand.y);
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        // Ensure the player has selected both a piece and a destination
        if (selectedPiece != null && destinationPosition != null)
        {
            // Map selected piece and destination to action space
            var command = new MoveCommand(selectedPiece, destinationPosition.x, destinationPosition.y);
            
            discreteActions[0] = reverseMoveCommands[command];

            // Reset selection after completing the action
            selectedPiece = null;
            destinationPosition = null;
        }
        else
        {
            discreteActions[0] = -1; 
        }
    }

    bool IsRepeatingPattern()
    {
        if (moveHistory.Count < MaxRepetitions)
            return false;

        return moveHistory[0] == moveHistory[2] && moveHistory[1] == moveHistory[3] && moveHistory[4] == moveHistory[6] && moveHistory[5] == moveHistory[7];
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        Debug.Log("Move commands count: " + moveCommands.Count);
        List<int> validIndexes = new List<int>();
        
        foreach(GameObject pieceObject in pieces){
            Chessman selectedPiece = pieceObject.GetComponent<Chessman>();
            if (!selectedPiece.isValidForAttack)
                continue;
            var moves = selectedPiece.GetValidMoves();
            foreach (var move in moves)
            {
                MoveCommand testCommand = new MoveCommand(selectedPiece, move.X, move.Y);
                if (reverseMoveCommands.TryGetValue(testCommand, out int index))
                {
                    validIndexes.Add(index);
                }
            }
            
        }
            
        foreach (int index in moveCommands.Keys)
        {
            if (!validIndexes.Contains(index))
                actionMask.SetActionEnabled(0, index, false);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for (int k = 0; k < board.Positions.GetLength(0); k++){
            for (int l = 0; l < board.Positions.GetLength(1); l++){
                if(board.Positions[k, l] !=null){
                    GameObject item = board.Positions[k, l];
                    Chessman piece = item.GetComponent<Chessman>();
                    sensor.AddObservation(piece.xBoard);
                    sensor.AddObservation(piece.yBoard);

                    sensor.AddOneHotObservation((int)piece.type, 7);
                    sensor.AddOneHotObservation((int)piece.color, 3);

                    // Add other stats
                    sensor.AddObservation(piece.attack);
                    sensor.AddObservation(piece.attackBonus);
                    sensor.AddObservation(piece.defense);
                    sensor.AddObservation(piece.defenseBonus);
                    sensor.AddObservation(piece.support);
                    sensor.AddObservation(piece.supportBonus);
                    sensor.AddObservation(GetAbilityObservation(piece));
                }
                else{
                    sensor.AddObservation(-1);
                    sensor.AddObservation(-1);
                    sensor.AddOneHotObservation((int)PieceType.None, 7); // 7 categories
                    sensor.AddOneHotObservation((int)PieceColor.None, 3);
                    sensor.AddObservation(-1);
                    sensor.AddObservation(-1);
                    sensor.AddObservation(-1);
                    sensor.AddObservation(-1);
                    sensor.AddObservation(-1);
                    sensor.AddObservation(-1);
                    sensor.AddObservation(new float[64]);
                }
            }
        }
            
        
    }
    public float[] GetAbilityObservation(Chessman piece)
    {
        float[] abilitiesObservation = new float[64];

         foreach (Ability ability in piece.abilities)
        {
            int index = AbilityDatabase.Instance.GetIndexFromAbility(ability);
            abilitiesObservation[index] = 1; 
        }

        return abilitiesObservation;
    }

    private IEnumerator ReloadScene()
    {
        yield return null; // Allow EndEpisode to complete
        SceneManager.LoadScene(1);
    }

    public void CaptureReward(Chessman attacker, Chessman defender){
        if(attacker.color==color){
            SetReward(GetPieceReward(defender));
        }
        else{
            SetReward(-GetPieceReward(defender));
        }
    }

    public float GetPieceReward(Chessman piece){
        float reward = 0f;
        switch (piece.type)
            {
                case PieceType.Queen:
                    reward+= 0.9f;
                    break;
                case PieceType.Knight:
                    reward+= 0.3f;
                    break;               
                case PieceType.Bishop:
                    reward+= 0.3f;              
                    break;
                case PieceType.King:
                    reward+= 1f;            
                    break;
                case PieceType.Rook:
                    reward+= 0.5f;               
                    break;
                case PieceType.Pawn:
                    reward+= 0.1f;                
                    break;
                default:
                    break;
            
            }
        reward += piece.attack * 0.1f;
        reward+= piece.defense * 0.1f;
        reward+= piece.support * 0.1f;
        reward+= piece.releaseCost * 0.1f;

        return reward;
    }

    public void SupportReward(Chessman supporter, Chessman attacker, Chessman defender){
        if(supporter.color==color)
            SetReward(0.05f);
    }

    public void BounceReward(Chessman attacker, Chessman defender, bool didBounceReduce){
        if(attacker.color==color && didBounceReduce)
            SetReward(0.05f);
        else{
            SetReward(-0.05f);
        }
    }
}
