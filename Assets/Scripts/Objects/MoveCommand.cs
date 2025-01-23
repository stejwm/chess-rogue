using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand
{
    public int x;
    public int y;
    public Chessman piece;
    public MoveCommand(Chessman piece, int xPositiion, int yPosition){
        this.piece=piece;
        this.x=xPositiion;
        this.y=yPosition;
    }
    public void SetPosition(int x, int y){
        this.x=x;
        this.y=y;
    }
    public override bool Equals(object obj)
    {
        var item = obj as MoveCommand;

        if (item == null)
        {
            return false;
        }

        return Equals(piece, item.piece) && this.x == item.x && this.y==item.y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(piece?.GetHashCode() ?? 0, this.x, this.y);
    }
}
