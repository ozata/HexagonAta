using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public GameObject hexagonPrefab;

    Transform canvasTransform;

    [SerializeField] int[, ] Grid;
    [SerializeField] int row, column;

    // 1/10 of width and height to initialize the hexagons.
    float widthPiece;
    float heightPiece;

    void Start () {
        canvasTransform = GameObject.FindGameObjectWithTag ("Canvas").transform;
        widthPiece = (float) Screen.width / 10;
        heightPiece = (float) Screen.height / 10;

        GenerateGrid ();
    }

    void Update () {

    }

    void GenerateGrid () {
        float height = (float) Math.Floor (heightPiece) * 4;
        float width = (float) Math.Floor (widthPiece) * -4;
        print (width);
        print (height);
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                GameObject hexagon = Instantiate (hexagonPrefab, new Vector3 (height, width, 0), Quaternion.identity) as GameObject;
                hexagon.transform.SetParent (canvasTransform, false);
                width = width + 100;
            }
            height = height - 100;
        }
    }
}