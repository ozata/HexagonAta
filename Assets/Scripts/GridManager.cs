using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {

    ScoreManager scoreManager;
    public Color[] colors = new Color[6];
    public GameObject hexagonPrefab;
    public Sprite hexagonSprite;

    Transform canvasTransform;

    public int row, column;
    int[] color;

    // This flag makes sure game points starts at 0
    // When the game starts some hexagons gets destroyed
    // Because they Match, and that makes player gain points
    // So set score to 0 one time using this flag
    bool initScore;

    // Data Structure that will hold the Hexagon List
    List<List<GameObject>> list = new List<List<GameObject>> ();
    // One row of hexagons
    List<GameObject> sublist;
    // Hex objects that'll be destroyed, this list will be unique
    // TODO: Change to a different data structure as lists are not best ones to hold unique objects
    List<GameObject> destroyList = new List<GameObject> ();
    // Currently selected List
    List<GameObject> selectedList = new List<GameObject> ();

    // 1/10 of width and height to initialize the hexagons on the Canvas.
    float widthPiece;
    float heightPiece;

    void Start () {
        initScore=true;
        GenerateColors ();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        color = new int[row * column];
        canvasTransform = GameObject.FindGameObjectWithTag ("Canvas").transform;
        widthPiece = (float) Screen.width / 10;
        heightPiece = (float) Screen.height / 10;
        CreateRandomColor ();
        GenerateGridOnScreen ();
    }

    void Update () {
        // Core Game Mechanics
        CheckMatches ();
        DestroyMatches ();
        FillEmptySpaces ();

        // Start score as 0
        if(initScore){
            scoreManager.SetScore(0);
            scoreManager.UpdateScoreText();
        }

    }

    void CheckMatches () {
        for (int i = 1; i < row - 1; i++) {
            for (int j = 1; j < column - 1; j++) {
                string name = i.ToString () + " " + j.ToString ();
                GameObject hex = GameObject.Find (name.ToString ());
                if(hex != null){
                    CheckTrioMatch (hex);
                }
            }
        }
    }

    // Creates a random integer array that'll be used for color.
    void CreateRandomColor () {
        int tmp = 0;
        Random.InitState (System.DateTime.Now.Millisecond);
        for (int i = 0; i < color.Length; i++) {
            color[i] = Random.Range (0, colors.Length);
            if (i > 1 && color[i - 1] == color[i]) {
                color[i - 1] = Random.Range (1, colors.Length - 1);
            }
            // Trying to add more randomization to the color array.
            if (10 < i && i < color.Length - 10) {
                tmp = color[i];
                color[i + 5] = tmp;
            }
        }

    }

    // TODO: This function does too much stuff, it has to be more modular, fix it.
    // Grid generation is wrong, should grid really be generated on Canvas?
    void GenerateGridOnScreen () {
        // if counter is even, height will be lower.
        int counter = 0;
        int id = 1;

        float height = (float) Math.Floor (heightPiece) * 1.5f;
        float widthInit = (float) Math.Floor (widthPiece) * -4;
        float width = widthInit;
        float addition = hexagonSprite.rect.height - 30;

        for (int i = 0; i < row; i++) {
            sublist = new List<GameObject> ();
            for (int j = 0; j < column; j++) {
                GameObject hexagon = Instantiate (hexagonPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
                hexagon.transform.SetParent (canvasTransform, false);
                hexagon.transform.localPosition = new Vector3 (width, height, 0);
                hexagon.transform.GetChild(0).GetComponent<Image> ().color = colors[color[i + j]];
                hexagon.transform.GetChild(0).GetComponent<Hexagon> ().color = color[i + j];
                hexagon.transform.name = i.ToString () + " " + j.ToString ();
                hexagon.transform.GetChild(0).name = i.ToString () + " "  + j.ToString () + " child";
                if (counter % 2 == 0) {
                    height -= (float) addition / 2f;
                } else {
                    height += (float) addition / 2f;
                }
                width += (((float) 3.0 / 4) * addition);
                counter++;
                if (counter == column && column % 2 != 0) {
                    height += (float) addition / 2f;
                    counter = 0;
                }
                id++;
                sublist.Add (hexagon);
            }
            list.Add (sublist);
            height = height - (addition);
            width = widthInit;
        }
    }

    // There are 6 cases that will go down as a match and destroy the hexagon trio.
    // Check every hexagon in the game to find if there is a Match
    void CheckTrioMatch (GameObject hexagon) {
        int[] hexRowCol = ParseHexagonNameToRowAndColumn(hexagon);
        int hexRow = hexRowCol[0];
        int hexColumn = hexRowCol[1];
        if (list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn + 1]);
            AddHexagonToDestroyList (list[hexRow][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn + 1].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow + 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow + 1][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow + 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn]);
        }
        if (list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].transform.GetChild(0).GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow][hexColumn - 1]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn - 1]);
        }
    }

    void AddHexagonToDestroyList (GameObject hexagon) {
        if (!destroyList.Contains (hexagon)) {
            destroyList.Add (hexagon);
        }
    }

    // Destroy all the hexagons that are meant to be destroyed, in
    void DestroyMatches () {
        for (int i = 0; i < destroyList.Count; i++) {
            //destroyList.Remove (hexagon);
            //Destroy (destroyList[i]);
            destroyList[i].SetActive (false);
            scoreManager.AddPoints();
            scoreManager.UpdateScoreText();
        }

    }

    void FillEmptySpaces () {
        //TODO: Fill empty spaces from the destroyed gameobjects
        for (int i = 0; i < destroyList.Count; i++) {
            int newHexColor = Random.Range (0, colors.Length);
            destroyList[i].transform.GetChild(0).GetComponent<Hexagon> ().color = newHexColor;
            destroyList[i].transform.GetChild(0).GetComponent<Image> ().color = colors[newHexColor];
            destroyList[i].gameObject.SetActive (true);
        }
        destroyList = new List<GameObject> ();
    }

    public void TurnRight () {

    }

    public void TurnLeft () {

    }

    // First hit is the position of the gameObject
    // Second hit is the gameObject that is hit
    public void HandleClicks(string position, string gameObjectName){
        int[] rowCol = new int[2];
        // row col of currently clicked item.
        int row,col;
        GameObject hexagon = GameObject.Find(gameObjectName);
        rowCol = ParseHexagonNameToRowAndColumn(hexagon);
        if(rowCol[0] == -1){
            return;
        }
        row = rowCol[0];
        col = rowCol[1];
        //hexagon.transform.parent.gameObject.GetComponent<Image>().enabled = true;
        if(position == "TopLeft"){
            if(col%2 != 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row][col-1].GetComponent<Image>().enabled = true;
                list[row-1][col].GetComponent<Image>().enabled = true;
            }else if (col%2 ==0){
                list[row-1][col].GetComponent<Image>().enabled = true;
                list[row][col].GetComponent<Image>().enabled = true;
                list[row-1][col-1].GetComponent<Image>().enabled = true;
            }
        }

        if(position =="TopRight"){
            if(col%2 != 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row][col+1].GetComponent<Image>().enabled = true;
                list[row-1][col].GetComponent<Image>().enabled = true;
            }else if (col%2 ==0){
                list[row-1][col].GetComponent<Image>().enabled = true;
                list[row-1][col+1].GetComponent<Image>().enabled = true;
                list[row][col].GetComponent<Image>().enabled = true;
            }
        }

        if(position == "Left"){
            if(col%2 != 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row][col-1].GetComponent<Image>().enabled = true;
                list[row+1][col-1].GetComponent<Image>().enabled = true;
            }else if (col%2 == 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row][col-1].GetComponent<Image>().enabled = true;
                list[row-1][col-1].GetComponent<Image>().enabled = true;
            }
        }

        if(position == "Right"){
            if(col%2 != 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row][col+1].GetComponent<Image>().enabled = true;
                list[row+1][col+1].GetComponent<Image>().enabled = true;
            }else if (col%2 == 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row][col+1].GetComponent<Image>().enabled = true;
                list[row-1][col+1].GetComponent<Image>().enabled = true;
            }
        }

        if(position == "BottomLeft"){
            if(col%2 != 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row+1][col].GetComponent<Image>().enabled = true;
                list[row+1][col+1].GetComponent<Image>().enabled = true;
            }else if (col%2 == 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row][col-1].GetComponent<Image>().enabled = true;
                list[row+1][col].GetComponent<Image>().enabled = true;
            }
        }

        if(position == "BottomRight"){
            if(col%2 != 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row+1][col].GetComponent<Image>().enabled = true;
                list[row+1][col+1].GetComponent<Image>().enabled = true;
            }else if (col%2 == 0){
                list[row][col].GetComponent<Image>().enabled = true;
                list[row+1][col].GetComponent<Image>().enabled = true;
                list[row][col+1].GetComponent<Image>().enabled = true;
            }
        }

    }

    void GenerateColors () {
        colors[0] = new Color32(200, 176, 41, 255);
        colors[1] = new Color32(122, 114, 229, 255);
        colors[2] = new Color32(20, 94, 152, 255);
        colors[3] = new Color32(171, 87, 151, 255);
        colors[4] = new Color32(218, 208, 192, 255);
        colors[5] = new Color32(43, 42, 92, 255);
    }

    int[] ParseHexagonNameToRowAndColumn(GameObject hexagon){
        int[] rowCol = new int[2];
        if(hexagon.transform.name.Contains(" ")){
            string[] hexRowCol = hexagon.transform.name.Split (' ');
            rowCol[0] = Int32.Parse(hexRowCol[0]);
            rowCol[1] = Int32.Parse(hexRowCol[1]);
            return rowCol;
        }
        rowCol[0] = -1;
        rowCol[1] = -1;
        return rowCol;
    }

}