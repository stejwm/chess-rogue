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
    public List<GameObject> myCapturedPieces= new List<GameObject>();
    //public List<GameObject> myPieces= new List<GameObject>();
    public List<GameObject> opponentCapturedPieces = new List<GameObject>();
    //public List<GameObject> opponentPieces = new List<GameObject>();
    public List<Chessman> selectedPieces = new List<Chessman>();
    public GameObject controller;
    public TMP_Text bloodText;
    public TMP_Text coinText;
    public int totalCost;
    public PieceColor selectedColor = PieceColor.None;


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
    }

    public void OpenMarket(){
        
        totalCost=0;
        Debug.Log("opening market");
        gameObject.SetActive(true);
        bloodText.text = ": "+Game._instance.hero.playerBlood;
        coinText.text = ": "+Game._instance.hero.playerCoins;
        myCapturedPieces=Game._instance.hero.capturedPieces;
        opponentCapturedPieces= Game._instance.opponent.capturedPieces;

        Game._instance.opponent.pieces.AddRange(myCapturedPieces);

        /* var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>(); */
        Debug.Log("captured pieces count "+myCapturedPieces.Count);
        if(myCapturedPieces.Count>0)
        foreach (GameObject piece in myCapturedPieces)
        {   
            piece.SetActive(true);
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
            }
            
        }
        if(opponentCapturedPieces.Count>0)
        foreach (GameObject piece in opponentCapturedPieces)
        {
            piece.SetActive(true);
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
            }
            
        }
    }

    public void CloseMarket(){
        selectedColor = PieceColor.None;
        opponentCapturedPieces = Game._instance.opponent.capturedPieces;
        if(opponentCapturedPieces.Count>0)
        foreach (GameObject piece in opponentCapturedPieces)
        {
            /* if(piece){
                if (piece.GetComponent<SpriteRenderer>())
                {
                    SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                    rend.sortingOrder = 1;
                }
            } */
            Destroy(piece);
            
        }
        if(myCapturedPieces.Count>0)
        foreach (GameObject piece in myCapturedPieces)
        {
            /* if(piece){
                if (piece.GetComponent<SpriteRenderer>())
                {
                    SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                    rend.sortingOrder = 1;
                }
            } */
            Destroy(piece);
        }
        Game._instance.state=ScreenState.RewardScreen;
        myCapturedPieces.Clear();
        opponentCapturedPieces.Clear();
        selectedPieces.Clear();
        Game._instance.OpenReward();
        gameObject.SetActive(false);
        
        
    }

    public void ReleasePieces(){
        foreach (Chessman item in selectedPieces)
        {
            Game._instance.hero.playerCoins+= item.releaseCost;
            SpriteRenderer sprite= item.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            item.gameObject.SetActive(false);
        }
        coinText.text = ": "+ Game._instance.hero.playerCoins;
        selectedPieces.Clear();
    }

    public void ReturnMyPieces(){
        foreach (GameObject obj in opponentCapturedPieces){
            Chessman piece = obj.GetComponent<Chessman>();
            if (selectedPieces.Contains(piece)){
                Game._instance.hero.playerCoins-= piece.releaseCost;
                SpriteRenderer sprite= piece.GetComponent<SpriteRenderer>();
                sprite.color = Color.white;
                obj.SetActive(false);
                Game._instance.hero.pieces.Add(obj);
            }
            else{
                Game._instance.opponent.capturedPieces.Remove(obj);
                Game._instance.hero.pieces.Remove(obj);
                Destroy(obj);
            }
        }
        coinText.text = ": "+Game._instance.hero.playerCoins;
        selectedPieces.Clear();
    }

    public void KillPieces(){
        
        foreach (Chessman item in selectedPieces)
        {
            Game._instance.hero.playerBlood+= item.blood;
            myCapturedPieces.Remove(item.gameObject);
            item.gameObject.SetActive(false);
        }
        bloodText.text = ": "+Game._instance.hero.playerBlood;
        selectedPieces.Clear();
    }

    public void AddPiece(Chessman piece){
        if(selectedPieces.Count==0 || selectedColor == PieceColor.None)
            selectedColor= piece.color;
        //Debug.Log(piece.name);
        //Debug.Log(Game._instance.hero.playerCoins);
        //Debug.Log(selectedPieces.Count);
        if(totalCost+piece.releaseCost>Game._instance.hero.playerCoins && piece.color==Game._instance.heroColor && !selectedPieces.Contains(piece))
            return;
        if(piece.color != selectedColor)
            return;

        if(selectedPieces.Contains(piece)){
            selectedPieces.Remove(piece);
            SpriteRenderer sprite= piece.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            if(piece.color==Game._instance.heroColor)
                totalCost-=piece.releaseCost;
        }
        else{
            selectedPieces.Add(piece);
            SpriteRenderer sprite= piece.GetComponent<SpriteRenderer>();
            sprite.color = Color.green;
            if(piece.color==Game._instance.heroColor)
                totalCost+=piece.releaseCost;
        }
    }


}
