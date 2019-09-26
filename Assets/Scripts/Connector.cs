using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Connector : MonoBehaviour {
   
    public int totalDotsConnected;
    public LineRenderer lineRenderer;
    public ConnectorLogic connectorLogic;

    private Vector3 startPosition;
    private Vector3 endPosition;



    #region Properties
    public int TotalDotsConnected {
        get {
            return totalDotsConnected;
        }
        set {
            totalDotsConnected = value;
        }
    }
    #endregion Properties

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();       
    }

    private void Start() {
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));        
        lineRenderer.material.color = Color.white;
    }

    public void SetStartingPoint(Vector3 iDotPosition) {
        startPosition = iDotPosition;
        lineRenderer.SetPosition(TotalDotsConnected, iDotPosition);
    }

    public void KeepDrawing(Vector3 iWorldPosition) {
        lineRenderer.SetPosition(TotalDotsConnected, iWorldPosition);
    }

    public void AddNewPoint(Vector3 iDotPosition) {
        TotalDotsConnected++;
        lineRenderer.positionCount = TotalDotsConnected+1;
        lineRenderer.SetPosition(TotalDotsConnected, iDotPosition);
    }

    private void Update() {

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.back * -10.0f;       

        if (Input.GetMouseButton(0)) {
            //if(isDrawing)
            KeepDrawing(worldPosition);
        }
    }

    public void ResetLineConnection() {
        TotalDotsConnected = 0; ;
    }

    public void ChangeColor(Color iColor) {
        lineRenderer.startColor = iColor;
        lineRenderer.endColor = iColor;
    }

    public void Reset() {
        ChangeColor(Color.white); 
        lineRenderer.SetPosition(0,new Vector3(0,0,-10));
        lineRenderer.SetPosition(1,new Vector3(0,0,-10));
    }
}
