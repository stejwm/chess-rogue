using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System;

public class AbilityDatabase : MonoBehaviour
{
    [SerializeField] private List<Ability> abilities;
    [SerializeField] private List<KingsOrder> orders;

    private static List<Ability> commonAbilities = new List<Ability>();
    private static List<Ability> uncommonAbilities= new List<Ability>();
    private static List<Ability> rareAbilities= new List<Ability>();
    private Dictionary<string, Ability> abilityDict = new Dictionary<string, Ability>();
    private Dictionary<string, KingsOrder> orderDict = new Dictionary<string, KingsOrder>();

    public static AbilityDatabase Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void Start()
    {
        foreach (var ability in abilities)
        {
            if (!abilityDict.ContainsKey(ability.abilityName))
            {
                string name = ability.abilityName;
                name = Regex.Replace(name, "<.*?>", string.Empty);
                name = name.ToLower().Replace(" ", String.Empty);
                abilityDict.Add(name, ability);
                //Debug.Log($"Ability: {name} added to dictionary as key");
                
            }
            else
            {
                Debug.LogWarning($"Duplicate ability found: {ability.abilityName}. Only the first instance will be used.");
            }
            switch (ability.rarity)
            {
                case Rarity.Common:
                    commonAbilities.Add(ability);
                    break;
                case Rarity.Uncommon:
                    uncommonAbilities.Add(ability);
                    break;
                case Rarity.Rare:
                    rareAbilities.Add(ability);
                    break;
                default:
                    break;

            }
        }
        foreach (var order in orders)
        {
            if (!orderDict.ContainsKey(order.Name))
            {
                string name = order.Name;
                name = Regex.Replace(name, "<.*?>", string.Empty);
                name = name.ToLower().Replace(" ", String.Empty);
                orderDict.Add(name, order);
            }
            else
            {
                Debug.LogWarning($"Duplicate order found: {order.Name}. Only the first instance will be used.");
            }
        }
    }
    public Ability GetRandomAbility()
    {
        if (abilities == null || abilities.Count == 0) return null;
        return abilities[UnityEngine.Random.Range(0, abilities.Count)].Clone();
    }
    public KingsOrder GetRandomOrder()
    {
        if (orders == null || orders.Count == 0) return null;
        return orders[UnityEngine.Random.Range(0, orders.Count)].Clone();
    }
    public Ability GetRandomAbility(int common, int uncommon, int rare)
    {
        int tolken = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"Random tolken for ability {tolken}");
        if (tolken >= (common + uncommon))
            return rareAbilities[UnityEngine.Random.Range(0, rareAbilities.Count)].Clone();
        if (tolken >= common && tolken < (common + uncommon))
            return uncommonAbilities[UnityEngine.Random.Range(0, uncommonAbilities.Count)].Clone();
        if (tolken >= 0 && tolken < common)
            return commonAbilities[UnityEngine.Random.Range(0, commonAbilities.Count)].Clone();
        else
            return null;

    }
    public Ability GetAbilityByName(string name)
    {
        return abilityDict[name.ToLower().Replace(" ","")].Clone();
    }

    public KingsOrder GetOrderByName(string name)
    {
        return orderDict[name.ToLower().Replace(" ","")].Clone();
    }

    public int GetIndexFromAbility(Ability ability)
    {
        return abilities.IndexOf(ability);
    }




}