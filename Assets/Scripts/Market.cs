using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{
    public ArrayList myCapturedPieces= new ArrayList();
    public ArrayList myPieces= new ArrayList();
    public ArrayList opponentCapturedPieces = new ArrayList();
    public ArrayList opponentPieces = new ArrayList();
    public List<Chessman> selectedPieces = new List<Chessman>();
    public Game game;
    public GameObject controller;
    public TMP_Text bloodText;
    public TMP_Text coinText;


    //current turn
    public static MarketManager _instance;


    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void Start()
    {
        gameObject.SetActive(false);
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
    }

    public void OpenMarket(){
        

        Debug.Log("opening market");
        gameObject.SetActive(true);

        if(game.heroColor==PieceColor.White){
            myPieces=game.playerWhite;
            opponentPieces=game.playerBlack;
        }
        else{
            myPieces=game.playerBlack;
            opponentPieces=game.playerWhite;
        }
        foreach (GameObject obj in myPieces)
        {
            if (!obj.activeSelf){
                Debug.Log(obj.name);
                obj.SetActive(true);
                myCapturedPieces.Add(obj);
            }
        }
        foreach (GameObject obj in opponentPieces)
        {
            if (!obj.activeSelf){
                Debug.Log(obj.name);
                obj.SetActive(true);
                opponentCapturedPieces.Add(obj);
            }
        }
        /* var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>(); */
        if(myCapturedPieces.Count>0)
        foreach (GameObject piece in myCapturedPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                //Fetch the SpriteRenderer from the selected GameObject
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                //Change the sorting layer to the name you specify in the TextField
                //Changes to Default if the name you enter doesn't exist
                //rend.sortingLayerName = m_Name.stringValue;
                //Change the order (or priority) of the layer
                rend.sortingOrder = 5;
            }
            
        }
        if(opponentCapturedPieces.Count>0)
        foreach (GameObject piece in opponentCapturedPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                //Fetch the SpriteRenderer from the selected GameObject
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                //Change the sorting layer to the name you specify in the TextField
                //Changes to Default if the name you enter doesn't exist
                //rend.sortingLayerName = m_Name.stringValue;
                //Change the order (or priority) of the layer
                rend.sortingOrder = 5;
            }
            
        }
    }

    public void CloseMarket(){
        /* var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();
        game.ClearCard();
        game.ClearPiece();*/
        if(opponentCapturedPieces.Count>0)
        foreach (GameObject piece in opponentCapturedPieces)
        {
            if(piece){
                if (piece.GetComponent<SpriteRenderer>())
                {
                    //Fetch the SpriteRenderer from the selected GameObject
                    SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                    //Change the sorting layer to the name you specify in the TextField
                    //Changes to Default if the name you enter doesn't exist
                    //rend.sortingLayerName = m_Name.stringValue;
                    //Change the order (or priority) of the layer
                    rend.sortingOrder = 1;
                }
            }
            
        }
        if(myCapturedPieces.Count>0)
        foreach (GameObject piece in myCapturedPieces)
        {
            if(piece){
                if (piece.GetComponent<SpriteRenderer>())
                {
                    //Fetch the SpriteRenderer from the selected GameObject
                    SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                    //Change the sorting layer to the name you specify in the TextField
                    //Changes to Default if the name you enter doesn't exist
                    //rend.sortingLayerName = m_Name.stringValue;
                    //Change the order (or priority) of the layer
                    rend.sortingOrder = 1;
                }
            }
            
        }
        game.SetBoardActive();
        game.state=ScreenState.RewardScreen;
        myCapturedPieces.Clear();
        opponentCapturedPieces.Clear();
        selectedPieces.Clear();
        InventoryManager._instance.OpenInventory();
        game.NewOpponent();
        gameObject.SetActive(false);
        
        
    }

    public void ReleasePieces(){
        foreach (Chessman item in selectedPieces)
        {
            game.playerCoins+= item.releaseCost;
            SpriteRenderer sprite= item.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            item.gameObject.SetActive(false);
        }
        coinText.text = ": "+game.playerCoins;
        selectedPieces.Clear();
    }

    public void KillPieces(){
        
        foreach (Chessman item in selectedPieces)
        {
            game.playerBlood+= item.blood;
            opponentCapturedPieces.Remove(item);
            item.gameObject.SetActive(false);
        }
        bloodText.text = ": "+game.playerBlood;
        selectedPieces.Clear();
    }

    public void AddPiece(Chessman piece){
        if(selectedPieces.Contains(piece)){
            selectedPieces.Remove(piece);
            SpriteRenderer sprite= piece.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
        else{
            selectedPieces.Add(piece);
            SpriteRenderer sprite= piece.GetComponent<SpriteRenderer>();
            sprite.color = Color.green;
        }
    }


}
