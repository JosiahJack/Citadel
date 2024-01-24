using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MinigamePing : MonoBehaviour {
    public RectTransform playerPaddle;
    public RectTransform computerPaddle;
    public RectTransform ball;
    private int lives;

    void OnEnable() {
        lives = 3;
        playerPaddle.x = 0;
        computerPaddle.x = 0;
        playerPaddle.x = 0;
        ball.x = 0;
        ball.y = 0;
    }

    void Update() {

    }
}
