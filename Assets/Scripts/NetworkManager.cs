using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public InputField messageInputField;
    public Text messageText;
    string nickname;

    SocketIOComponent socket;

    void Start()
    {
        GameObject io = GameObject.Find("SocketIO");
        socket = io.GetComponent<SocketIOComponent>();

        nickname = "Goya";      // TODO: 서버 닉네임 가져오기

        socket.On("chat", UpdateMessage);
    }

    void UpdateMessage(SocketIOEvent e)
    {
        string nick = e.data.GetField("nick").str;
        string msg = e.data.GetField("msg").str;
        
        messageText.text += string.Format("{0}:{1}\n", nick, msg);
    }


    public void Send()
    {
        // 자신의 메시지 화면에 표시
        string message = messageInputField.text;
        messageText.text += string.Format("{0}:{1}\n", nickname, message);

        // 자신이 입력한 메시지 서버에 전송
        JSONObject obj = new JSONObject();
        obj.AddField("nick", nickname);
        obj.AddField("msg", message);

        socket.Emit("message", obj);

        messageInputField.text = "";
    }
}

