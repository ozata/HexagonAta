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
    List<List<Hexagon>> list = new List<List<Hexagon>> ();
    List<Hexagon> sublist = new List<Hexagon> ();

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
        CreateGridList ();
        GenerateGridOnScreen ();

        //CheckMatch (HexCanvasToList (GameObject.Find ("1")));

    }

    void Update () {

    }

    void CreateRandomColor () {
        Random.InitState (System.DateTime.Now.Millisecond);
        for (int i = 0; i < color.Length; i++) {
            color[i] = Random.Range (0, colors.Length);
            print (color[i]);
        }
    }

    void CreateGridList () {
        int id = 1;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                Hexagon hex = new Hexagon (id, color[i + j], 0, 0);
                sublist.Add (hex);
                id++;
            }
            list.Add (sublist);
        };
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
            for (int j = 0; j < column; j++) {
                GameObject hexagon = Instantiate (hexagonPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
                hexagon.transform.SetParent (canvasTransform, false);
                hexagon.transform.localPosition = new Vector3 (width, height, 0);
                hexagon.GetComponent<Image> ().color = colors[color[i + j]];
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
            }
            height = height - (addition);
            width = widthInit;
        }
    }

    // Get Hex GameObject and find the hex on the list with the same id.
    Hexagon HexCanvasToList (GameObject hexagon) {
        int i, j, id = 1;
        for (i = 0; i < row; i++) {
            for (j = 0; j < column; j++) {
                if (string.Equals (id.ToString (), hexagon.name)) {
                    return list[i][j];
                }
            }
        }
        return null;
    }

    // There are 6 cases that will go down as a match and destroy the hexagon trio.
    // Check every hexagon in the game to find if there is a Match
    void CheckMatch (GameObject hexagon) {
        string[] hexRowCol = hexagon.name.Split (' ');
        int hexRow = Int32.Parse (hexRowCol[0]);
        int hexColumn = Int32.Parse (hexRowCol[1]);

        if (list[hexRow][hexColumn].color == list[hexRow - 1][hexColumn + 1].color &&
            list[hexRow][hexColumn].color == list[hexRow - 1][hexColumn - 1].color &&
            list[hexRow - 1][hexColumn + 1].color == list[hexRow - 1][hexColumn - 1].color) {
            Destroy (hexagon);
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