using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public Vector3[] spawnLocations;

    private const float spawnInterval = 10F;

    private int score = 0;
    private bool isPlaying = false;
    private float checkoffTime = 15F;
    private float spawnCountdown;

    public GameObject sampleGombaPrefab;
    public GameObject axeGombaPrefab;
    public GameObject killScoreTextPrefab;
    private GameObject playerObject;
    private PlayerController playerController;
    private HashSet<EnemyController> enemyControllerSet;
    private HashSet<Vector3> initialSampleGombaPositionSet, initialAxeGombaPositionSet;
    private UIController uiController;
    private GameObject enemiesParent;
    private GameObject killScoreTextParent;
    private System.Random rng = new System.Random();

    void Start() {

        // Get player references
        playerObject = GameObject.FindWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();

        // Get UIController reference
        uiController = GameObject.Find("UI").GetComponent<UIController>();

        // Get parent for all enemies
        enemiesParent = GameObject.Find("Enemies");

        // Get parent of kill score popup text
        killScoreTextParent = GameObject.Find("Kill Score Texts");

        // Store positions of all initial gombas
        initialSampleGombaPositionSet = new HashSet<Vector3>();
        initialAxeGombaPositionSet = new HashSet<Vector3>();
        
        foreach(GameObject g in GameObject.FindGameObjectsWithTag(Utils.enemyTag)) {
            Vector3 gombaPosition = g.transform.position;
            EnemyController e = g.GetComponent<EnemyController>();
            switch(e.GetEnemyType()) {
                case Utils.EnemyType.SampleGomba:
                    initialSampleGombaPositionSet.Add(gombaPosition);
                    break;
                case Utils.EnemyType.AxeGomba:
                    initialAxeGombaPositionSet.Add(gombaPosition);
                    break;
                default:
                    Debug.Log("Enemy type not implemented");
                    break;
            }
            
        }
    }

    // Start a new game
    public void StartGame() {

        isPlaying = true;

        // Reset score
        score = 0;

        // Reset spawn time
        spawnCountdown = checkoffTime;

        // Track and clear all existing Gombas
        HashSet<GameObject> existingEnemies = new HashSet<GameObject>();
        foreach(GameObject g in GameObject.FindGameObjectsWithTag(Utils.enemyTag)) {
            GameObject.Destroy(g,0F);
            existingEnemies.Add(g);
        }

        // Recreate initial Gombas under enemiesParent
        foreach(Vector3 v in initialSampleGombaPositionSet) {
            GameObject.Instantiate(sampleGombaPrefab,v,Quaternion.identity,enemiesParent.transform);
        }

        foreach(Vector3 v in initialAxeGombaPositionSet) {
            GameObject.Instantiate(axeGombaPrefab,v,Quaternion.identity,enemiesParent.transform);
        }

        // Initialize HashSet of existing enemies
        enemyControllerSet = new HashSet<EnemyController>();
        
        // Get list of existing enemies
        foreach (GameObject g in GameObject.FindGameObjectsWithTag(Utils.enemyTag)) {
            if (existingEnemies.Contains(g)) continue;
            enemyControllerSet.Add(g.GetComponent<EnemyController>());
        }

        // Initialize player and start it
        playerController.InitializeStates();
        playerController.Resume();

        // Initialize and start each existing enemy
        foreach (EnemyController g in enemyControllerSet) {
            g.InitializeStates();
            g.Resume();
        }

        uiController.StartGameUI();

    }

    // Trigger game over
    public void SubmitGameOver() {

        isPlaying = false;

        // Halt player
        playerController.Pause();

        // Halt all enemies
        foreach (EnemyController g in enemyControllerSet) {
            g.Pause();
        }

        // Bring UI to game over modee
        uiController.EndGameUI();
    }

    // Trigger enemy kill
    public void SubmitKill(GameObject target) {

        if (!isPlaying) return;

        EnemyController enemyController = target.GetComponent<EnemyController>();

        // Remove from list of existing enemies
        enemyControllerSet.Remove(enemyController);

        // Get kill score text spawn position
        Vector3 killScoreTextPosition = enemyController.GetKillScorePosition();

        // Kill off the enemy
        enemyController.Kill();

        // Get score increment
        int scoreIncrement = 0;
        
        switch(enemyController.GetEnemyType()) {
            case Utils.EnemyType.SampleGomba:
                scoreIncrement = 100;
                break;
            case Utils.EnemyType.AxeGomba:
                scoreIncrement = 200;
                break;
            default:
                Debug.Log("Error: Enemy type not found");
                break;
        }
        
        // Update score
        score += scoreIncrement;

        // Update score on UI
        uiController.UpdateScore(score);

        // Spawn kill score text
        SpawnKillScoreText(killScoreTextPosition,scoreIncrement);
        
    }

    public void SubmitSampleGombaJumpOver() {
        if (!isPlaying) return;
        
        // Update score
        score += 1;

        // Update score on UI
        uiController.UpdateScore(score);
    }

    // Spawn text indicating point increment
    private void SpawnKillScoreText(Vector3 position, int score) {
        GameObject g = GameObject.Instantiate(killScoreTextPrefab,position,Quaternion.identity,killScoreTextParent.transform);
        g.GetComponent<KillScoreTextController>().SetScore(score);
    }

    void FixedUpdate() {
        if (!isPlaying) return;
        spawnCountdown -= Time.deltaTime;
        if (spawnCountdown > 0F) return;
        spawnCountdown = spawnInterval;

        GameObject newEnemy = GameObject.Instantiate(axeGombaPrefab,spawnLocations[rng.Next()%spawnLocations.Length],
            Quaternion.identity,enemiesParent.transform);
        EnemyController e = newEnemy.GetComponent<EnemyController>();
        enemyControllerSet.Add(e);
        e.InitializeStates();
        e.Resume();
    }
}
