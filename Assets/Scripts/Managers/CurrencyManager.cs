using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text bloodText;

    private int lastCoins = -1;
    private int lastBlood = -1;
    private Board board;

    private void Update()
    {
        if (board == null || board.Hero == null)
            return;
        UpdateCurrencyUI();
    }

    public void Initialize(Board b)
    {
        board = b;
        UpdateCurrencyUI(force: true);
    }

    public void UpdateCurrencyUI(bool force = false)
    {
        if (board == null || board.Hero == null)
            return;

        int coins = board.Hero.playerCoins;
        int blood = board.Hero.playerBlood;

        if (force || coins != lastCoins)
        {
            coinText.text = $": {coins}";
            lastCoins = coins;
        }
        if (force || blood != lastBlood)
        {
            bloodText.text = $": {blood}";
            lastBlood = blood;
        }
    }
}
