using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class TicTacToeNetwork : MonoBehaviour
{
    // Start Panel
    public RectTransform startPanel;
    public Text startMessageText;
    public Button connectButton;
    public Button closeButton;

    TicTacToeManager gameManager;
    Vector2 startPanelPos;
    PlayerType playerType;

    string myRoomId;

    // Socket.io
    SocketIOComponent socket;

    public void Connect()
    {
        socket.Connect();
        startMessageText.text = "상대를 기다리는 중...";
        connectButton.gameObject.SetActive(false);
        //connectButton.interactable = false;
    }

    public void Close()
    {
        socket.Close();
    }

    private void Start()
    {
        GameObject so = GameObject.Find("SocketIO");
        socket = so.GetComponent<SocketIOComponent>();

        gameManager = GetComponent<TicTacToeManager>();

        socket.On("createRoom", CreateRoom);
        socket.On("joinRoom", JoinRoom);
        socket.On("startGame", StartGame);

        socket.On("doOpponent", DoOpponent);

        socket.On("exitRoom", ExitRoom);

        startPanel.gameObject.SetActive(true);

        closeButton.interactable = false;
    }

    void StartGame(SocketIOEvent e)
    {
        startPanel.gameObject.SetActive(false);
        closeButton.interactable = true;

        // 게임 시작
        gameManager.StartGame(playerType);
    }

    void ExitRoom(SocketIOEvent e)
    {
        socket.Close();
    }

    void JoinRoom(SocketIOEvent e)
    {
        string roomId = e.data.GetField("room").str;

        if (!string.IsNullOrEmpty(roomId))
        {
            myRoomId = roomId;
        }

        playerType = PlayerType.PlayerTwo;
    }

    void CreateRoom(SocketIOEvent e)
    {
        string roomId = e.data.GetField("room").str;

        if (!string.IsNullOrEmpty(roomId))
        {
            myRoomId = roomId;
        }

        playerType = PlayerType.PlayerOne;
    }

    // 플레이어 게임 정보 서버로 전송
    public void DoPlayer(int index)
    {
        JSONObject playInfo = new JSONObject();
        playInfo.AddField("position", index);
        playInfo.AddField("room", myRoomId);

        socket.Emit("doPlayer", playInfo);
    }

    void DoOpponent(SocketIOEvent e)
    {
        int cellIndex = -1;
        e.data.GetField(ref cellIndex, "position");
        gameManager.DrawMark(cellIndex, Player.Opponent);
    }
}
