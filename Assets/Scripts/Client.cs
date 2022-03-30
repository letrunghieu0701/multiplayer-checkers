using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;

public class Client : MonoBehaviour
{
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public bool ConnectedToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);

            socketReady = true;
        }
        catch (Exception ex)
        {
            Debug.Log("[Socket error]: " + ex.Message);
        }

        return socketReady;
    }

    // Client read message from the server
    private void OnInComingData(string data)
    {
        Debug.Log("Client receive from server: " + data);
    }
    // Client send
    private void SendToServer(string data)
    {
        if (socketReady == false)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }










    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Constantly checking for messages
        if (socketReady == false)
            return;

        if (stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if (data != null)
                OnInComingData(data);
        }
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }

    private void CloseSocket()
    {
        if (socketReady == false)
            return;

        writer.Close();
        reader.Close();
        stream.Close();
        socket.Close();
        socketReady = false;
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}
