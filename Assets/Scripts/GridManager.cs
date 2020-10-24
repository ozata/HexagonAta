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

    [SerializeField] int[, ] Grid;
    [SerializeField] int row, column;

    List<List<Hexagon>> list = new List<List<Hexagon>> ();
    List<Hexagon> sublist = new List<Hexagon> ();

    // 1/10 of width and height to initialize the hexagons.
    float widthPiece;
    float heightPiece;

    void Start () {
        GenerateColors ();
        canvasTransform = GameObject.FindGameObjectWithTag ("Canvas").transform;
        widthPiece = (float) Screen.width / 10;
        heightPiece = (float) Screen.height / 10;

        CreateGridList ();
        GenerateGridOnScreen ();
    }

    void Update () {

    }

    void CheckMatch (Hexagon hexagon) {

    }

    void CreateGridList () {
        int id = 1;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                Hexagon hex = new Hexagon (id, Color.white, 0, 0);
                sublist.Add (hex);
                id++;
            }
            list.Add (sublist);
        };
    }

    void GenerateGridOnScreen () {
        // if counter is even, height will be lower.
        int counter = 0;

        float height = (float) Math.Floor (heightPiece) * 2;
        float widthInit = (float) Math.Floor (widthPiece) * -4;
        float width = widthInit;
        float addition = hexagonSprite.rect.height - 30;

        print ("Addition: " + addition);
        print ("Height " + height);
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                GameObject hexagon = Instantiate (hexagonPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
                hexagon.transform.SetParent (canvasTransform, false);
                hexagon.transform.localPosition = new Vector3 (width, height, 0);
                hexagon.GetComponent<Image> ().color = colors[Random.Range (0, colors.Length)];
                hexagon.name = list[i][j].id.ToString ();

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
            }
            height = height - (addition);
            width = widthInit;
        }
    }

    void GenerateColors () {
        colors[0] = Color.cyan;
        colors[1] = Color.red;
        colors[2] = Color.green;
        colors[3] = Color.blue;
        colors[4] = Color.yellow;
        colors[5] = Color.magenta;
    }
}