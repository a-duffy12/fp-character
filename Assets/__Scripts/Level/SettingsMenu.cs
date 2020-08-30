using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static float mouseSensX; // universal horizontal sensitivity multiplier
    public static float mouseSensY; // universal vertical sensitivity multiplier
    public static float verticalFOV; // first person camera field of view
    public static bool toggleSprint; // whether sprint is toggle or hold
    public static bool toggleCrouch; // whether crouch is toggle or hold
    public static bool toggleLean; // whether lean is toggle or hold

    public Camera cam; // first person camera
    public GameObject settingsMenuUI; // ui panel for settings menu
    public Slider xSlider; // slider for x sens
    public Slider ySlider; // slider for y sens
    public Slider fovSlider; // slider for vertical fov
    public Button sprintButton; // button to choose sprint mode
    public Button crouchButton; // button to choose crouch mode
    public Button leanButton; // button to choose lean mode
    public Text xText; // text for x sens 
    public Text yText; // text for y sens
    public Text fovText; // text for vertical fov
    public Text sprintText; // text for sprint mode
    public Text crouchText; // text for crouch mode
    public Text leanText; // text for lean mode

    private bool _openSettings; // if settings is open

    // Start is called before the first frame update
    void Start()
    {
        // default values
        mouseSensX = 50f;
        mouseSensY = 50f;
        verticalFOV = 60f;
        toggleSprint = false;
        toggleCrouch = false;
        toggleLean = false;
        xSlider.minValue = 1;
        ySlider.minValue = 1;
        fovSlider.minValue = 60;
        xSlider.maxValue = 100;
        ySlider.maxValue = 100;
        fovSlider.maxValue = 90;
        xSlider.value = mouseSensX;
        ySlider.value = mouseSensY;
        fovSlider.value = verticalFOV;

        // TODO load saved preferences

        // set slider and button text based on saved data or default fields
        xText.text = xSlider.value.ToString("#");
        yText.text = ySlider.value.ToString("#");
        fovText.text = fovSlider.value.ToString("#");

        // change field of view
        cam.fieldOfView = fovSlider.value;

        // set sprint, crouch, and lean texts
        if (toggleSprint)
        {
            sprintText.text = "Toggle";
        }
        else if (!toggleSprint)
        {
            sprintText.text = "Hold";
        }

        if (toggleCrouch)
        {
            crouchText.text = "Toggle";
        }
        else if (!toggleCrouch)
        {
            crouchText.text = "Hold";
        }

        if (toggleLean)
        {
            leanText.text = "Toggle";
        }
        else if (!toggleLean)
        {
            leanText.text = "Hold";
        }

        _openSettings = false;
        settingsMenuUI.SetActive(false); // settings menu is closed by default

        // liseteners for all the buttons
        sprintButton.onClick.AddListener(SprintMode);
        crouchButton.onClick.AddListener(CrouchMode);
        leanButton.onClick.AddListener(LeanMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") && !_openSettings)
        {
            Time.timeScale = 0; // freeze scene
            settingsMenuUI.SetActive(true); // open settings
            _openSettings = true; // set to open
            Cursor.lockState = CursorLockMode.Confined; // unlock cursor
        }
        else if (Input.GetButtonDown("Pause") && _openSettings)
        {
            settingsMenuUI.SetActive(false); // close settings
            _openSettings = false; // set to closed
            Cursor.lockState = CursorLockMode.Locked; // lock cursor
            Time.timeScale = 1; // unfreeze scene
        }

        xText.text = xSlider.value.ToString("#"); // display updated x sens
        yText.text = ySlider.value.ToString("#"); // display update y sens
        fovText.text = fovSlider.value.ToString("#"); // display updated vertical fov
        mouseSensX = xSlider.value; // set updated x sens value
        mouseSensY = ySlider.value; // set updated y sens value
        verticalFOV = fovSlider.value; // set updated field of view
        cam.fieldOfView = fovSlider.value; // apply updated field of view
    }

    // function to change sprint mode
    void SprintMode()
    {
        if (toggleSprint)
        {
            toggleSprint = false;
            sprintText.text = "Hold";
        }
        else if (!toggleSprint)
        {
            toggleSprint = true;
            sprintText.text = "Toggle";
        }
    }

    // function to change crouch mode
    void CrouchMode()
    {
        if (toggleCrouch)
        {
            toggleCrouch = false;
            crouchText.text = "Hold";
        }
        else if (!toggleCrouch)
        {
            toggleCrouch = true;
            crouchText.text = "Toggle";
        }
    }

    // function to change lean mode
    void LeanMode()
    {
        if (toggleLean)
        {
            toggleLean = false;
            leanText.text = "Hold";
        }
        else if (!toggleLean)
        {
            toggleLean = true;
            leanText.text = "Toggle";
        }
    }
}