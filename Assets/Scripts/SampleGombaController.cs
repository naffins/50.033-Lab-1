using UnityEngine;
using System;

public class SampleGombaController : EnemyController
{

    // Movement period
    private const float period = 5F;
    private const float quarterPeriod = 0.25F * period;

    // Distance to be walked in each direction before returning to starting x
    private const float amplitude = 5F;

    // Bounds for accepting jumps over sample gombas
    // Maximum x-distance
    private const float jumpOverX = 0.5F;
    // Minimum y-distance
    private const float jumpOverY = 0.25F;

    
    // Variable indicating time since start of movement period
    private float elapsedTime = 0F;
    private Vector3 initialTransformPosition;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private PlayerController playerController;
    private MainController mainController;

    // Awake() is called immediately upon instantiation unlike Start(),
    // so we'll use this for now
    void Awake()
    {
        // Get components and initial position
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialTransformPosition = transform.position;
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerController = GameObject.FindWithTag(Utils.playerTag).GetComponent<PlayerController>();
        mainController = GameObject.FindWithTag(Utils.mainControllerTag).GetComponent<MainController>();
        InitializeStates();
    }

    protected override void OnInitializeStates()
    {
        // Disable collider, set rigid body to kinematic, set elapsed movement time to 0, record initial position
        boxCollider2D.enabled = false;
        rigidBody2D.isKinematic = true;
        elapsedTime = 0;
        transform.position = initialTransformPosition;
    }

    // On pause, enable collider and set rigid body to kinematic; invert for resuming
    protected override void OnPause() {boxCollider2D.enabled = false; rigidBody2D.isKinematic = true;}
    protected override void OnResume() {boxCollider2D.enabled = true; rigidBody2D.isKinematic = false;}
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsPaused()) return;

        // Update time elapsed since goomba was started
        UpdateTime();

        // Move the goomba
        UpdatePosition();

        // Check if Mario jumped over this goomba during his current jump, if any
        CheckJumpedOver();
    }

    // Movement:
    // Oscillate across starting position with defined amplitude and period
    void UpdatePosition() {
        if (elapsedTime < quarterPeriod) {
            transform.position = new Vector3(0,transform.position.y,0) + new Vector3(initialTransformPosition.x,0,0) + new Vector3(elapsedTime/quarterPeriod*amplitude,0,0);
        }
        else if (elapsedTime < period - quarterPeriod) {
            transform.position = new Vector3(0,transform.position.y,0) + new Vector3(initialTransformPosition.x,0,0) - new Vector3((elapsedTime-(quarterPeriod*2F))/quarterPeriod*amplitude,0,0);
        }
        else transform.position = new Vector3(0,transform.position.y,0) + new Vector3(initialTransformPosition.x,0,0) - new Vector3((period-elapsedTime)/quarterPeriod*amplitude,0,0);
    }

    // Track current time relative to start of movement
    void UpdateTime() {
        if (IsPaused()) return;
        elapsedTime = (elapsedTime + Time.deltaTime) % period;      
    }

    // Kill criterion: player bottom must be at least 0.25 units above Gomba core
    public override bool KillCheck(PlayerController p) {
        // Get bottom of player collider
        float playerBottom = p.transform.position.y - (0.5F * p.GetColliderBounds().y);
        
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

    // Boost Mario up by abit when he kills a Gomba
    // (For bonus effect, perhaps add a "Yahoo!" sound effect?)
    public override void GetKillEffect(PlayerController p)
    {
        p.GetRigidbody2D().velocity = Vector2.up * p.verticalSpeed * 0.8F;
    }

    public override Utils.EnemyType GetEnemyType()
    {
        return Utils.EnemyType.SampleGomba;
    }

    // Criteria for "jumping over":
    // - x values within 0.5F
    // - player y > gomba y + 0.25F
    private void CheckJumpedOver() {
        bool jumpedOver = (Math.Abs(playerController.transform.position.x - transform.position.x) < jumpOverX)
            && (playerController.transform.position.y > transform.position.y + jumpOverY);
        if (!jumpedOver) return;
        if (playerController.tryAddJumpedOver(this)) mainController.SubmitSampleGombaJumpOver();
    }

    // Get location at which to spawn score text
    public override Vector3 GetKillScorePosition()
    {
        return transform.position + new Vector3(0,0.5F,0);
    }
}
