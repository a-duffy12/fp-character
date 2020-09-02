using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    public float hp; // amount of health player has

    public Text hpText; // text to display remaining health
    public AudioClip hurtAudio; // sound for taking damage
    public AudioClip deathAudio; // sound for dying

    [HideInInspector]
    public Vector3 spawnPos; // where the player spawned

    private AudioSource _source; // source for player audio

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>(); // gets audio source
        _source.playOnAwake = false; // does not play on startup
        _source.spatialBlend = 1f; // makes the sound 3D

        spawnPos = gameObject.transform.position; // get spawn position

        hp = 100f; // set default hp
        hpText.text = "HP: " + hp.ToString("#"); // display default heaplth
    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = "HP: " + hp.ToString("#"); // update health ui

        DeathCheck(); // check is player is dead
    }

    // funtion to damage player
    public void Damage(float dmg)
    {
        if (dmg > 0)
        {
            hp -= dmg; // reduce hp by the damage done
            _source.clip = hurtAudio; // set hurt audio
            _source.Play(); // plays hurt audio
        }
    }

    // function to check if player is dead
    void DeathCheck()
    {
        if (hp <= 0) // check for no hp
        {
            hp = 0;
            _source.clip = deathAudio; // set death audio
            _source.Play(); // plays death audio
            StartCoroutine(Respawn()); // respawns specific player
        }
    }

    // function to respawn WIP
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f); // wait for 3s
        gameObject.transform.position = spawnPos;
        hp = 100; // respawn player with full hp
    }
}
