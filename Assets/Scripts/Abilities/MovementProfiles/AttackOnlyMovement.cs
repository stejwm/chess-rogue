using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
public class AttackOnlyMovement : MovementProfile
{
    MovementProfile oldProfile;
    Game game;
    public AttackOnlyMovement(MovementProfile old){
        oldProfile=old;
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
    }
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        List<BoardPosition> moves = new List<BoardPosition>();
        var StandardMoves =oldProfile.GetValidMoves(piece);
        foreach (var position in StandardMoves)
        {
            if (Game._instance.currentMatch.GetPieceAtPosition(position.x,position.y)!=null && Game._instance.currentMatch.GetPieceAtPosition(position.x,position.y).GetComponent<Chessman>().team!=piece.team)
                moves.Add(position);
        }
        return moves;
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return oldProfile.GetValidSupportMoves(piece);
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldProfile.GetDirections(piece);
    } 
}