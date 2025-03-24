using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class ArmyManager : MonoBehaviour
{
    public List<GameObject> myPieces;
    public TMP_Text bloodText;
    public TMP_Text coinText;

    public List<GameObject> pieces = new List<GameObject>();
    public Chessman selectedPiece;

    //current turn
    public static ArmyManager _instance;


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

    public void SelectPiece(Chessman piece)
    {
        selectedPiece=piece;
        piece.highlightedParticles.Play(); 
    }
    public void DeselectPiece(Chessman piece)
    {
        selectedPiece=null;
        piece.highlightedParticles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear); 
    }


    public void PieceSelect(Chessman piece){
        if(selectedPiece==null){
            SelectPiece(piece);
        }
        else if (selectedPiece==piece){
            DeselectPiece(piece);
        }
        else if (selectedPiece && Game._instance.hero.playerCoins>=10){
            BoardPosition position1 = selectedPiece.startingPosition;
            BoardPosition position2 = piece.startingPosition;
            selectedPiece.startingPosition=position2;
            piece.startingPosition=position1;
            selectedPiece.xBoard=position2.x;
            selectedPiece.yBoard=position2.y;
            piece.xBoard=position1.x;
            piece.yBoard=position1.y;
            selectedPiece.UpdateUIPosition();
            piece.UpdateUIPosition();
            Game._instance.hero.playerCoins-=10;
            UpdateCurrency();
            DeselectPiece(selectedPiece);
        }
        else{
            piece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }
    public void PositionSelect(BoardPosition position){
        if(selectedPiece==null){
            return;
        }
        else if (selectedPiece && Game._instance.hero.playerCoins>=5){
            selectedPiece.startingPosition=position;
            selectedPiece.xBoard=position.x;
            selectedPiece.yBoard=position.y;
            selectedPiece.UpdateUIPosition();
            Game._instance.hero.playerCoins-=5;
            UpdateCurrency();
            DeselectPiece(selectedPiece);
        }
        else{
            selectedPiece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }
    public IEnumerator GetManagementInput(){

        Game._instance.tileSelect=true;
        yield return new WaitUntil(() => BoardManager._instance.selectedPosition !=null);
        Game._instance.tileSelect=false;
        BoardPosition targetPosition = BoardManager._instance.selectedPosition;
        BoardManager._instance.selectedPosition=null;
        var Chessobj = Game._instance.currentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
        if(Chessobj==null && selectedPiece!=null && Game._instance.hero.playerCoins>=5){
            if (Game._instance.hero.openPositions.Contains(targetPosition)){
                selectedPiece.startingPosition=targetPosition;
                selectedPiece.xBoard=targetPosition.x;
                selectedPiece.yBoard=targetPosition.y;
                selectedPiece.UpdateUIPosition();
                Game._instance.hero.playerCoins-=5;
            }
            yield break;
        }
        if(Chessobj==null && selectedPiece==null){
            Debug.Log("No piece at possition");
            yield break;
        }
        Chessman piece = Chessobj.GetComponent<Chessman>();
        if(selectedPiece==null){
            selectedPiece=piece;
            yield break;
        }
        
    }

    public void OpenShop(){
        //Game._instance.isInMenu=false;
        gameObject.SetActive(true);
        BoardManager._instance.CreateManagementBoard();
        UpdateCurrency();
        myPieces=Game._instance.hero.pieces;
        Debug.Log("Management piece count :"+myPieces.Count);
        Game._instance.toggleAllPieceColliders(false);
        foreach (GameObject piece in myPieces)
        {
            if (piece !=null && piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 6;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=4;
            }
            piece.GetComponent<Chessman>().UpdateUIPosition();
        }
        Game._instance.togglePieceColliders(myPieces, true);
        //Game._instance.toggleAllPieceColliders(false);
        
        //CreatePieces();
    }

    public void CreatePieces(){
        GameObject obj;
        for(int i=0; i<3;i++){
            Vector2 localPosition = new Vector2(i, 2);
            obj = PieceFactory._instance.CreateRandomPiece();
            obj.transform.position=localPosition;
            Chessman cm = obj.GetComponent<Chessman>();
            cm.xBoard=4+i;
            cm.yBoard = 5;
            cm.UpdateUIPosition();
            SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
            rend.sortingOrder = 5;
            pieces.Add(obj);
        }
    }

    public void UpdateCurrency(){
        bloodText.text = ": "+Game._instance.hero.playerBlood;
        coinText.text = ": "+Game._instance.hero.playerCoins;
    }

    public void CloseShop(){
        ManagementStatManager._instance.HideStats();
        foreach (GameObject piece in myPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 1;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
            }
        }
        foreach (GameObject piece in Game._instance.hero.inventoryPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 1;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
            }
        }
        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }
        BoardManager._instance.DestroyBoard();
        Game._instance.CloseShop();
        gameObject.SetActive(false);
    }

    


}
