using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    public int id;
    public Color color;
    // for bomb hexagons
    public int count;
    public int type;

    public Hexagon (int id, Color color, int count, int type) {
        this.id = 1;
        this.color = color;
        this.count = count;
        this.type = type;
    }

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }
}