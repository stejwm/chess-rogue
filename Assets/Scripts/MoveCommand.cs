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

        return this.piece.Equals(item.piece) && this.x == item.x && this.y==item.y;
    }
}
