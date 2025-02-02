using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Rand= System.Random;

public abstract class Player: MonoBehaviour
{
    public int playerCoins = 0;
    public int playerBlood = 0;
    public PieceColor color;
    public List<GameObject> pieces;
    public List<GameObject> capturedPieces = new List<GameObject>();
    public List<GameObject> inventoryPieces = new List<GameObject>();
    public List<BoardPosition> openPositions = new List<BoardPosition>();
    private static Rand rng = new Rand();
    public Player(List<GameObject> pieces)
    {
        this.pieces=pieces;
    }

    public abstract void Initialize();
    public abstract void MakeMove(ChessMatch match);

    public virtual void CreateMoveCommandDictionary(){}

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
    public virtual void LevelUp(int level){
        for (int i =0; i<level; i++)
            foreach (GameObject piece in pieces)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                switch (rng.Next(3)){
                    case 0:
                        cm.defense+=1;
                        break;
                    case 1:
                        cm.attack+=1;
                        break;
                    case 2:
                        cm.support+=1;
                        break;
                }
            }
    }
    public virtual void RandomAbilities(){
        
        foreach (GameObject piece in pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            int index = rng.Next(30);
            if (Game._instance.AllAbilities.Count>index){
                cm.AddAbility(Game._instance.AllAbilities[index].Clone());
            }
        }
    }
}



