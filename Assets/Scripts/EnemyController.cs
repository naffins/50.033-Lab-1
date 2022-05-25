using UnityEngine;

public abstract class EnemyController : ResettableStateControllerBase
{

    // Check if player killed enemy (true) or enemy killed player (false)
    public abstract bool KillCheck(PlayerController p);

    // Sequence of actions for when enemy is killed
    public abstract void Kill();

    // Effect(s) on player when enemy is killed
    public abstract void GetKillEffect(PlayerController p);

    // Get type of enemy
    public abstract Utils.EnemyType GetEnemyType();

    // Get position at which to spawn kill score
    public abstract Vector3 GetKillScorePosition();

    // If collide with other enemies or (invisible) walls, ignore in the future
    private void OnCollisionStay2D(Collision2D other) {
        if ((other.gameObject.tag==Utils.enemyTag) || (other.gameObject.tag==Utils.wallTag)) {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(),other.collider);
        }
    }
}