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
    // Hex objects that'll be destroyed.
    List<GameObject> destroyList;

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
        CheckMatches ();

        PrintList ();
        print ("LİSTE 22: " + list[2][2].GetComponent<Hexagon> ().color);
    }

    void PrintList () {
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                print ("List:" + list[i][j]);
            }
        }
    }

    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            Clicked ();
        }

    }

    void CheckMatches () {
        for (int i = 1; i < row - 1; i++) {
            for (int j = 1; j < column - 1; j++) {
                print ("girdi");
                string name = i.ToString () + " " + j.ToString ();
                print ("İsim: " + name);
                GameObject hex = GameObject.Find (name.ToString ());
                CheckTrioMatch (hex);
            }
        }
    }

    // Creates a random integer array that'll be used for color.
    void CreateRandomColor () {
        Random.InitState (System.DateTime.Now.Millisecond);
        for (int i = 0; i < color.Length; i++) {
            color[i] = Random.Range (0, colors.Length);
        }

        // For each spot in the array, pick
        // a random item to swap into that spot.
        /*
        for (int i = 0; i < color.Length - 1; i++) {
            int j = Random.Range (i, color.Length);
            int temp = color[i];
            color[i] = color[j];
            color[j] = temp;

            if (color[i] == color[i + 1] && color[i + 1] + 1 < colors.Length) {
                color[i + 1] = 4;
                print (true);
            }
        }*/
    }

    void GenerateGridOnScreen () {
        // if counter is even, height will be lower.
        int counter = 0;
        int id = 1;

        float height = (float) Math.Floor (heightPiece) * 2;
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
                print ("i: " + i + "j: " + j + " " + hexagon.GetComponent<Hexagon> ().color);

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
                print (hexagon);
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
        print (hexRow + " " + hexColumn + " " + "Color: " + list[hexRow][hexColumn].GetComponent<Hexagon> ().color);
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color) {

            print ("Destroyed Hexagon: " + " " + hexRow + " " + hexColumn + " " + list[hexRow][hexColumn]);

        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color) {
            print ("Destroyed Hexagon: " + " " + hexRow + " " + hexColumn + " " + list[hexRow][hexColumn]);
            Destroy (list[hexRow][hexColumn]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow - 1][hexColumn + 1].GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color) {
            print ("Destroyed Hexagon: " + " " + hexRow + " " + hexColumn + " " + list[hexRow][hexColumn]);
            Destroy (list[hexRow][hexColumn]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn + 1].GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color) {
            print ("Destroyed Hexagon: " + " " + hexRow + " " + hexColumn + " " + list[hexRow][hexColumn]);
            Destroy (list[hexRow][hexColumn]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow + 1][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color) {
            print ("Destroyed Hexagon: " + " " + hexRow + " " + hexColumn + " " + list[hexRow][hexColumn]);
            Destroy (list[hexRow][hexColumn]);
        }
        if (list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color &&
            list[hexRow][hexColumn - 1].GetComponent<Hexagon> ().color == list[hexRow - 1][hexColumn - 1].GetComponent<Hexagon> ().color) {
            print ("Destroyed Hexagon: " + " " + hexRow + " " + hexColumn + " " + list[hexRow][hexColumn]);
            Destroy (list[hexRow][hexColumn]);
        }
    }

    void DestroyMatches () {

    }

    void TurnRight () {

    }

    void TurnLeft () {

    }

    void Clicked () {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2 mousePos2D = new Vector2 (mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast (mousePos2D, Vector2.zero);
        if (hit.collider != null) {
            Debug.Log (hit.collider.gameObject.name);
        }
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