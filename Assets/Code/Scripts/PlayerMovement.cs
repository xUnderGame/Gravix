using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerBehaviour player;

    [SerializeField] private float bufferTime = 0.08f;
    [SerializeField] private float coyoteTime = 0.015f;
    private float bufferCounter;
    private float coyoteCounter;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        player = GameManager.Instance.player;
        SceneManager.sceneLoaded += OnSceneChange;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Moving left
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && player.canMove && !GameManager.Instance.isPaused && !GameManager.Instance.player.waitForRespawn)
        {
            rb.velocity = new Vector2(-player.maxHSpeed, rb.velocity.y);
            rb.AddForce(-transform.right, ForceMode2D.Force);
            spriteRenderer.flipX = true;
            player.ChangeAnimatorState("Walk");
        }

        // Moving right
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && player.canMove && !GameManager.Instance.isPaused && !GameManager.Instance.player.waitForRespawn)
        {
            rb.velocity = new Vector2(player.maxHSpeed, rb.velocity.y);
            rb.AddForce(transform.right, ForceMode2D.Force);
            spriteRenderer.flipX = false;
            player.ChangeAnimatorState("Walk");
        }

        // "Frame buffering" to change gravity
        if (Input.GetKeyDown(KeyCode.Z)) {
            bufferCounter = bufferTime;
        } else {
            bufferCounter -= Time.deltaTime;
        }

        // Coyote time to change gravity
        if (Mathf.Abs(rb.velocity.y) < 0.01f) {
            coyoteCounter = coyoteTime;
        } else {
            coyoteCounter -= Time.deltaTime;
        }

        // Swapping gravity when "Z" is pressed
        if (bufferCounter > 0 && coyoteCounter > 0 && !player.restrictedGrav && player.canSwap && !GameManager.Instance.isPaused && !GameManager.Instance.player.waitForRespawn) {
            player.ChangeGravity();
            player.swaps += 1;
            bufferCounter = 0;
        }

        // Force respawn player when "R" (restart) is pressed
        if (Input.GetKeyDown(KeyCode.R) && player.canForceRespawn && !GameManager.Instance.isPaused && !GameManager.Instance.player.waitForRespawn) {
            GameManager.Instance.spawnpoint.forceDefaultCheckpoint = false;
            player.Respawn(true, false, false, true);
        }

        // Pauses the game by pressing the "ESC" key
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.isPaused && !GameManager.Instance.player.waitForRespawn && SceneManager.GetActiveScene().name != "Main Menu") {
            // Immediatly hide room name 
            if (GameManager.Instance.roomNameFadeCoroutine != null) {
                StopCoroutine(GameManager.Instance.roomNameFadeCoroutine);
                GameManager.Instance.roomNameFadeCoroutine = StartCoroutine(GameManager.Instance.FadeRoomName(0f));
            }
            GameManager.Instance.GamePause();
        }

        // Resume the game
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.isPaused && !GameManager.Instance.player.waitForRespawn) {
            GameManager.Instance.GameResume();
        }

        // Setting horizontal velocity to 0 if nothing or both directions are being held
        if ((!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) || (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            player.ChangeAnimatorState("Idle");
        }

        // Vertical speed cap
        if (!player.gravity && rb.velocity.y != 0) rb.velocity = new Vector2(rb.velocity.x, -player.maxVSpeed);
        if (player.gravity && rb.velocity.y != 0) rb.velocity = new Vector2(rb.velocity.x, player.maxVSpeed);

        // Horizontal speed cap
        if (rb.velocity.x < -player.maxHSpeed) rb.velocity = new Vector2(-player.maxHSpeed, rb.velocity.y);
        if (rb.velocity.x > player.maxHSpeed) rb.velocity = new Vector2(player.maxHSpeed, rb.velocity.y);
    }   

    // Player changed scene!
    void OnSceneChange(Scene scene, LoadSceneMode sceneMode)
    {
        // Respawning sheningans
        if (GameManager.Instance.spawnpoint.forceDefaultCheckpoint) { 
            player.MoveTo(GameManager.Instance.spawnpoint.GetSpawnpoint(GameManager.Instance.spawnpoint.sceneChange));
            // player.ForceGravity(false);
        }
        else player.MoveTo(GameManager.Instance.spawnpoint.coordinates);

        // Destroy "deleteOnLoad" objects
        GameManager.Instance.deleteOnLoad.ForEach(toBeDestroyed => {
            GameObject toDestroy = GameObject.Find(toBeDestroyed);
            if (toDestroy != null) { Destroy(toDestroy); return; }
        });

        // UI should be enabled?
        if (scene.buildIndex == 0) GameManager.Instance.UI.SetActive(false);

        // Change current area song
        GameManager.Instance.ChangeAreaSong();

        // Update room name
        GameManager.Instance.roomName.SetActive(true);
        GameManager.Instance.roomName.GetComponent<Text>().text = SceneManager.GetActiveScene().name;
        GameManager.Instance.UI.GetComponent<Canvas>().worldCamera = Camera.main;
        
        // Hide room name
        if (GameManager.Instance.roomNameFadeCoroutine != null) StopCoroutine(GameManager.Instance.roomNameFadeCoroutine);
        GameManager.Instance.roomNameFadeCoroutine = StartCoroutine(GameManager.Instance.FadeRoomName());
    }
}
