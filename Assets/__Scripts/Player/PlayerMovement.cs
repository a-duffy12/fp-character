using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static float speedMod = 1f; // used to modify movement speed
    public static float jumpMod = 1f; // used to modify jumping height
    public static float staminaRegenMod = 1f; // used to modify rate at which stamina accumulates
    public static float staminaUsageMod = 1f; // used to modify usage of stamina
    public static float lilG = -9.81f; // value of gravity (default 9.81)
    public static float stamina = 100f; // stamina value

    public CharacterController controller; // access character controller component
    public Transform groundCheck; // check to see if player is on the ground
    public float defaultSpeed = 10f; // default movement speed
    public float walkSpeedMod = 1f; // walking speed modifier
    public float sprintSpeedMod = 2f; // sprinting speed modifier
    public float crouchSpeedMod = 0.5f; // crouching speed modifier
    public float airSpeedMod = 0.2f; // airstrafing speed modifier
    public float defaultJump = 3f; // default jump strength
    public float sprintStamina = 10f; // stamina lost by sprinting
    public float crouchStamina = 25f; // stamina lost by crouching
    public float jumpStamina = 50f; // stamina lost by jumping
    public float groundCheckRadius = 0.2f; // radius of ground check sphere
    public LayerMask groundMask;

    private Vector3 _fallVel; // vector3 to track falling velocity
    private float _speed; // final speed modifier
    private bool _isGrounded; // whether the player is on the ground or not
    private bool _isSprinting; // whether the player is sprinting or not
    private bool _isCrouching; // whether the player is crouched or not
    private AudioSource _source; // source for player audio

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>(); // gets audio source
        _source.playOnAwake = false; // does not play on startup
        _source.spatialBlend = 1f; // makes the sound 3D]
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck(); // ensure player is on the ground     

        Move(); // move player forward/backward/left/right

        Jump(); // move player up

        ApplyGravity(); // move player down
        
        StaminaCheck(); // regain or reset player's stamina
    }

    // function to keep player on the ground
    void GroundCheck()
    {
        // check if player is on the ground
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask); // check using ground sphere and ground mask
        if (_isGrounded && _fallVel.y < 0) 
        {
            _fallVel.y = -1f; // use small value to make sure player is fully on the ground
        }
    }

    // function to apply the effect of gravity
    void ApplyGravity()
    {
        // adding gravity
        _fallVel.y += 2f * lilG * Time.deltaTime; // calculate y displacement
        controller.Move(_fallVel * Time.deltaTime); // apply gravity
    }

    // function to apply movement to the player
    void Move()
    {
        // collect movement and create movement vector // TODO enable/disable airstrafing
        float xMove = Input.GetAxis("Horizontal"); // get horizontal movement
        float zMove = Input.GetAxis("Vertical"); // get vertical movement
        Vector3 playerMove = transform.right * xMove + transform.forward * zMove; // add WASD movement to player

        // get correct speed modifier
        // TODO if statement for walking, sprinting, crouching
        _speed = defaultSpeed * speedMod;

        controller.Move(playerMove * _speed * Time.deltaTime); // apply movement to character
    }

    // function to make player jump
    void Jump() 
    {
        if (_isGrounded && (Input.GetButtonDown("Jump")) && (stamina >= (jumpStamina * staminaUsageMod)))
        {
            _fallVel.y = Mathf.Sqrt(defaultJump * -2f * lilG); // adds velocity to jump
            stamina -= jumpStamina * staminaUsageMod;
        }
    }

    // function to regain and reset stamina
    void StaminaCheck()
    {
        stamina += 10 * staminaRegenMod * Time.deltaTime; // increase stamina by +10/s
        
        // max stamina is 100
        if (stamina > 100)
        {
            stamina = 100; // reset stamina
        }
        else if (stamina < 0)
        {
            stamina = 0; // reset stamina
        }
    }
}
