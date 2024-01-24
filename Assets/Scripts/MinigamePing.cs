using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MinigamePing : MonoBehaviour {
    public RectTransform playerPaddle;
    public RectTransform computerPaddle;
    public RectTransform ball;
    public RectTransform minigameCursor;
    public Text playerScoreText;
    public Text computerScoreText;
    public int playerScore;
    public int computerScore;
    private int lives;
    private float mouseX;
    private float mouseY;
    private float computerVel;
    private float playerVel;
    private float ballVel;
    private Vector2 ballDir;

    void OnEnable() {
        lives = 3;
        playerScore = 0;
        computerScore = 0;
        playerPaddle.localPosition = new Vector3(0f,-100f,0f);
        computerPaddle.localPosition = new Vector3(0f,100f,0f);
        ResetBall();
    }

    private Vector2 GetNewBallDirection() {
        float dirX = Random.Range(-0.5f,0.5f);
        float dirY = Random.Range(0f,1f) < 0.5f ? -1f : 1f;
        return new Vector2(dirX,dirY);
    }

    private void ResetBall() {
        ball.localPosition = new Vector3(0f,0f,0f);
        ballDir = GetNewBallDirection();
    }

    void Update() {
        mouseX = MouseCursor.a.cursorPosition.x;
        mouseY = MouseCursor.a.cursorPosition.y;
        ComputerPaddleUpdate();
    }

    private void BallUpdate() {
        float x = ball.localPosition.x;
        float y = ball.localPosition.y;
        x = Mathf.SmoothDamp(x,x + ballDir.x,ref ballVel,1f);
        y = Mathf.SmoothDamp(y,y + ballDir.y,ref ballVel,1f);
        ball.localPosition = new Vector3(x,y,0f);
        if (ball.localPosition.y < -106f) {
            computerScore++;
            computerScoreText.text = computerScore.ToString();
            ResetBall();
        } else if (ball.localPosition.y > 106f) {
            playerScore++;
            playerScoreText.text = playerScore.ToString();
            ResetBall();
        }
    }

    private void ComputerPaddleUpdate() {
        computerPaddle.localPosition = new Vector3(Mathf.SmoothDamp(computerPaddle.localPosition.x,
                                 ball.localPosition.x,ref computerVel,1f),
                100f,
                0f);
    }
}
