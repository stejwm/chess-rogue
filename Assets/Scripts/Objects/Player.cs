using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class Player: MonoBehaviour
{
    public int playerCoins = 0;
    public int playerBlood = 0;
    public List<GameObject> pieces;
    public List<GameObject> capturedPieces = new List<GameObject>();
    public List<BoardPosition> openPositions = new List<BoardPosition>();
    public Player(List<GameObject> pieces)
    {
        this.pieces=pieces;
    }

    public abstract void Initialize();
    public abstract void MakeMove(ChessMatch match);

    public void Destroy(){
        foreach (GameObject item in pieces)
        {
            Destroy(item);
        }
        Destroy(this);
    }

    public virtual void DestroyPieces(){
        foreach (GameObject item in pieces)
        {
            Destroy(item);
        }
        //Destroy(this);
    }
}



