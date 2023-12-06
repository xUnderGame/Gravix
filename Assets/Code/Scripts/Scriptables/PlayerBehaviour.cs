using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(menuName = "Player Behaviour")]
public class PlayerBehaviour : ScriptableObject
{
    public float maxHSpeed;
    public float maxVSpeed;
    public bool gravity;
    public int deaths;
    public int swaps;
    public bool restrictedGrav;
    public bool canMove;
    public bool canSwap;
    public bool invulnerable;
    public bool canForceRespawn;
    public bool waitForRespawn;
    public AudioClip deathSound;
    public AudioClip swapSound;

    [DoNotSerialize] public SpriteRenderer renderer;
    private GameObject playerObject;
    private Rigidbody2D rb;
    private AudioSource audio;
    private Animator animator;
    private string currentAnimatorState;

    private void OnEnable()
    {
        canMove = true;
        canSwap = true;
        invulnerable = false;
        canForceRespawn = true;
        restrictedGrav = false;
        waitForRespawn = false;
        gravity = false;
        deaths = 0;
        swaps = 0;
    }

    // Force a specific gravity
    public void ForceGravity(bool newGravity)
    {
        // Flip sprite
        SpriteRenderer sr = playerObject.GetComponentInParent<SpriteRenderer>();
        if (sr) sr.flipY = newGravity;

        // Force gravity
        GameManager.Instance.player.gravity = newGravity;
        if (newGravity) { rb.gravityScale = -1; } else { rb.gravityScale = 1;}
        ApplyGravityVelocity();
    }

    // Swap player gravity
    public void ChangeGravity()
    {
        // Play swap sound
        PlaySound(swapSound, 0.5f);

        // Flip sprite
        SpriteRenderer sr = playerObject.GetComponentInParent<SpriteRenderer>();
        sr.flipY = !sr.flipY;

        // Change gravity
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.gravityScale *= -1;
        gravity = !gravity;
        ApplyGravityVelocity();
    }

    // Applies the up/down velocity to the player rb
    private void ApplyGravityVelocity()
    {
        // Apply velocity based on gravity
        if (!gravity) rb.velocity = new Vector2(rb.velocity.x, -maxVSpeed);
        else rb.velocity = new Vector2(rb.velocity.x, maxVSpeed);
    }

    // Changes scenes (this looks nicer)
    public void ChangeScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    // Moves the player rigidbody2D to a specified location
    public void MoveTo(Vector2 pos)
    {
        rb.position = pos;
    }

    // Respawn the player
    public void Respawn(bool playSound = true, bool playAnimation = true, bool doFreezeFrames = true, bool isRestarting = false, bool countDeath = true)
    {
        if (invulnerable && !isRestarting) return;

        // Play dead animation
        if (playAnimation) ChangeAnimatorState("Dead");

        // Play death sound
        if (playSound) PlaySound(deathSound);
        
        // Respawn normally
        GameManager.Instance.InitiateRespawn(doFreezeFrames);
    
        // Update deaths
        if (countDeath) deaths += 1;
    }

    // Updates player variables/references
    public void UpdatePlayerVars() {
        playerObject = GameObject.Find("Meem");
        rb = playerObject.GetComponent<Rigidbody2D>();
        renderer = playerObject.GetComponent<SpriteRenderer>();
        audio = playerObject.GetComponent<AudioSource>();
        animator = playerObject.GetComponent<Animator>();
    }

    // Changes animator state with another animation
    public void ChangeAnimatorState(string newState) {
        // Do not play the same animation!
        if (currentAnimatorState == newState || waitForRespawn) return;

        // Plays an animation & updates the current state
        animator.Play(newState);
        currentAnimatorState = newState; 
    }

    // Plays a sound given an audioclip and a desired volume.
    public void PlaySound(AudioClip sound, float volume = 1.0f) {
        audio.clip = sound;
        audio.volume = volume;
        audio.Play();
    }
}
