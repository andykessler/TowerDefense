﻿using UnityEngine;
using System.Collections;

public class BuildManager
{
    private readonly static float PREVIEW_OPACITY = 0.15f;
    private readonly static Color PREVIEW_RED = new Color(1.0f, 0.0f, 0.0f, PREVIEW_OPACITY);
    private readonly static Color PREVIEW_GREEN = new Color(0.0f, 1.0f, 0.0f, PREVIEW_OPACITY);
    private readonly static Vector3 PREVIEW_OFFSET = new Vector3(0.0f, 0.69f, 0.0f);

    GridManager gridManager;

    bool buildMode;

    bool buildMenu;

    BuildingType currentBuildingType;
    GameObject selectedObject;


    public BuildManager(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    public void update()
    {
        updateBuildingPreview();
        handleInput();

    }

    void handleInput()
    {

        if (buildMenu) {
            if (Input.GetKeyUp(KeyCode.Alpha1)) {
                selectBuildingType(BuildingType.Wall1x1);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2)) {
                selectBuildingType(BuildingType.Wall2x2);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3)) {
                selectBuildingType(BuildingType.Wall2x1);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4)) {
                selectBuildingType(BuildingType.Wall1x2);
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            if(buildAtMouse())
            {
                exitBuildMode();
            }
        }
        else if (Input.GetMouseButtonUp(1)) {
            buildMenu = true;
        }
    }

    void updateBuildingPreview()
    {
        if (!buildMode || buildMenu) return;

        Color previewColor;
        float distance = 0;
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Building building = BuildingFactory.createBuilding(currentBuildingType);
            if (selectedObject == null)
            {
                selectedObject = Object.Instantiate(building.prefab, PREVIEW_OFFSET, Quaternion.identity) as GameObject;
                Object.Destroy(selectedObject.GetComponent<NavMeshObstacle>());
            }

            Vector3 intersect = ray.GetPoint(distance);
            int row = Mathf.FloorToInt(intersect.x);
            int col = Mathf.FloorToInt(intersect.z);
            if (gridManager.canBuildAt(building, row, col))
            {
                previewColor = PREVIEW_GREEN;
            }
            else
            {
                previewColor = PREVIEW_RED;
            }

            Renderer renderer = selectedObject.GetComponent<Renderer>();
            renderer.material.color = previewColor;

            Vector3 center = gridManager.getCenterAt2(building, row, col);
            if (center != Vector3.zero)
            {
                selectedObject.transform.position = center + PREVIEW_OFFSET;
            }
        }
    }

    public void enterBuildMode()
    {
        buildMode = true;
        buildMenu = true;
    }

    public void exitBuildMode()
    {
        buildMode = false;
        buildMenu = false;
        Object.Destroy(selectedObject);
    }

    public bool inBuildMode()
    {
        return buildMode;
    }

    void selectBuildingType(BuildingType type)
    {
        currentBuildingType = type;
        buildMenu = false;
    }

    bool buildAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance = 0;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 intersect = ray.GetPoint(distance);
            int row = Mathf.FloorToInt(intersect.x);
            int col = Mathf.FloorToInt(intersect.z);
            return gridManager.createObjectAt(BuildingFactory.createBuilding(currentBuildingType), row, col);
        }
        return false;
    }
}