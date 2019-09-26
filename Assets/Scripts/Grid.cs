using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    public int rowSize { get; private set; }
    public int columnSize { get; private set; }

    public float seperationX;
    public float seperationY;

    public GameObject prefab;
    public GameObject[,] grid;
    public Transform canvasreference;

    public List<Dots> allDots;

    private float delayBetweenSpawns = 0.2f;

    private void Start() {
        rowSize = 6;
        columnSize = 6;

        grid = new GameObject[rowSize, columnSize];
        StartCoroutine(SpawnGrid());
    }    

    IEnumerator SpawnGrid() {
        yield return new WaitForSeconds(0.5f);

        for(int i = 0; i < 6; i++) {
            for(int j = 0; j < 6;  j++) {
                StartCoroutine(SpawnDotCoroutine(i, j));
            }
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
        
    }

    IEnumerator SpawnDotCoroutine(int iRowNumber, int iColumnNumber) {
        //TODO: Remove the hard code values later
        grid[iRowNumber, iColumnNumber] = Instantiate(prefab, new Vector3(iColumnNumber * seperationX + -2f, 10f, 0), Quaternion.identity, canvasreference);

        Dots temp = grid[iRowNumber, iColumnNumber].GetComponent<Dots>();
        temp.gameObject.name = "Dot: " + iRowNumber + " " + iColumnNumber;

        StartCoroutine(temp.AnimatePosition(
            new Vector3(iColumnNumber * seperationX + -2f, 10f, 0f),
            new Vector3(iColumnNumber * seperationX + -2f, iRowNumber * seperationY - 3.5f, 0f), 0.3f));

        allDots.Add(temp);

        temp.RowNumber = iRowNumber;
        temp.ColumnNumber = iColumnNumber;

        yield return null;
    }
}
