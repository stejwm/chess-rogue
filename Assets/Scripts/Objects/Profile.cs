using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    [SerializeField] private TMP_Text pieceName;
    [SerializeField] private TMP_Text gender;
    [SerializeField] private TMP_Text age;
    [SerializeField] private TMP_Text height;
    [SerializeField] private TMP_Text weight;
    [SerializeField] private TMP_Text pieceClass;
    [SerializeField] private Image sprite;
    // Start is called before the first frame update
    public void SetProfile(Chessman piece)
    {
        //this.piece = piece;
        pieceName.text = piece.name;
        gender.text = piece.gender.ToString();
        age.text = piece.age.ToString();
        height.text = piece.height + "cm";
        weight.text = piece.weight + "lbs";
        pieceClass.text = piece.type.ToString();
        sprite.sprite = piece.isometricSprite;
        if (piece.color == PieceColor.Black)
            sprite.color = new Color32(69, 69, 69, 255);
        else
            sprite.color = Color.white;
    }
}
