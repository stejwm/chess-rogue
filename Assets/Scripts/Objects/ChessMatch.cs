using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Integrations.Match3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class ChessMatch
{
    private Board board;
    public PieceColor currentPlayerColor;
    public Player currentPlayer;
    public Player white;
    public Player black;
    int turnReward = 30;
    public int reward;
    public bool isSetUpPhase = true;
    public int turns = 0;
    private EventHub eventHub;
    private LogManager logManager;

    #region Overrides
    public bool AdamantAssaultOverride = false;
    public bool BloodThirstOverride = false;
    public bool AvengingStrikeOverride = false;
    public bool SwiftOverride = false;
    public bool AvengerActive = false;
    public bool isDecimating = false;
    #endregion





    public ChessMatch(Board board, Player white, Player black, EventHub eventHub, LogManager logManager)
    {
        this.board = board;
        this.white = white;
        this.black = black;
        this.eventHub = eventHub;
        this.logManager = logManager;
        SetBoard();
    }

    public void StartMatch()
    {
        reward = 6;
        Debug.Log("Match Starting");
        //KingsOrderManager._instance.Setup();
        white.CreateMoveCommandDictionary();
        black.CreateMoveCommandDictionary();
        isSetUpPhase = false;
        SetWhiteTurn();
        eventHub.RaiseChessMatchStart();
    }

    public void ExecuteTurn(Chessman piece, int x, int y)
    {
        HandleMove(piece, x, y);
    }

    public void HandleMove(Chessman piece, int x, int y)
    {

        eventHub.RaisePieceMoved(piece, board.GetTileAt(x, y));
        //Set the Chesspiece's original location to be empty
        SetPositionEmpty(piece.xBoard, piece.yBoard);


        if (board.GetPieceAtPosition(x, y) != null)
        {
            Chessman attackedPiece = board.GetPieceAtPosition(x, y).GetComponent<Chessman>();
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"> {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)} attacks <sprite=\"{attackedPiece.color}{attackedPiece.type}\" name=\"{attackedPiece.color}{attackedPiece.type}\"> on {BoardPosition.ConvertToChessNotation(x, y)}");
            CoroutineRunner.instance.StartCoroutine(HandleAttack(piece, attackedPiece));
        }
        else
        {
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"> " + BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard) + " to " + BoardPosition.ConvertToChessNotation(x, y));
            MovePiece(piece, x, y);
            SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.pieceMove);
            eventHub.RaiseRawMoveEnd(piece, board.GetTileAt(x, y));
            board.ClearTiles();
            piece.flames.Stop();
            NextTurn();
            board.IsInMove = false;
        }
    }
    public IEnumerator HandleAttack(Chessman attacker, Chessman defender)
    {
        eventHub.RaiseAttackStart(attacker, defender);
        List<GameObject> defendingUnits = new List<GameObject>();
        List<GameObject> attackingUnits = new List<GameObject>();
        SetPositionEmpty(defender.xBoard, defender.yBoard);

        if (defender.color == PieceColor.White)
        {
            defendingUnits = white.pieces;
            attackingUnits = black.pieces;
        }
        else if (defender.color == PieceColor.Black)
        {
            defendingUnits = black.pieces;
            attackingUnits = white.pieces;
        }

        //Calculate total supports
        var attackingSupportDictionary = FindSupporters(attackingUnits, defender.xBoard, defender.yBoard, attacker, defender);
        var defendingSupportDictionary = FindSupporters(defendingUnits, defender.xBoard, defender.yBoard, attacker, defender);


        yield return CoroutineRunner.instance.StartCoroutine(ShowSupport(attackingSupportDictionary));
        yield return new WaitForSeconds(Settings.Instance.WaitTime);
        yield return CoroutineRunner.instance.StartCoroutine(ShowSupport(defendingSupportDictionary));

        int attackingSupport = attackingSupportDictionary.Values.Sum();
        int defendingSupport = defendingSupportDictionary.Values.Sum();

        eventHub.RaiseAttacked(attacker, attackingSupport, defendingSupport, board.GetTileAt(defender.xBoard, defender.yBoard));

        bool isCapture = attacker.CalculateAttack() + attackingSupport >= defender.CalculateDefense() + defendingSupport;
        yield return new WaitForSeconds(Settings.Instance.WaitTime);
        yield return CoroutineRunner.instance.StartCoroutine(ShowBattlePanel(attacker, defender, attackingSupport, defendingSupport, isCapture));
        yield return CoroutineRunner.instance.StartCoroutine(ResolveBattlePanel(attacker, defender, attackingSupportDictionary, defendingSupportDictionary, isCapture));
        CompleteAttack(attacker, defender, attackingSupport, defendingSupport, isCapture);

    }


    private Dictionary<GameObject, int> FindSupporters(List<GameObject> pieces, int x, int y, Chessman attackingPiece, Chessman defendingPiece)
    {
        Dictionary<GameObject, int> supporters = new Dictionary<GameObject, int>();
        foreach (GameObject pieceObject in pieces)
        {
            var piece = pieceObject.GetComponent<Chessman>();
            if (piece == attackingPiece || piece == defendingPiece)
                continue;
            foreach (var coordinate in piece.GetValidSupportMoves())
            {
                if (coordinate.X == x && coordinate.Y == y)
                {
                    eventHub.RaiseSupportAdded(attackingPiece, defendingPiece, piece);
                    supporters.Add(pieceObject, piece.CalculateSupport());
                    break;
                }
            }
        }
        return supporters;
    }

    private IEnumerator ShowSupport(Dictionary<GameObject, int> supporters)
    {
        foreach (GameObject pieceObject in supporters.Keys)
        {
            Chessman piece = pieceObject.GetComponent<Chessman>();
            Vector2 localPosition = new Vector2(2, 2);
            int pieceSupport = supporters[pieceObject];
            var bonusPopUpInstance = SpawnsBonusPopups.Instance.BonusAdded(pieceSupport, localPosition, 1f);
            RectTransform rt = bonusPopUpInstance.GetComponent<RectTransform>();
            rt.position = pieceObject.transform.position;

            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\">{BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)} <color=green>+{pieceSupport}</color> on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
        }
    }

    private IEnumerator ShowBattlePanel(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport, bool isCapture)
    {
        float pitch = 1f;

        int attackVal = attacker.CalculateAttack();
        int defendVal = defender.CalculateDefense();

        if (attacker.color == PieceColor.White)
        {
            float scale = 0.05f;
            if (!isCapture)
                scale = -0.05f;
            board.BattlePanel.DropIn(attacker.name, attacker.droppingSprite, defender.name, defender.droppingSprite, true, attacker, defender);
            board.BattlePanel.SetAndShowHeroAttack(attackVal, pitch);
            pitch += attackVal * .05f;
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            board.BattlePanel.SetAndShowHeroSupport(attackSupport, pitch);
            pitch += attackSupport * .05f;
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            board.BattlePanel.SetAndShowHeroTotal(attackVal + attackSupport, pitch);


            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            board.BattlePanel.SetAndShowEnemyAttack(defendVal, pitch);
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            pitch += scale;
            board.BattlePanel.SetAndShowEnemySupport(defenseSupport, pitch);
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            pitch += scale;
            board.BattlePanel.SetAndShowEnemyTotal(defendVal + defenseSupport, pitch);
        }
        else if (defender.color == PieceColor.White)
        {
            float scale = 0.05f;
            if (isCapture)
                scale = -0.05f;
            board.BattlePanel.DropIn(defender.name, defender.droppingSprite, attacker.name, attacker.droppingSprite, false, defender, attacker);
            board.BattlePanel.SetAndShowEnemyAttack(attackVal, pitch);
            pitch += .05f;
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            board.BattlePanel.SetAndShowEnemySupport(attackVal, pitch);
            pitch += .05f;
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            board.BattlePanel.SetAndShowEnemyTotal(attackSupport + attackVal, pitch);
            yield return new WaitForSeconds(Settings.Instance.WaitTime);


            board.BattlePanel.SetAndShowHeroAttack(defendVal, pitch);
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            pitch += scale;
            board.BattlePanel.SetAndShowHeroSupport(defenseSupport, pitch);
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
            pitch += scale;
            board.BattlePanel.SetAndShowHeroTotal(defendVal + defenseSupport, pitch);
        }
        SoundManager.Instance.ResetPitch();
    }

    public IEnumerator ResolveBattlePanel(Chessman attacker, Chessman defender, Dictionary<GameObject, int> attackingSupporters, Dictionary<GameObject, int> defendingSupporters, bool isCapture)
    {
        Tile destination = board.GetTileAt(defender.xBoard, defender.yBoard);
        if (isCapture)
        {
            defender.captured++;
            attacker.captures++;
            foreach (GameObject supporter in attackingSupporters.Keys)
                supporter.GetComponent<Chessman>().supportsAttacking++;

            board.AbilityLogger.AddLogToQueue($"<sprite=\"{attacker.color}{attacker.type}\" name=\"{attacker.color}{attacker.type}\"> captures <sprite=\"{defender.color}{defender.type}\" name=\"{defender.color}{defender.type}\"> on {BoardPosition.ConvertToChessNotation(destination)}");

            if (black.pieces.Contains(defender.gameObject))
                black.pieces.Remove(defender.gameObject);
            if (white.pieces.Contains(defender.gameObject))
                white.pieces.Remove(defender.gameObject);


            if (isDecimating)
            {
                board.BattlePanel.SetAndShowResults("Decimate!");
                board.BattlePanel.Feedback.PlayFeedbacks();
                SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.capture);
                yield return new WaitForSeconds(board.BattlePanel.Feedback.TotalDuration);
                destination.SetBloodTile();
                if (attacker.canStationarySlash)
                    MovePiece(attacker, attacker.xBoard, attacker.yBoard);
                else
                    MovePiece(attacker, defender.xBoard, defender.yBoard);
                eventHub.RaisePieceCaptured(attacker, defender);
                yield return new WaitForSeconds(Settings.Instance.WaitTime);
                board.EventHub.RaisePieceRemoved(defender);
                defender.DestroyPiece();
            }
            else
            {
                board.BattlePanel.SetAndShowResults("Capture!");
                attacker.owner.capturedPieces.Add(defender.gameObject);
                board.BattlePanel.Feedback.PlayFeedbacks();
                SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.capture);
                yield return new WaitForSeconds(board.BattlePanel.Feedback.TotalDuration);
                destination.SetBloodTile();
                if (attacker.canStationarySlash)
                    MovePiece(attacker, attacker.xBoard, attacker.yBoard);
                else
                    MovePiece(attacker, defender.xBoard, defender.yBoard);
                defender.gameObject.SetActive(false);
                eventHub.RaisePieceCaptured(attacker, defender);
            }

        }
        else
        {
            defender.bouncing++;
            attacker.bounced++;
            foreach (var supporter in defendingSupporters.Keys)
                supporter.GetComponent<Chessman>().supportsDefending++;
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{attacker.color}{attacker.type}\" name=\"{attacker.color}{attacker.type}\"> failed to capture <sprite=\"{defender.color}{defender.type}\" name=\"{defender.color}{defender.type}\"> on {BoardPosition.ConvertToChessNotation(destination)}");
            //defender.defenseBonus = Mathf.Max(-defender.defense, defender.defenseBonus - attacker.CalculateAttack());
            defender.SetBonus(StatType.Defense, Mathf.Max(-defender.defense, defender.defenseBonus - attacker.CalculateAttack()), "Attack Damage");
            board.BattlePanel.SetAndShowResults("Bounce!");
            board.BattlePanel.Feedback.PlayFeedbacks();
            SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.bounce);
            yield return new WaitForSeconds(board.BattlePanel.Feedback.TotalDuration);
            MovePiece(defender, defender.xBoard, defender.yBoard);
            MovePiece(attacker, attacker.xBoard, attacker.yBoard);
            eventHub.RaisePieceBounced(attacker, defender);
        }
    }
    private void CompleteAttack(Chessman attacker, Chessman defender, int attackingSupport, int defendingSupport, bool isCapture)
    {
        board.BattlePanel.HideResults();
        board.BattlePanel.HideStats();
        eventHub.RaiseAttackEnd(attacker, defender, attackingSupport, defendingSupport);
        board.ClearTiles();
        attacker.flames.Stop();

        if (isCapture && defender.type == PieceType.King)
        {
            if (defender.color == PieceColor.Black)
                EndMatch();
            else if (defender.color == PieceColor.White)
                EndGame();
            board.IsInMove = false;
        }
        else
        {
            NextTurn();
            board.IsInMove = false;
        }

    }

    public void SetBoard()
    {
        Array.Clear(board.Positions, 0, board.Positions.Length);
        foreach (GameObject piece in white.pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            board.Positions[cm.xBoard, cm.yBoard] = piece;
        }
        if (black != null)
            foreach (GameObject piece in black.pieces)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                board.Positions[cm.xBoard, cm.yBoard] = piece;
            }

    }

    public void SetWhiteTurn()
    {
        currentPlayer = white;
        foreach (GameObject item in white.pieces)
        {
            CheckHex(item.GetComponent<Chessman>());
            if (item.GetComponent<Chessman>().paralyzed)
            {
                item.GetComponent<Chessman>().isValidForAttack = false;
                item.GetComponent<Chessman>().paralyzed = false;
            }
            else
            {
                item.GetComponent<Chessman>().isValidForAttack = true;
            }
        }
        foreach (GameObject item in black.pieces)
        {
            item.GetComponent<Chessman>().isValidForAttack = false;
        }

        white.MakeMove(this);
    }

    public void SetPiecesValidForAttack(Player player)
    {
        foreach (GameObject item in player.pieces)
        {
            item.GetComponent<Chessman>().isValidForAttack = true;
        }

    }
    public void SetBlackTurn()
    {
        currentPlayer = black;
        foreach (GameObject item in black.pieces)
        {
            CheckHex(item.GetComponent<Chessman>());
            if (item.GetComponent<Chessman>().paralyzed)
            {
                item.GetComponent<Chessman>().isValidForAttack = false;
                item.GetComponent<Chessman>().paralyzed = false;
            }
            else
            {
                item.GetComponent<Chessman>().isValidForAttack = true;
            }
        }
        foreach (GameObject item in white.pieces)
        {
            item.GetComponent<Chessman>().isValidForAttack = false;
        }
        black.MakeMove(this);
    }

    public void NextTurn()
    {
        //Debug.Log("IsTurnOverride? "+turnOverride);
        if (BloodThirstOverride || AdamantAssaultOverride || AvengingStrikeOverride || SwiftOverride)
            return;
        if (currentPlayerColor == PieceColor.White)
        {
            currentPlayerColor = PieceColor.Black;
            //currentPlayer=black;
            SetBlackTurn();
        }
        else
        {
            currentPlayerColor = PieceColor.White;
            //currentPlayer=white;
            SetWhiteTurn();
            turns++;
        }
    }

    public void CheckHex(Chessman piece)
    {
        if (piece.hexed)
        {
            foreach (var ability in piece.abilities)
            {
                ability.Remove(piece);
            }
            piece.hexed = false;
            piece.wasHexed = true;
        }
        else if (piece.wasHexed)
        {
            piece.wasHexed = false;
            List<Ability> abilitiesCopy = new List<Ability>(piece.abilities);
            piece.abilities.Clear();
            foreach (var ability in abilitiesCopy)
            {
                ability.Apply(board, piece);
            }
        }
    }



    public void SetPositionEmpty(int x, int y)
    {
        board.Positions[x, y] = null;
    }

    public void MovePiece(Chessman piece, int x, int y)
    {
        piece.xBoard = x;
        piece.yBoard = y;
        board.Positions[x, y] = piece.gameObject;
        piece.UpdateUIPosition();
        if (piece.color == board.Hero.color)
            StatBoxManager._instance.SetAndShowStats(piece);
        else
            StatBoxManager._instance.SetAndShowEnemyStats(piece);
    }

    public void MyTurn(PieceColor player)
    {
        currentPlayerColor = player;
    }
    public void EndMatch()
    {
        eventHub.RaiseGameEnd(board.Hero.color);
        board.BattlePanel.HideResults();
        board.BattlePanel.HideStats();
        logManager.ClearLogs();
        reward += board.Opponent.pieces.Count;
        white.playerCoins += reward;
        board.EndMatch();
    }
    public void EndGame()
    {
        board.BattlePanel.HideResults();
        board.BattlePanel.HideStats();
        logManager.ClearLogs();
        board.GameOver();
    }
    
    
}