using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public ArrayList myPieces;

    //current turn
    private string currentPlayer = "white";
    private InventoryManager _instance;


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

    public void OpenInventory(){
        gameObject.SetActive(true);
        var controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();

        if(game.heroColor==PieceColor.White)
            myPieces=game.playerWhite;
        else
            myPieces=game.playerBlack;

        foreach (GameObject piece in myPieces)
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

    public void CloseInventory(){
        

        foreach (GameObject piece in myPieces)
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
        gameObject.SetActive(false);
    }


}
