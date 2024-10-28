using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    void Update()
    {
        Movement(Inputs());
    }

    private void Movement (Vector3 moveDir)
    {
        transform.position += speed * Time.deltaTime * moveDir;
    }

    private Vector3 Inputs()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new(xInput, 0f, zInput);

        moveDirection.Normalize();

        return moveDirection;
    }
}
