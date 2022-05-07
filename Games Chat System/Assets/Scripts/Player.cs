using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Joystick joystick;

    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 move;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private PhotonView pv;

    [SerializeField]
    private GameObject ghost;

    [SerializeField]
    private float startTimeToSpawn = 0.15f;

    private float timeToSpawn;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        joystick = FindObjectOfType<Joystick>();

        timeToSpawn = startTimeToSpawn;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            move.x = joystick.Horizontal * speed;
            move.y = joystick.Vertical * speed;

            if (timeToSpawn <= 0)
            {
                Instantiate(ghost, transform.position, Quaternion.identity);
                timeToSpawn = startTimeToSpawn;
            }
            else
            {
                timeToSpawn -= Time.deltaTime;
            }

            Move();
        }

    }

    void Move()
    {
        rb.AddForce(new Vector2(move.x, move.y));
    }
}
