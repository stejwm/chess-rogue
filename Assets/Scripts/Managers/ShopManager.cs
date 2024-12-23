using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public List<GameObject> myPieces;
    public TMP_Text bloodText;
    public TMP_Text coinText;

    private List<GameObject> pieces = new List<GameObject>();

    //current turn
    public static ShopManager _instance;


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

    public void OpenShop(){
        //Game._instance.isInMenu=false;
        gameObject.SetActive(true);
        UpdateCurrency();
        myPieces=Game._instance.hero.pieces;

        foreach (GameObject piece in myPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
            }
            
        }
        CreatePieces();
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
        ShopStatManager._instance.HideStats();
        foreach (GameObject piece in myPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 1;
            }
        }
        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }
        Game._instance.CloseShop();
        gameObject.SetActive(false);
    }

    


}
