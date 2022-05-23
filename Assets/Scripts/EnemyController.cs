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
}