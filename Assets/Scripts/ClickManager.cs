using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        void Update () {
            if (Input.GetMouseButtonDown (0)) {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
                if (hit) {
                    Debug.Log (hit.collider.gameObject.name);
                }
            }
        }
    }

    public void Clicked () {
        print (this.gameObject.name);
    }
}