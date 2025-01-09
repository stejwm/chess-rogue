using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
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
    private float pitch=1f;
    public static MoveManager _instance;
    public MMF_Player ResultFeedback;
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
        Game._instance.audioSource.clip = Game._instance.move; 

        Game._instance.audioSource.pitch=pitch;
        Game._instance.audioSource.Play();
        pitch+=.1f;
        
        if (match.GetPieceAtPosition(x,y)!=null)
        {
            Chessman attackedPiece = match.GetPieceAtPosition(x,y).GetComponent<Chessman>();
            LogManager._instance.WriteLog($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"> {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)} attacks <sprite=\"{attackedPiece.color}{attackedPiece.type}\" name=\"{attackedPiece.color}{attackedPiece.type}\"> on {BoardPosition.ConvertToChessNotation(x, y)}");
            HandleAttack(movingPiece, attackedPiece);   
        }
        else{
            LogManager._instance.WriteLog($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"> "+ BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)+" to "+ BoardPosition.ConvertToChessNotation(x, y));
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
        StartCoroutine(AddSupport(movingPiece, attackedPiece, true));
    }

    private IEnumerator AddSupport(Chessman movingPiece, Chessman attackedPiece, bool isAttacking){
        ArrayList pieces;
        Chessman targetPiece;
        int supportPower=0;
        pitch=1f;
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
                    int pieceSupport =piece.CalculateSupport();
                    var bonusPopUpInstance= SpawnsBonusPopups.Instance.BonusAdded(pieceSupport, localPosition, pitch);
                    RectTransform rt = bonusPopUpInstance.GetComponent<RectTransform>();
                    rt.position = pieceObject.transform.position; 
                    //piece.gameObject.GetComponent<MMFloatingTextMeshPro>().
                    //piece.showSupportFloatingText();
                    //Debug.Log("Spawned bonus for " + piece.name + " at position " + BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard));
                    LogManager._instance.WriteLog($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\">{BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)} <color=green>+{pieceSupport}</color> on {BoardPosition.ConvertToChessNotation(targetedX, targetedY)}");
                    yield return new WaitForSeconds(Game._instance.waitTime/2);
                    Game._instance.OnSupportAdded.Invoke(piece, movingPiece, attackedPiece);
                    pitch+=.05f;
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
        pitch=1f;

        if(!gameOver)
            StartCoroutine(CallNextTurn());
        
    }

    private IEnumerator CallNextTurn(){
        yield return new WaitForSeconds(Game._instance.waitTime);
        match.NextTurn();
    }

    private IEnumerator ShowBattlePanel(Chessman movingPiece, Chessman attackedPiece){
        pitch=1f;
        var attackVal = movingPiece.CalculateAttack();
        var supportVal= attackSupport;
        var sprite = movingPiece.GetComponent<SpriteRenderer>().sprite;

        var defendVal = attackedPiece.CalculateDefense();
        var defendSupportVal= defenseSupport;
        var defendSprite = attackedPiece.GetComponent<SpriteRenderer>().sprite;
        //If hero is attacking
        if(movingPiece.team==Team.Hero){
            float scale=0.05f;
            if (totalAttackPower<totalDefensePower)
                scale=-0.05f;
            BattlePanel._instance.SetAndShowAttackingStats(attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,
            sprite,defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,defendSprite);
            BattlePanel._instance.SetAndShowHeroAttack(attackVal, pitch);
            pitch+=attackVal*.05f;
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowHeroSupport(supportVal,pitch);
            pitch+=supportVal*.05f;
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowHeroTotal(totalAttackPower,pitch);
            

            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemyAttack(defendVal,pitch);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            pitch+=scale;
            BattlePanel._instance.SetAndShowEnemySupport(defendSupportVal,pitch);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            pitch+=scale;
            BattlePanel._instance.SetAndShowEnemyTotal(totalDefensePower,pitch);
        }
        //If enemy is attacking
        else if(attackedPiece.team==Team.Hero){
            float scale=0.05f;
            if (totalAttackPower>=totalDefensePower)
                scale=-0.05f;
            BattlePanel._instance.SetAndShowDefendingStats(defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,
            defendSprite,attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,sprite);
            
            BattlePanel._instance.SetAndShowEnemyAttack(attackVal,pitch);
            pitch+=.05f;
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemySupport(supportVal,pitch);
            pitch+=.05f;
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            BattlePanel._instance.SetAndShowEnemyTotal(totalAttackPower,pitch);
            yield return new WaitForSeconds(Game._instance.waitTime/2);

            
            BattlePanel._instance.SetAndShowHeroAttack(defendVal,pitch);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            pitch+=scale;
            BattlePanel._instance.SetAndShowHeroSupport(defendSupportVal,pitch);
            yield return new WaitForSeconds(Game._instance.waitTime/2);
            pitch+=scale;
            BattlePanel._instance.SetAndShowHeroTotal(totalDefensePower,pitch);
            
            //yield return new WaitForSeconds(Game._instance.waitTime);
            
        }
        

        yield return new WaitForSeconds(Game._instance.waitTime);
        if(totalAttackPower>=totalDefensePower){
            
            LogManager._instance.WriteLog($"<sprite=\"{movingPiece.color}{movingPiece.type}\" name=\"{movingPiece.color}{movingPiece.type}\"> captures <sprite=\"{attackedPiece.color}{attackedPiece.type}\" name=\"{attackedPiece.color}{attackedPiece.type}\"> on {BoardPosition.ConvertToChessNotation(targetedX, targetedY)}");
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
            ResultFeedback.PlayFeedbacks();
            yield return new WaitForSeconds(ResultFeedback.TotalDuration);
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
            //Debug.Log(movingPiece.name + " failed to capture "+ attackedPiece.name +" on "+ BoardPosition.ConvertToChessNotation(targetedX, targetedY));
            //LogManager._instance.WriteLog(movingPiece.name + " failed to capture "+ attackedPiece.name +" on "+ BoardPosition.ConvertToChessNotation(targetedX, targetedY));
            LogManager._instance.WriteLog($"<sprite=\"{movingPiece.color}{movingPiece.type}\" name=\"{movingPiece.color}{movingPiece.type}\"> failed to capture <sprite=\"{attackedPiece.color}{attackedPiece.type}\" name=\"{attackedPiece.color}{attackedPiece.type}\"> on {BoardPosition.ConvertToChessNotation(targetedX, targetedY)}");
            isBounceReduced=false;

            //Reset attacked pieces position if capture failed 
            attackedPiece.SetXBoard(targetedX);
            attackedPiece.SetYBoard(targetedY);
            //update x and y coords with original location
            targetedX = movingPiece.GetXBoard();
            targetedY = movingPiece.GetYBoard();
            /* if(attackedPiece.CalculateAttack()>0){
                attackedPiece.attackBonus-=1;
                isBounceReduced =true;
            } */
            if(attackedPiece.CalculateDefense()>0){
                attackedPiece.defenseBonus-=movingPiece.CalculateAttack();
                if(attackedPiece.CalculateDefense()<0)
                    attackedPiece.defenseBonus+=Math.Abs(attackedPiece.CalculateDefense());
                isBounceReduced =true;
            }
            //Debug.Log("Setting attacked piece Position back to: "+attackingPiece.xBoard+", "+attackedPiece)
            match.MovePiece(attackedPiece, attackedPiece.xBoard, attackedPiece.yBoard);
            //Debug.Log("Setting bounce tone");
            Game._instance.audioSource.clip = Game._instance.bounce;
            BattlePanel._instance.SetAndShowResults("Bounce!"); 
            ResultFeedback.PlayFeedbacks();
            yield return new WaitForSeconds(ResultFeedback.TotalDuration);
            AttackCleanUp(movingPiece, attackedPiece);
            Game._instance.OnPieceBounced.Invoke(movingPiece, attackedPiece, isBounceReduced);
            
            
            

        }
        
        //Debug.Log("Playing audio");
        Game._instance.audioSource.pitch=pitch;
        Game._instance.audioSource.Play();
        yield return new WaitForSeconds(Game._instance.waitTime);
        
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        
        yield break;
    }
}