using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;

public class Rocket : MonoBehaviour
{
    // Const
    enum RotationDirection { Static, Left, Right };
    enum State { Alive, Dying, Transcending }

    // Tunables
    [Header("Controls")]
    [SerializeField] float thrusterForce = 10.0f;
    [SerializeField] float rotationScaler = 10.0f;
    [SerializeField] float delayAfterLevelComplete = 2.0f;
    [Header("SFX")]
    [SerializeField] AudioClip thrusterSound = null;
    [SerializeField] AudioClip explosionSound = null;
    [SerializeField] AudioClip successSound = null;
    [Header("VFX")]
    [SerializeField] ParticleSystem thrusterVFX = null;
    [SerializeField] ParticleSystem explosionVFX = null;
    [SerializeField] ParticleSystem successVFX = null;

    // State
    RotationDirection queueRotation = 0;
    bool thrustersEngaged = false;
    State state = State.Alive;

    // Cached references
    Rigidbody myRigidbody = null;
    AudioSource audioSource = null;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = thrusterSound;
        audioSource.loop = true;
        thrusterVFX = transform.Find("Rocket Jet Particles").gameObject.GetComponent<ParticleSystem>();
        explosionVFX = transform.Find("Explosion Particles").gameObject.GetComponent<ParticleSystem>();
        successVFX = transform.Find("Success Particles").gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            ProcessInput();
        }
    }

    private void FixedUpdate()
    {
        RotateShip();
    }

    private void OnCollisionEnter(Collision otherCollider)
    {
        if (state != State.Alive) { return; }

        switch (otherCollider.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finished":
                state = State.Transcending;
                successVFX.Play();
                QueueSFX(successSound);
                Invoke("ProcessFinished", delayAfterLevelComplete);
                break;
            default:
                state = State.Dying;
                explosionVFX.Play();
                QueueSFX(explosionSound);
                Invoke("DestroyShip", delayAfterLevelComplete);
                break;
        }
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
            queueRotation = RotationDirection.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            queueRotation = RotationDirection.Right;
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
        myRigidbody.AddRelativeForce(thrusterForce * Vector3.up * Time.deltaTime);
        thrusterVFX.Play();
        if (!thrustersEngaged)
        {
            thrustersEngaged = true;
            audioSource.Play();
        }
    }

    private void HaltThrusters()
    {
        thrusterVFX.Stop();
        audioSource.Stop();
        thrustersEngaged = false;
    }

    private void RotateShip()
    {
        myRigidbody.freezeRotation = true; // take manual control of rotation

        float rotationPreFactor = rotationScaler * Time.deltaTime;
        if (queueRotation == RotationDirection.Left)
        {
            transform.Rotate(rotationPreFactor * Vector3.forward);
        }
        else if (queueRotation == RotationDirection.Right)
        {
            transform.Rotate(-rotationPreFactor * Vector3.forward);
            
        }
        queueRotation = RotationDirection.Static;
        myRigidbody.freezeRotation = false; // back to you boss

    }

    private void QueueSFX(AudioClip inputAudio)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(inputAudio);
    }

    private void DestroyShip()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ProcessFinished()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
