using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    int score;
    public TextMeshProUGUI scoreText;

    public int GetScore() {
        return this.score;
    }

    public void AddPoints () {
        score += 5;
    }

    public void ChangeScoreText(){
        scoreText.text = "Score: " + score;
    }

}