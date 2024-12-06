using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class MovementProfile : ScriptableObject
{
    public abstract List<BoardPosition> GetValidMoves(Chessman piece);
    public abstract List<BoardPosition> GetValidSupportMoves(Chessman piece);
    public abstract List<Vector2Int> GetDirections(Chessman piece);
}