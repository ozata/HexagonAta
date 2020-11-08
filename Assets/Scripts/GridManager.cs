using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {

    const int SelectedHexCount = 3;
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
        // Reset- score as 0 because some hexagons will match at beginning and explode and add to score.
        if(initScore){
            CoreGameplay();
            scoreManager.SetScore(0);
            scoreManager.UpdateScoreText();
            initScore = false;
        }

    }

    bool CoreGameplay(){
        CheckMatches ();
        DestroyMatches ();
        bool match = FillEmptySpaces ();
        return match;
    }

    void SelectTrio(){
        for(int i = 0 ; i < selectedList.Count ; i++){
             selectedList[i].GetComponent<Image>().enabled = true;
             print(selectedList[i]);
        }
    }

    void DeselectTrio(){
        for(int i = 0 ; i < selectedList.Count ; i++){
            selectedList[i].GetComponent<Image>().enabled = false;
        }
        selectedList = new List<GameObject>();
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
        for (int i = 0; i < color.Length; i++) {
            Random.InitState (System.DateTime.Now.Millisecond);
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

    bool FillEmptySpaces () {
        //TODO: Fill empty spaces from the destroyed gameobjects
        bool match = false;
        for (int i = 0; i < destroyList.Count; i++) {
            int newHexColor = Random.Range (0, colors.Length);
            destroyList[i].transform.GetChild(0).GetComponent<Hexagon> ().color = newHexColor;
            destroyList[i].transform.GetChild(0).GetComponent<Image> ().color = colors[newHexColor];
            destroyList[i].gameObject.SetActive (true);
            match = true;
        }
        destroyList = new List<GameObject> ();
        return match;
    }

    public void TurnRight () {
        if(selectedList.Count == 0) return;
        // Core hexagon that is selected
        int[] hexRowCol = ParseHexagonNameToRowAndColumn(selectedList[0]);
        int hexRow = hexRowCol[0];
        int hexCol = hexRowCol[1];
        // Last element is the which position e.g. TopRight, Left, BottomRight
        //string position = selectedList[3].gameObject.name;

        GameObject copy1;
        GameObject copy2;
        GameObject copy3;
        // If column is even numbered
        if(hexCol % 2 == 0 ){

        } else if (hexCol % 2 != 0){
            for(int i = 0 ; i < SelectedHexCount; i++){
                copy1 = CreateNewHexagon(hexRow,hexCol+1,
                                        list[hexRow][hexCol+1].transform.localPosition.x,
                                        list[hexRow][hexCol+1].transform.localPosition.y,
                                        list[hexRow][hexCol].transform.GetChild(0).GetComponent<Hexagon>().color,
                                        list[hexRow][hexCol].transform.GetChild(0).GetComponent<Image>().color);
                copy2 = CreateNewHexagon(hexRow+1, hexCol+1,
                                        list[hexRow+1][hexCol+1].transform.localPosition.x,
                                        list[hexRow+1][hexCol+1].transform.localPosition.y,
                                        list[hexRow][hexCol+1].transform.GetChild(0).GetComponent<Hexagon>().color,
                                        list[hexRow][hexCol+1].transform.GetChild(0).GetComponent<Image>().color);
                copy3 = CreateNewHexagon(hexRow, hexCol,
                                        list[hexRow][hexCol].transform.localPosition.x,
                                        list[hexRow][hexCol].transform.localPosition.y,
                                        list[hexRow+1][hexCol+1].transform.GetChild(0).GetComponent<Hexagon>().color,
                                        list[hexRow+1][hexCol+1].transform.GetChild(0).GetComponent<Image>().color);

                list[hexRow][hexCol] = copy3;
                list[hexRow][hexCol+1] = copy1;
                list[hexRow+1][hexCol+1] = copy2;
                bool match = CoreGameplay();
                if(match){
                    return;
                }
            }
        }

    }

    public void TurnLeft () {
        print("Turn Left");
        if(selectedList.Count == 0){
            return;
        }
    }

    // First hit is the position of the gameObject
    // Second hit is the gameObject that is hit
    public void HandleClicks(string position, string gameObjectName){
        DeselectTrio();
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
        if(row == 0 && col == 0 || row == this.row-1 && col == this.column -1){
            return;
        }

        //hexagon.transform.parent.gameObject.GetComponent<Image>().enabled = true;
        if(position == "TopLeft"){
            if(col%2!=0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row][col-1]);
                selectedList.Add(list[row-1][col]);
            }else if (col%2 ==0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row-1][col]);
                selectedList.Add(list[row-1][col-1]);
            }
        }

        if(position =="TopRight"){
            if(col%2 != 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row][col+1]);
                selectedList.Add(list[row-1][col]);
            }else if (col%2 ==0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row-1][col+1]);
                selectedList.Add(list[row-1][col]);
            }
        }

        if(position == "Left"){
            if(col%2 != 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row][col-1]);
                selectedList.Add(list[row+1][col-1]);
            }else if (col%2 == 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row][col-1]);
                selectedList.Add(list[row-1][col-1]);
            }
        }

        if(position == "Right"){
            if(col%2 != 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row][col+1]);
                selectedList.Add(list[row+1][col+1]);
            }else if (col%2 == 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row][col+1]);
                selectedList.Add(list[row-1][col+1]);
            }
        }

        if(position == "BottomLeft"){
            if(col%2 != 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row+1][col-1]);
                selectedList.Add(list[row+1][col]);
            }else if (col%2 == 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row][col-1]);
                selectedList.Add(list[row+1][col]);
            }
        }

        if(position == "BottomRight"){
            if(col%2 != 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row+1][col]);
                selectedList.Add(list[row+1][col+1]);
            }else if (col%2 == 0){
                selectedList.Add(list[row][col]);
                selectedList.Add(list[row+1][col]);
                selectedList.Add(list[row][col+1]);
            }
        }

        SelectTrio();
    }

    void GenerateColors () {
        colors[0] = new Color32(200, 176, 41, 255);
        colors[1] = new Color32(122, 114, 229, 255);
        colors[2] = new Color32(20, 94, 152, 255);
        colors[3] = new Color32(171, 87, 151, 255);
        colors[4] = new Color32(218, 208, 192, 255);
        colors[5] = new Color32(43, 42, 92, 255);
    }

    GameObject CreateNewHexagon(int i, int j, float width, float height, int colorCode, Color color){
        GameObject hexagon = Instantiate (hexagonPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
        hexagon.transform.SetParent (canvasTransform, false);
        hexagon.transform.name = i.ToString () + " " + j.ToString ();
        hexagon.transform.GetChild(0).name = i.ToString () + " "  + j.ToString () + " child";
        hexagon.transform.localPosition = new Vector3 (width, height, 0);
        hexagon.transform.localPosition = new Vector3 (width, height, 0);
        hexagon.transform.GetChild(0).GetComponent<Hexagon> ().color = colorCode;
        hexagon.transform.GetChild(0).GetComponent<Image> ().color = color;
        return hexagon;
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