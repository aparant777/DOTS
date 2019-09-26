using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dots : MonoBehaviour {
       
    private Image image_background;

    [SerializeField]
    private int rowNumber;

    [SerializeField]
    private int columnNumber;

    public int dotType;
    public Color[] colorTypes;
    public Image image_dot;
    public bool isAlreadyConnected;

    private void Start() {
        DotType = Random.Range(0, 5);
        image_dot.color = colorTypes[dotType];
        isAlreadyConnected = false;        
    }
   
    public void SetNewCoordinates(int iRowNumber, int iColumnNumber) {
        RowNumber = iRowNumber;
        ColumnNumber = iColumnNumber;
    }

    //properties
    #region Properties
    public int DotType {
        get {
            return dotType;
        }
        set {
            dotType = value;
        }
    }

    public int RowNumber {
        get {
            return rowNumber;
        }
        set {
            rowNumber = value;
        }
    }


    public int ColumnNumber {
        get {
            return columnNumber;
        }
        set {
            columnNumber = value;
        }
    }
    #endregion Properties
  
    //is that dot your neighbor?
    public bool IsNeighbor(Dots iDot) {
        if(this == iDot || iDot.dotType != dotType) {
            return false;
        } else {
            int rowDifference = Mathf.Abs(iDot.RowNumber - RowNumber);
            int columnDifference = Mathf.Abs(iDot.ColumnNumber - ColumnNumber);

            if(columnDifference > 0 && rowDifference > 0) {
                return false;
            }
            if(columnDifference > 1 || rowDifference > 1) {
                return false;
            }
            return true;
        }
    }

    /*Resetting and recycling dots allows us to keep the same amount of dots
     * on board without destroying them again.
     * Dots are only spawned during the start of the game. 
     * This eliminates 
     * 1. constant instantiate-destory cycles and 
     * 2. pooling
     */ 

    //this will be sent for recycling
    public void ResetDot(int iRowNumber, int iColumnNumber) {
        RowNumber = iRowNumber;
        ColumnNumber = iColumnNumber;

        DotType = Random.Range(0, 5);
        image_dot.color = colorTypes[DotType];
        isAlreadyConnected = false;
    }

    //change the position according to dot's row and column number
    public void RecycleDot(int iRowNumber, int iColumnNumber, float seperationX, float seperationY) {                
        transform.position = new Vector3(iColumnNumber * seperationX + -2f, iRowNumber * seperationY + 10, 0);
    }
   
    //asynchronous coroutine sets 
    public IEnumerator AnimatePosition(Vector3 iOrigin, Vector3 iTarget, float iDuration) {
        float journey = 0f;
        while (journey <= iDuration) {
            journey += Time.deltaTime;
            float percent = Mathf.Clamp01(journey / iDuration);

            transform.position = Vector3.Lerp(iOrigin, iTarget, percent);
            yield return null;
        }
        StartCoroutine(FollowUpAfterRecycle(gameObject.transform.position, gameObject.transform.position + new Vector3(0, 0.1f, 0), 0.1f));
    }
    private IEnumerator FollowUpAfterRecycle(Vector3 iOrigin, Vector3 iTarget, float iDuration) {
        float journey = 0f;
        while (journey <= iDuration) {
            journey += Time.deltaTime;
            float percent = Mathf.Clamp01(journey / iDuration);

            transform.position = Vector3.Lerp(iOrigin, iTarget, percent);
            yield return null;
        }
        StartCoroutine(FinalAnim(gameObject.transform.position, gameObject.transform.position + new Vector3(0, -0.2f, 0), 0.05f));
    }
    private IEnumerator FinalAnim(Vector3 iOrigin, Vector3 iTarget, float iDuration) {
        float journey = 0f;
        while (journey <= iDuration) {
            journey += Time.deltaTime;
            float percent = Mathf.Clamp01(journey / iDuration);

            transform.position = Vector3.Lerp(iOrigin, iTarget, percent);
            yield return null;
        }

    }
}