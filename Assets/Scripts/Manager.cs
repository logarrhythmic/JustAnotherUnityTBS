using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : Photon.MonoBehaviour
{
    public Tile[] tiles;
    public GameObject[] tileObjects;
    public GameObject tile;
    public int tileGap, rowGap, tileWidth;
    public bool ready = false;
    public float resoWidth = 1280, resoHeight = 720;
    public string gamestate = "connecting", guistate = "connecting";
    public int levelWidth = 50, levelHeight = 50;
    public List<PhotonPlayer> players;

    void Start()
    {
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
                        photonView.RPC("Ready", PhotonNetwork.masterClient);
                break;
            default:
                GUI.Label(new Rect(600, 700, 80, 20), "Loading....");
                break;
        }
    }

    void Update()
    {
        if (players.Count == PhotonNetwork.playerList.Length)
        {
            gamestate = "generating";
            guistate = "generating";
            Generate(levelWidth, levelHeight);
        }
    }

    void Generate(int width, int height)
    {
        int count = width * height;
        tiles = new Tile[count];
        tileObjects = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            GameObject go = (GameObject)PhotonNetwork.Instantiate("tile", new Vector3(i % width * tileWidth + (i / width) % 2 * rowGap, 0, i / height * tileWidth + tileGap), Quaternion.identity, 0);
            tiles[i] = go.GetComponent<Tile>();
            tileObjects[i] = go;
        }
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
    void Ready(PhotonPlayer sender)
    {
        if (players.Contains(sender))
            players.Remove(sender);
        else
            players.Add(sender);
    }
}
