using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class AbilityDatabase : MonoBehaviour
{
    [SerializeField] private static List<Ability> abilities;
    [SerializeField] private static List<KingsOrder> orders;
    private Dictionary<string, Ability> abilityDict = new Dictionary<string, Ability>();
    private Dictionary<string, KingsOrder> orderDict = new Dictionary<string, KingsOrder>();

    public static AbilityDatabase _instance;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public void Start()
    {
        foreach (var ability in abilities)
        {
            if (!abilityDict.ContainsKey(ability.abilityName))
            {
                abilityDict.Add(ability.abilityName.ToLower().Trim(), ability);
            }
            else
            {
                Debug.LogWarning($"Duplicate ability found: {ability.abilityName}. Only the first instance will be used.");
            }
        }
        foreach (var order in orders)
        {
            if (!orderDict.ContainsKey(order.Name))
            {
                orderDict.Add(order.Name.ToLower().Trim(), order);
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
        return abilities[Random.Range(0, abilities.Count)];
    }
    public Ability GetAbilityByName(string name)
    {
        return abilityDict[name.ToLower().Trim()].Clone();
    }

    public KingsOrder GetOrderByName(string name)
    {
        return orderDict[name.ToLower().Trim()].Clone();
    }

    



}