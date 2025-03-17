using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<GameObject> myPieces;

    //current turn
    public static InventoryManager _instance;


    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenInventory(){
        gameObject.SetActive(true);
        Game._instance.togglePieceColliders(Game._instance.opponent.pieces, false);
        Game._instance.togglePieceColliders(Game._instance.hero.pieces, true);

        Game._instance.CreateCards();
        myPieces=Game._instance.hero.pieces;

        foreach (GameObject piece in myPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
                piece.GetComponent<Chessman>().flames.GetComponent<Renderer>().sortingOrder=6;
            }
            
        }
    }

    public void CloseInventory(){
        Game._instance.ClearCard();
        Game._instance.ClearPiece();
        foreach (GameObject piece in myPieces)
        {
            if (piece !=null && piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 1;
                piece.GetComponent<Chessman>().flames.GetComponent<Renderer>().sortingOrder=2;
            }
        }
        Game._instance.CloseReward();
        gameObject.SetActive(false);
    }


}
