using UnityEngine;
using System.Collections;

public class Building
{
    public Texture2D icon;
    public string buildingName = "New Building";
    public int price, spent;
    public int production, gold, food, faith, culture, science, happiness;

    public Building(Texture2D _icon, int _price)
    {
        price = _price;
    }
}
