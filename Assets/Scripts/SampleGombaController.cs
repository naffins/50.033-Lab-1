using UnityEngine;
using System;

public class SampleGombaController : EnemyController
{

    private const float period = 5F;
    private const float quarterPeriod = 0.25F * period;
    private const float amplitude = 5F;
    
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
        boxCollider2D.enabled = false;
        rigidBody2D.isKinematic = true;
        elapsedTime = 0;
        transform.position = initialTransformPosition;
    }

    protected override void OnPause() {boxCollider2D.enabled = false; rigidBody2D.isKinematic = true;}
    protected override void OnResume() {boxCollider2D.enabled = true; rigidBody2D.isKinematic = false;}
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsPaused()) return;
        UpdateTime();
        UpdatePosition();
        CheckJumpedOver();
    }

    void UpdatePosition() {
        if (elapsedTime < quarterPeriod) {
            transform.position = new Vector3(0,transform.position.y,0) + new Vector3(initialTransformPosition.x,0,0) + new Vector3(elapsedTime/quarterPeriod*amplitude,0,0);
        }
        else if (elapsedTime < period - quarterPeriod) {
            transform.position = new Vector3(0,transform.position.y,0) + new Vector3(initialTransformPosition.x,0,0) - new Vector3((elapsedTime-(quarterPeriod*2F))/quarterPeriod*amplitude,0,0);
        }
        else transform.position = new Vector3(0,transform.position.y,0) + new Vector3(initialTransformPosition.x,0,0) - new Vector3((period-elapsedTime)/quarterPeriod*amplitude,0,0);
    }

    void UpdateTime() {
        if (IsPaused()) return;
        elapsedTime = (elapsedTime + Time.deltaTime) % period;      
    }

    public float GetMidY() {
        Rect spriteRect = spriteRenderer.sprite.rect;
        return spriteRect.y + (0.5F * spriteRect.height);
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

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag==Utils.enemyTag) {
            Physics2D.IgnoreCollision(boxCollider2D,other.collider);
        }
    }

    // Criteria for "jumping over":
    // - x values within 0.5F
    // - player y > gomba y + 0.25F
    private void CheckJumpedOver() {
        bool jumpedOver = (Math.Abs(playerController.transform.position.x - transform.position.x) < 0.5F)
            && (playerController.transform.position.y > transform.position.y + 0.25F);
        if (!jumpedOver) return;
        if (playerController.tryAddJumpedOver(this)) mainController.SubmitSampleGombaJumpOver();
    }

    public override Vector3 GetKillScorePosition()
    {
        return transform.position + new Vector3(0,0.5F,0);
    }
}
