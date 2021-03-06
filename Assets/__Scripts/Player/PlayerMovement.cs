﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static float lilG = -9.81f; // value of gravity (default 9.81)

    public float speedMod = 1f; // used to modify movement speed
    public float jumpMod = 1f; // used to modify jumping height
    public float staminaRegenMod = 1f; // used to modify rate at which stamina accumulates
    public float staminaUsageMod = 1f; // used to modify usage of stamina
    public float stamina = 100f; // stamina value

    public CharacterController controller; // access character controller component
    public Transform groundCheck; // check to see if player is on the ground
    public float defaultSpeed = 10f; // default movement speed
    public float walkSpeedMod = 1f; // walking speed modifier
    public float sprintSpeedMod = 2f; // sprinting speed modifier
    public float crouchSpeedMod = 0.5f; // crouching speed modifier
    public float airSpeedMod = 0.25f; // airstrafing speed modifier
    public float defaultJump = 3f; // default jump strength
    public float sprintStamina = 7f; // stamina lost by sprinting
    public float crouchStamina = 9f; // stamina lost by crouching
    public float jumpStamina = 20f; // stamina lost by jumping
    public float groundCheckRadius = 0.2f; // radius of ground check sphere
    public LayerMask groundMask;

    public Text staminaText; // text to display current stamina level
    public Text speedText; // text to display current speed

    public AudioClip walkAudio; // sound for jumping
    public AudioClip sprintAudio; // sound for sprinting
    public AudioClip crouchWalkAudio; // sound for crouch walking
    public AudioClip crouchAudio; // sound for crouching
    public AudioClip jumpAudio; // sound for jumping
    public AudioClip dropAudio; // sound for dropping
    public AudioClip lowStaminaAudio; // sound for low stamina

    [HideInInspector]
    public SettingsMenu settings; // access player's settings
    public HealthSystem hpSys; // access player's health system

    private Vector3 _fallVel; // vector3 to track falling velocity
    private float _speed; // final speed modifier
    private bool _isGrounded; // whether the player is on the ground or not
    private bool _isSprinting; // whether the player is sprinting or not
    private bool _isCrouching; // whether the player is crouched or not
    private int _leanState; // whether the player is straight (0), leaning left (1), or leaning right (2)
    private bool _inAir; // used to track player status for things like dropping audio and fall damage
    private float[] _airVals = new float[2]; // array to keep track of start and end position in a jump
    private AudioSource _source; // source for player audio

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>(); // gets audio source
        _source.playOnAwake = false; // does not play on startup
        _source.spatialBlend = 1f; // makes the sound 3D

        settings = gameObject.GetComponent<SettingsMenu>(); // get settings access
        hpSys = gameObject.GetComponentInChildren<HealthSystem>(); // get hp alteration access

        // set default stat text values
        staminaText.text = "Stm: " + stamina.ToString("#.#") + "%";
        speedText.text = "Spd: " + _speed.ToString("#.#") + "m/s";
    }

    // Update is called once per frame
    void Update()
    {
        if ((settings != null) && (hpSys != null))
        {
            GroundCheck(); // ensure player is on the ground  

            SprintCheck(); // check if player is sprinting

            CrouchCheck(); // check if player is crouching

            LeanCheck(); // check if player is leaning   

            Move(); // move player forward/backward/left/right

            Jump(); // move player up

            Lean(); // lean player body left or right

            ApplyGravity(); // move player down
            
            StaminaCheck(); // regain or reset player's stamina

            // update stat texts
            staminaText.text = "Stm: " + stamina.ToString("#.#") + "%";
            speedText.text = "Spd: " +  _speed.ToString("#.#") + "m/s";
        }
        else
        {
            Debug.Log("Houston, we have a really big fucking problem coming right up");
        }
    }

    // function to keep player on the ground
    void GroundCheck()
    {
        // check if player is on the ground
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask); // check using ground sphere and ground mask
        if (_isGrounded && _fallVel.y < 0) 
        {
            _fallVel.y = -1f; // use small value to make sure player is fully on the ground
            if (_inAir)
            {
                _inAir = false; // set in air status to false again
                _airVals[1] = controller.transform.position.y; // get end position
                
                _source.clip = dropAudio; // sets drop audio
                _source.volume = 1f; // sets drop volume
                _source.Play(); // plays drop audio

                FallDamage(_airVals[0], _airVals[1]); // apply fall damage
            }
        }
        else if (!_isGrounded)
        {
            if (!_inAir)
            {
                _inAir = true; // set in air status to true
                _airVals[0] = controller.transform.position.y; // get start position
            }
        }
    }

    // function to apply the effect of gravity
    void ApplyGravity()
    {
        // adding gravity
        _fallVel.y += 2f * lilG * Time.deltaTime; // calculate y displacement
        controller.Move(_fallVel * Time.deltaTime); // apply gravity
    }

    // function to check if player is sprinting
    void SprintCheck()
    {
        if (settings.toggleSprint && Input.GetButtonDown("Sprint") && !_isSprinting)
        {
            _isSprinting = true; // start sprinting
            _isCrouching = false; // stop crouching
        }
        else if (settings.toggleSprint && Input.GetButtonDown("Sprint") && _isSprinting)
        {
            _isSprinting = false; // stop sprinting
        }
        else if (!settings.toggleSprint && Input.GetButton("Sprint"))
        {
            _isSprinting = true; // sprint
            _isCrouching = false; // don't crouch
        }
        else if (!settings.toggleSprint && !Input.GetButton("Sprint"))
        {
            _isSprinting = false; // don't sprint
        }
    }

    // function to check if player is crouching
    void CrouchCheck()
    {
        if (settings.toggleCrouch && Input.GetButtonDown("Crouch") && !_isCrouching && (stamina >= (crouchStamina * staminaUsageMod)))
        {
            _isCrouching = true; // start crouching
            _isSprinting = false; // stop sprinting
            stamina -= crouchStamina * staminaUsageMod; // reduce stamina
            _source.clip = crouchAudio; // sets crouch audio
            _source.volume = 1f; // sets crouch volume
            _source.Play(); // plays crouch audio
        }
        else if (settings.toggleCrouch && Input.GetButtonDown("Crouch") && _isCrouching && (stamina >= (crouchStamina * staminaUsageMod)))
        {
            _isCrouching = false; // stop crouching
            stamina -= crouchStamina * staminaUsageMod; // reduce stamina
            _source.clip = crouchAudio; // sets crouch audio
            _source.volume = 1f; // sets crouch volume
            _source.Play(); // plays crouch audio
        }
        else if (!settings.toggleCrouch && Input.GetButton("Crouch") && (stamina >= (crouchStamina * staminaUsageMod)))
        {
            _isCrouching = true; // crouch
            _isSprinting = false; // don't sprint
        }
        else if (!settings.toggleCrouch && !Input.GetButton("Crouch"))
        {
            _isCrouching = false; // don't crouch
        }
    }

    // function to check if player is leaning
    void LeanCheck()
    {
        if (settings.toggleLean && Input.GetButtonDown("Lean Left") && (_leanState == 0 || _leanState == 2))
        {
            _leanState = 1; // lean left
        }
        else if (settings.toggleLean && Input.GetButtonDown("Lean Left") && _leanState == 1)
        {
            _leanState = 0; // unlean left
        }
        else if (!settings.toggleLean && Input.GetButton("Lean Left"))
        {
            _leanState = 1; // lean left
        }
        else if (settings.toggleLean && Input.GetButtonDown("Lean Right") && (_leanState == 0 || _leanState == 1))
        {
            _leanState = 2; // lean right
        }
        else if (settings.toggleLean && Input.GetButtonDown("Lean Right") && _leanState == 2)
        {
            _leanState = 0; // unlean right
        }
        else if (!settings.toggleLean && Input.GetButton("Lean Right"))
        {
            _leanState = 2; // lean right
        }
    }

    // function to apply movement to the player
    void Move()
    {
        // collect movement and create movement vector // TODO enable/disable airstrafing
        float xMove = Input.GetAxis("Horizontal"); // get horizontal movement
        float zMove = Input.GetAxis("Vertical"); // get vertical movement
        Vector3 playerMove = transform.right * xMove + transform.forward * zMove; // add WASD movement to player

        // get correct speed modifier
        if (_isSprinting) // sprinting
        {
            _speed = defaultSpeed * speedMod * sprintSpeedMod;
            stamina -= sprintStamina * staminaUsageMod * Time.deltaTime; // use stamina

            if ((xMove != 0 || zMove != 0) && _isGrounded && !_source.isPlaying)
            {
                _source.clip = sprintAudio; // sets sprint audio
                _source.volume = 1f; // sets sprint volume
                _source.Play(); // plays sprint audio
            } 
        }
        else if (_isCrouching) // crouching
        {
            _speed = defaultSpeed * speedMod * crouchSpeedMod;
        
            if ((xMove != 0 || zMove != 0) && _isGrounded && !_source.isPlaying)
            {
                _source.clip = crouchWalkAudio; // sets crouch walk audio
                _source.volume = 0.5f; // sets crouch walk volume
                _source.Play(); // plays crouch walk audio
            }
        }
        else if (!_isGrounded) // falling
        {
            _speed = defaultSpeed * speedMod * airSpeedMod;
        }
        else // walking
        {
            _speed = defaultSpeed * speedMod * walkSpeedMod;

            if ((xMove != 0 || zMove != 0) && _isGrounded && !_source.isPlaying)
            {
                _source.clip = walkAudio; // sets walk audio
                _source.volume = 0.7f; // sets walk volume
                _source.Play(); // plays walk audio
            }
        }
       
        controller.Move(playerMove * _speed * Time.deltaTime); // apply movement to character
    }

    // function to make player jump
    void Jump() 
    {
        if (_isGrounded && (Input.GetButtonDown("Jump")) && (stamina >= (jumpStamina * staminaUsageMod)))
        {
            _fallVel.y = Mathf.Sqrt(defaultJump * -2f * lilG); // adds velocity to jump
            stamina -= jumpStamina * staminaUsageMod;
            _source.clip = jumpAudio; // sets jump audio
            _source.volume = 1f; // sets jump volume
            _source.Play(); // plays jump audio
        }
    }

    // function to make player lean left or right
    void Lean()
    {
        // TODO
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
        else if (stamina <= 40)
        {
            if (_isGrounded && !_source.isPlaying)
            {
                _source.clip = lowStaminaAudio; // sets low stamina audio
                _source.volume = 1f; // sets low stamina volume
                _source.Play(); // plays low stamina audio
            }
        }
        else if (stamina <= 0)
        {
            stamina = 0; // reset stamina
            _isSprinting = false; // stop sprinting
            _isCrouching = false; // stop crouching
        }
    }

    // function to apply fall damage
    void FallDamage(float start, float end)
    {
        float distance = (start - end) * 10; // calculate fall distance
        float fallDamage = (distance * distance) - 10; // calculate applicable fall damage
        
        if (fallDamage > 0) // apply fall damage if necessary
        {
            hpSys.Damage(fallDamage); // decrease hp
        }
    }
}
