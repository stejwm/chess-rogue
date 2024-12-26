using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


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
    public void Set(ChessMatch match, Chessman piece, int x, int y)
    {
        this.match = match;
        this.targetedX = x;
        this.targetedY = y;
        
        // Save the move state before anything changes
        if (Game._instance.debugMode)
        {
            Game._instance.SaveMove(piece, piece.GetXBoard(), piece.GetYBoard(), x, y);
        }
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
        // Remove the SaveMove from here since we're doing it in Set
        var targetPiece = Game._instance.currentMatch.GetPieceAtPosition(x, y)?.GetComponent<Chessman>();
        if (targetPiece != null)
        {
            targetedX = x;
            targetedY = y;
            HandleAttack(piece, targetPiece);
            return;
        }

        // Handle normal moves (including castling)
        if (piece is King && !piece.hasMoved && Mathf.Abs(x - piece.xBoard) == 2)
        {
            // Determine if kingside or queenside castling
            bool isKingside = x > piece.xBoard;
            int rookStartX = isKingside ? piece.xBoard + 3 : piece.xBoard - 4;
            int rookEndX = isKingside ? x - 1 : x + 1;
            
            var rookObj = Game._instance.currentMatch.GetPieceAtPosition(rookStartX, piece.yBoard);
            if (rookObj != null)
            {
                var rook = rookObj.GetComponent<Chessman>();
                Game._instance.currentMatch.SetPositionEmpty(rookStartX, piece.yBoard);
                rook.xBoard = rookEndX;
                rook.yBoard = piece.yBoard;
                rook.hasMoved = true;
                Game._instance.currentMatch.MovePiece(rook, rookEndX, piece.yBoard);
                rook.UpdateUIPosition();
            }
        }

        // Normal move handling
        Game._instance.currentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
        piece.xBoard = x;
        piece.yBoard = y;
        piece.hasMoved = true;
        if (piece is Pawn pawn)
        {
            pawn.OnMove();
        }
        Game._instance.currentMatch.MovePiece(piece, x, y);
        piece.UpdateUIPosition();
        piece.DestroyMovePlates();
        Game._instance.isInMenu = false;

        Game._instance.currentMatch.UpdateBoard();
        StartCoroutine(CallNextTurn());
    }
    public void HandleAttack(Chessman movingPiece, Chessman attackedPiece)
    {
        // Save the attack state before executing
        Game._instance.SaveMove(movingPiece, movingPiece.GetXBoard(), movingPiece.GetYBoard(), 
            attackedPiece.GetXBoard(), attackedPiece.GetYBoard(), 
            attackedPiece.gameObject, true);
        
        // Verify the attacked piece is still active
        if (!attackedPiece.gameObject.activeSelf)
        {
            // If the piece is inactive, treat it as a normal move
            HandleMove(movingPiece, attackedPiece.xBoard, attackedPiece.yBoard);
            return;
        }

        match.SetPositionEmpty(attackedPiece.xBoard, attackedPiece.yBoard);
        
        if (attackedPiece.color == PieceColor.White)
        {
            defendingUnits = new ArrayList();
            foreach (GameObject piece in match.white.pieces)
            {
                if (piece.activeSelf)
                    defendingUnits.Add(piece);
            }
            attackingUnits = new ArrayList();
            foreach (GameObject piece in match.black.pieces)
            {
                if (piece.activeSelf)
                    attackingUnits.Add(piece);
            }
        }
        else if (attackedPiece.color == PieceColor.Black)
        {
            defendingUnits = new ArrayList();
            foreach (GameObject piece in match.black.pieces)
            {
                if (piece.activeSelf)
                    defendingUnits.Add(piece);
            }
            attackingUnits = new ArrayList();
            foreach (GameObject piece in match.white.pieces)
            {
                if (piece.activeSelf)
                    attackingUnits.Add(piece);
            }
        }

        // Calculate attacking support first
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
        movingPiece.DestroyMovePlates();

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
                Game._instance.audioSource.clip = Game._instance.capture;
                BattlePanel._instance.SetAndShowResults("Checkmate!");
                attackedPiece.gameObject.SetActive(false);
                movingPiece.owner.capturedPieces.Add(attackedPiece.gameObject);
                AttackCleanUp(movingPiece, attackedPiece);
                match.EndGame();
                yield break;
            }

            if (match.black.pieces.Contains(attackedPiece.gameObject))
                match.black.pieces.Remove(attackedPiece.gameObject);
            if(match.white.pieces.Contains(attackedPiece.gameObject))
                match.white.pieces.Remove(attackedPiece.gameObject);
            
            Game._instance.audioSource.clip = Game._instance.capture;
            BattlePanel._instance.SetAndShowResults("Capture!");   
            attackedPiece.gameObject.SetActive(false);
            movingPiece.owner.capturedPieces.Add(attackedPiece.gameObject);
            AttackCleanUp(movingPiece, attackedPiece);
            Game._instance.OnPieceCaptured.Invoke(movingPiece);
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