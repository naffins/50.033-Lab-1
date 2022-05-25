using UnityEngine;
using System;

public class AxeGombaController : EnemyController {


    // Minimum time between axe activations
    private const float axeReloadTime = 3F;

    // Range from player within which goomba can change directions
    private const float switchableDistance = 2.5F;

    // Range from player out of which, given switchableDistance==true, goomba should switch
    // to head towards player if not already doing so
    private const float switchDistance = 5F;

    // Minimum movement speed
    private const float baseMovementSpeed = 4F;

    // Maximum increase in movement speed
    private const float movementSpeedVariation = 0.5F;

    // x-range within which axe is eligible for activation
    private const float activateXRange = 2F;

    // y-range within which axe is eligible for activation
    private const float activateYRange = 3F;
    
    private PlayerController playerController;
    private MainController mainController;
    private AxeController axeController;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;

    private bool isMovingRight = false;

    // Remaining reload time for goomba
    private float reloadTimeLeft = 0F;

    // Goomba's personal movement speed
    private float thisMovementSpeed;

    void Awake() {

        // Get player and main controller
        playerController = GameObject.FindWithTag(Utils.playerTag).GetComponent<PlayerController>();
        mainController = GameObject.FindWithTag(Utils.mainControllerTag).GetComponent<MainController>();    

        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        reloadTimeLeft = 0F;
        isMovingRight = false;
        
        rigidBody2D.isKinematic = true;
        boxCollider2D.enabled = false;
    }

    protected override void OnInitializeStates() {
        
        // Get child axe's controller
        foreach (Transform t in transform) {
            axeController = t.gameObject.GetComponent<AxeController>();
        }

        // Initialize said controller
        axeController.InitializeStates();

        // Set axe to be activation-ready
        reloadTimeLeft = 0F;

        // Orientate goomba movement towards player
        isMovingRight = playerController.transform.position.x > transform.position.x;
        
        rigidBody2D.isKinematic = true;
        boxCollider2D.enabled = false;

        // Randomize goomba speed
        thisMovementSpeed = baseMovementSpeed + (((float)(new System.Random().NextDouble() - 0.5F)) * movementSpeedVariation);
    }

    void FixedUpdate() {
        if (IsPaused()) return;

        // Move horizontally and check whether it's time for ze axe
        MoveHorizontal();
        CheckAxeTrigger();
    }

    // Movement mechanics: move towards player until player reaches a certain distance in direction opposite
    // of goomba's movement direction
    private void MoveHorizontal() {

        // If player runs in opposite direction to goomba and becomes too far, goomba shall switch direction
        float oppositeDirectionDistance = (transform.position.x - playerController.transform.position.x)
            * (isMovingRight? 1F: -1F);
        if (oppositeDirectionDistance > switchDistance) {
            isMovingRight = !isMovingRight;
        }
    
        transform.position = transform.position + (new Vector3(thisMovementSpeed,0F,0F) * Time.deltaTime * (isMovingRight? 1F: -1F));
    }

    // Check whether it's time and/or the right conditions to trigger the axe
    private void CheckAxeTrigger() {

        // Decrement reload timer
        if (reloadTimeLeft>0F) {
            reloadTimeLeft-=Time.deltaTime;
        }
        if (reloadTimeLeft>0F) return;
        
        // Perform checking for whether player is within range
        bool toActivate = (Math.Abs(transform.position.x - playerController.transform.position.x) <= activateXRange)
            && (transform.position.y < playerController.transform.position.y)
            && (playerController.transform.position.y - transform.position.y < activateYRange);

        // If Mario is within range, reset reload timer and swoosh
        if (toActivate) {
            reloadTimeLeft = axeReloadTime;
            axeController.Activate(isMovingRight);
        }
    }

    protected override void OnPause()
    {
        rigidBody2D.isKinematic = true;
        boxCollider2D.enabled = false;

        // Pause axe
        axeController.Pause();
    }

    protected override void OnResume()
    {
        rigidBody2D.isKinematic = false;
        boxCollider2D.enabled = true;

        // Resume axe
        axeController.Resume();
    }

    public override Utils.EnemyType GetEnemyType() {
        return Utils.EnemyType.AxeGomba;
    }

    // Again, Mario gets to bounce up (yahoo!)
    public override void GetKillEffect(PlayerController p)
    {
        p.GetRigidbody2D().velocity = Vector2.up * p.verticalSpeed * 0.8F;
    }

    public override Vector3 GetKillScorePosition() {
        return transform.position + new Vector3(0,0.5F,0);
    }

    public override bool KillCheck(PlayerController p) {
        // Get bottom of player collider
        float playerBottom = p.transform.position.y - (0.5F * p.GetColliderBounds().y);
        
        // Kill criterion: player bottom must be at least 0.25 units above Gomba core
        return playerBottom - transform.position.y >= 0F;
    }

    public override void Kill() {
        // Halt Gomba collider and movement
        Pause();

        // Squish the Gomba
        transform.localScale = new Vector3(1F,0.5F,1F);
        transform.position = transform.position - new Vector3(0,0.25F,0);

        // Destroy the Gomba :(
        Destroy(gameObject,0.5F);
    }

}