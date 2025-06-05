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
    public Player player;
    public List<GameObject> pieces;
    public PieceColor color;
    //public ChessMatch currentMatch;
    public Dictionary<int,MoveCommand> moveCommands=new Dictionary<int, MoveCommand>();
    public Dictionary<MoveCommand,int> reverseMoveCommands=new Dictionary<MoveCommand, int>();
    private List<MoveCommand> moveHistory = new List<MoveCommand>();
    public Chessman selectedPiece;
    public BoardPosition destinationPosition;
    float previousState = 0f;

    private const int MaxRepetitions = 8;

    public void StartUp(){
        Debug.Log("Starting up agent");
        Game._instance.OnGameEnd.AddListener(GameEnd);
        
        
        

    }

    public void ShutDown(){
        moveCommands.Clear();
        Game._instance.OnGameEnd.RemoveListener(GameEnd);
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

                Tile tile = BoardManager._instance.GetTileAt(actualX, actualY); // Get the tile at this position
                Chessman piece = tile.getPiece(); // Get the piece on the tile (can be null)

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
                        if (piece != null)
                            reverseMoveCommands.Add(moveCommand, index);


                        index++; // Increment the index
                    }
                }
            }
        }
        previousState = Game._instance.currentMatch.CalculateBoardState(player);

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
        if (selectedMoveCommandIndex == -1)
            return;
        //Debug.Log("branch 0: "+selectedMoveCommandIndex);
        MoveCommand selectedMoveCommand = GetMoveCommandFromIndex(selectedMoveCommandIndex);

        moveHistory.Add(selectedMoveCommand);
        if (moveHistory.Count > MaxRepetitions)
            moveHistory.RemoveAt(0);

        // Check if the last few moves form a back-and-forth pattern
        if (IsRepeatingPattern())
        {
            GameEnd(PieceColor.None);
        }

        Debug.Log("Action Recieved attempting to execute move from " + BoardPosition.ConvertToChessNotation(selectedMoveCommand.piece.xBoard, selectedMoveCommand.piece.yBoard) + " to " + BoardPosition.ConvertToChessNotation(selectedMoveCommand.x, selectedMoveCommand.y));
        Game._instance.currentMatch.ExecuteTurn(selectedMoveCommand.piece, selectedMoveCommand.x, selectedMoveCommand.y);
        float currentState = Game._instance.currentMatch.CalculateBoardState(selectedMoveCommand.piece.owner);
        Debug.Log("Current State: " + currentState);
        Debug.Log($"State Change: {currentState - previousState}");
        AddReward( currentState - previousState);
        previousState = currentState;
        
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
        //Debug.Log("Starting Masking");
        List<int> validIndexes = new List<int>();
        //Debug.Log("movecommands count="+moveCommands.Count);
        //Debug.Log("pieces count="+pieces.Count);
        foreach(KeyValuePair<int, MoveCommand> entry in moveCommands)
        {
            //Debug.Log(entry.Value);
            foreach(GameObject pieceObject in pieces){
                Chessman selectedPiece = pieceObject.GetComponent<Chessman>();
                if(!selectedPiece.isValidForAttack)
                    continue;
                selectedPiece.SetValidMoves();
                var moves = selectedPiece.GetAllValidMoves();
                //Debug.Log(moves.Count);
                
                foreach (var move in moves)
                {
                    //Debug.Log(entry);
                    MoveCommand testCommand = new MoveCommand(selectedPiece,move.x,move.y);
                    //Debug.Log("Valid move: "+entry.Value.piece.name+" to "+entry.Value.x+","+entry.Value.y);
                    //Debug.Log("Test move: "+selectedPiece.name+" to "+testCommand.x+","+testCommand.y);
                    if(testCommand.Equals(entry.Value)){
                        validIndexes.Add(entry.Key);
                    }
                }
            }
            
        }
        //Debug.Log("Found all valid move commands. Count = "+validIndexes.Count);
        foreach(int index in moveCommands.Keys)
        {
            if(!validIndexes.Contains(index))
                actionMask.SetActionEnabled(0,index,false);
            //else
                //Debug.Log("Not masking: "+index);
        }
        //Debug.Log("Actions masked");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for (int k = 0; k < Game._instance.currentMatch.GetPositions().GetLength(0); k++){
            for (int l = 0; l < Game._instance.currentMatch.GetPositions().GetLength(1); l++){
                if(Game._instance.currentMatch.GetPositions()[k, l] !=null){
                    GameObject item = Game._instance.currentMatch.GetPositions()[k, l];
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
            int index = Game._instance.AllAbilities.IndexOf(ability);
            abilitiesObservation[index] = 1; // Mark the ability as present
        }

        return abilitiesObservation;
    }

    public void GameEnd(PieceColor color){
        if(this.color==color){
            SetReward(10f);
            Debug.Log(color+"Won ! Recieved 1 reward");
            if (!Game._instance.endEpisode){
                Game._instance.endEpisode = true;
                //EndEpisode();
                //StartCoroutine(ReloadScene());
            }
        }
        else{
            SetReward(-10f);
            if (!Game._instance.endEpisode){
                Game._instance.endEpisode = true;
            }
            Debug.Log(this.color+"Lost ! Recieved -1 reward");
        }
        
    }
    private IEnumerator ReloadScene()
    {
        yield return null; // Allow EndEpisode to complete
        SceneManager.LoadScene(1);
    }



}
