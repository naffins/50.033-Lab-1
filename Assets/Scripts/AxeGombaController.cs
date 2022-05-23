using UnityEngine;
using System;

public class AxeGombaController : EnemyController {

    private const float axeReloadTime = 3F;
    private const float switchableDistance = 2.5F;
    private const float switchDistance = 5F;
    private const float baseMovementSpeed = 4F;
    private const float movementSpeedVariation = 0.5F;
    private const float activateXRange = 2F;
    private const float activateYRange = 3F;
    
    private PlayerController playerController;
    private MainController mainController;
    private AxeController axeController;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;

    private bool movementSwitchable = false;
    private bool isMovingRight = false;
    private float reloadTimeLeft = 0F;
    private float thisMovementSpeed;

    void Awake() {
        playerController = GameObject.FindWithTag(Utils.playerTag).GetComponent<PlayerController>();
        mainController = GameObject.FindWithTag(Utils.mainControllerTag).GetComponent<MainController>();    

        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        movementSwitchable = false;
        reloadTimeLeft = 0F;
        isMovingRight = false;
        
        rigidBody2D.isKinematic = true;
        boxCollider2D.enabled = false;
    }

    protected override void OnInitializeStates() {
        
        foreach (Transform t in transform) {
            axeController = t.gameObject.GetComponent<AxeController>();
        }

        axeController.InitializeStates();

        movementSwitchable = false;
        reloadTimeLeft = 0F;
        isMovingRight = playerController.transform.position.x > transform.position.x;
        
        rigidBody2D.isKinematic = true;
        boxCollider2D.enabled = false;

        thisMovementSpeed = baseMovementSpeed + (((float)(new System.Random().NextDouble() - 0.5F)) * movementSpeedVariation);
    }

    void FixedUpdate() {
        if (IsPaused()) return;
        MoveHorizontal();
        CheckAxeTrigger();
    }

    private void OnCollisionStay2D(Collision2D other) {
        if ((other.gameObject.tag==Utils.enemyTag) || (other.gameObject.tag==Utils.wallTag)) {
            Physics2D.IgnoreCollision(boxCollider2D,other.collider);
        }
    }

    private void MoveHorizontal() {
        if (!movementSwitchable) {
            movementSwitchable = Math.Abs(transform.position.x - playerController.transform.position.x) <= switchableDistance;
        }
        if (movementSwitchable) {
            float oppositeDirectionDistance = (transform.position.x - playerController.transform.position.x)
                * (isMovingRight? 1F: -1F);
            if (oppositeDirectionDistance > switchDistance) {
                isMovingRight = !isMovingRight;
                movementSwitchable = false;
            }
        }
        transform.position = transform.position + (new Vector3(thisMovementSpeed,0F,0F) * Time.deltaTime * (isMovingRight? 1F: -1F));
    }

    private void CheckAxeTrigger() {
        if (reloadTimeLeft>0F) {
            reloadTimeLeft-=Time.deltaTime;
        }
        if (reloadTimeLeft>0F) return;
        
        // Perform checking for whether player is within range
        bool toActivate = (Math.Abs(transform.position.x - playerController.transform.position.x) <= activateXRange)
            && (transform.position.y < playerController.transform.position.y)
            && (playerController.transform.position.y - transform.position.y < activateYRange);
        if (toActivate) {
            reloadTimeLeft = axeReloadTime;
            axeController.Activate(isMovingRight);
        }
    }

    protected override void OnPause()
    {
        rigidBody2D.isKinematic = true;
        boxCollider2D.enabled = false;
        axeController.Pause();
    }

    protected override void OnResume()
    {
        rigidBody2D.isKinematic = false;
        boxCollider2D.enabled = true;
        axeController.Resume();
    }

    public override Utils.EnemyType GetEnemyType() {
        return Utils.EnemyType.AxeGomba;
    }

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