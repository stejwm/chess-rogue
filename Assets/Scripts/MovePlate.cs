using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    //Some functions will need reference to the controller
    public GameObject controller;
    
    //The Chesspiece that was tapped to create this MovePlate
    GameObject reference = null;
    public float waitTime = .1f;

    //Location on the board
    int matrixX;
    int matrixY;

    //false: movement, true: attacking
    public bool attack = false;
    private bool readyForCleanup =false;

    private int totalAttackPower;
    private int totalDefensePower;
    private Chessman attackedPiece;
    private GameObject attackedPieceObject;
    private ArrayList defense;
    private ArrayList support;
    private bool _readyForSupport=true;
    public AudioSource audioSource;
    public AudioClip capture;
    public AudioClip bounce;

    public void Start()
    {
        if (attack)
        {
            //Set to red
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        Chessman movingPiece = reference.GetComponent<Chessman>();
        controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();
        

        totalAttackPower = movingPiece.attack;

        //Set the Chesspiece's original location to be empty
            controller.GetComponent<Game>().SetPositionEmpty(movingPiece.GetXBoard(), movingPiece.GetYBoard());

        //Destroy the victim Chesspiece
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);
            attackedPieceObject=cp;
            attackedPiece = cp.GetComponent<Chessman>();
            totalDefensePower=attackedPiece.defense;

            //Set attacked piece to empty for valid support
            controller.GetComponent<Game>().SetPositionEmpty(attackedPiece.GetXBoard(), attackedPiece.GetYBoard());
            
            if(cp.name.Contains("white")){
                defense = controller.GetComponent<Game>().playerWhite;
                support = controller.GetComponent<Game>().playerBlack;
            }
            else if(cp.name.Contains("black")){
                defense = controller.GetComponent<Game>().playerBlack;
                support = controller.GetComponent<Game>().playerWhite;
            }
            
            //Debug.Log("Ready for support?: "+_readyForSupport);
            StartCoroutine(AddSupport(totalAttackPower,movingPiece,support,true));
           
        }
        else{
            //Move reference chess piece to this position
            reference.GetComponent<Chessman>().SetXBoard(matrixX);
            reference.GetComponent<Chessman>().SetYBoard(matrixY);
            reference.GetComponent<Chessman>().SetCoords();

            //Update the matrix
            controller.GetComponent<Game>().SetPosition(reference);

            //Switch Current Player
            controller.GetComponent<Game>().NextTurn();

            //Destroy the move plates including self
            reference.GetComponent<Chessman>().DestroyMovePlates();
        }
            
    }
    private IEnumerator AddSupport(int totalPower, Chessman targetPiece, ArrayList pieces, bool isAttacking){
        _readyForSupport=false;
        foreach (GameObject pieceObject in pieces)
        {
            var piece =pieceObject.GetComponent<Chessman>() ;
            if (piece == targetPiece)
                continue; 
            foreach (var coordinate in piece.GetValidSupportMoves()){
                if (coordinate.Item1==matrixX && coordinate.Item2==matrixY){
                    totalPower+= piece.support;
                    Vector2 localPosition = new Vector2(2, 2);
                    var bonusPopUpInstance= SpawnsBonusPopups.Instance.BonusAdded(piece.support, localPosition);
                    RectTransform rt = bonusPopUpInstance.GetComponent<RectTransform>();
                    rt.position = pieceObject.transform.position;
                    Debug.Log("Spawned bonus for " + piece.name + " at position " + rt.position);
                    yield return new WaitForSeconds(waitTime/2);
                    break;
                }
            }
        }
        yield return new WaitForSeconds(waitTime);
        if(isAttacking){
            totalAttackPower=totalPower;
        }else{
            totalDefensePower = totalPower;
        }
        Debug.Log("Ready for battle panel?: "+ readyForCleanup);
        if(readyForCleanup){
            RunBattlePanel();
            yield break;
        }
        readyForCleanup=true;
        Debug.Log("Running support check on other pieces.");
        yield return StartCoroutine(AddSupport(totalDefensePower,attackedPiece,defense,false));
        
    }
    
    private void RunBattlePanel(){
        Debug.Log("Starting Battle Panel");
        controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();


        Chessman movingPiece = reference.GetComponent<Chessman>();

        if (attackedPieceObject.name == "white_king") game.Winner("black");
        if (attackedPieceObject.name == "black_king") game.Winner("white");

        

        StartCoroutine(ShowBattlePanel(movingPiece, attackedPiece));
    }
    private void AttackCleanUp(){
        Debug.Log("Starting cleanup");
        
        controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();

        //if (attackedPieceObject.name == "white_king") game.Winner("black");
        //if (attackedPieceObject.name == "black_king") game.Winner("white");

        //Move reference chess piece to this position
        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        //Update the matrix
        controller.GetComponent<Game>().SetPosition(reference);

        //Switch Current Player
        controller.GetComponent<Game>().NextTurn();

        //Destroy the move plates including self
        reference.GetComponent<Chessman>().DestroyMovePlates();
        totalAttackPower=0;
        totalDefensePower=0;
        readyForCleanup=false;

    }

    private IEnumerator ShowBattlePanel(Chessman movingPiece, Chessman attackedPiece){
        controller = GameObject.FindGameObjectWithTag("GameController");
        var game = controller.GetComponent<Game>();

        var attackVal = movingPiece.attack;
        var supportVal=totalAttackPower-attackVal;
        var sprite = movingPiece.GetComponent<SpriteRenderer>().sprite;

        var defendVal = attackedPiece.attack;
        var defendSupportVal=totalDefensePower-defendVal;
        var defendSprite = attackedPiece.GetComponent<SpriteRenderer>().sprite;
        //If hero is attacking
        if(movingPiece.type==Type.Hero){
            BattlePanel._instance.SetAndShowAttackingStats(attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,
            sprite,defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,defendSprite);
            BattlePanel._instance.SetAndShowHeroAttack(attackVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowHeroSupport(supportVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowHeroTotal(totalAttackPower);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowEnemyAttack(defendVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowEnemySupport(defendSupportVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowEnemyTotal(totalDefensePower);
            //yield return new WaitForSeconds(waitTime);

        }
        //If enemy is attacking
        else if(attackedPiece.type==Type.Hero){
            BattlePanel._instance.SetAndShowDefendingStats(defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,
            defendSprite,attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,sprite);
            
            BattlePanel._instance.SetAndShowEnemyAttack(attackVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowEnemySupport(supportVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowEnemyTotal(totalAttackPower);
            yield return new WaitForSeconds(waitTime);

            BattlePanel._instance.SetAndShowHeroAttack(defendVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowHeroSupport(defendSupportVal);
            yield return new WaitForSeconds(waitTime);
            BattlePanel._instance.SetAndShowHeroTotal(totalDefensePower);
            //yield return new WaitForSeconds(waitTime);
            
        }
        

        yield return new WaitForSeconds(waitTime);
        if(totalAttackPower>=totalDefensePower){
            Debug.Log(movingPiece.name + " captures "+ attackedPiece.name);
            if(game.playerBlack.Contains(attackedPieceObject))
                game.playerBlack.Remove(attackedPieceObject);
            if(game.playerWhite.Contains(attackedPieceObject))
                game.playerWhite.Remove(attackedPieceObject);
            Debug.Log("Setting capture tone");
            audioSource.clip = capture; 
            BattlePanel._instance.SetAndShowResults("Capture!");   
            Destroy(attackedPieceObject);
        }
        else{
            Debug.Log(movingPiece.name + " failed to capture "+ attackedPiece.name);
            //Reset attacked pieces position if capture failed 
            attackedPiece.SetXBoard(matrixX);
            attackedPiece.SetYBoard(matrixY);
            //update x and y coords with original location
            matrixX = movingPiece.GetXBoard();
            matrixY = movingPiece.GetYBoard();
            attackedPiece.attack-=1;
            attackedPiece.defense-=1;
            controller.GetComponent<Game>().SetPosition(attackedPieceObject);
            attackedPieceObject.GetComponent<Chessman>().SetCoords();
            Debug.Log("Setting bounce tone");
            audioSource.clip = bounce;
            BattlePanel._instance.SetAndShowResults("Bounce!"); 
             
            
            

        }
        
        Debug.Log("Playing audio");
        audioSource.Play();
        yield return new WaitForSeconds(waitTime*2);
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        AttackCleanUp();
        yield break;
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}