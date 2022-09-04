using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject endScreen;
    public Text winText;

    public static GameManager Instance;

    bool alreadyQuit = false;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    public void EndGame(string winnerName, string loserName)
    {
        endScreen.SetActive(true);
        winText.text = winnerName + " DEFEATED " + loserName;

        Health winnerHealth = GameObject.Find(winnerName).GetComponent<Health>();
        winnerHealth.healthSlider.gameObject.SetActive(false);
        winnerHealth.nameText.gameObject.SetActive(false);
        winnerHealth.damageable = false;

        alreadyQuit = true;
        PhotonNetwork.LeaveRoom();
    }

    public void HandleForceQuitGame(string loserName)
    {
        endScreen.SetActive(true);
        winText.text = loserName + " LEFT THE GAME";

        Health winnerHealth = GameObject.Find(PhotonNetwork.NickName).GetComponent<Health>();
        winnerHealth.healthSlider.gameObject.SetActive(false);
        winnerHealth.nameText.gameObject.SetActive(false);
        winnerHealth.damageable = false;

        alreadyQuit = true;
        PhotonNetwork.LeaveRoom();
    }

    public void LeaveCurrentRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(alreadyQuit == true)
        {
            return;
        }
        HandleForceQuitGame(otherPlayer.NickName);
    }
}
