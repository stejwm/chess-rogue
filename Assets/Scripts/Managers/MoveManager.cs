using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MoveManager: MonoBehaviour
{   
    private ChessMatch match;
    private Chessman attackingPiece;
    private Chessman defendingPiece;
    private int targetedX;
    private int targetedY;
    private ArrayList defendingUnits;
    private ArrayList attackingUnits;
    private bool isBounceReduced;
    private int totalAttackPower=0;
    private int totalDefensePower=0;
    private int attackSupport=0;
    private int defenseSupport=0;
    private int baseAttack=0;
    private int baseDefense=0;
    private bool readyForCleanup=false;
    private bool gameOver = false;
    public static MoveManager _instance;
    public void Set(ChessMatch match, Chessman attackingPiece, int x, int y){
        this.match = match;
        this.attackingPiece=attackingPiece;
        targetedX = x;
        targetedY = y;
    }

    public void Awake(){
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }
    public void HandleMove(Chessman piece, int x, int y){
        
        Chessman movingPiece = piece;
        
        Game._instance.OnMove.Invoke(piece);
        //Set the Chesspiece's original location to be empty
        match.SetPositionEmpty(movingPiece.xBoard, movingPiece.yBoard);
        
        if (match.GetPieceAtPosition(x,y)!=null)
        {
            Chessman attackedPiece = match.GetPieceAtPosition(x,y).GetComponent<Chessman>();
            HandleAttack(movingPiece, attackedPiece);   
        }
        else{
            match.MovePiece(movingPiece, x,y);
            BoardManager._instance.ClearTiles();
            match.NextTurn();
            Game._instance.isInMenu=false;
            

        }
    }
    public void HandleAttack(Chessman movingPiece, Chessman attackedPiece){
        //Set attacked piece to empty for valid support
        match.SetPositionEmpty(attackedPiece.xBoard, attackedPiece.yBoard);
        
        if(attackedPiece.color==PieceColor.White){
            defendingUnits = new ArrayList(match.white.pieces);
            attackingUnits = new ArrayList(match.black.pieces);
        }
        else if(attackedPiece.color==PieceColor.Black){
            defendingUnits = new ArrayList(match.black.pieces);
            attackingUnits = new ArrayList(match.white.pieces);
        }

        //Calculate attacking support first
        Debug.Log("Running support check for attacking pieces");
        StartCoroutine(AddSupport(movingPiece, attackedPiece, true));
    }

    private IEnumerator AddSupport(Chessman movingPiece, Chessman attackedPiece, bool isAttacking){
        ArrayList pieces;
        Chessman targetPiece;
        int supportPower=0;
        if(isAttacking){
            pieces=attackingUnits;
            targetPiece=movingPiece;
        }
        else{
            pieces=defendingUnits;
            targetPiece=attackedPiece;
        }
        
        foreach (GameObject pieceObject in pieces)
        {
            //Debug.Log("Checking support from "+ pieceObject.name);
            var piece =pieceObject.GetComponent<Chessman>() ;
            if (piece == targetPiece)
                continue; 
            foreach (var coordinate in piece.GetValidSupportMoves()){
                if (coordinate.x==targetedX && coordinate.y==targetedY){
                    supportPower+= piece.CalculateSupport();
                    Vector2 localPosition = new Vector2(2, 2);
                    var bonusPopUpInstance= SpawnsBonusPopups.Instance.BonusAdded(piece.CalculateSupport(), localPosition);
                    RectTransform rt = bonusPopUpInstance.GetComponent<RectTransform>();
                    rt.position = pieceObject.transform.position;
                    Debug.Log("Spawned bonus for " + piece.name + " at position " + rt.position);
                    yield return new WaitForSeconds(Game._instance.waitTime/2);
                    Game._instance.OnSupportAdded.Invoke(piece);
                    break;
                }
            }
        }
        Game._instance.OnAttack.Invoke(targetPiece, supportPower, isAttacking, new BoardPosition(targetedX, targetedY)); 
        yield return new WaitForSeconds(Game._instance.waitTime);
        if(isAttacking){
            baseAttack=targetPiece.CalculateAttack();
            totalAttackPower=baseAttack+supportPower;
            attackSupport=supportPower;
        }else{   
            baseDefense=targetPiece.CalculateDefense();
            totalDefensePower = baseDefense+supportPower;
            defenseSupport=supportPower;
        }
        if(readyForCleanup){
            RunBattlePanel(movingPiece, attackedPiece);
            yield break;
        }
        readyForCleanup=true;
        Debug.Log("Running support check for defending pieces");
        yield return StartCoroutine(AddSupport(movingPiece, attackedPiece, false));
        
    }
    
    private void RunBattlePanel(Chessman movingPiece, Chessman attackedPiece){
        //Debug.Log("Starting Battle Panel");
        StartCoroutine(ShowBattlePanel(movingPiece, attackedPiece));
    }
    private void AttackCleanUp(Chessman movingPiece, Chessman attackedPiece){
        Game._instance.OnAttackEnd.Invoke(movingPiece, attackedPiece, attackSupport, defenseSupport); 
        //Move reference chess piece to this position
        match.MovePiece(movingPiece, targetedX, targetedY);
        BoardManager._instance.ClearTiles();

        //reset all variables
        targetedX = -1;
        targetedY =-1;
        totalAttackPower=0;
        totalDefensePower=0;
        attackSupport=0;
        defenseSupport=0;
        baseAttack=0;
        baseDefense=0;
        readyForCleanup=false;
        defendingUnits.Clear();
        attackingUnits.Clear();
        Game._instance.isInMenu=false;

        if(!gameOver)
            StartCoroutine(CallNextTurn());
        
    }

    private IEnumerator CallNextTurn(){
        yield return new WaitForSeconds(Game._instance.waitTime);
        match.NextTurn();
    }

    private IEnumerator ShowBattlePanel(Chessman movingPiece, Chessman attackedPiece){

        var attackVal = movingPiece.CalculateAttack();
        var supportVal= attackSupport;
        var sprite = movingPiece.GetComponent<SpriteRenderer>().sprite;

        var defendVal = attackedPiece.CalculateDefense();
        var defendSupportVal= defenseSupport;
        var defendSprite = attackedPiece.GetComponent<SpriteRenderer>().sprite;
        //If hero is attacking
        if(movingPiece.team==Team.Hero){
            BattlePanel._instance.SetAndShowAttackingStats(attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,
            sprite,defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,defendSprite);
            BattlePanel._instance.SetAndShowHeroAttack(attackVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowHeroSupport(supportVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowHeroTotal(totalAttackPower);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemyAttack(defendVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemySupport(defendSupportVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemyTotal(totalDefensePower);

        }
        //If enemy is attacking
        else if(attackedPiece.team==Team.Hero){
            BattlePanel._instance.SetAndShowDefendingStats(defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,
            defendSprite,attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,sprite);
            
            BattlePanel._instance.SetAndShowEnemyAttack(attackVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemySupport(supportVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemyTotal(totalAttackPower);
            yield return new WaitForSeconds(Game._instance.waitTime/2);

            BattlePanel._instance.SetAndShowHeroAttack(defendVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowHeroSupport(defendSupportVal);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowHeroTotal(totalDefensePower);
            //yield return new WaitForSeconds(Game._instance.waitTime);
            
        }
        

        yield return new WaitForSeconds(Game._instance.waitTime);
        if(totalAttackPower>=totalDefensePower){
            
            Debug.Log(movingPiece.name + " captures "+ attackedPiece.name +" on "+ BoardPosition.ConvertToChessNotation(targetedX, targetedY));
            if (attackedPiece.type==PieceType.King){
                gameOver=true;
            }
            if (match.black.pieces.Contains(attackedPiece.gameObject))
                match.black.pieces.Remove(attackedPiece.gameObject);
            if(match.white.pieces.Contains(attackedPiece.gameObject))
                match.white.pieces.Remove(attackedPiece.gameObject);
            //Debug.Log("Setting capture tone");
            Game._instance.audioSource.clip = Game._instance.capture; 
            BattlePanel._instance.SetAndShowResults("Capture!");   
            attackedPiece.gameObject.SetActive(false);
            movingPiece.owner.capturedPieces.Add(attackedPiece.gameObject);
            AttackCleanUp(movingPiece, attackedPiece);
            Game._instance.OnPieceCaptured.Invoke(movingPiece);  // Trigger the event
            if (gameOver){
                Game._instance.OnGameEnd.Invoke(movingPiece.color);
                match.EndGame();
                gameOver=false;
                yield break;
            }
              
        }
        else{
            Debug.Log(movingPiece.name + " failed to capture "+ attackedPiece.name +" on "+ BoardPosition.ConvertToChessNotation(targetedX, targetedY));
            isBounceReduced=false;

            //Reset attacked pieces position if capture failed 
            attackedPiece.SetXBoard(targetedX);
            attackedPiece.SetYBoard(targetedY);
            //update x and y coords with original location
            targetedX = movingPiece.GetXBoard();
            targetedY = movingPiece.GetYBoard();
            if(attackedPiece.CalculateAttack()>0){
                attackedPiece.attackBonus-=1;
                isBounceReduced =true;
            }
            if(attackedPiece.CalculateDefense()>0){
                attackedPiece.defenseBonus-=1;
                isBounceReduced =true;
            }
            //Debug.Log("Setting attacked piece Position back to: "+attackingPiece.xBoard+", "+attackedPiece)
            match.MovePiece(attackedPiece, attackedPiece.xBoard, attackedPiece.yBoard);
            //Debug.Log("Setting bounce tone");
            Game._instance.audioSource.clip = Game._instance.bounce;
            BattlePanel._instance.SetAndShowResults("Bounce!"); 
            AttackCleanUp(movingPiece, attackedPiece);
            Game._instance.OnPieceBounced.Invoke(movingPiece, attackedPiece, isBounceReduced);

            
            

        }
        
        //Debug.Log("Playing audio");
        Game._instance.audioSource.Play();
        yield return new WaitForSeconds(Game._instance.waitTime);
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        
        yield break;
    }
}