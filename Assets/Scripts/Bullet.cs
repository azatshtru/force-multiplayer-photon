using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    private float destroyAfter = 3f;

    private Rigidbody rb;

    PhotonView view;

    private void Awake()
    {

        view = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!view.IsMine)
        {
            GetComponent<Renderer>().material.color = new Color(255f / 255f, 56f / 255f, 62f / 255f, 1f);
        }

        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 20f, ForceMode.VelocityChange);

        Destroy(gameObject, destroyAfter);
    }

    [PunRPC]
    void DestroySelf()
    {
        if (view.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PhotonView>().Owner != view.Owner)
            { 
                other.GetComponent<Health>().TakeDamage(view);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
