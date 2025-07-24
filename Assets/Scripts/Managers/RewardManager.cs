using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    private Card selectedCard;
    private Chessman selectedPiece;
    private Board board;
    private bool applyingAbility;
    private List<GameObject> cards = new List<GameObject>();
    public void Start()
    {
        gameObject.SetActive(false);
    }
    public void Update()
    {
        if (selectedPiece && selectedCard && !applyingAbility){
            applyingAbility = true;
            StartCoroutine(ApplyAbility(selectedPiece));
        }
    }

    public void OpenReward(Board board)
    {
        this.board = board;
        gameObject.SetActive(true);
        cards = CardFactory.Instance.CreateCards(3, board.Hero.RarityWeights);
        int index = 0;
        foreach (var card in cards)
        {
            Vector3 localPosition = new(index * 2 - 1.96f, 2, -2);
            card.transform.position = localPosition;
            index++;
        }

    }

    private IEnumerator ApplyAbility(Chessman target)
    {
        if (selectedCard.price.activeSelf)
        {
            if (selectedCard.ability.Cost > board.Hero.playerCoins)
            {
                selectedCard.GetComponent<MMSpringPosition>().BumpRandom();
                selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                selectedPiece.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCard = null;
                selectedPiece = null;
                yield break;
            }
            board.Hero.playerCoins -= selectedCard.ability.Cost;
        }
        yield return new WaitForSeconds(Settings.Instance.WaitTime);
        StartCoroutine(selectedCard.Dissolve());
        selectedCard.Use(board, target);
        yield return new WaitUntil(() => selectedCard.isDissolved);
        StatBoxManager._instance.SetAndShowStats(selectedPiece);
        Destroy(selectedCard.gameObject);
        ClearSelections();
        applyingAbility = false;
        yield return new WaitForSeconds(Settings.Instance.WaitTime);
        board.CloseReward();
        yield break;
    }
    public void ClearSelections()
    {
        selectedCard = null;
        selectedPiece = null;
        foreach (var card in cards)
        {
            if (card != null)
                Destroy(card);
        }

    }

    public void SelectedCard(Card card)
    {
        if (selectedCard != null && selectedCard == card)
        {
            card.flames.Stop();
            selectedCard = null;
        }
        else if (selectedCard != null && selectedCard != card)
        {
            selectedCard.flames.Stop();
            selectedCard = card;
            selectedCard.flames.Play();

        }
        else
        {
            selectedCard = card;
            selectedCard.flames.Play();
        }
    }
    public void SelectedPiece(Chessman piece)
    {
        if (selectedPiece != null && selectedPiece == piece)
        {
            selectedPiece.flames.Stop();
            selectedPiece = null;
        }
        else if (selectedPiece != null && selectedPiece != piece)
        {
            selectedPiece.flames.Stop();
            selectedPiece = piece;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
        else
        {
            selectedPiece = piece;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
    }

    public void CloseReward(Board board)
    {
        gameObject.SetActive(false);
    }


}
