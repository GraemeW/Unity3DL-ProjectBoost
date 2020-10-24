using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    // Tunables
    [SerializeField] Vector3 movement = new Vector3(0f, 0f, 0f);
    /*[Range(0f, 1f)] [SerializeField]*/ float movementFactor = 0f;
    [Range(0f, 1f)] [SerializeField] float initialMovementOffset = 0f;
    [SerializeField] float movementPeriod = 1.0f;

    // State
    Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        movementFactor = GetMovementBySin(0f);
    }

    private void SetMovementFactor()
    {
        if (movementPeriod <= Mathf.Epsilon) { return; }
        movementFactor = GetMovementBySin(Time.time / movementPeriod);
    }

    private float GetMovementBySin(float input)
    {
        float basePosition = 3 * Mathf.PI / 4;
        float positionOffset = Mathf.PI * initialMovementOffset;
        return Mathf.Clamp((Mathf.Sin(basePosition + positionOffset + input) + 1) / 2, 0f, 1f);
    }

    private void FixedUpdate()
    {
        SetMovementFactor();
        transform.position = initialPosition + movement * movementFactor;
    }

}
