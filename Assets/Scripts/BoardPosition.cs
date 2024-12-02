using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPosition
{
    public int x;
    public int y;
    public BoardPosition(int xPositiion, int yPosition){
        this.x=xPositiion;
        this.y=yPosition;
    }
    public void SetPosition(int x, int y){
        this.x=x;
        this.y=y;
    }
    public BoardPosition GetPosition(){
        return this;
    }
}
