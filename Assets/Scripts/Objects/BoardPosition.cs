using System;
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
    public static string ConvertToChessNotation(int x, int y)
    {
        // Ensure x and y are within the chessboard range
        if (x < 0 || x > 7 || y < 0 || y > 7)
        {
            throw new Exception("x and y must be within the range 0 to 7.");
        }

        // Convert x to file (a-h)
        char file = (char)('a' + x);

        // Convert y to rank (1-8)
        int rank = y + 1;

        // Combine file and rank into chess notation
        return $"{file}{rank}";
    }
    public static string ConvertToChessNotation(Tile tile)
    {
        // Ensure x and y are within the chessboard range
        if (tile.X < 0 || tile.X > 7 || tile.Y < 0 || tile.Y > 7)
        {
            throw new Exception("x and y must be within the range 0 to 7.");
        }

        // Convert x to file (a-h)
        char file = (char)('a' + tile.X);

        // Convert y to rank (1-8)
        int rank = tile.Y + 1;

        // Combine file and rank into chess notation
        return $"{file}{rank}";
    }

    public static bool IsPositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= 8 || y >= 8) return false;
        return true;
    }

    public override bool Equals(object obj)
    {
        var item = obj as BoardPosition;

        if (item == null)
        {
            return false;
        }

        return this.x == item.x && this.y == item.y;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}
