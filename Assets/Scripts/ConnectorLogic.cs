/*Connector Logic*/
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ConnectorLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler {

    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;


    public Connector connector;
    public List<GameObject> connectedDotsList;
    

    private bool isPressed = true;
    private bool isSquare = false;
    private int connectDotsListType;    
    private Dots currentDot;
    private Grid grid;
    
    

    public int[] dotsRemovedInColumn;

    private void Awake() {
        connector = GameObject.Find("Connector").GetComponent<Connector>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    private void Start() {
        //Fetch the Raycaster from the GameObject (the Canvas)
        raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        eventSystem = GetComponent<EventSystem>();

        currentDot = null;
    }

   public void OnPointerDown(PointerEventData eventData) {
        pointerEventData = new PointerEventData(eventSystem);
        //Set the Pointer Event Position to that of the mouse position
        pointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        raycaster.Raycast(pointerEventData, results);

        connector.lineRenderer.positionCount = 2;

        if (results[0].gameObject.tag == "Dot") {
            if (results[0].gameObject.GetComponent<Dots>().isAlreadyConnected == false) { 
                
                isPressed = true;

                /* set the found dot as current
                 * Then set its initial parameters, that is connected
                 * Add it to the connected list and set the connected list type the same as dot's type
                 */ 
                currentDot = results[0].gameObject.GetComponent<Dots>();               
                currentDot.isAlreadyConnected = true;

                connectedDotsList.Add(currentDot.gameObject);

                connectDotsListType = currentDot.DotType;

                //draw the line by setting the starting point as the dot's position

                connector.SetStartingPoint(currentDot.transform.position);

                connector.TotalDotsConnected++;
                connector.ChangeColor(currentDot.image_dot.color);

            }
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        if (isPressed) {
            
            currentDot = null;

            if(connectedDotsList.Count < 2) {                

                //we need to set it again to false, else the dot is marked which will prevent connection 
                foreach(GameObject g in connectedDotsList) {
                    g.GetComponent<Dots>().isAlreadyConnected = false;
                }

                connectedDotsList.Clear();
                connector.TotalDotsConnected = 0;
                connector.ChangeColor(Color.white);
                isPressed = false;
                return;
            }       

            CheckStatus();

            connectedDotsList.Clear();
            connector.TotalDotsConnected = 0;
            connector.lineRenderer.positionCount = 2;

            //we don't want to leave a trace of the line renderer when the dots are done
            connector.ChangeColor(Color.white);
            connector.Reset();            
            isPressed = false;
            isSquare = false;

        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (isPressed) {
            pointerEventData = new PointerEventData(eventSystem);
            //Set the Pointer Event Position to that of the mouse position
            pointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();
            //Debug.Log(results.Capacity);

            //Raycast using the Graphics Raycaster and mouse click position
            raycaster.Raycast(pointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            //Debug.Log(results[0].gameObject.tag);

            //are you a 'dot'?
            if (currentDot != null) {
                if (results[0].gameObject.tag == "Dot") {
                    Dots temp = results[0].gameObject.GetComponent<Dots>();

                    //are you a similar type of dot? and hope you ain't connected dude
                    if (temp.DotType == currentDot.DotType && temp.isAlreadyConnected == false) {

                        //are you a neighbor of current dot's north/south/east/west
                        if (temp.IsNeighbor(currentDot)) {

                            //you are now an eligible dot and the new current dot                        
                            currentDot = temp;

                            //you are now connected.
                            currentDot.isAlreadyConnected = true;

                            connectedDotsList.Add(temp.gameObject);

                            connector.AddNewPoint(currentDot.transform.position);
                            connector.KeepDrawing(currentDot.transform.position);

                            /*KeepDrawing() is called from Update() as well as from this function.
                                * Calling it in Update() gives us the free flowing line we need to connect.
                                * But, the call from Update() will always override the call from this function.
                                * So, once we have found a rough point on the dot, we go to the next dot. 
                                * Meanwhile, once on the next dot we fix the position of the line of the previous dot.
                                * 
                                * This can be avoided, if we have a list of linerenderers with us, but they suck!
                                */
                            if (connector.TotalDotsConnected > 1) {
                                connector.lineRenderer.SetPosition(connector.TotalDotsConnected - 1,
                                    connectedDotsList[connector.TotalDotsConnected - 1].transform.position);
                            }
                        }
                    }
  
                    //just for some better visibility                    
                    if (connectedDotsList.Count() < 3)
                        return;
                    
                    if (!(temp.DotType == currentDot.DotType && temp.isAlreadyConnected == true)) 
                        return;

                    if ((temp == currentDot)) 
                        return;

                    if (!currentDot.IsNeighbor(temp))
                        return;

                    if (temp.RowNumber != connectedDotsList[connectedDotsList.Count - 3].GetComponent<Dots>().RowNumber)
                        return;

                    if (temp.ColumnNumber != connectedDotsList[connectedDotsList.Count - 4].GetComponent<Dots>().ColumnNumber)
                        return;

                    if (!(isSquare == false)) {
                        return;

                    connector.AddNewPoint(currentDot.transform.position);
                    connector.KeepDrawing(currentDot.transform.position);

                    currentDot = temp;

                    currentDot.isAlreadyConnected = true;

                    isSquare = true;
                                            
                    }
                }
            }
        }
    }


    public void CheckStatus() {

        dotsRemovedInColumn = new int[6];

        if (isSquare) {
            connectedDotsList.Clear();
            foreach (var d in grid.allDots) {
                if (d.DotType == connectDotsListType){
                    connectedDotsList.Add(d.gameObject);
                    d.isAlreadyConnected = true;
                }
            }
            isSquare = false;
        }

        foreach (GameObject g in connectedDotsList) {
            dotsRemovedInColumn[g.GetComponent<Dots>().ColumnNumber]++;
        }
       

        for (int i = 0; i < grid.columnSize; i++) {
            //if there has been no change in dots in that specific column, we ignore that column
            if (dotsRemovedInColumn[i] == 0) {                
                continue;
            }

            GetSpecificDots(i, (dot) => {
                //we do not need to calclulate under dots for first row
                if (dot.RowNumber != 0 && !dot.isAlreadyConnected) {
                    int difference = GetConnectedDotsUnderConnectedDot(dot);
                
                    dot.SetNewCoordinates((dot.RowNumber - difference), i);
                  
                    dot.gameObject.name = "Dot: " + dot.RowNumber + " " + dot.ColumnNumber;

                    Vector3 targetPosition = new Vector3(dot.ColumnNumber * grid.seperationX -2f,
                       dot.RowNumber * grid.seperationY - 3.5f, 0);
                   
                    StartCoroutine(dot.AnimatePosition(dot.transform.position,targetPosition, 0.1f));
                }
            });               
        }


        for(int i = 0; i < 6; i++) {

            int removedCount = dotsRemovedInColumn[i];

            for(int j = 0; j < removedCount; j++) {

                int row = grid.rowSize - (removedCount - j);
                int lastRowIndex = connectedDotsList.Count - 1;
                Dots dot = connectedDotsList[lastRowIndex].GetComponent<Dots>();

                connectedDotsList.RemoveAt(lastRowIndex);

                dot.RecycleDot(row, i, grid.seperationX, grid.seperationY);
                dot.ResetDot(row, i);

                StartCoroutine(dot.AnimatePosition(dot.transform.position,
                    new Vector3(dot.ColumnNumber * grid.seperationX - 2f,
                        dot.RowNumber * grid.seperationY - 3.5f, 0), 0.1f));                
            }
        }
        connectedDotsList.Clear();
    }

    delegate void DotTask(Dots dot);

    //calls all the dots
    private void GetSpecificDots(DotTask callback) {
        foreach (var d in grid.allDots) {
            callback(d);
        }
    }
    
    //calls the dots with said column number
    private void GetSpecificDots(int column, DotTask callback) {
        foreach (var d in grid.allDots) {
            if (d.ColumnNumber == column) {
                callback(d);                
            }
        }
    }

    //returns the number of dots connected below the said dot. 
    //This will essentially help us find how may rows will the above dots fall
    private int GetConnectedDotsUnderConnectedDot(Dots dot) {
        int count = 0;
        GetSpecificDots(dot.ColumnNumber, (otherDot) => {
            if (otherDot.isAlreadyConnected == true && otherDot.RowNumber < dot.RowNumber) {
                count++;
            }
        });
        return count;
    }
}