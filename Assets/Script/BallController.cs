using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private enum HitDirection { None, Forward, Back, Left, Right }
    private string collisionSide;
    private Rigidbody rb;
    private int coloredWallCounter = 0;
    private TouchController touchController;
    private int moveCounter = 0;

    public int MoveCounter
    {
        get => moveCounter;
        set => moveCounter = value;
    }

    public int ColoredWallCounter
    {
        get => coloredWallCounter;
        set => coloredWallCounter = value;
    }

    private void Start()
    {
        touchController = GetComponent<TouchController>();
        rb = GetComponent<Rigidbody>();
        ballSetStop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            if (collision.gameObject.GetComponent<Renderer>().material.color != Color.blue)
            {
                coloredWallCounter++;
                collision.gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }
        }
    }

    private void ballSetStop()
    {
        touchController.BallMove = false;
        rb.velocity = new Vector3(0, 0, 0);
        this.transform.Translate(new Vector3(0, 0, 0));
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    private void ballMove(Vector3 direction, float time)
    {
        Ray ballRay = new Ray(transform.position, direction);
        RaycastHit ballHit;
        Debug.DrawRay(transform.position, direction, Color.black);
        if (Physics.Raycast(ballRay, out ballHit, 1))
        {
            if (ballHit.collider != null)
            {
                moveCounter++;
                ballSetStop();
                collisionSide = returnDirection(ballHit);
            }
        }
        else
        {
            touchController.BallMove = true;
            transform.Translate(direction * time * 50);
            transform.localScale = new Vector3(1, 1, 0.7f);
        }
    }

    private string returnDirection(RaycastHit hit)
    { 
    HitDirection hitDirection = HitDirection.None;

    Vector3 MyNormal = hit.normal;
    MyNormal = hit.transform.TransformDirection(MyNormal);

    if (MyNormal == hit.transform.forward) { hitDirection = HitDirection.Forward; }
    if (MyNormal == -hit.transform.forward) { hitDirection = HitDirection.Back; }
    if (MyNormal == hit.transform.right) { hitDirection = HitDirection.Right; }
    if (MyNormal == -hit.transform.right) { hitDirection = HitDirection.Left; }

    return hitDirection.ToString();
    }

    private void FixedUpdate()
    {
        if (touchController.Direction == Vector2.up && collisionSide != "Back")
        {
            ballMove(Vector3.forward, Time.deltaTime);
        }
        else if (touchController.Direction == Vector2.down && collisionSide != "Forward")
        {
            ballMove(-Vector3.forward, Time.deltaTime);
        }
        else if (touchController.Direction == Vector2.right && collisionSide != "Left")
        {
             ballMove(Vector3.right, Time.deltaTime);
        }
        else if (touchController.Direction == Vector2.left && collisionSide != "Right")
        {
            ballMove(-Vector3.right, Time.deltaTime);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
