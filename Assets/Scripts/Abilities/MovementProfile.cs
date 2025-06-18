using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class MovementProfile
{
    protected Board board;
    public MovementProfile(Board board)
    {
        this.board = board;
    }
    public abstract List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture = false);
    public abstract List<Tile> GetValidSupportMoves(Chessman piece);
    public abstract List<Vector2Int> GetDirections(Chessman piece);
}