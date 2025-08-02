using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TutorialManager : MonoBehaviour
{
    
    //current turn
    public static DialogueManager _instance;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;
    [SerializeField] private TutorialSlide attack;
    [SerializeField] private TutorialSlide capture;
    [SerializeField] private TutorialSlide bounce;
    [SerializeField] private TutorialSlide diplomacy;
    [SerializeField] private TutorialSlide shop;
    [SerializeField] private TutorialSlide reward;
    [SerializeField] private TutorialSlide map;
    [SerializeField] private GameObject circle;
    private List<GameObject> circles = new List<GameObject>();
    private bool newUnit = false;
    private bool newUpgrade = false;
    private Board board;


    public void Initialize(Board board)
    {
        this.board = board;
        gameObject.SetActive(false);
        if (Settings.Instance.Tutorial)
        {
            board.EventHub.OnAttackStart.AddListener(ShowAttackTutorial);
            board.EventHub.OnSupportAdded.AddListener(ShowSupportTutorial);
            board.EventHub.OnPieceCaptured.AddListener(ShowCaptureTutorial);
            board.EventHub.OnPieceBounced.AddListener(ShowBounceTutorial);
            board.EventHub.OnAbilityAdded.AddListener(ShowAbilityTutorial);
            board.EventHub.OnAbilityAdded.AddListener(ShowDiplomacyTutorial);
            board.EventHub.OnGameEnd.AddListener(ShowMarketTutorial);
        }
    }


    public void ShowAttackTutorial(Chessman attacker=null, Chessman defender=null)
    {
        circles.Add(Instantiate(circle, attacker.transform.position, Quaternion.identity));
        circles.Add(Instantiate(circle, defender.transform.position, Quaternion.identity));
        string message = $"{ColorChessman(attacker)} is attacking {ColorChessman(defender)} you can check the log window on the right of the screen for move history and piece locations\n\n";
        message += $"When attacking, the attacker's<sprite name=\"sword\"> stat is compared against the defender's<sprite name=\"shield\"> stat\n\n";
        message += $"- {ColorChessman(attacker)} {attacker.CalculateAttack()}<sprite name=\"sword\"> vs {ColorChessman(defender)} {defender.CalculateDefense()}<sprite name=\"shield\">\n";
        ShowTutorial("Attacking", message);
        board.EventHub.OnAttackStart.RemoveListener(ShowAttackTutorial);
    }

    public void ShowSupportTutorial(Chessman attacker, Chessman defender, Chessman supporter)
    {
        Dictionary<GameObject, int> attackingSupportDictionary;
        Dictionary<GameObject, int> defendingSupportDictionary;
        if (attacker.owner == board.Hero)
        {
            attackingSupportDictionary = board.CurrentMatch.FindSupportersNoInvoke(board.Hero.pieces, defender.xBoard, defender.yBoard, attacker, defender);
            defendingSupportDictionary = board.CurrentMatch.FindSupportersNoInvoke(board.Opponent.pieces, defender.xBoard, defender.yBoard, attacker, defender);
        }
        else
        {
            defendingSupportDictionary = board.CurrentMatch.FindSupportersNoInvoke(board.Hero.pieces, defender.xBoard, defender.yBoard, attacker, defender);
            attackingSupportDictionary = board.CurrentMatch.FindSupportersNoInvoke(board.Opponent.pieces, defender.xBoard, defender.yBoard, attacker, defender);
        }
        string message = $"A piece adds its<sprite name=\"cross\"> stat to the attack or defense total when it can also attack the square of combat\n\n";
        foreach (var entry in attackingSupportDictionary)
        {
            message += $"- {ColorChessman(entry.Key.GetComponent<Chessman>())} <color=green>+{attacker.CalculateSupport()}</color><sprite name=\"cross\"> to {ColorChessman(attacker)} attack total\n\n";
            circles.Add(Instantiate(circle, entry.Key.transform.position, Quaternion.identity));
        }
        foreach (var entry in defendingSupportDictionary)
        {
            message += $"- {ColorChessman(entry.Key.GetComponent<Chessman>())} <color=green>+{attacker.CalculateSupport()}</color><sprite name=\"cross\"> to {ColorChessman(defender)} defense total\n\n";
            circles.Add(Instantiate(circle, entry.Key.transform.position, Quaternion.identity));
        }
        ShowTutorial("Supporting", message);
        board.EventHub.OnSupportAdded.RemoveListener(ShowSupportTutorial);
    }

    public void ShowCaptureTutorial(Chessman attacker, Chessman defender)
    {
        string message = $"If the attack total is higher or equal to the defense total the defending piece is captured\n\n";
        message += $"When a piece is captured it is removed from the board, there will be an oppurtunity to return it to the ranks later\n\n";
        message += $"- {ColorChessman(defender)} has been captured by {ColorChessman(attacker)}\n";
        ShowTutorial("Capturing", message);
        board.EventHub.OnPieceCaptured.RemoveListener(ShowCaptureTutorial);
    }

    public void ShowBounceTutorial(Chessman attacker, Chessman defender)
    {
        string message = $"If the attack total is less than the defense total the attacking piece is bounced\n\n";
        message += $"When a piece is bounced both attacking and defending support is ignored. Instead the defender's<sprite name=\"shield\"> stat is reduced by the attacker's<sprite name=\"sword\"> stat\n\n";
        message += $"- {ColorChessman(defender)}<sprite name=\"shield\"> <color=red>-{attacker.CalculateAttack()}</color> from {ColorChessman(attacker)}\n\n";
        message += $"This reduction lasts until the end of a match when each stat is reset to it's base amount\n\n";
        ShowTutorial("Bouncing", message);
        board.EventHub.OnPieceBounced.RemoveListener(ShowBounceTutorial);
    }

    public void ShowMarketTutorial(PieceColor color)
    {
        string message = $"Once the king is captured, the match ends. you recieve a flat reward of <color=yellow>5 coins</color> plus an additional <color=yellow>coin</color> for each of the opponents remaining pieces\n\n";
        message += $"In the prisoners market you may select to <color=green>release</color> or <color=red>kill</color> captured pieces\n\n";
        message += $"- Releasing your pieces costs the piece's return cost and returns the piece to your ranks\n\n";
        message += $"- Releasing opponent pieces returns the piece to the opponents ranks\n\n";
        message += $"- killing pieces gives you its blood value\n\n";
        message += $"- any pieces left in the market are abandoned\n\n";
        ShowTutorial("Prisoner's Market", message);
        board.EventHub.OnGameEnd.RemoveListener(ShowMarketTutorial);
    }

    public void ShowAbilityTutorial(Chessman piece, Ability ability)
    {
        if (board.BoardState == BoardState.RewardScreen)
        {
            string message = $"Each match nets you a reward. Once a piece and an ability are selected, the ability is applied to the piece\n\n";
            message += $"- Unless an ability explicitly notes a permanent modifier, all stat modifiers are temporary. Modifiers are applied at match start or when specifically triggered\n\n";
            message += $"- All modifiers are removed at match end and pieces are reset to there base stats\n\n";
            message += $"- Some abilities are stackable and allow you to place multiple instances of the same ability on a piece\n\n";
            ShowTutorial("Reward & Abilities", message);
            board.EventHub.OnAbilityAdded.RemoveListener(ShowAbilityTutorial);
        }
    }

    public void ShowDiplomacyTutorial(Chessman piece, Ability ability)
    {
        if (board.BoardState == BoardState.None)
        {
            string message = $"Each piece has a diplomacy score noted in the top right of the piece view box\n\n";
            message += $"- A pieces Diplomacy score notes how many abilities it can safely have\n\n";
            message += $"- If one of your captured pieces has more abilities than its diplomacy score, the opponent has a chance to kill the piece before you can release it\n\n";
            message += $"- For each ability over the diplomacy score, a 20% chance is added to its chance to die\n\n";
            ShowTutorial("Diplomacy", message);
            board.EventHub.OnAbilityAdded.RemoveListener(ShowDiplomacyTutorial);
        }
    }

    public void ShowShopTutorial()
    {
        string message = $"In the shop you can purchase new abilities, pieces, and King's orders\n\n";
        message += $"- Selecting an ability and a piece will purchase and apply the ability\n\n";
        message += $"- Clicking a mercenary will purchase the piece and add it to your inventory\n\n";
        message += $"- Clicking a king's order will purchase it and add it to your inventory\n\n";
        ShowTutorial("Shop", message);
        
    }

    public void ShowNewPieceTutorial()
    {
        if (!newUnit)
        {
            newUnit = true;
            string message = $"When a new unit is purchased they are placed in your inventory and can be added to your ranks\n\n";
            message += $"- An inventory icon appears which can be used to immediately place the piece allowing you to add abilities and boost stats while in the shop\n\n";
            message += $"- If not placed immediately you will be prompted to place the piece before the next match\n\n";
            ShowTutorial("New Unit", message);
        }
        
    }

    public void ShowKingsOrderTutorial()
    {
        string message = $"King's orders are powerful one time use items\n\n";
        message += $"- Selecting the cards on the bottom left of the screen brings up the king's order menu\n\n";
        message += $"- All orders can be used during a match\n\n";
        message += $"- Only some orders can be used outside of a match and only usable cards will appear in the menu outside of a match\n\n";
        ShowTutorial("King's Orders", message);
        
    }
    
    public void ShowManagementTutorial()
    {
        string message = $"In the Management window you can place new pieces swap positions and upgrade piece stats\n\n";
        message += $"- Placing a piece is free, you have the first three rows to place pieces\n\n";
        message += $"- Swapping pieces, either with each other or to an open square, costs 2 coins per piece moved\n\n";
        message += $"- Right clicking a piece brings up the info menu, where you can upgrade its stats, this can also be done from the shop menu\n\n";
        ShowTutorial("Management", message);
        
    }

    public void ShowUpgradeTutorial()
    {
        if (!newUpgrade)
        {
            newUpgrade = true;
            string message = $"You can spend blood earned from killing pieces to upgrade your units stats\n\n";
            message += $"- The + buttons below each stat cost <color=red>1 blood</color> and are a permanent increase to the units chosen stat\n\n";
            message += $"- The + button next to the book in the top left is a permenant increase to the units diplomacy and costs <color=yellow>3 coins</color>\n\n";
            message += $"- This menu may be brought up in any screen, however, upgrades can only be made in the shop or management screen\n\n";
            message += $" Always Make sure to use your blood before continuing to the next match";
            ShowTutorial("Stat Upgrades", message);
        }
    }

    public void ShowMapTutorial()
    {
        string message = $"You start your journey on the left and must make your way to the king on the right\n\n";
        message += $"- Select a piece connected to the starting piece to enter the next match\n\n";
        message += $"- The game auto saves once you reach the map screen, any progress will be lost back to your last map screen\n\n";
        message += $"- This is your last tutorial, tutorials can still be viewed from the settings menu\n\n";
        message += $"- good luck\n\n";
        ShowTutorial("Map", message);
        Settings.Instance.Tutorial = false;
        board.SavePrefs();
        
    }

    public string ColorChessman(Chessman piece)
    {
        if (piece.owner == board.Hero)
            return $"<color=green>{piece.name}</color>";
        else
            return $"<color=red>{piece.name}</color>";
    }


    public void ShowTutorial(string titleName, string message)
    {
        Time.timeScale=0;
        title.text = titleName;
        description.text = message;
        this.gameObject.SetActive(true);
    }

    public void CloseTutorial()
    {
        gameObject.SetActive(false);
        foreach (GameObject circle in circles)
        {
            Destroy(circle);
        }
        Time.timeScale = 1;
    }

}
