using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }



    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        Debug.Log("Connect button pressed");
    }

    public void HostButton()
    {
        Debug.Log("Host button pressed");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
