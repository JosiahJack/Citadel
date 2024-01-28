using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MinigamePing : MonoBehaviour {
    public Image playerPaddleImg;
    public RectTransform playerPaddle;
    public Image computerPaddleImg;
    public RectTransform computerPaddle;
    public Image ballImg;
    public RectTransform ball;
    public Text playerScoreText;
    public Text computerScoreText;
    private float computerRate = 12f;
    private float playerRate = 12f;
    public float ballSpeed = 10f;
    public int playerScore;
    public int computerScore;
    private float computerVel;
    private float playerVel;
    private Vector2 ballDir;
    private float paddleWidthH = 18f; // Half lengths
    private float paddleHeightH = 6f;
    public float ballSideH = 0f;
    public GameObject gameOver;
    public Text winText;

    private float x;
    private float y;
    private float frameFinished = 0f;
    private Color ballHitColor = new Color(0.93f,0.93f,0.39f);
    private Color ballColor = new Color(0.6886f,0.6886f,0.6886f);
    private Color paddleHitColor = new Color(0.8902f + 0.05f,0.8745f + 0.05f,0.0f);

    void OnEnable() {
        Reset();
    }

    private void Reset() {
        playerScore = 0;
        computerScore = 0;
        playerPaddle.localPosition = new Vector3(0f,-100f,0f);
        computerPaddle.localPosition = new Vector3(0f,100f,0f);
        gameOver.SetActive(false);
        ResetBall();
    }

    private Vector2 GetNewBallDirection() {
        float dirX = Random.Range(-0.3f,0.3f);
        float dirY = Random.Range(0f,1f) < 0.5f ? Random.Range( 0.8f, 1.0f)
                                                : Random.Range(-1.0f,-0.8f);
        return new Vector2(dirX,dirY);
    }

    private void ResetBall() {
        ball.localPosition = new Vector3(0f,0f,0f);
        computerPaddle.localPosition = new Vector3(0f,100f,0f);
        ballDir = GetNewBallDirection();
    }

    void Update() {
        if (frameFinished >= Time.time) return;

        frameFinished = Time.time + (1f/30f); // 30fps, it's a potato.
        PlayerPaddleUpdate();
        ComputerPaddleUpdate();
        BallUpdate();
    }

    private void BallUpdate() {
        ballImg.color = Color.Lerp(ballImg.color,ballColor,1.2f);
        x = ball.localPosition.x;
        y = ball.localPosition.y;
        x += ballDir.x * ballSpeed;
        y += ballDir.y * ballSpeed;
        ball.localPosition = new Vector3(x,y,0f);
        if (ball.localPosition.y < -160f) { // More than 133 gives delay.
            computerScore++;
            computerScoreText.text = computerScore.ToString();
            if (computerScore == 11) { GameOver(); return; }

            computerPaddleImg.color = Const.a.ssGreenText;
            playerPaddleImg.color = Const.a.ssRedText;
            ResetBall();
            return;
        } else if (ball.localPosition.y > 160f) { // More than 133 gives delay.
            playerScore++;
            if (playerScore == 11) { GameOver(); return; }

            playerScoreText.text = playerScore.ToString();
            playerPaddleImg.color = Const.a.ssGreenText;
            computerPaddleImg.color = Const.a.ssRedText;
            ResetBall();
            return;
        }

        // Hit sides
        if (ball.localPosition.x >  122f || ball.localPosition.x < -122f) {
            ballDir.x *= -1f;
            ballImg.color = ballHitColor;
        }

        // Hit paddles
        if (ballDir.y < 0f) {
            if (ball.localPosition.y
                < (playerPaddle.localPosition.y + paddleHeightH + ballSideH + ballSideH)
                && ball.localPosition.y
                > (playerPaddle.localPosition.y - paddleHeightH - ballSideH)) {

                if (ball.localPosition.x
                    < (playerPaddle.localPosition.x + paddleWidthH + ballSideH)
                    && ball.localPosition.x
                    > (playerPaddle.localPosition.x - paddleWidthH - ballSideH)) {

                    ballDir.y *= -1f;
                    playerPaddleImg.color = paddleHitColor;

                    // Hit edge of paddle
                    if (   ball.localPosition.x >
                             playerPaddle.localPosition.x + paddleWidthH
                        || ball.localPosition.x <
                             playerPaddle.localPosition.x - paddleWidthH) {

                        ballDir.x *= -1f;
                        playerPaddleImg.color = Const.a.ssYellowText;
                    }

                    float add = playerVel * 0.75f;
                    ballDir.x += add;
                    if (ballDir.x > 1f) ballDir.x = 1f;
                    else if (ballDir.x < -1f) ballDir.x = -1f;

                    ball.localPosition = new Vector3(
                        ball.localPosition.x,
                        playerPaddle.localPosition.y + paddleHeightH + ballSideH + 0.05f,
                        0f
                    );

                    ballImg.color = ballHitColor;
                }
            }
        } else if (ballDir.y > 0f) {
            if (ball.localPosition.y
                > (computerPaddle.localPosition.y - paddleHeightH - ballSideH - ballSideH)
                && ball.localPosition.y
                < (computerPaddle.localPosition.y + paddleHeightH + ballSideH)) {

                if (ball.localPosition.x
                    < (computerPaddle.localPosition.x + paddleWidthH + ballSideH)
                    && ball.localPosition.x
                    > (computerPaddle.localPosition.x - paddleWidthH - ballSideH)) {

                    ballDir.y *= -1f;
                    computerPaddleImg.color = paddleHitColor;

                    // Hit edge of paddle
                    if (   ball.localPosition.x >
                             playerPaddle.localPosition.x + paddleWidthH
                        || ball.localPosition.x <
                             playerPaddle.localPosition.x - paddleWidthH) {

                        ballDir.x *= -1f;
                        computerPaddleImg.color = Const.a.ssYellowText;
                    }

                    float add = computerVel * 0.75f;
                    ballDir.x += add;
                    if (ballDir.x > 1f) ballDir.x = 1f;
                    else if (ballDir.x < -1f) ballDir.x = -1f;

                    ball.localPosition = new Vector3(
                        ball.localPosition.x,
                        computerPaddle.localPosition.y - paddleHeightH - ballSideH - 0.05f,
                        0f
                    );

                    ballImg.color = ballHitColor;
                }
            }
        }
    }

    private void PlayerPaddleUpdate() {
        playerPaddleImg.color = Color.Lerp(playerPaddleImg.color,Color.white,2f);
        x = playerPaddle.localPosition.x;
        playerVel = (MinigameCursor.a.minigameMouseX - x) / 48f;
        playerVel = Mathf.Clamp(playerVel,-1f,1f);
        x += playerVel * playerRate;
        if (x < (-128f + paddleWidthH)) {
            x = -128f + paddleWidthH;
            playerVel = 0f;
        } else if (x > (128f - paddleWidthH)) {
            x = 128f - paddleWidthH;
            playerVel = 0f;
        }
        playerPaddle.localPosition = new Vector3(x,-100f,0f);
    }

    private void ComputerPaddleUpdate() {
        computerPaddleImg.color = Color.Lerp(computerPaddleImg.color,Color.white,2f);
        x = computerPaddle.localPosition.x;
        computerVel = (ball.localPosition.x - x) / 48f;
        computerVel = Mathf.Clamp(computerVel,-1f,1f);
        x += computerVel * computerRate;
        if (x < (-128f + paddleWidthH)) {
            x = -128f + paddleWidthH;
            computerVel = 0f;
        } else if (x > (128f - paddleWidthH)) {
            x = 128f - paddleWidthH;
            computerVel = 0f;
        }
        computerPaddle.localPosition = new Vector3(x,100f,0f);
    }

    private void GameOver() {
        gameOver.SetActive(true);
    }

    public void ResetOnGameOver() {
        if (playerScore > computerScore) winText.text = "YOU WON!";
        else winText.text = "YOU LOSE";

        Reset();
    }
}
