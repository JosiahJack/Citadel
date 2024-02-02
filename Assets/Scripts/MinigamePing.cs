using UnityEngine;
using UnityEngine.UI;

public class MinigamePing : MonoBehaviour {
    public Image playerPaddleImg;
    public RectTransform playerPaddle;
    public Image computerPaddleImg;
    public RectTransform computerPaddle;
    public Image ballImg;
    public RectTransform ball;
    public Text playerScoreText;
    public Text computerScoreText;
    public GameObject playerPizzaz;
    public GameObject computerPizzaz;
    public GameObject gameOver;
    public Text winText;
    public GameObject winPizzazz;
    public int playerScore;
    public int computerScore;

    private float computerRate = 12f;
    private float playerRate = 15f;
    private float ballSpeed = 10f;
    private float computerVel;
    private float playerVel;
    private Vector2 ballDir;
    private float paddleWidthH = 18f; // Half lengths
    private float paddleHeightH = 6f;
    private float ballSideH = 6f;
    private int servecount;
    private bool playerServing;
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
        playerScore = 0;
        computerScore = 0;
        servecount = 0;
        playerServing = Random.Range(0f,1f) < 0.5f ? true : false;
        UpdateScoreText();
        playerPaddle.localPosition = new Vector3(0f,-100f,0f);
        computerPaddle.localPosition = new Vector3(0f,100f,0f);
        gameOver.SetActive(false);
        winPizzazz.SetActive(false);
        ResetBall();
    }

    private void UpdateScoreText() {
        computerScoreText.text = computerScore.ToString();
        playerScoreText.text = playerScore.ToString();
    }

    private Vector2 GetNewBallDirection() {
        float dirX = Random.Range(-0.3f,0.3f);
        float dirY = 1f;
        if (playerScore == 0 && computerScore == 0) {
            dirY = Random.Range(0f,1f) < 0.5f ? Random.Range( 0.8f, 1.0f)
                                              : Random.Range(-1.0f,-0.8f);
        } else {
            if (playerServing) {
                dirY = Random.Range(0.8f,1.0f);
            } else {
                dirY = Random.Range(-1.0f,-0.8f);
            }
        }

        return new Vector2(dirX,dirY);
    }

    private void ResetBall() {
        ball.localPosition = new Vector3(0f,0f,0f);
        computerPaddle.localPosition = new Vector3(0f,100f,0f);
        ballDir = GetNewBallDirection();
        ballResetFinished = PauseScript.a.relativeTime + 2.5f;
    }

    void Update() {
        if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
        if (frameFinished >= PauseScript.a.relativeTime) return;
        if (gameOver.activeInHierarchy) return;

        frameFinished = PauseScript.a.relativeTime + (1f/30f); // 30fps, it's a potato.
        PlayerPaddleUpdate();
        if (ballResetFinished >= PauseScript.a.relativeTime + 1f) return;

        ComputerPaddleUpdate();
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
            computerScore++;
            servecount++;
            if (servecount >= 2 || (computerScore >= 10 && playerScore >= 10)) {
                servecount = 0;
                playerServing = !playerServing; // No one will notice...
            }

            UpdateScoreText();
            if (computerScore >= 11 && (computerScore - playerScore) >= 2) {
                GameOver();
                return;
            }

            computerPaddleImg.color = Const.a.ssGreenText;
            playerPaddleImg.color = Const.a.ssRedText;
            Utils.Activate(computerPizzaz);
            ResetBall();
            return;
        } else if (ball.localPosition.y > 160f) { // More than 133 gives delay.
            playerScore++;
            servecount++;
            if (servecount >= 2 || (computerScore >= 10 && playerScore >= 10)) {
                servecount = 0;
                playerServing = !playerServing; // ... but it rules
            }

            if (playerScore >= 11 && (playerScore - computerScore) >= 2) {
                GameOver();
                return;
            }

            UpdateScoreText();
            playerPaddleImg.color = Const.a.ssGreenText;
            computerPaddleImg.color = Const.a.ssRedText;
            Utils.Activate(playerPizzaz);
            ResetBall();
            return;
        }

        // Hit sides
        if (ball.localPosition.x < -122f) {
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
                    if (computerVel < 0.05f) {
                        float neg = Random.Range(0f,1f) < 0.5f ? -1f : 1f;
                        add = 0.2f * neg * 0.75f; // Don't get stuck with no x.
                    }

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

    private void ComputerPaddleUpdate() {
        computerPaddleImg.color = Color.Lerp(computerPaddleImg.color,Color.white,3f);
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
        if (playerScore > computerScore) {
            winText.text = "YOU WON!";
            winPizzazz.SetActive(true);
        } else winText.text = "YOU LOSE";
    }

    public void ResetOnGameOver() {
        Reset();
    }
}
