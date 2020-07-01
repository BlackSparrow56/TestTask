using UnityEngine;
using UnityEngine.UI;

public class Details : MonoBehaviour
{
    public Text idText, _nameText, surnameText, howLongDoIsHeConsistInTheGuildText, lastYearEarningsText, summaryEarningsText;

    public void Set(Trader trader)
    {
        idText.text = $"Идентификатор: {trader.id.ToString()}";
        _nameText.text = $"Имя: {trader._name}";
        surnameText.text = $"Фамилия: {trader.surname}";
        howLongDoIsHeConsistInTheGuildText.text = $"Как давно состоит в гильдии: {trader.howLongDoIsHeConsistInTheGuild.ToString()} год / лет";
        lastYearEarningsText.text = $"Доход за последний год: {trader.lastYearEarnings.ToString()} золотых(ой)";
        summaryEarningsText.text = $"Общий доход: {trader.summaryEarnings.ToString()} золотых(ой)";
    }
}
