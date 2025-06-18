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
    public int soulBondedPieces = 0;
    public PieceColor color;
    public List<GameObject> pieces;
    public List<GameObject> capturedPieces = new List<GameObject>();
    public List<GameObject> inventoryPieces = new List<GameObject>();
    public List<Tile> openPositions = new List<Tile>();
    public List<KingsOrder> orders = new List<KingsOrder>();
    private static Rand rng = new Rand();
    private int abandonedPieces = 0;
    public Dictionary<Rarity, int> RarityWeights = new Dictionary<Rarity, int>()
        {
            { Rarity.Common, 55 },
            { Rarity.Uncommon, 35 },
            { Rarity.Rare, 10 }
        };

    public int AbandonedPieces { get => abandonedPieces; set => abandonedPieces = value; }

    public Player(List<GameObject> pieces)
    {
        this.pieces = pieces;
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
    public virtual void LevelUp(int level, EnemyType enemyType){
        switch(enemyType){
            case EnemyType.Knights:
                level=level*2;
                break;
            case EnemyType.RoyalFamily:
                level=level*5;
                break;
            case EnemyType.Fortress:
                level+=2;
                break;
            case EnemyType.Thieves:
                playerCoins= UnityEngine.Random.Range(6,12)*level;
                break;
            default:
                break;
        }
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
    

    public virtual Chessman GetHighestCapturer(){
        Chessman highestCapturer = null;
        int maxCaptures = -1;

        foreach (GameObject piece in pieces) {
            Chessman cm = piece.GetComponent<Chessman>();
            if (cm.captures > maxCaptures) {
                maxCaptures = cm.captures;
                highestCapturer = cm;
            }
        }
        
        return highestCapturer;
    }

    public virtual Chessman GetHighestCaptured(){
        Chessman highest = null;
        int maxValue = -1;
        foreach (GameObject piece in pieces) {
            Chessman cm = piece.GetComponent<Chessman>();
            if (cm.captured > maxValue) {
                maxValue = cm.captured;
                highest = cm;
            }
        }
        return highest;
    }

    public virtual Chessman GetHighestBounced(){
        Chessman highest = null;
        int maxValue = -1;
        foreach (GameObject piece in pieces) {
            Chessman cm = piece.GetComponent<Chessman>();
            if (cm.bounced > maxValue) {
                maxValue = cm.bounced;
                highest = cm;
            }
        }
        return highest;
    }

    public virtual Chessman GetHighestBouncing(){
        Chessman highest = null;
        int maxValue = -1;
        foreach (GameObject piece in pieces) {
            Chessman cm = piece.GetComponent<Chessman>();
            if (cm.bouncing > maxValue) {
                maxValue = cm.bouncing;
                highest = cm;
            }
        }
        return highest;
    }

    public virtual Chessman GetHighestSupportsDefensive(){
        Chessman highest = null;
        int maxValue = -1;
        foreach (GameObject piece in pieces) {
            Chessman cm = piece.GetComponent<Chessman>();
            if (cm.supportsDefending > maxValue) {
                maxValue = cm.supportsDefending;
                highest = cm;
            }
        }
        return highest;
    }

    public virtual Chessman GetHighestSupportsAttacks(){
        Chessman highest = null;
        int maxValue = -1;
        foreach (GameObject piece in pieces) {
            Chessman cm = piece.GetComponent<Chessman>();
            if (cm.supportsAttacking > maxValue) {
                maxValue = cm.supportsAttacking;
                highest = cm;
            }
        }
        return highest;
    }
}



