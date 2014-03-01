using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Manager : Photon.MonoBehaviour
{
    // player stats
    public int science, gold, happiness, goldenage, culture;
    public enum civilization { Greeks, Egyptians };

    public Tile[,] tiles;
    public float tileGap, rowGap, tileWidth;
    public bool ready = false;
    public float resoWidth = 1280, resoHeight = 720;
    public string gamestate = "connecting", guistate = "connecting";
    public int levelWidth = 50, levelHeight = 50;
    public List<PhotonPlayer> readPlayers;

    int pendingChunks = 0;
    Vector2 spawnCoordinates;
    List<Tile> spawns = new List<Tile>();
    GameObject levelGeometry;
    List<Unit> units;

    void Start()
    {
        Application.runInBackground = true;
        PhotonNetwork.ConnectUsingSettings("1.0");
        readPlayers = new List<PhotonPlayer>();
        spawnCoordinates = new Vector2();
        levelGeometry = new GameObject("levelGeometry");
        levelGeometry.transform.position = Vector3.zero;
        units = new List<Unit>();
    }

    void OnGUI()
    {
        Vector3 s = new Vector3(Screen.width / resoWidth, Screen.height / resoHeight, 1f);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, s);

        switch (guistate)
        {
            case "start":
                break;
            case "readycheck":
                GUI.Label(new Rect(0, 0, 1280, 40), "The Game");
                GUI.Box(new Rect(0, 40, 640, 380), "Players");
                GUILayout.BeginArea(new Rect(0, 40 + 32, 640, 348));
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    if (player == PhotonNetwork.player)
                    {
                        string str = player.name;
                        if (str == "")
                            str = "Change this";
                        str = GUILayout.TextField(str, GUILayout.Width(125));
                        player.name = str;
                    }
                    else
                        if (player.name == "")
                            GUILayout.Label("no name", GUILayout.Width(125));
                        else
                            GUILayout.Label(player.name, GUILayout.Width(125));
                    // civilization here
                    GUILayout.Space(125);
                    if (readPlayers.Contains(player))
                        GUILayout.Label("Ready", GUILayout.Width(75));
                    else
                        GUILayout.Label("Not ready", GUILayout.Width(75));
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
                GUI.Box(new Rect(0, 420, 640, 300), "");
                GUI.Box(new Rect(640, 40, 640, 600), "Game information");
                GUI.Box(new Rect(640, 640, 640, 80), "");
                GUILayout.BeginArea(new Rect(640, 640, 640, 80));
                if (readPlayers.Contains(PhotonNetwork.player))
                {
                    if (GUILayout.Button("Not ready!", GUILayout.Width(640), GUILayout.Height(80)))
                    {
                        photonView.RPC("PlayerReadyChange", PhotonTargets.All, PhotonNetwork.player);
                    }
                }
                else
                    if (GUILayout.Button("Ready!", GUILayout.Width(640), GUILayout.Height(80)))
                        photonView.RPC("PlayerReadyChange", PhotonTargets.MasterClient, PhotonNetwork.player);
                GUILayout.EndArea();
                break;
            case "game":
                GUI.Box(new Rect(0, 0, 1280, 40), "");
                GUILayout.BeginArea(new Rect(0, 0, 1280, 32));
                GUILayout.BeginVertical();
                GUILayout.EndVertical();
                GUILayout.EndArea();
                break;
            case "generating":
                break;
            default:
                GUI.Label(new Rect(600, 700, 80, 20), "Loading....");
                break;
        }
    }

    void Update()
    {
        switch (gamestate)
        {
            case "start":
                if (readPlayers.Count == PhotonNetwork.playerList.Length)
                {
                    readPlayers = new List<PhotonPlayer>();
                    gamestate = "generating";
                    guistate = "generating";
                    string[] data = Generate(levelWidth, levelHeight);
                    photonView.RPC("GetChunkAmount", PhotonTargets.All, levelWidth, levelHeight);
                    for (int i = 0; i < levelHeight; i++)
                    {
                        string[] newdata = new string[levelWidth];
                        Array.Copy(data, i * levelWidth, newdata, 0, levelWidth);
                        photonView.RPC("CallLevel", PhotonTargets.All, newdata);
                    }
                    StartCoroutine("RandomSpawns");
                }
                break;
            case "generating":
                if (gamestate == "generating" && pendingChunks == 0)
                {
                    StartCoroutine("Neighbours");
                    guistate = "game";
                }
                break;
            case "spawn":
                PhotonNetwork.Instantiate("Settler", tiles[(int)spawnCoordinates.x, (int)spawnCoordinates.y].gameObject.transform.position, Quaternion.identity, 0);
                gamestate = "waitingforstart";
                photonView.RPC("PlayerReadyChange", PhotonNetwork.masterClient, PhotonNetwork.player);
                break;
            case "waitingforstart":
                if (PhotonNetwork.isMasterClient)
                {
                    if (readPlayers.Count == PhotonNetwork.playerList.Length)
                    {
                        photonView.RPC("StartTurn", PhotonTargets.All);
                    }
                }
                break;
        }
    }

    [RPC]
    void StartTurn()
    {

    }

    IEnumerator RandomSpawns()
    {
        while (true)
        {
            if (pendingChunks == 0 && gamestate == "generating")
            {
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    int x = UnityEngine.Random.Range(0, levelWidth-1);
                    int y = UnityEngine.Random.Range(0, levelHeight-1);
                    photonView.RPC("GetSpawn", player, new Vector2(x, y));
                }
                break;
            }
            yield return null;
        }
    }

    [RPC]
    void GetSpawn(Vector2 spawn)
    {
        spawnCoordinates = spawn;
        gamestate = "spawn";
    }

    IEnumerator Neighbours()
    {
        for (int y = 0; y < tiles.GetLength(1); y++)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                if (x - 1 >= 0 && y - 1 >= 0)
                {
                    tiles[x, y].neighbours.Add(tiles[x - 1, y - 1]);
                }
                if (y - 1 >= 0)
                {
                    tiles[x, y].neighbours.Add(tiles[x, y - 1]);
                }
                if (x - 1 >= 0)
                {
                    tiles[x, y].neighbours.Add(tiles[x - 1, y]);
                }
                if (x + 1 <= tiles.GetLength(0) - 1)
                {
                    tiles[x, y].neighbours.Add(tiles[x + 1, y]);
                }
                if (y - 1 >= tiles.GetLength(1) - 1 && x - 1 >= 0)
                {
                    tiles[x, y].neighbours.Add(tiles[y - 1, x - 1]);
                }
                if (y - 1 >= tiles.GetLength(1) - 1)
                {
                    tiles[x, y].neighbours.Add(tiles[y - 1, x]);
                }
            }
        }
        yield return null;
    }

    string[] Generate(int width, int height)
    {
        int count = width * height;
        string[] leveldata = new string[count];
        for (int i = 0; i < count; i++)
        {
            Tile t = new Tile();
            t.x = i % width;
            t.y = i / width;
            leveldata[i] = "x:" + t.x + ":y:" + t.y + ":gold:" + t.gold + ":production:" + t.production + ":tourism:" + t.tourism + ":faith:" + t.faith + ":science:" + t.science + ":culture:" + t.culture + ":food:" + t.food;
        }
        return leveldata;
    }

    [RPC]
    void GetChunkAmount(int width, int height)
    {
        pendingChunks = height;
        tiles = new Tile[levelWidth, levelHeight];
        gamestate = "generating";
        guistate = "generating";
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.JoinRoom("thegame");
        gamestate = "connected";
    }

    void OnPhotonJoinRoomFailed()
    {
        PhotonNetwork.CreateRoom("thegame", true, true, 64);
    }

    void OnJoinedRoom()
    {
        gamestate = "start";
        guistate = "readycheck";
    }

    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        if (PhotonNetwork.isMasterClient)
        {

        }
    }

    void OnPhotonCreateRoomFailed()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    [RPC]
    void CallLevel(string[] level)
    {
        StartCoroutine("Level", level);
    }

    IEnumerator Level(string[] level)
    {
        for (int i = 0; i < level.Length; i++)
        {
            GameObject go = Instantiate(Resources.Load("tile"), Vector3.zero, Quaternion.identity) as GameObject;
            Tile t = go.GetComponent<Tile>();
            string[] data = level[i].Split(':');
            for (int j = 0; j < data.Length; j++)
            {
                switch (data[j])
                {
                    case "x":
                        t.x = int.Parse(data[j + 1]);
                        break;
                    case "y":
                        t.y = int.Parse(data[j + 1]);
                        break;
                    case "gold":
                        t.gold = int.Parse(data[j + 1]);
                        break;
                    case "production":
                        t.production = int.Parse(data[j + 1]);
                        break;
                    case "food":
                        t.food = int.Parse(data[j + 1]);
                        break;
                    case "science":
                        t.science = int.Parse(data[j + 1]);
                        break;
                    case "faith":
                        t.faith = int.Parse(data[j + 1]);
                        break;
                    case "tourism":
                        t.tourism = int.Parse(data[j + 1]);
                        break;
                    default:
                        break;
                }
            }
            tiles[t.x, t.y] = t;
            go.transform.position = new Vector3(tileWidth * t.x + rowGap * t.y % 2, 0, tileWidth * t.y);
            go.transform.parent = levelGeometry.transform;
            if (i % 10 == 0)
                yield return null;
        }
        pendingChunks--;
    }

    [RPC]
    void PlayerReadyChange(PhotonPlayer player)
    {
        if (readPlayers.Contains(player))
            readPlayers.Remove(player);
        else
            readPlayers.Add(player);
    }
}
