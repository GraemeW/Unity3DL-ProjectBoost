using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Rocket : MonoBehaviour
{
    // Const
    static int ROTATE_DIRECTION_STATIC = 0;
    static int ROTATE_DIRECTION_LEFT = 1;
    static int ROTATE_DIRECTION_RIGHT = 2;

    // Tunables
    [SerializeField] float thrusterForce = 10.0f;
    [SerializeField] float rotationScaler = 10.0f;
    [SerializeField] AudioClip thrusterSound = null;

    // State
    int queueRotation = 0;
    bool thrustersEngaged = false;

    // Cached references
    Rigidbody myRigidbody = null;
    AudioSource audioSource = null;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = thrusterSound;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        RotateShip();
    }

    private void ProcessInput()
    {
        Thrust();
        SetUpRotate();
    }

    private void SetUpRotate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            queueRotation = ROTATE_DIRECTION_LEFT;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            queueRotation = ROTATE_DIRECTION_RIGHT;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            FireThrusters();
        }
        else
        {
            HaltThrusters();
        }
    }

    private void FireThrusters()
    {
        myRigidbody.AddRelativeForce(thrusterForce * Vector3.up);
        if (!thrustersEngaged)
        {
            thrustersEngaged = true;
            audioSource.Play();
        }
    }

    private void HaltThrusters()
    {
        audioSource.Stop();
        thrustersEngaged = false;
    }

    private void RotateShip()
    {
        myRigidbody.freezeRotation = true; // take manual control of rotation

        float rotationPreFactor = rotationScaler * Time.deltaTime;
        if (queueRotation == ROTATE_DIRECTION_LEFT)
        {
            transform.Rotate(rotationPreFactor * Vector3.forward);
        }
        else if (queueRotation == ROTATE_DIRECTION_RIGHT)
        {
            transform.Rotate(-rotationPreFactor * Vector3.forward);
            
        }
        queueRotation = ROTATE_DIRECTION_STATIC;
        myRigidbody.freezeRotation = false; // back to you boss

    }
}
