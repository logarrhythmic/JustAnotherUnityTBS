using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Manager : Photon.MonoBehaviour
{
    public GameObject tile;
    public int tileGap, rowGap, tileWidth;
    public bool ready = false;
    public float resoWidth = 1280, resoHeight = 720;
    public string gamestate = "connecting", guistate = "connecting";
    public int levelWidth = 50, levelHeight = 50;
    public List<PhotonPlayer> players;

    void Start()
    {
        Application.runInBackground = true;
        PhotonNetwork.ConnectUsingSettings("1.0");
        players = new List<PhotonPlayer>();
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
                if (GUILayout.Button("Ready"))
                    if (PhotonNetwork.isMasterClient)
                        players.Add(PhotonNetwork.player);
                    else
                        photonView.RPC("Ready", PhotonNetwork.masterClient, PhotonNetwork.player);
                break;
            default:
                GUI.Label(new Rect(600, 700, 80, 20), "Loading....");
                break;
        }
    }

    void Update()
    {
        if (players.Count == PhotonNetwork.playerList.Length && gamestate == "start")
        {
            gamestate = "generating";
            guistate = "generating";
            string[] data = Generate(levelWidth, levelHeight);
            for (int i = 0; i < levelHeight; i++)
            {
                string[] newdata = new string[levelWidth];
                Array.Copy(data, i * levelWidth, newdata, 0, levelWidth);
                photonView.RPC("Level",PhotonTargets.All, newdata);
            }
            gamestate = "game";
            guistate = "game";
        }
    }

    string[] Generate(int width, int height)
    {
        int count = width * height;
        string[] leveldata = new string[count];
        for (int i = 0; i < count; i++)
        {
            GameObject go = (GameObject)Instantiate(tile, new Vector3(i % width * tileWidth + (i / width) % 2 * rowGap, 0, i / height * tileWidth + tileGap), Quaternion.identity);
            Tile t = go.GetComponent<Tile>();
            t.x = i % width;
            t.y = i / width;
            leveldata[i] = "x:" + t.x + ":y:" + t.y + ":gold:" + t.gold + ":production:" + t.production + ":tourism:" + t.tourism + ":faith:" + t.faith + ":science:" + t.science + ":culture:" + t.culture + ":food:" + t.food;
        }
        return leveldata;
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
        if (PhotonNetwork.isMasterClient)
        {

        }
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
    void Level(string[] level)
    {
        Debug.Log(level.Length);
        for (int i = 0; i < level.Length; i++)
        {
            GameObject go = Resources.Load("tile") as GameObject;
            Tile t = go.GetComponent<Tile>();
            string[] data = level[i].Split(':');
            for (int j = 0; j < data.Length; j++)
            {
                switch (data[j])
                {
                    case "x":
                        t.x = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    case "y":
                        t.y = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    case "gold":
                        t.gold = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    case "production":
                        t.production = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    case "food":
                        t.food = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    case "science":
                        t.science = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    case "faith":
                        t.faith = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    case "tourism":
                        t.tourism = int.Parse(data[j + 1]);
                        j += 2;
                        break;
                    default:
                        break;
                }
            }
            go.transform.position = new Vector3(tileWidth * t.x + tileGap * t.x, 0, tileWidth * t.y + tileGap * t.y + t.y % 2 * rowGap);
        }
    }

    [RPC]
    void Ready(PhotonPlayer player)
    {
        if (players.Contains(player))
            players.Remove(player);
        else
            players.Add(player);
    }
}
