using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScoreTextController : MonoBehaviour
{

    private const float disappearTime = 0.5F;

    void Awake() {
        GameObject.Destroy(gameObject,disappearTime);
    }

    public void SetScore(int score) {
        GetComponent<TextMesh>().text = score.ToString();
    }

}
