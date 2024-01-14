using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYoffset = 0.06f;
    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer CellIndicatorRenderer;
    private void Start() {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        CellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size){
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if(size.x>0 || size.y>0){
            cellIndicator.transform.localScale = new Vector3(size.x,1,size.y);
            CellIndicatorRenderer.material.mainTextureScale = size;    
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers){
            Material[] materials = renderer.materials;
            for(int i=0; i<materials.Length; i++){
                materials[i]=previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview(){
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity){
        MovePreview(position);
        MoveCursor(position);
        ApplyFeedbacl(validity);
    }

    private void ApplyFeedbacl(bool validity)
    {
        Color c = validity ? Color.white:Color.red;
        CellIndicatorRenderer.material.color = c;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x,position.y+ previewYoffset,position.z);
    }
}
