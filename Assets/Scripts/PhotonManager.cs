using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //https://doc-api.photonengine.com/ja-jp/pun/current/class_photon_1_1_pun_1_1_photon_network.html
    //https://doc-api.photonengine.com/ja-jp/pun/current/class_photon_1_1_pun_1_1_mono_behaviour_pun_callbacks.html


    public static PhotonManager instance;

    public GameObject loadingPanel;

    public Text loadingText;

    public GameObject buttons;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CloseMenuUI();

        loadingPanel.SetActive(true);
        loadingText.text = "ネットワークに接続中...";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void CloseMenuUI()
    {
        loadingPanel.SetActive(false);

        buttons.SetActive(false);
    }

    public void LobbyMenuDisplay()
    {
        CloseMenuUI();
        buttons.SetActive(true);
    }
}