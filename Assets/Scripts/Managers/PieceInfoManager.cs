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
    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenPieceInfo(Chessman piece)
    {
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
        gameObject.SetActive(true);
    }

    public void ClosePieceInfo()
    {
        gameObject.SetActive(false);
    }
    


}
