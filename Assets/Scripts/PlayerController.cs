using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : ResettableStateControllerBase
{

    // Configurable horizontal and vertical speed of player
    public float horizontalSpeed, verticalSpeed;

    // Component references
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer spriteRenderer;

    private MainController mainController;
    private BoxCollider2D boxCollider2D;

    // Configurable reset/saved states
    private Vector3 initialTransformPosition;
    private Vector2 savedVelocity;
    private bool canJump = false;
    private HashSet<SampleGombaController> jumpedOverSet;

    void Awake()
    {
        
        // Get references for components
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        mainController = GameObject.Find("Main Controller").GetComponent<MainController>();

        // Get player's initial position: we will always have the player reset from here
        initialTransformPosition = transform.position;

        // Perform reset of states
        InitializeStates();

    }

    // Initialize player
    protected override void OnInitializeStates() {

        // Enable collider and set body to kinematic (not affected by forces)
        boxCollider2D.enabled = false;
        rigidBody2D.isKinematic = true;

        // Move player to initial position
        transform.position = initialTransformPosition;

        // Set saved velocity to zero
        savedVelocity = Vector2.zero;

        // Disable jumping in air
        canJump = false;

        // Create set of gombas that have been jumped over
        jumpedOverSet = new HashSet<SampleGombaController>();
    }

    protected override void OnPause()
    {
        // Disable collider, set body to not be affected by forces
        boxCollider2D.enabled = false;
        rigidBody2D.isKinematic = true;

        // Save pre-pause velocity, then set velocity to zero
        savedVelocity = rigidBody2D.velocity;
        rigidBody2D.velocity = Vector2.zero;
    }

    protected override void OnResume()
    {
        // Enable collider, set body to dynamic
        boxCollider2D.enabled = true;
        rigidBody2D.isKinematic = false;

        // Restore original pre-pause velocity
        rigidBody2D.velocity = savedVelocity;
    }

    void FixedUpdate(){
        if (IsPaused()) return;

        // Handle horizontal and vertical movement separately
        MoveBodyHorizontal();
        MoveBodyVertical();
    }

    void Update() {
        if (IsPaused()) return;

        // Visuals: handle flipping of sprite to indicate direction faced
        ManageSpriteFlip();
    }

    void MoveBodyHorizontal() {

        // Simulate movement by moving player at each frame
        transform.position = transform.position + new Vector3(Utils.GetHorizontalScalar() * Time.deltaTime * horizontalSpeed, 0, 0);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        // I think we can ignore IsPaused() checking here
        // If enter ground's collision box, enable jump and empty list of gombas jumped over
        if (other.gameObject.CompareTag(Utils.groundTag)) {
            canJump = true;
            jumpedOverSet.Clear();
        }

        else if (other.gameObject.CompareTag(Utils.enemyTag)) {

            // Get enemy controller
            EnemyController e = other.gameObject.GetComponent<EnemyController>();

            // Check if enemy is to be killed (true)
            if (e.KillCheck(this)) {

                // Apply post-kill effect to player
                e.GetKillEffect(this);
                // Indicate to main controller that enemy is to be killed
                mainController.SubmitKill(other.gameObject);
            }
            // Or if enemy is to kill player (false)
            else {
                // Indicate death to main controller
                mainController.SubmitGameOver();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (IsPaused()) return;
        
        if (other.gameObject.CompareTag(Utils.enemyWeaponTag)) {

            mainController.SubmitGameOver();
        }
        
    }

    void MoveBodyVertical() {

        // Check if up button is pressed and that player is on ground
        if (Input.GetKey(Utils.upMoveKey) && canJump) {

            // Disable jumping
            canJump = false;

            // Add impulse to simulate jumping
            rigidBody2D.AddForce(Vector2.up * verticalSpeed, ForceMode2D.Impulse);
        }
    }
    void ManageSpriteFlip() {
        if (!Utils.IsMovingHorizontal()) return;

        // Flip sprite in the direction it is moving, if it is moving
        spriteRenderer.flipX = Utils.GetHorizontalScalar() < 0F;        
    }

    // Get this player's collider's bounds
    public Vector3 GetColliderBounds() {
        return boxCollider2D.bounds.size;
    }

    // Get this player's Rigidbody2D
    public Rigidbody2D GetRigidbody2D() {
        return rigidBody2D;
    }

    public bool tryAddJumpedOver(SampleGombaController s) {
        return jumpedOverSet.Add(s);
    }
}
