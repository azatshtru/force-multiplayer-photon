using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Health : MonoBehaviour
{
    public Slider healthSlider;
    public Text nameText;

    [HideInInspector]
    public bool damageable = true;

    public GameObject explosionParticles;

    [SerializeField]
    private float health = 100;

    private PhotonView view;


    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;

        nameText.text = view.Owner.NickName;
    }

    void UpdateHealthSlider()
    {
        healthSlider.value = health;
    }

    public void TakeDamage(PhotonView bulletView)
    {
        if (!view.IsMine)
        {
            return;
        }

        if (!damageable)
        {
            return;
        }

        health -= 25;
        UpdateHealthSlider();

        //Send Health
        view.RPC("SetHealth", RpcTarget.All, new object[] { health, bulletView.Owner.NickName });

        bulletView.RPC("DestroySelf", bulletView.Owner);
    }

    [PunRPC]
    public void SetHealth(float healthToSet, string shooterName)
    {
        if (!damageable)
        {
            return;
        }

        health = healthToSet;

        if (health <= 0)
        {
            Die(shooterName);
        }

        UpdateHealthSlider();
    }

    public void Die(string shooterName)
    {
        if (!damageable)
        {
            return;
        }

        //HandleDisconnection
        //Debug.Log(gameObject.name + " died. So sad :(");

        GameManager.Instance.EndGame(shooterName, gameObject.name);

        Instantiate(explosionParticles, transform.position, Quaternion.identity);

        if (view.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
