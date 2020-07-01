using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Этот скрипт занимается созданием таблиц.
public class Table : MonoBehaviour
{
    public Guild guild; // Торговая гильдия, данные о которой будем отображать.
    public Transform content;

    public GameObject linePrefab; // Префаб строки.
    public GameObject[] linesObjects;
    public Line[] lines;

    private void Start()
    {
        guild.table = this;

        lines = new Line[guild.traders.Length];
        linesObjects = new GameObject[guild.traders.Length];
    }

    public void Set()
    {
        for (int i = 0; i < guild.traders.Length; i++)
        {
            Destroy(linesObjects[i]);

            GameObject newLine = Instantiate(linePrefab, content);
            linesObjects[i] = newLine;

            lines[i] = newLine.GetComponent<Line>();
            lines[i].table = this;
            lines[i].index = i;

            lines[i].Set(guild.traders[i]);
        }
    }
}
