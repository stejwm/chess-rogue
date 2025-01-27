using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class PlayerAgent : Agent
{
    public List<GameObject> pieces;
    public PieceColor color;
    //public ChessMatch currentMatch;
    public Dictionary<int,MoveCommand> moveCommands=new Dictionary<int, MoveCommand>();

    public void StartUp(){
        Debug.Log("Starting up agent");
        Game._instance.OnGameEnd.AddListener(GameEnd);
        Game._instance.OnPieceCaptured.AddListener(CaptureReward);
        Game._instance.OnPieceBounced.AddListener(BounceReward);
        
        
        GenerateMoveCommandDictionary();

    }

    public void ShutDown(){
        moveCommands.Clear();
        Game._instance.OnGameEnd.RemoveListener(GameEnd);
        Game._instance.OnPieceCaptured.RemoveListener(CaptureReward);
        Game._instance.OnPieceBounced.RemoveListener(BounceReward);
    }
    public void GenerateMoveCommandDictionary()
    {
        //Debug.Log("Generating dictionary of all possible moves");
        //Debug.Log(pieces==null);

        int index = 0;  // This will go from 0 to 1023
        foreach (GameObject pieceObject in pieces)
        {
            Chessman piece = pieceObject.GetComponent<Chessman>();

            // Loop through all 64 positions on the board
            for (int x = 0; x < 8; x++)  // x-coordinate (0 to 7)
            {
                for (int y = 0; y < 8; y++)  // y-coordinate (0 to 7)
                {
                    // Create a MoveCommand for this piece and destination
                    MoveCommand moveCommand = new MoveCommand(piece, x, y);
                    moveCommands.Add(index, moveCommand);

                    index++;  // Increment the index
                }
            }
        }
        //Debug.Log("Move Dictionary generated. Count="+moveCommands.Count);
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
        //Debug.Log("branch 0: "+selectedMoveCommandIndex);
        MoveCommand selectedMoveCommand = GetMoveCommandFromIndex(selectedMoveCommandIndex);
        Debug.Log("Action Recieved attempting to execute move from "+ BoardPosition.ConvertToChessNotation(selectedMoveCommand.piece.xBoard, selectedMoveCommand.piece.yBoard)+" to "+BoardPosition.ConvertToChessNotation(selectedMoveCommand.x, selectedMoveCommand.y));
        Game._instance.currentMatch.ExecuteTurn(selectedMoveCommand.piece, selectedMoveCommand.x, selectedMoveCommand.y);
        
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
                    sensor.AddOneHotObservation((int)piece.type, 7);
                    sensor.AddOneHotObservation((int)piece.color, 3);

                    // Add other stats
                    sensor.AddObservation(piece.attack);
                    sensor.AddObservation(piece.attackBonus);
                    sensor.AddObservation(piece.defense);
                    sensor.AddObservation(piece.defenseBonus);
                    sensor.AddObservation(piece.support);
                    sensor.AddObservation(piece.supportBonus);
                }
                else{
                    sensor.AddOneHotObservation((int)PieceType.None, 7); // 7 categories
                    sensor.AddOneHotObservation((int)PieceColor.None, 3);
                    sensor.AddObservation(0);
                    sensor.AddObservation(0);
                    sensor.AddObservation(0);
                    sensor.AddObservation(0);
                    sensor.AddObservation(0);
                    sensor.AddObservation(0);
                }
            }
        }
            
        
    }

    public void GameEnd(PieceColor color){
        if(this.color==color){
            SetReward(1f);
            Debug.Log(color+"Won ! Recieved 1 reward");
            EndEpisode();
            StartCoroutine(ReloadScene());
        }
        else{
            SetReward(-1f);
            Debug.Log(color+"Lost ! Recieved -1 reward");
        }
        
    }
    private IEnumerator ReloadScene()
    {
        yield return null; // Allow EndEpisode to complete
        //SceneManager.LoadScene(0);
    }

    public void CaptureReward(Chessman attacker){
        if(attacker.color==color){
            //Debug.Log("Capture +");
            SetReward(0.3f);
        }
        else{
            SetReward(-0.3f);
        }
    }

    public void BounceReward(Chessman attacker, Chessman defender, bool didBounceReduce){
        if(attacker.color==color && didBounceReduce)
            SetReward(0.1f);
        else{
            SetReward(-0.1f);
        }
    }
}
