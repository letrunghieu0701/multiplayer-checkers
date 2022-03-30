using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;

public class Server : MonoBehaviour
{
    public int port = 6321;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    private TcpListener server;
    private bool serverStarted;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log("[Socket error] Failed to host server: " + ex.Message);
        }
    }
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);
        Debug.Log("Someone has connected");

        StartListening();
    }
    private void Update()
    {
        if (serverStarted == false)
            return;

        foreach(ServerClient client in clients)
        {
            // Is the client still connected?
            if (Isconnected(client.tcpClient) == false)
            {
                client.tcpClient.Close();
                disconnectList.Add(client);
                continue;
            }
            else
            {
                NetworkStream stream = client.tcpClient.GetStream();
                if (stream.DataAvailable)
                {
                    StreamReader reader = new StreamReader(stream, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnInComingData(client, data);
                }
            }
        }

        for (int i = 0; i < disconnectList.Count; i++)
        {
            clients.Remove(disconnectList[i]);
            disconnectList.Remove(disconnectList[i]);

            // Tell our players somebody has disconnected


        }
    }

    // Server send
    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcpClient.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception ex)
            {
                Debug.Log("[Socket error] Can't broadcast to client: " + sc.clientName +
                    "\nError: " + ex.Message);
            }
        }
    }
    // Server read
    private void OnInComingData(ServerClient client, string data)
    {
        Debug.Log(client.clientName + " has send: " + data);
    }
    private bool Isconnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch(Exception ex)
        {
            Debug.Log("A client has disconnected: " + c.ToString() + "\n[Socket error]: " + ex.Message);
            return false;
        }
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcpClient;

    public ServerClient(TcpClient tcp)
    {
        this.tcpClient = tcp;
    }
}