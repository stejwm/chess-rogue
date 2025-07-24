using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Rand= System.Random;
using MoreMountains.Feedbacks;
public class ShopManager : MonoBehaviour
{

    public List<GameObject> pieces = new List<GameObject>();
    private List<GameObject> cards = new List<GameObject>();
    private List<GameObject> orders = new List<GameObject>();
    private Card selectedCard;
    private Chessman selectedPiece;
    [SerializeField] private TMP_Text rerollCostText;
    [SerializeField] private GameObject managementButton;
    private bool applyingAbility;


    [SerializeField] private Board board;

    public void Start()
    {
        managementButton.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (selectedPiece && selectedCard && !applyingAbility)
        {
            applyingAbility = true;
            StartCoroutine(ApplyAbility(selectedPiece));
        }
        if (board && board.Hero.inventoryPieces.Count > 0)
        {
            managementButton.SetActive(true);
        }
        else
        {
            managementButton.SetActive(false);
        }
    }
    public void OpenShop(Board board)
    {
        this.board = board;
        gameObject.SetActive(true);

        CreatePieces();
        if (board.LastingLegacyAbility != null)
        {
            List<Ability> abilities = new List<Ability> { board.LastingLegacyAbility.Clone(), board.LastingLegacyAbility.Clone(), board.LastingLegacyAbility.Clone() };
            cards = CardFactory.Instance.CreateCardsWithAbilities(abilities);
            board.LastingLegacyAbility = null;
        }
        else
        {
            cards = CardFactory.Instance.CreateCards(3, board.Hero.RarityWeights);

        }
        int index = 0;
        foreach (var card in cards)
        {
            Vector3 localPosition = new(index * 2 - (1.96f * 2), 2, -2);
            //card.transform.position = localPosition;
            var spring = card.GetComponent<MMSpringPosition>();
            spring.MoveTo(localPosition);
            card.GetComponent<Card>().ShowPrice();
            index++;
        }
        orders = CardFactory.Instance.CreateRandomKOCards(2);
        index = 0;
        foreach (var order in orders)
        {
            Vector3 localPosition = new(index * 2 + (1.96f * 2), 2, -2);
            var spring = order.GetComponent<MMSpringPosition>();
            spring.MoveTo(localPosition);
            order.GetComponent<Card>().ShowPrice();
            index++;
        }
        rerollCostText.text = board.RerollCost.ToString();

    }
    private IEnumerator ApplyAbility(Chessman target)
    {
        board.BoardState = BoardState.None;
        if (selectedCard.price.activeSelf)
        {
            if (selectedCard.ability.Cost > board.Hero.playerCoins)
            {
                selectedCard.GetComponent<MMSpringPosition>().BumpRandom();
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
        board.BoardState = BoardState.ShopScreen;
        yield return new WaitForSeconds(Settings.Instance.WaitTime);
        yield break;
    }
    public void RerollAbilities()
    {
        if (applyingAbility)
            return;
        if (board.Hero.playerCoins < board.RerollCost)
            return;

        board.Hero.playerCoins -= board.RerollCost;
        foreach (var card in cards)
        {
            Destroy(card);
        }
        cards = CardFactory.Instance.CreateCards(3, board.Hero.RarityWeights);
        int index = 0;
        foreach (var card in cards)
        {
            Vector3 localPosition = new(index * 2 - (1.96f * 2), 2, -2);
            //card.transform.position = localPosition;
            var spring = card.GetComponent<MMSpringPosition>();
            spring.MoveTo(localPosition);
            card.GetComponent<Card>().ShowPrice();
            index++;
            //card.GetComponent<MMSpringPosition>().BumpRandom();
        }
        board.RerollCost += board.RerollCostIncrease;
        rerollCostText.text = board.RerollCost.ToString();

    }
    public void CreatePieces()
    {
        GameObject obj;
        for (int i = 0; i < 3; i++)
        {
            Vector2 localPosition = new Vector2(i - 4, 2);
            obj = PieceFactory._instance.CreateRandomPiece(board);
            obj.transform.position = localPosition;
            Chessman cm = obj.GetComponent<Chessman>();
            cm.xBoard = 5 + i;
            cm.yBoard = 3;
            board.PlacePiece(cm, board.GetTileAt(5 + i, 3));
            SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
            cm.flames.GetComponent<Renderer>().sortingOrder = 6;
            rend.sortingOrder = 5;
            pieces.Add(obj);
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
    public void PurchasePiece(Chessman piece)
    {
        if (board.Hero.playerCoins >= piece.releaseCost && board.Hero.openPositions.Count > board.Hero.inventoryPieces.Count)
        {
            board.Hero.playerCoins -= piece.releaseCost;
            board.Hero.inventoryPieces.Add(piece.gameObject);
            piece.owner = board.Hero;
            piece.UpdateUIPosition();
            SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.purchase, 1f, Settings.Instance.SfxVolume);
            pieces.Remove(piece.gameObject);
            piece.gameObject.SetActive(false);
        }
        else
        {
            piece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }
    public void SelectedOrder(Card card)
    {
        if (board.Hero.playerCoins >= card.order.Cost)
        {
            board.Hero.playerCoins -= card.order.Cost;
            board.Hero.orders.Add(card.order);
            Destroy(card.gameObject);
        }
        else
        {
            card.GetComponent<MMSpringPosition>().BumpRandom();
        }

    }
    public void ClearSelections()
    {
        selectedCard = null;
        selectedPiece = null;
    }
    public void CloseShop()
    {
        foreach (var card in cards)
        {
            Destroy(card);
        }
        foreach (var order in orders)
        {
            Destroy(order);
        }
        foreach (var piece in pieces)
        {
            Destroy(piece);
        }
        PopUpManager._instance.HideValues();
        gameObject.SetActive(false);
    }

    public void HideShop()
    {
        foreach (var card in cards)
        {
            if(card)
                card.SetActive(false);
        }
        foreach (var order in orders)
        {
            if(order)
                order.SetActive(false);
        }
        foreach (var piece in pieces)
        {
            if(piece)
                piece.SetActive(false);
        }
        gameObject.SetActive(false);

    }

    public void OpenShopFromManagement()
    {
        gameObject.SetActive(true);
        foreach (var card in cards)
        {
            if(card)
                card.SetActive(true);
        }
        foreach (var order in orders)
        {
            if(order)
                order.SetActive(true);
        }
        foreach (var piece in pieces)
        {
            if(piece)
                piece.SetActive(true);
        }
    }
}
