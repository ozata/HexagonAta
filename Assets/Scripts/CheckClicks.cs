using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckClicks : MonoBehaviour {
    // Normal raycasts do not work on UI elements, they require a special kind
    GraphicRaycaster raycaster;
    GridManager gridManager;

    // First and second clicked game objects
    // First hit will be the position that hexagon is hit.
    // Second hit is the hexagon that is hit.
    string[] hits;
    void Awake () {

        this.raycaster = GetComponent<GraphicRaycaster> ();
        this.gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        // Maximum hit could be 5 but only 2 of them is needed
        hits = new string[5];
    }

    void Update () {
        //Check if the left Mouse button is clicked
        HandleClicks();
    }

    void HandleClicks(){
        if (Input.GetMouseButtonDown (0)) {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData (EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult> ();

            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast (pointerData, results);
            // Iterator to fill the array
            int i = 0;
            // For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            // Assume there will be two hits
            foreach (RaycastResult result in results) {
                hits[i] = result.gameObject.name;
                i++;
            }
            if(results[0].gameObject){
                gridManager.HandleClicks(hits[0],hits[1]);
            }
        }
    }
}