using UnityEngine;
using UnityEngine.UI;

public class MinigameBotBounce : MonoBehaviour {
    public Image playerPaddleImg;
    public RectTransform playerPaddle;
    public Image ballImg;
    public RectTransform ball;
    public Text playerScoreText;
    public GameObject playerPizzaz;
    public GameObject gameOver;
    public Text winHeader;
    public Text winText;
    public GameObject winPizzazz;
    public int playerScore;
    public int numAlive;

    private float playerRate = 15f;
    private float ballSpeed = 10f;
    private float computerVel;
    private float playerVel;
    private Vector2 ballDir;
    private float paddleWidthH = 18f; // Half lengths
    private float paddleHeightH = 6f;
    private float ballSideH = 6f;
    private float x;
    private float y;
    private float frameFinished = 0f;
    private float ballResetFinished = 0;
    private Color ballHitColor = new Color(0.93f,0.93f,0.39f);
    private Color ballColor = new Color(0.6886f,0.6886f,0.6886f);
    private Color paddleHitColor = new Color(0.8902f + 0.05f,0.8745f + 0.05f,0.0f);

    void OnEnable() {
        Reset();
    }

    private void Reset() {
        playerScore = 3;
        numAlive = 24;
        UpdateScoreText();
        playerPaddle.localPosition = new Vector3(0f,-100f,0f);
        gameOver.SetActive(false);
        winPizzazz.SetActive(false);
        ResetBall();
    }

    private void UpdateScoreText() {
        playerScoreText.text = playerScore.ToString();
    }

    private Vector2 GetNewBallDirection() {
        float dirX = Random.Range(-0.5f,0.5f);
        float dirY = Random.Range(0.8f,1.0f);
        return new Vector2(dirX,dirY);
    }

    private void ResetBall() {
        ball.localPosition = new Vector3(0f,-100f,0f);
        ballDir = GetNewBallDirection();
        ballResetFinished = PauseScript.a.relativeTime + 2.5f;
    }

    void Update() {
        if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
        if (frameFinished >= PauseScript.a.relativeTime) return;
        if (gameOver.activeInHierarchy) return;

        if (numAlive <= 0) {
            GameOver();
            return;
        }

        frameFinished = PauseScript.a.relativeTime + (1f/30f); // 30fps, it's a potato.
        PlayerPaddleUpdate();
        if (ballResetFinished >= PauseScript.a.relativeTime) return;

        BallUpdate();
    }

    private void BallUpdate() {
        ballImg.color = Color.Lerp(ballImg.color,ballColor,3f);
        x = ball.localPosition.x;
        y = ball.localPosition.y;
        x += ballDir.x * ballSpeed;
        y += ballDir.y * ballSpeed;
        ball.localPosition = new Vector3(x,y,0f);
        if (ball.localPosition.y < -160f) { // More than 133 gives delay.
            playerPaddleImg.color = Const.a.ssRedText;
            playerScore--;
            Utils.Activate(playerPizzaz);
            ResetBall();
            if (playerScore <= 0) {
                GameOver();
                return;
            }

            return;
        }

        // Hit top
        if (ball.localPosition.y > 133f) { // More than 133 gives delay.
            ballDir.y *= -1f;
            ballImg.color = ballHitColor;
            ball.localPosition = new Vector3(ball.localPosition.x,132f,0f);
        } else if (ball.localPosition.x < -122f) { // Hit sides
            ballDir.x *= -1f;
            ballImg.color = ballHitColor;
            ball.localPosition = new Vector3(-121.9f,ball.localPosition.y,0f);
        } else if (ball.localPosition.x > 122f) {
            ballDir.x *= -1f;
            ballImg.color = ballHitColor;
            ball.localPosition = new Vector3(121.9f,ball.localPosition.y,0f);
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
        }
    }

    private void PlayerPaddleUpdate() {
        playerPaddleImg.color = Color.Lerp(playerPaddleImg.color,Color.white,3f);
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

    private void GameOver() {
        gameOver.SetActive(true);
        winHeader.text = "GAME OVER";
        if (numAlive <= 0) {
            winText.text = "YOU WON!";
            winPizzazz.SetActive(true);
        } else winText.text = "YOU LOSE";
    }

    public void ResetOnGameOver() {
        Reset();
    }
}
