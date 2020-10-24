using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon {

    public int id;
    public int color;
    // Count if the hexagon type is bomb
    public int count;
    // TODO: Change this to ENUM if time.
    // type: 1 = normal, 2 = bomb
    public int type;

    public Hexagon (int id, int color, int count, int type) {
        this.id = id;
        this.color = color;
        this.count = count;
        this.type = type;
    }
}