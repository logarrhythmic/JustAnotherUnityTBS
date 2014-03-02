using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class City : MonoBehaviour
{
    public int production;
    public bool productionReady;
    public List<Building> buildings = new List<Building>();
    public string name = "New City";
}
