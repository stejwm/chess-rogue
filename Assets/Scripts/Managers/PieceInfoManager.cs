using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PieceInfoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text pieceName;
    [SerializeField] private TMP_Text gender;
    [SerializeField] private TMP_Text age;
    [SerializeField] private TMP_Text height;
    [SerializeField] private TMP_Text weight;
    [SerializeField] private TMP_Text pieceClass;
    [SerializeField] private TMP_Text attackVal;
    [SerializeField] private TMP_Text defenseVal;
    [SerializeField] private TMP_Text supportVal;
    [SerializeField] private Image sprite;
    [SerializeField] private GameObject abilitiesContainer;
    [SerializeField] private GameObject historyContainer;
    [SerializeField] private GameObject increaseContainer;
    private Chessman piece;
    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenPieceInfo(Board board, Chessman piece)
    {
        this.piece = piece;
        pieceName.text = piece.name;
        gender.text = piece.gender.ToString();
        age.text = piece.age.ToString();
        height.text = piece.height+"cm";
        weight.text = piece.weight+"lbs";
        pieceClass.text = piece.type.ToString();
        attackVal.text = piece.attack.ToString();
        defenseVal.text = piece.defense.ToString();
        supportVal.text = piece.support.ToString();
        sprite.sprite = piece.isometricSprite;
        if (piece.color == PieceColor.Black)
            sprite.color = new Color32(69, 69, 69, 255);
        else
            sprite.color = Color.white;
        if (board.previousBoardState != BoardState.ActiveMatch)
            increaseContainer.SetActive(true);
        else
            increaseContainer.SetActive(false);
        gameObject.SetActive(true);
    }

    public void IncreaseAttack()
    {
        if (piece.owner.playerBlood >= 1)
        {
            piece.attack += 1;
            piece.owner.playerBlood -= 1;
            attackVal.text = Int32.Parse(attackVal.text) + 1.ToString();
        }

    }
    public void IncreaseDefense()
    {
        if (piece.owner.playerBlood >= 1)
        {
            piece.defense += 1;
            piece.owner.playerBlood -= 1;
            defenseVal.text = Int32.Parse(defenseVal.text) + 1.ToString();
        }
    }
    public void IncreaseSupport()
    {
        if (piece.owner.playerBlood >= 1)
        {
            piece.support += 1;
            piece.owner.playerBlood -= 1;
            supportVal.text = Int32.Parse(supportVal.text) + 1.ToString();
        }
    }

    public void ClosePieceInfo()
    {
        gameObject.SetActive(false);
    }
    


}
