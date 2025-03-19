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
        //Debug.Log("Opening market");
        gameObject.SetActive(true);
        bloodText.text = ": "+Game._instance.hero.playerBlood;
        coinText.text = ": "+Game._instance.hero.playerCoins;
        myCapturedPieces=Game._instance.hero.capturedPieces;
        opponentCapturedPieces= Game._instance.opponent.capturedPieces;

        Game._instance.opponent.pieces.AddRange(myCapturedPieces);
        Game._instance.toggleAllPieceColliders(false);
        Game._instance.togglePieceColliders(myCapturedPieces, true);
        Game._instance.togglePieceColliders(opponentCapturedPieces, true);

        /* var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>(); */
        if(myCapturedPieces.Count>0)
        foreach (GameObject piece in myCapturedPieces)
        {   
            piece.SetActive(true);
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=4;
                piece.GetComponent<Chessman>().flames.GetComponent<Renderer>().sortingOrder=6;
            }
            
        }
        var decimatedPieces = new List<GameObject>();
        if(opponentCapturedPieces.Count>0)
        foreach (GameObject piece in opponentCapturedPieces)
        {
            piece.SetActive(true);
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=4;
                piece.GetComponent<Chessman>().flames.GetComponent<Renderer>().sortingOrder=2;

            }
            Chessman chessman = piece.GetComponent<Chessman>();
            if(chessman.owner == Game._instance.hero){
                if(chessman.abilities.Count>chessman.diplomacy){
                    Debug.Log("checking diplomacy for "+piece.name);
                    int survive = Random.Range(1,10);
                    Debug.Log("Rolled "+survive+" and diplomacy is "+chessman.diplomacy);
                    if(survive<=((chessman.abilities.Count -chessman.diplomacy)*2)){
                        Debug.Log("decimated from diplomacy check");
                        decimatedPieces.Add(piece);
                        Destroy(piece);
                    }
                }
            }

            
        }
        opponentCapturedPieces.RemoveAll(x => decimatedPieces.Contains(x));
    }

    public void CloseMarket(){
        selectedColor = PieceColor.None;
        opponentCapturedPieces = Game._instance.opponent.capturedPieces;
        if(opponentCapturedPieces.Count>0)
        foreach (GameObject piece in opponentCapturedPieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            Game._instance.hero.openPositions.Add(cm.startingPosition);
            Destroy(piece);
            Game._instance.abandonedPieces++;
            Debug.Log("AbandonedPieces :"+Game._instance.abandonedPieces);
            
        }
        if(myCapturedPieces.Count>0)
        foreach (GameObject piece in myCapturedPieces)
        {
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
            item.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            item.gameObject.SetActive(false);
        }
        coinText.text = ": "+ Game._instance.hero.playerCoins;
        selectedPieces.Clear();
    }

    public void ReturnMyPieces(){
        foreach (Chessman piece in selectedPieces){
            if(piece.owner != Game._instance.hero)
                break;
            if (selectedPieces.Contains(piece)){
                Game._instance.hero.playerCoins-= piece.releaseCost;
                SpriteRenderer sprite= piece.GetComponent<SpriteRenderer>();
                piece.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                piece.gameObject.SetActive(false);
                Game._instance.hero.pieces.Add(piece.gameObject);
                opponentCapturedPieces.Remove(piece.gameObject);
                myCapturedPieces.Remove(piece.gameObject);
                Game._instance.opponent.pieces.Remove(piece.gameObject);

            }
        }
        totalCost=0;
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
            piece.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if(piece.color==Game._instance.heroColor)
                totalCost-=piece.releaseCost;
        }
        else{
            selectedPieces.Add(piece);
            piece.highlightedParticles.Play();
            if(piece.color==Game._instance.heroColor)
                totalCost+=piece.releaseCost;
        }
    }


}
