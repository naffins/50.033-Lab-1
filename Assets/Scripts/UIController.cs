using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIController : MonoBehaviour
{

    // Constants indicating game object names
    private const string panelName = "Panel";
    private const string scoreDisplayName = "Score Value";
    private const string buttonName = "Start Button";
    private const string gameOverIndicatorName = "Game Over Text";

    private GameObject panel, startButton, gameOverText;
    private Text scoreValue;

    // Start is called before the first frame update
    void Start()
    {

        // Get the children (not sure if this is less expensive than Find though?)
        foreach (Transform t in transform) {
            switch(t.gameObject.name) {
                case panelName:
                    panel = t.gameObject;
                    break;
                case scoreDisplayName:
                    scoreValue = t.gameObject.GetComponent<Text>();
                    break;
                case buttonName:
                    startButton = t.gameObject;
                    break;
                case gameOverIndicatorName:
                    gameOverText = t.gameObject;
                    break;
                default:
                    break;
            }
        }
    }

    // Update score display
    public void UpdateScore(int score) {

        // Show at least 7 digits
        scoreValue.text = score.ToString("D7");
    }

    // Set UI to play mode (disable panel, start button, game over text, set score to 0)
    public void StartGameUI() {
        UpdateScore(0);
        panel.SetActive(false);
        startButton.SetActive(false);
        gameOverText.SetActive(false);
    }

    // Set UI to game over mode (enable panel, start button, game over text)
    public void EndGameUI() {
        panel.SetActive(true);
        startButton.SetActive(true);
        gameOverText.SetActive(true);
    }
}
