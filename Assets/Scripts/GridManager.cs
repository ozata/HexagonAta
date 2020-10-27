using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {

    public Color[] colors = new Color[6];
    public GameObject hexagonPrefab;
    public Sprite hexagonSprite;

    Transform canvasTransform;

    public int row, column;
    int[] color;

    // Data Structure that will hold the Hexagon List
    List<List<GameObject>> list = new List<List<GameObject>> ();
    // One row of hexagons
    List<GameObject> sublist;
    // Hex objects that'll be destroyed, this list will be unique
    // TODO: Change to a different data structure as lists are not best ones to hold unique objects
    List<GameObject> destroyList = new List<GameObject> ();

    // 1/10 of width and height to initialize the hexagons on the Canvas.
    float widthPiece;
    float heightPiece;

    void Start () {
        GenerateColors ();

        color = new int[row * column];
        canvasTransform = GameObject.FindGameObjectWithTag ("Canvas").transform;
        widthPiece = (float) Screen.width / 10;
        heightPiece = (float) Screen.height / 10;
        CreateRandomColor ();
        GenerateGridOnScreen ();
    }

    void Update () {
        CheckMatches ();
        DestroyMatches ();
        FillEmptySpaces ();
    }

    void CheckMatches () {
        for (int i = 1; i < row - 1; i++) {
            for (int j = 1; j < column - 1; j++) {
                string name = i.ToString () + " " + j.ToString ();
                GameObject hex = GameObject.Find (name.ToString ());
                CheckTrioMatch (hex);
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
                hexagon.GetComponent<Image> ().color = colors[color[i + j]];
                hexagon.GetComponent<Hexagon> ().color = color[i + j];
                hexagon.name = i.ToString () + " " + j.ToString ();
                if (counter % 2 == 0) {
                    height -= (float) addition / (float) 2.0;
                } else {
                    height += (float) addition / (float) 2.0;
                }
                width += (((float) 3.0 / 4) * addition);
                counter++;
                if (counter == column && column % 2 != 0) {
                    height += (float) addition / (float) 2.0;
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
        string[] hexRowCol = hexagon.name.Split (' ');
        int hexRow = Int32.Parse (hexRowCol[0]);
        int hexColumn = Int32.Parse (hexRowCol[1]);
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn + 1]);
            AddHexagonToDestroyList (list[hexRow][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow + 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow][hexColumn + 1]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow + 1][hexColumn]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color) {
            AddHexagonToDestroyList (list[hexRow][hexColumn]);
            AddHexagonToDestroyList (list[hexRow][hexColumn - 1]);
            AddHexagonToDestroyList (list[hexRow - 1][hexColumn - 1]);
        }
    }

    void AddHexagonToDestroyList (GameObject hexagon) {
        if (!destroyList.Contains (hexagon)) {
            print ("Hexagon Added: " + hexagon);
            destroyList.Add (hexagon);
        }
    }

    // Destroy all the hexagons that are meant to be destroyed, in
    void DestroyMatches () {
        for (int i = 0; i < destroyList.Count; i++) {
            //destroyList.Remove (hexagon);
            //Destroy (destroyList[i]);
            destroyList[i].SetActive (false);
            print (destroyList[i]);
        }

    }

    void FillEmptySpaces () {
        //TODO: Fill empty spaces from the destroyed gameobjects
        for (int i = 0; i < destroyList.Count; i++) {
            int newHexColor = Random.Range (0, colors.Length);
            destroyList[i].GetComponent<Hexagon> ().color = newHexColor;
            destroyList[i].GetComponent<Image> ().color = colors[newHexColor];
            destroyList[i].SetActive (true);
            print ("Filled!!!!");
        }
        destroyList = new List<GameObject> ();
    }

    public void TurnRight () {

    }

    public void TurnLeft () {

    }

    void GenerateColors () {
        colors[0] = Color.cyan;
        colors[1] = Color.red;
        colors[2] = Color.green;
        colors[3] = Color.blue;
        colors[4] = Color.yellow;
        colors[5] = new Color (0.3f, 0.4f, 0.6f, 1f);
    }

}