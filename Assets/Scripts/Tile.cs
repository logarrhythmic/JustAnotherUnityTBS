using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public int x, y;
    public int gold, food, production, tourism, faith, science, culture;
    public List<Tile> neighbours = new List<Tile>();
    public enum tiletype { plain, forest, jungle, desert, shore, ocean, mountain, hill, swamp, tundra };
}
