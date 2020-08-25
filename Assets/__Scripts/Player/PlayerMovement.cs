using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static float speedMod = 1f; // used to modify movement speed
    public static float jumpMod = 1f; // used to modify jumping height
    public static float lilG = -9.81f; // value of gravity (default 9.81)
    public CharacterController controller; // access character controller component
    public Transform groundCheck; // check to see if player is on the ground
    public float defaultSpeed = 10f; // default movement speed
    public float walkSpeedMod = 1f; // walking speed modifier
    public float sprintSpeedMod = 2f; // sprinting speed modifier
    public float crouchSpeedMod = 0.5f; // crouching speed modifier
    public float airSpeedMod = 0.2f; // airstrafing speed modifier
    public float defaultJump = 5f; // default jump strength
    public float groundCheckRadius = 0.2f; // radius of ground check sphere
    public LayerMask groundMask;

    private Vector3 _fallVel; // vector3 to track falling velocity
    private float _speed; // final speed modifier
    private bool _isGrounded; // whether the player is on the ground or not

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // check if player is on the ground
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask); // check using ground sphere and ground mask

        if (!_isGrounded) // if player is not on the ground
        {
            // adding gravity
            _fallVel.y += 0.5f * lilG * Time.deltaTime; // calculate y displacement
            controller.Move(_fallVel * Time.deltaTime); // apply gravity
        }
        else if (_isGrounded && _fallVel.y < 0) 
        {
            _fallVel.y = -1f; // use small value to make sure player is fully on the ground
        }

        // collect movement and create movement vector // TODO enable/disable airstrafing
        float xMove = Input.GetAxis("Horizontal"); // get horizontal movement
        float zMove = Input.GetAxis("Vertical"); // get vertical movement
        Vector3 playerMove = transform.right * xMove + transform.forward * zMove; // add WASD movement to player

        // get correct speed modifier
        // TODO if statement for walking, sprinting, crouching
        _speed = defaultSpeed * speedMod;

        controller.Move(playerMove * _speed * Time.deltaTime); // apply movement to character

        
    }
}
