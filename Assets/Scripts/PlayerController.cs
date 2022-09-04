using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float forceMagnitude = 6f;
    [SerializeField]
    private float gravitationalAcceleration = 200f;

    public GameObject bulletPrefab;
    public Image energyBar;

    private Rigidbody rb;
    private Plane plane;

    private Vector3 movement;
    private bool what;

    private PhotonView view;
    private Vector3 recievedPos;

    private float energy = 100;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();

        gameObject.name = view.Owner.NickName;

        rb = GetComponent<Rigidbody>();
        plane = new Plane(Vector3.forward, Vector3.zero);

        if (!view.IsMine)
        {
            Destroy(rb);
            energyBar.gameObject.SetActive(false);
        }

        if (view.IsMine)
        {
            CameraController.Instance.AddPlayerView(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, recievedPos, Time.deltaTime * 36f);
            if (Vector3.Distance(transform.position, recievedPos) > 4f)
            {
                transform.position = recievedPos;
            }
            return;
        }

        float enter = 0.0f;

        energy += Time.deltaTime * 98f;
        if (energy > 100f)
        {
            energy = 100f;
        }
        UpdateEnergyBar();

        if (energy < 30)
        {           
            return;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hit = ray.GetPoint(enter);
                movement = (hit - transform.position).normalized * forceMagnitude;
                what = true;
            }
        }
#elif UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hit = ray.GetPoint(enter);
                movement = (hit - transform.position).normalized * forceMagnitude;
                what = true;
            }
        }
#elif UNITY_ANDROID
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hit = ray.GetPoint(enter);
                movement = (hit - transform.position).normalized * forceMagnitude;
                what = true;
            }
        }
#endif

    }

    private void FixedUpdate()
    {
        if (!view.IsMine)
        {
            return;
        }

        if (what && energy >= 30)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(-movement, ForceMode.VelocityChange);

            Shoot(movement.normalized);

            energy -= 30;
            UpdateEnergyBar();

            what = false;
        }

        rb.AddForce(Vector3.down * gravitationalAcceleration, ForceMode.Force);
        
    }

    void UpdateEnergyBar()
    {
        energyBar.fillAmount = energy/100f;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        if (stream.IsReading)
        {
            recievedPos = (Vector3)stream.ReceiveNext();
        }
    }

    private void Shoot(Vector3 lookDir)
    {
        ShootBullet(lookDir);
    }

    private void ShootBullet(Vector3 lookDir)
    {
        if (!view.IsMine)
        {
            return;
        }

        GameObject bulletGO = PhotonNetwork.Instantiate(bulletPrefab.name, transform.position, Quaternion.LookRotation(lookDir, Vector3.forward), 0);
    }
}
