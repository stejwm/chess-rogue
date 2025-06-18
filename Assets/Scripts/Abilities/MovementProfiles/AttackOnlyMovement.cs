using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
public class AttackOnlyMovement : MovementProfile
{
    MovementProfile oldProfile;
    GameManager game;
    public AttackOnlyMovement(Board board, MovementProfile old) : base(board) {oldProfile = old;}
    public override List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture) {
        List<Tile> moves = new List<Tile>();
        var StandardMoves =oldProfile.GetValidMoves(piece);
        foreach (var position in StandardMoves)
        {
            if (board.GetPieceAtPosition(position.X,position.Y)!=null)
                moves.Add(position);
        }
        if (allowFriendlyCapture)
            return moves;
        else
            return Movement.RemoveFriendlyPieces(board, moves,piece);
    }
    public override List<Tile> GetValidSupportMoves( Chessman piece){
        return oldProfile.GetValidSupportMoves(piece);
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldProfile.GetDirections(piece);
    } 
}