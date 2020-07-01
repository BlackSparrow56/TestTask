using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    public Table table;

    public string _name = "name", surname = "surname";
    public int  lastYearEarnings = 0;

    public Text nameText, lastYearEarningsText, strategyTxt; // Компоненты текста.
    public Button details; // Кнопка показа деталей.

    public int index;

    public void Set(Trader trader)
    {
        _name = trader._name;
        surname = trader.surname;
        lastYearEarnings = trader.lastYearEarnings;

        nameText.text = $"{_name} {surname}";
        lastYearEarningsText.text = $"{lastYearEarnings.ToString()} золотых";
        strategyTxt.text = trader.strategy.ToString();

        details.onClick.AddListener((() => table.guild.details.Set(table.guild.traders[index])));
    }
}
