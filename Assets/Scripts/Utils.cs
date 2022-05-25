using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    // Key binding constants
    public const KeyCode leftMoveKey = KeyCode.LeftArrow;
    public const KeyCode rightMoveKey = KeyCode.RightArrow;
    public const KeyCode upMoveKey = KeyCode.Space;
    public const string restartKey = "s";

    // Tag constants
    public const string groundTag = "Ground";
    public const string enemyTag = "Enemy";
    public const string playerTag = "Player";
    public const string mainControllerTag = "GameController";
    public const string enemyWeaponTag = "EnemyWeapon";
    public const string wallTag = "Wall";

    // Infinitesimal value to test for zero
    public static float epsilon = 0.0000001F;

    // Get scalar indicating horizontal direction to move, as implied by movement keys
    // -1: Left, 0: None, 1: Right
    public static float GetHorizontalScalar() {
        if (Input.GetKey(leftMoveKey) ^ Input.GetKey(rightMoveKey)) return Input.GetKey(rightMoveKey)? 1F: -1F;
        return 0F;
    }

    // Check if key presses indicate net horizontal movement
    public static bool IsMovingHorizontal() {
        return Math.Abs(GetHorizontalScalar()) >= epsilon;
        
    }

    // Get score awarded for killing a given enemy type
    public static int GetKillScore(EnemyType e) {
        int scoreIncrement = 0;
        
        switch(e) {
            case Utils.EnemyType.SampleGomba:
                scoreIncrement = 100;
                break;
            case Utils.EnemyType.AxeGomba:
                scoreIncrement = 200;
                break;
        }

        return scoreIncrement;
    }

    // Enum of enemy types
    public enum EnemyType {
        SampleGomba,
        AxeGomba
    }
}
