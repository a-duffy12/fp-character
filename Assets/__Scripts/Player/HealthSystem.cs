using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public static float hp; // amount of health player has

    public Text hpText; // text to display remaining health

    private AudioSource _source; // source for player audio

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>(); // gets audio source
        _source.playOnAwake = false; // does not play on startup
        _source.spatialBlend = 1f; // makes the sound 3D

        hp = 100f; // set default hp
        hpText.text = "HP: " + hp; // display default heapth
    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = "HP: " + hp; // update health ui
    }
}
