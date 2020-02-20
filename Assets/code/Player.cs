using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public Camera camera;
    public PhysicMaterial noBounce;
    private Collider col;
    private Collider cameraCollider;
    private Vector3 fNormal;

    //ui stuff
    public Text timeUI;

    //variables the player sets
    public float maxAngularVelocity = 30;
    public float speed = 100;
    public float arialSpeed = 0.05f;
    public float drag = 0.5f;
    public float jumpPower = 300;
    public float wallVerticalJump = 0.7f;
    public bool respawnEnabled = true;

    public bool rotateAroundPlayer = true;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;
    [Range(0f, 2.0f)]
    public float mouseSpeed = 10;

    //respawn stuff
    private Vector3 respawnTransform;

    //timer stop watch stuff
    private float timer;
    private bool timing = false;

    //materials
    private PhysicMaterial currentPhysicsMat;

    private int jumpDelay = 5;
    private int jumpTimer;

    private Vector3 offset;
    private Vector3 cameraPosition;
    private Vector3 pastPosition;
    private float distToGround;

    //networking varaibles
    public bool multiplayer = true;
    private bool driveEnabled = true;

    //Network Setup stuff
    public void setup(Camera camera, bool driveEnabled)
    {
        if (multiplayer)
        {
            print("asdasd");
            //this.camera = camera;
            this.driveEnabled = driveEnabled;
        }
    }

    //timer stuff
    public void startTimer()
    {
        timing = true;
        timer = 0;
        print(timer);
    }

    void updateTimer()
    {
        if (timing)
        {
            timer += Time.fixedDeltaTime;
        }
        //print(timer);
        timeUI.text = "Time: " + getTimer();
        timeUI.SetAllDirty();
    }

    public void stopTimer()
    {
        timing = false;
        print(timer);
    }

    //collision cases
    private void OnCollisionEnter(Collision collision)
    {
        fNormal = collision.contacts[0].normal;
    }

    //respawn method if the person dies or somtin
    public void respawn()
    {
        rb.position = respawnTransform;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void setRespawn(Vector3 newRespawn)
    {
        respawnTransform = newRespawn;
    }

    public bool IsGrounded() {
        //Collider col = rb.GetComponent<Collider>();
        return Physics.Raycast(transform.position, -fNormal, col.bounds.size.y/2 + 0.1f);
    }

    private void updateKeys()
    {
        //cursor lock
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //respawn
        if (Input.GetKey(KeyCode.R) && respawnEnabled)
        {
            respawn();
        }

        //stop rolling
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("XboxX"))
        {
            rb.angularDrag = drag * 10;
        }
        else
        {
            rb.angularDrag = drag;
        }

        //stop bouncing
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("XboxLT"))
        {
            col.material = noBounce;
        }
        else
        {
            col.material = currentPhysicsMat;
        }

        //jumping
        if ((Input.GetKey(KeyCode.Space) || Input.GetButton("XboxRT")) && IsGrounded() && jumpTimer == 0)
        {
            Vector3 fN = fNormal;
            //fN.x /= 2;
            //fN.z /= 2;
            print(fN.y);
            fN.y += wallVerticalJump;
            
            rb.AddForce(fN * jumpPower);
            jumpTimer += 1;
        }

        if (jumpTimer != 0)
        {
            jumpTimer += 1;
            if (jumpTimer >= jumpDelay)
            {
                jumpTimer = 0;
            }
        }

        //wall running left
        if (Input.GetKey(KeyCode.Q) || Input.GetButton("XboxLB"))
        {
            rb.AddTorque(Vector3.up * speed);
        }
        //wall running right
        if (Input.GetKey(KeyCode.E) || Input.GetButton("XboxRB"))
        {
            rb.AddTorque(Vector3.up * -speed);
        }
    }

    public void cameraCollided()
    {
        //camera.transform.position = pastPosition;
    }

    private void updateCamera()
    {
        Vector3 offsetX = offset;
        Vector3 offsetY = offset;

        //camera stuff
        if (rotateAroundPlayer && Cursor.lockState == CursorLockMode.Locked)
        {

            Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSpeed, Vector3.up);
            //Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -mouseSpeed, Vector3.right);
            //Quaternion temp = camTurnAngleX * camTurnAngleY;
            //temp.Normalize();

            offset = camTurnAngleX * offset;
            //offsetY = camTurnAngleY * offset;
        }

        //pastPosition = camera.transform.position;
        camera.transform.position = Vector3.Slerp(camera.transform.position, rb.transform.position + offset, smoothFactor);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -(rb.transform.position - camera.transform.position).normalized, out hit, col.bounds.size.y / 2 + Vector3.Distance(rb.transform.position, camera.transform.position)))
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, rb.transform.position, Vector3.Distance(rb.transform.position, camera.transform.position) - hit.distance + 0.1f);
        }

        if (rotateAroundPlayer)
        {
            camera.transform.LookAt(rb.transform);
        }
    }

    private void updateRoll()
    {
        //normal rolling update
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //rolling shit
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = right * y - forward * x;

        //print((Mathf.Abs(rb.angularVelocity.x) + Mathf.Abs(rb.angularVelocity.y) + Mathf.Abs(rb.angularVelocity.z) / 3));
        rb.AddTorque(movement * (speed));

        //if in the air then have arial movement
        if (!IsGrounded())
        {
            movement = right * x + forward * y;
            rb.AddForce(movement * speed * arialSpeed);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        cameraCollider = camera.GetComponent<Collider>();
        offset = camera.transform.position - rb.transform.position;
        jumpTimer = 0;

        currentPhysicsMat = col.material;
        rb.maxAngularVelocity = maxAngularVelocity;
        respawnTransform = rb.position;
    }

    //main method
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!multiplayer || driveEnabled)
        {
            updateTimer();

            updateCamera();
            updateKeys();

            updateRoll();
        }
    }

    //getters
    public float getTimer()
    {
        return (float)Math.Round(timer * 100f) / 100f;
    }
}
