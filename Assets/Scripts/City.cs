using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class City : MonoBehaviour
{
    public int production = 50;
    public bool productionReady;
    public List<Building> buildings = new List<Building>();
    public string cityName = "New City";
    public Tile tile;
    public List<Building> availableBuildings = new List<Building>();
    public Building currentProduction;

    int productionPerTurn = 0;

    public void newTurn()
    {
        if (currentProduction != null)
        {
            if (currentProduction.spent >= currentProduction.price)
            {
                productionReady = true;
            }
            else
            {
                currentProduction.spent += production;
            }
        }
    }

    public void Build(Building building, Manager manager)
    {
        currentProduction = building;
        manager.tasks.Remove(manager.currentTask);
        manager.currentTask = null;
    }
}
