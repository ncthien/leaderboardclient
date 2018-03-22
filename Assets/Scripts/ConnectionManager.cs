using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WebSocketSharp;

public class ConnectionManager : MonoBehaviourSingleton<ConnectionManager>
{
    private enum MessageType
    {
        SEND_SCORE = 0,
        NOTIFY_SCORE = 1,
        REQUEST_LEADERBOARD = 2,
        REPLY_LEADERBOARD = 3,
        REQUEST_SCORE = 4,
        REPLY_SCORE = 5,
    }

    public enum State
    {
        OPEN = 0,
        CONNECTING,
        CLOSED,
    }

    private State state;

    private NetBuffer buffer;
    private object bufferLock;

    private WebSocket ws = null;

    private Game game = null;

    public void Init(Game game)
    {
        state = State.CLOSED;

        this.game = game;

        buffer = new NetBuffer();
        bufferLock = new object();
    }

    private void ws_OnOpen(object sender, EventArgs e)
    {
        OnOpen();
    }

    private void ws_OnClose(object sender, CloseEventArgs e)
    {
        OnClose();
    }

    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        OnMessage(e.RawData);
    }

    private void ws_OnError(object sender, ErrorEventArgs e)
    {
        OnError(e.Message);
    }

    private void OnOpen()
    {
        state = State.OPEN;
        Debug.Log("Open Connection");
    }

    private void OnClose()
    {
        state = State.CLOSED;
        Debug.Log("Close Connection");
    }

    private void OnError(string message)
    {
    }

    protected void OnMessage(byte[] data)
    {
        MessageType messageType = (MessageType)data[0];

        switch (messageType)
        {
            case MessageType.NOTIFY_SCORE:
                ProcessNotifyScore(data);
                break;

            case MessageType.REPLY_SCORE:
                ProcessReplyScore(data);
                break;

            case MessageType.REPLY_LEADERBOARD:
                ProcessReplyLeaderboard(data);
                break;

            default:
                break;
        }
    }

    private void ProcessNotifyScore(byte[] data)
    {
        uint score = 0;
        uint oldScore = 0;
        string name = string.Empty;

        lock (bufferLock)
        {
            buffer.Data = data;
            buffer.Position = 1;

            int nameLen = buffer.ReadByte();
            int bufferOffset = buffer.Position;
            name = Encoding.UTF8.GetString(buffer.Data, bufferOffset, nameLen);
            buffer.Skip(nameLen);

            score = buffer.ReadUInt32();
            oldScore = buffer.ReadUInt32();
        }

        if (game != null)
        {
            game.NotifyScore_Thread(name, (int)score, (int)oldScore);
        }
    }

    private void ProcessReplyScore(byte[] data)
    {
        uint score = 0;

        lock (bufferLock)
        {
            buffer.Data = data;
            buffer.Position = 1;

            score = buffer.ReadUInt32();
        }

        if (game != null)
        {
            game.SetUserHighScore_Thread((int)score);
        }
    }

    private void ProcessReplyLeaderboard(byte[] data)
    {
        LeaderboardData leaderboardData = new LeaderboardData();

        lock (bufferLock)
        {
            buffer.Data = data;
            buffer.Position = 1;

            int numItems = buffer.ReadByte();
            for (int i = 0; i < numItems; ++i)
            {
                int nameLen = buffer.ReadByte();
                int bufferOffset = buffer.Position;
                string name = Encoding.UTF8.GetString(buffer.Data, bufferOffset, nameLen);
                buffer.Skip(nameLen);

                int score = (int)buffer.ReadUInt32();

                leaderboardData.AddItem(name, score);
            }
        }

        if (game != null)
        {
            game.SetLeaderboard_Thread(leaderboardData);
        }
    }

    public void Connect()
    {
        if (state != State.CLOSED) return;
        state = State.CONNECTING;

        ws = new WebSocket("ws://" + Settings.Host + ":" + Settings.PortWS);
        //ws.Compression = CompressionMethod.Deflate;

        ws.OnOpen += ws_OnOpen;
        ws.OnClose += ws_OnClose;
        ws.OnMessage += ws_OnMessage;
        ws.OnError += ws_OnError;

        ws.Connect();
    }

    public void Close()
    {
        if (state != State.OPEN) return;
        ws.Close();
        ws = null;
    }

    private void Send(byte[] data)
    {
        ws.Send(data);
    }

    public State GetState()
    {
        return state;
    }

    public bool IsOpen()
    {
        return state == State.OPEN;
    }

    public bool IsClosed()
    {
        return state == State.CLOSED;
    }

    public void RequestLeaderboard(int count)
    {
        if (state != State.OPEN) return;

        MessageType messageType = MessageType.REQUEST_LEADERBOARD;

        byte[] arr = new byte[1 + 6];

        lock (bufferLock)
        {
            buffer.Data = arr;
            buffer.Position = 0;

            buffer.Write((byte)messageType);
            buffer.Write(count);
        }

        Send(arr);
    }

    public void RequestScore(string name)
    {
        if (state != State.OPEN) return;

        MessageType messageType = MessageType.REQUEST_SCORE;

        byte[] nameArr = Encoding.UTF8.GetBytes(name);
        byte[] arr = new byte[1 + 1 + nameArr.Length];

        lock (bufferLock)
        {
            buffer.Data = arr;
            buffer.Position = 0;

            buffer.Write((byte)messageType);
            buffer.Write((byte)nameArr.Length);
            buffer.Write(nameArr);
        }

        Send(arr);
    }

    public void SendScore(string name, int score)
    {
        if (state != State.OPEN) return;

        MessageType messageType = MessageType.SEND_SCORE;

        byte[] nameArr = Encoding.UTF8.GetBytes(name);
        byte[] arr = new byte[1 + 1 + nameArr.Length + 4];

        lock (bufferLock)
        {
            buffer.Data = arr;
            buffer.Position = 0;

            buffer.Write((byte)messageType);
            buffer.Write((byte)nameArr.Length);
            buffer.Write(nameArr);
            buffer.Write(score);
        }

        Send(arr);
    }
}
