using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public Text findingPlayerText;

    public GameObject playerPrefab;

    public Transform[] spawnPoints;

    int spawnNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnNumber = PhotonNetwork.PlayerList.Length - 1;
        StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);

        for(int i = 0; i < 3; i++)
        {
            findingPlayerText.text = "Battle starting in " + (3-i).ToString();
            yield return new WaitForSeconds(1);
        }

        Spawn();
    }

    void Spawn()
    {
        Transform spawnPoint = spawnPoints[spawnNumber];

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);

        findingPlayerText.gameObject.SetActive(false);
    }

}
