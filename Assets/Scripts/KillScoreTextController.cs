using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScoreTextController : MonoBehaviour
{

    // Time taken for text to disappear
    private const float disappearTime = 0.5F;

    // Destroy score text soon after it is spawned
    void Awake() {
        GameObject.Destroy(gameObject,disappearTime);
    }

    // Update score shown upon kill
    public void SetScore(int score) {
        GetComponent<TextMesh>().text = score.ToString();
    }

}
