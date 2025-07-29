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
    [SerializeField] private TMP_Text diplomacy;
    [SerializeField] private Image sprite;
    [SerializeField] private GameObject abilitiesContainer;
    [SerializeField] private GameObject historyContainer;
    [SerializeField] private GameObject increaseContainer;
    [SerializeField] private GameObject wheelContainer;
    [SerializeField] private GameObject profile;
    [SerializeField] private GameObject abilityBox;
    [SerializeField] private GameObject abilityUI;
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
        height.text = piece.height + "cm";
        weight.text = piece.weight + "lbs";
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
        SetAbilities();
        gameObject.SetActive(true);

        foreach (Transform child in wheelContainer.transform)
        {
            Destroy(child.gameObject);
        }

        var pieces = piece.owner.pieces;
        int count = pieces.Count;
        

        for (int i = 0; i < count; i++)
        {
            GameObject item = pieces[i];
            GameObject newProfile = Instantiate(profile, wheelContainer.transform);
            newProfile.GetComponent<Profile>().SetProfile(item.GetComponent<Chessman>());

            // Rotate: right if above halfway, left if below
            float angle = 2f; // degrees to rotate
            if (i < count / 2f)
                newProfile.transform.rotation = Quaternion.Euler(0, 0, angle * i);
            else if (i > count / 2f)
                newProfile.transform.rotation = Quaternion.Euler(0, 0, -angle * i);
            else
                newProfile.transform.rotation = Quaternion.identity; // middle one, no rotation
        }
    }

    public void SetAbilities()
    {
        List<Ability> multiples = new List<Ability>();
        foreach (Transform child in abilityBox.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var ability in piece.abilities)
        {
            if(multiples.Contains(ability))
                continue;

            int abilityCount = piece.abilities.Where(s=>s!=null && s.Equals(ability)).Count();
            if(abilityCount>1){
                var icon=Instantiate(abilityUI, abilityBox.transform);
                icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
                icon.GetComponent<AbilityUI>().ability=ability;
                icon.GetComponentInChildren<TMP_Text>().text=$"x{abilityCount}";
                multiples.Add(ability);
            }else{
                var icon=Instantiate(abilityUI, abilityBox.transform);
                icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
                icon.GetComponent<AbilityUI>().ability=ability;
                icon.GetComponentInChildren<TMP_Text>().text="";
            }
            
            
        }
    }

    public void IncreaseAttack()
    {
        if (piece.owner.playerBlood >= 1)
        {
            piece.attack += 1;
            piece.owner.playerBlood -= 1;
            attackVal.text = (Int32.Parse(attackVal.text) + 1).ToString();
        }

    }
    public void IncreaseDefense()
    {
        if (piece.owner.playerBlood >= 1)
        {
            piece.defense += 1;
            piece.owner.playerBlood -= 1;
            defenseVal.text = (Int32.Parse(defenseVal.text) + 1).ToString();
        }
    }
    public void IncreaseSupport()
    {
        if (piece.owner.playerBlood >= 1)
        {
            piece.support += 1;
            piece.owner.playerBlood -= 1;
            supportVal.text = (Int32.Parse(supportVal.text) + 1).ToString();
        }
    }

    public void IncreaseDiplomacy()
    {
        if (piece.owner.playerCoins >= 3)
        {
            piece.diplomacy += 1;
            piece.owner.playerBlood -= 3;
            diplomacy.text = ":" + piece.diplomacy;
        }
    }

    public void ClosePieceInfo()
    {
        gameObject.SetActive(false);
    }
    


}
