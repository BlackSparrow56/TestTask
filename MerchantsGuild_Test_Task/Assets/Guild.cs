using System;
using System.Collections;
using UnityEngine;

public class Guild : MonoBehaviour
{
    public Trader[] traders = new Trader[60];
    public Details details;
    public Table table;

    private void Start()
    {
        for (int i = 0; i < 60; i++)
        {
            traders[i] = new Trader((Trader.Strategy)(i % 6)); // Присваивание стратегий.
        }

        table.Set();
    }

    public void Skip(int years)
    {
        for (int i = 0; i < years; i++)
        {
            for (int j = 0; j < traders.Length; j++)
            {
                traders[j].lastYearEarnings = 0;
                traders[j].howLongDoIsHeConsistInTheGuild++;

                for (int k = 0; k < traders.Length; k++)
                {
                    if (j != k) traders[j].Trade(traders[k]); // Торгуемся!
                }
            }

            Array.Sort(traders, new CompareByLastYearEarnings()); // Сортируем массив по доходам за последний год.

            for (int j = 0; j < traders.Length * 0.2f; j++)
            {
                traders[traders.Length - j - 1] = new Trader(traders[j].strategy); // Прогоняем банкротов и приглашаем выпускников Гарварда.
            }
        }

        table.Set();
    }
}

public class CompareByLastYearEarnings : IComparer // Класс для сравнения торговцев по их доходам за последний год.
{
    int IComparer.Compare(object x, object y)
    {
        Trader X = (Trader)x;
        Trader Y = (Trader)y;

        return Y.lastYearEarnings - X.lastYearEarnings;
    }
}


public struct TradeInfo
{
    public StepInfo[] steps; // Информация о всех ходах сделки.
    public int current; // Текущий ход сделки.

    public struct StepInfo
    {
        bool isTricked; // Был обманут?

        public StepInfo(bool b)
        {
            isTricked = b;
        }

        public bool Deconstruct()
        {
            return isTricked;
        }
    }

    public bool IsTricked() // Возвращает true, если против него хоть раз сжульничали.
    {
        foreach (StepInfo step in steps)
        {
            if (step.Deconstruct())
            {
                return true;
            }
        }

        return false;
    }

    public TradeInfo(int stepsCount)
    {
        steps = new StepInfo[stepsCount];
        current = 0;
    }
}

public class Trader
{
    public static readonly string[] names =
    {
        "Багоня", "Батура", "Бобырь", "Богомяк", "Борил", "Бронислав", "Велинег", "Вихура", "Вольга", "Гамаюн",
        "Годота", "Гостенег", "Гудила", "Добродей", "Довгуш", "Докука", "Дреман", "Елага", "Ероха", "Жегло",
        "Желыба", "Жуяга", "Замята", "Звездан", "Златояр", "Иверень", "Истома", "Капица", "Кресомысл", "Кислоквас",
        "Лунь", "Лызло", "Малюта", "Микула", "Огурец", "Окула", "Осмак", "Перепят", "Попель", "Потан",
        "Ратибор", "Рутын", "Свирыня", "Сева", "Скордяй", "Твердополк", "Томило", "Угрюм ", "Хомун ", "Цедеда",
        "Чаян", "Чулок", "Шемяка", "Шишка", "Юша", "Ягнило", "Ядыка", "Яролик", "Яромудр", "Ярополк"
    }; // Список допустимых имён (60 шт.). Все имена настоящие. Искал здесь: https://pomnirod.ru/articles/istorii-familii/imena/imena-raznyh-narodov/slavyanskie-imena-kotorye-byli-tradicionnymi-na-rusi.html

    public static readonly string[] surnames =
    {
        "Апухтин", "Бантыш-Каменский", "Бараташвили", "Безсонов", "Бенкендорф", "Блок", "Буксгевден", "Веренич-Стаховский", "Вабищевич-Плотницкий", "Валуев",
        "Веригин", "Волынский", "Воронцов-Дашков", "Всеволожский", "Вяземский", "Гамалей", "Гедиминович", "Гечба", "Гурко-Ромейко", "Джавахишвили",
        "Джендубаев", "Дзяпш-Ипа", "Дмитриев-Мамонов", "Дондуков-Корсаков", "Друцкой-Любецкий", "Ермолов", "Ермолов", "Жабин", "Ждан-Пушкин", "Жедринский",
        "Желтухин", "Жихарев", "Жоховский", "Жуковский", "Жуков", "Заблоцкий-Десятовский", "Заболоцкий", "Завиша", "Зайончек", "Закревский",
        "Замятин", "Заремба", "Збаражский", "Злобин", "Значко-Яворский", "Зыбин", "Зыков", "Иашвили", "Иванчин-Писарев", "Извольский",
        "Изенбург", "Икскуль-Гильденбандт", "Инал-Ипа", "Ипатович-Горанский", "Искрицкий", "Йозефович", "Кавендиш", "Казембек", "Калитеевский", "Калугин",
        "Кампенгаузен", "Кар", "Кац", "Караффа-Корбут", "Карп", "Касаткин-Ростовский", "Керн", "Клиффорд", "Ковальский", "Кноп",
        "Кольцов-Мосальский", "Комар", "Корнилов", "Корсак", "Крейц", "Криштафович", "Крылов", "Кузмин-Караваев", "Курош", "Кюхельбекер",
        "Лазарев-Станищев", "Ламбсдорф", "Лобанов-Ростовский", "Маврокордато", "Магалашвили", "Мачабели", "Меллер-Закомельский", "Меншиков", "Мусоргский", "Мышецкий",
        "Невзоров", "Нижарадзе", "Оболенский", "Орлов", "Остен-Сакен", "Палавандишвили", "Фальц-Фейн", "Чарторыйский", "Чичагов", "Щербатов"
    }; // 100 фамилий. Взяты отсюда: https://www.waylux.ru/imena_dvoryanskie.html

    public static int tradersCount = 0;

    public enum Strategy
    {
        Altruist, // Альтруист.
        Trickster, // Кидала.
        Cunning, // Хитрец.
        Unpredictable, // Непредсказуемый.
        Vindictive, // Злопамятный.
        Quirky // Ушлый.
    }

    public Strategy strategy; // Стратегия поведения конкректного торговца.

    public string _name, surname; // Имя и фамилия торговца.

    public int id, // Индивидуальный идентификатор торговца на случай совпадения фамилии.
               howLongDoIsHeConsistInTheGuild, // Как долго он состоит в гильдии.
               lastYearEarnings = 0, // Выручка за последний год.
               summaryEarnings = 0; // Суммарная выручка за все сделки.

    public void Trade(Trader trader)
    {
        int tradesCount = UnityEngine.Random.Range(5, 11); // Количество сделок (случайное, от 5 до 10).

        TradeInfo myTrade = new TradeInfo(tradesCount);
        TradeInfo partnerTrade = myTrade;

        for (int i = 0; i < tradesCount; i++)
        {
            bool myChoose = TrickOrNot(myTrade),
                 partnerChoose = trader.TrickOrNot(partnerTrade);

            myTrade.steps[myTrade.current] = new TradeInfo.StepInfo(partnerChoose);
            myTrade.current++;

            partnerTrade.steps[partnerTrade.current] = new TradeInfo.StepInfo(myChoose);
            partnerTrade.current++;

            int myGonorar, partnerGonorar; // Сколько кто заработает.

            if (!myChoose && !partnerChoose)
            {
                myGonorar = 4;
                partnerGonorar = myGonorar;
            }
            else if (myChoose && !partnerChoose)
            {
                myGonorar = 5;
                partnerGonorar = 1;
            }
            else if (!myChoose && partnerChoose)
            {
                myGonorar = 1;
                partnerGonorar = 5;
            }
            else
            {
                myGonorar = 2;
                partnerGonorar = 2;
            }

            Pay(myGonorar);
            trader.Pay(partnerGonorar);
        }
    }

    public void Pay(int payment)
    {
        lastYearEarnings += payment;
        summaryEarnings += payment;
    }

    public bool TrickOrNot(TradeInfo trade) // Возвращает true, если намерен обмануть.
    {
        bool isTricking = false;

        switch (strategy)
        {
            case Strategy.Altruist:
                break;
            case Strategy.Trickster:
                isTricking = true;
                break;
            case Strategy.Cunning:
                if (trade.current != 0) isTricking = trade.steps[trade.current - 1].Deconstruct();
                break;
            case Strategy.Unpredictable:
                isTricking = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;
                break;
            case Strategy.Vindictive:
                if (trade.IsTricked()) isTricking = true;
                break;
            case Strategy.Quirky:
                if (trade.current == 0 || trade.current == 2 || trade.current == 3) break;
                else if (trade.current == 1) isTricking = true;
                else
                {
                    if (trade.IsTricked()) goto case Strategy.Trickster; // Простите за goto!
                    else goto case Strategy.Cunning; // Я больше так не буду!!!
                }
                break;
        }

        return (UnityEngine.Random.Range(0, 101) > 5) ? isTricking : !isTricking; // Ошибка с вероятностью 5%.
    }

    public Trader()
    {
        _name = names[UnityEngine.Random.Range(0, names.Length)]; // Присваиваем случайное имя из массива.
        surname = surnames[UnityEngine.Random.Range(0, surnames.Length)]; // Поступаем также с фамилией.
        id = tradersCount; // Присваиваем идентификатор.
        tradersCount++; // Увеличиваем количество торговцев.
    }

    public Trader(Strategy strategy) : this()
    {
        this.strategy = strategy; // Присвоить стратегию.
    }
}
