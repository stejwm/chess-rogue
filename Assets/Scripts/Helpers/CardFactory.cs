using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using MoreMountains.Feel;

public class CardFactory : MonoBehaviour
{
    public static CardFactory Instance { get; private set; }

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Material holoMaterial;
    [SerializeField] private Sprite abilitySprite;
    [SerializeField] private Sprite orderSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<GameObject> CreateCardsWithAbilities(List<Ability> abilities)
    {
        List<GameObject> cards = new List<GameObject>();
        for (int i = 0; i < abilities.Count; i++)
        {
            GameObject obj = Instantiate(cardPrefab, new Vector3(0, 0, -2), Quaternion.identity);
            obj.GetComponent<Card>().ability = abilities[i];

            if (obj.GetComponent<Card>().ability.rarity > 0)
            {
                obj.GetComponent<SpriteRenderer>().material = holoMaterial;
            }

            cards.Add(obj);


        }
        return cards;
    }

    public GameObject CreateCard(Ability ability)
    {
        GameObject obj = Instantiate(cardPrefab, new Vector3(0, 0, -2), Quaternion.identity);
        obj.GetComponent<SpriteRenderer>().sprite = abilitySprite;
        obj.GetComponent<Card>().ability = ability;

        if (ability.rarity > 0)
        {
            obj.GetComponent<SpriteRenderer>().material = holoMaterial;
        }

        return obj;
    }

    public GameObject CreateCard(KingsOrder order)
    {
        GameObject obj = Instantiate(cardPrefab, new Vector3(0, 0, -2), Quaternion.identity);
        obj.GetComponent<Card>().order = order;
        return obj;
    }
    public List<GameObject> CreateCards(List<KingsOrder> orders)
    {
        List<GameObject> cards = new List<GameObject>();
        foreach (var order in orders)
        {
            GameObject obj = Instantiate(cardPrefab, new Vector3(0, 0, -2), Quaternion.identity);
            obj.GetComponent<SpriteRenderer>().sprite = orderSprite;
            obj.GetComponent<Card>().order = order;
            cards.Add(obj);
        }
        return cards;
    }

    public GameObject CreateRandomKOCard()
    {
        GameObject obj = Instantiate(cardPrefab, new Vector3(0, 0, -2), Quaternion.identity);
        obj.GetComponent<Card>().order = AbilityDatabase.Instance.GetRandomOrder();
        return obj;
    }

    public List<GameObject> CreateRandomKOCards(int n)
    {
        List<GameObject> orders = new List<GameObject>();
        for (int i = 0; i < n; i++)
        {
            GameObject obj = Instantiate(cardPrefab, new Vector3(0, 0, -2), Quaternion.identity);
            obj.GetComponent<Card>().order = AbilityDatabase.Instance.GetRandomOrder();
            orders.Add(obj);
        }
        return orders;
    }

    public List<GameObject> CreateCards(int n, Dictionary<Rarity, int> rarities)
    {
        List<GameObject> cards = new List<GameObject>();
        for (int i = 0; i < n; i++)
        {
            var card = CreateCard(AbilityDatabase.Instance.GetRandomAbility(rarities[Rarity.Common], rarities[Rarity.Uncommon], rarities[Rarity.Rare]));
            cards.Add(card);
        }
        return cards;
    }
    
    public GameObject CreateCardOfRarity(Rarity rarity)
    {
        return CreateCard(AbilityDatabase.Instance.GetAbilityOfRarity(rarity));
    }

    
}