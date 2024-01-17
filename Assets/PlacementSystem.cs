using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private objectPool objectPool;
    private int SelectedObjectIndex = -1;
    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData, furnitureData;
    
    private List<GameObject> placedGameObject= new();
    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private void Start() {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }
    public void StartPlacement(int ID){
        StopPlacement();
        SelectedObjectIndex = objectPool.objectsData.FindIndex(data =>data.ID == ID);
        if(SelectedObjectIndex < 0){
            Debug.LogError($"No ID found{ID}");
            return;
        }
        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(objectPool.objectsData[SelectedObjectIndex].Prefab,objectPool.objectsData[SelectedObjectIndex].Size);
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
        bool placementValidity = CheckPlacementValidity(gridPosition,SelectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }
        GameObject newObject = Instantiate(objectPool.objectsData[SelectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition); 
        newObject.transform.rotation =Quaternion.AngleAxis(90f,Vector3.up);
        placedGameObject.Add(newObject);
        GridData selectedData = objectPool.objectsData[SelectedObjectIndex].ID == 0 ? floorData: furnitureData;
        selectedData.AddObjectAt(gridPosition,objectPool.objectsData[SelectedObjectIndex].Size,objectPool.objectsData[SelectedObjectIndex].ID,placedGameObject.Count -1);
    }

    public void StopPlacement(){
        SelectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }
    private void Update() {
        if(SelectedObjectIndex < 0){
            return;
        }
        Vector3 mousePosition = inputManager.getSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition,SelectedObjectIndex);
            mouseIndicator.transform.position = mousePosition;       
            preview.UpdatePosition(grid.CellToWorld(gridPosition),true);
            lastDetectedPosition = gridPosition;    
        }
        
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = objectPool.objectsData[selectedObjectIndex].ID == 0 ? floorData: furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition,objectPool.objectsData[selectedObjectIndex].Size);
    }
}
