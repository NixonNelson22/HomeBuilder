using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private objectPool objectPool;
    private int SelectedObjectIndex = -1;
    [SerializeField]
    private GameObject gridVisualization;

    private void Start() {
        StopPlacement();
    }
    public void StartPlacement(int ID){
        StopPlacement();
        SelectedObjectIndex = objectPool.objectsData.FindIndex(data =>data.ID == ID);
        if(SelectedObjectIndex < 0){
            Debug.LogError($"No ID found{ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI()){
            return;
        }
        Vector3 mousePosition = inputManager.getSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        GameObject newObject = Instantiate(objectPool.objectsData[SelectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition); 
    }

    public void StopPlacement(){
        SelectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }
    private void Update() {
        if(SelectedObjectIndex < 0){
            return;
        }
        Vector3 mousePosition = inputManager.getSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;       
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);    
    }
}
