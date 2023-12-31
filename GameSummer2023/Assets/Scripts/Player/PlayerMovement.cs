using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Player attributes")]
    [SerializeField] float movementSpeed = 15f;
    [SerializeField] float rotationSpeed = 720f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Vector3 groundDetectHalfExtend = new Vector3(0.4f, 0.2f, 0.4f);
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float jumpBufferTime = 0.2f;

        
    [Header("Caching Attributes")]
    Rigidbody rigidbodyComponent;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask groundLayer;

    private Quaternion toRotation;
    private Vector3 movementDirection;
    private float coyoteTimeCounter;
    private float jumpBufferTimeCounter;
    private float previousHorizontalInput = 0f;
    float horizontalInput;
    class InputString {
        public static string Horizontal = "Horizontal";
        public static string Jump = "Jump";
    }

    private void Start() {
        rigidbodyComponent = GetComponent<Rigidbody>();
    }
    private void Update() {
        ReadInput();
        RotatePlayer();
        Jump();
    }
    private void FixedUpdate() {
        MovePlayer();
    }

    private void ReadInput() {
        horizontalInput = Input.GetAxisRaw(InputString.Horizontal);
    }
    private void MovePlayer() {
        rigidbodyComponent.velocity = new Vector3(
            horizontalInput * movementSpeed,
            rigidbodyComponent.velocity.y,
            rigidbodyComponent.velocity.z);
    }
    private void RotatePlayer() {
        if (horizontalInput == 0) {
            return;
        }
        if (horizontalInput == previousHorizontalInput) {
            return;
        }
        StopCoroutine(RotatingCoroutine());
        previousHorizontalInput = horizontalInput;
        StartCoroutine(RotatingCoroutine());
    }
    IEnumerator RotatingCoroutine() {
        movementDirection = new Vector3(0, 0, horizontalInput);
        toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        while (transform.rotation != toRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
    /* Jump() function description
     * Coyote time help player jump even if they leave the platform no more than 0.2 seconds
     * Jump buffering make the jumping mechanic more responsive by allowing jump action happen
     * even if player's input is a bit early
     */
    private void Jump() {
        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown(InputString.Jump)) {
            jumpBufferTimeCounter = jumpBufferTime;
        } else {
            jumpBufferTimeCounter -= Time.deltaTime;
        }

        if (jumpBufferTimeCounter > 0f && coyoteTimeCounter > 0f) {
            rigidbodyComponent.velocity = new Vector3(
                rigidbodyComponent.velocity.x,
                jumpForce,
                rigidbodyComponent.velocity.z);
            jumpBufferTimeCounter = 0f;
        }
        if (Input.GetButtonUp(InputString.Jump)) {
            coyoteTimeCounter = 0f;
        }
    }
    private bool IsGrounded() {
        return Physics.CheckBox(groundCheckPoint.position, groundDetectHalfExtend,
            Quaternion.identity, groundLayer);
    }
}

