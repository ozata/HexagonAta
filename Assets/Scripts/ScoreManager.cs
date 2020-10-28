using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    int score;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start () {
        scoreText.text = "Score: deneme" + score;
    }

    // Update is called once per frame
    void Update () { }

    public int GetScore() {
        return this.score;
    }

    public void AddScore () {
        score += 5;
    }

}