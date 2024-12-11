using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();

        Debug.Log("opening market");
        gameObject.SetActive(true);

        if(game.heroColor==PieceColor.White){
            myPieces=game.playerWhite;
            opponentPieces=game.playerBlack;
        }
        else{
            myPieces=game.playerWhite;
            opponentPieces=game.playerBlack;
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
        var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();
        /* var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();
        game.ClearCard();
        game.ClearPiece();*/
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
                rend.sortingOrder = 1;
            }
            
        }
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
                rend.sortingOrder = 1;
            }
            
        }
        game.SetBoardActive();
        InventoryManager._instance.OpenInventory();
        gameObject.SetActive(false);
        
        
    }

    public void AddPiece(Chessman piece){
        selectedPieces.Add(piece);
        SpriteRenderer sprite= piece.GetComponent<SpriteRenderer>();
        sprite.color = Color.green;
    }


}
