using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using System.Linq;

public class KingsOrderManager : MonoBehaviour
{
    public List<GameObject> myPieces;
    public List<GameObject> pieces = new List<GameObject>();
    public List<GameObject> cards = new List<GameObject>();
    public Board board;
    [SerializeField] private GameObject noOrdersText;
    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenManagement(Board board)
    {
        this.board = board;
        if (board.previousBoardState == BoardState.ManagementScreen)
        {
            cards = CardFactory.Instance.CreateCards(board.Hero.orders.Where(x => x.canBeUsedFromManagement).ToList());
        }
        else{
            cards = CardFactory.Instance.CreateCards(board.Hero.orders);
        }
        float defaultX = -6.75f;
        float y = -2f;
        float z = -2f;
        float distanceBetweenCards = 1.92f;
        int cardCount = cards.Count;
        float fanAngle = 12f; // total angle to fan out (degrees)
        float angleStep = cardCount > 1 ? fanAngle / (cardCount - 1) : 0;
        float startAngle = -fanAngle / 2f;
        this.gameObject.SetActive(true);

        for (int i = 0; i < cardCount; i++)
        {
            // Center cards around defaultX
            float offset = (i - (cardCount - 1) / 2f) * distanceBetweenCards / cardCount;
            float x = defaultX + offset;
            float angle = startAngle + i * -angleStep;
            cards[i].transform.position = new Vector3(x, y, z + (-0.1f * i));
            cards[i].transform.rotation = Quaternion.Euler(0, 0, angle);
            StartCoroutine(cards[i].GetComponent<Card>().CardHovered());
        }
        if (cardCount > 0)
        {
            noOrdersText.SetActive(false);
        }
        else
        {
            noOrdersText.SetActive(true);
        }
        
    }

    public void ResetCards()
    {
        cards.RemoveAll(item => item == null);
        //cards = CardFactory.Instance.CreateCards(board.Hero.orders);
        float defaultX = -6.75f;
        float y = -2f;
        float z = -2f;
        float distanceBetweenCards = 1.92f;
        int cardCount = cards.Count;
        float fanAngle = 12f; // total angle to fan out (degrees)
        float angleStep = cardCount > 1 ? fanAngle / (cardCount - 1) : 0;
        float startAngle = -fanAngle / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            // Center cards around defaultX
            float offset = (i - (cardCount - 1) / 2f) * distanceBetweenCards / cardCount;
            float x = defaultX + offset;
            float angle = startAngle + i * -angleStep;
            cards[i].transform.position = new Vector3(x, y, z + (-0.1f * i));
            cards[i].transform.rotation = Quaternion.Euler(0, 0, angle);
            StartCoroutine(cards[i].GetComponent<Card>().CardHovered());
        }
    }
    public void HoverCard(Card card)
    {
        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -2f + (-0.1f*cards.Count));
    }

    public void CloseManagement()
    {
        foreach (var card in cards)
            Destroy(card);


        gameObject.SetActive(false);

    }

    


}
