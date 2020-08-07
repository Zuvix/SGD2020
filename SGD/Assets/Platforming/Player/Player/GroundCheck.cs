using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public GameObject player;
    PlayerBehaviour pb;
    private void Awake()
    {
        pb = player.GetComponent<PlayerBehaviour>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        if (collision.gameObject.CompareTag("Ground"))
        {
            pb.isOnGround = true;
            pb.doubleJump = true;
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            pb.isOnGround = false;
        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            pb.isOnGround = true;
        }

    }
}
