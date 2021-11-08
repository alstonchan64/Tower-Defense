using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    public Vector2 WorldPosition
    {
        get
        {
            return GetComponent<SpriteRenderer>().bounds.center;
        }
    }

    public bool IsEmpty { get; private set; }

    private Tower myTower;

    private Color32 fullColor = new Color32(255, 118, 118, 255);
    private Color32 emptyColor = new Color32(96, 255, 90, 255);

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        IsEmpty = true;
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        // Check to see if mouse is clicking a button or if player has clicked buy button
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
        {
            if (IsEmpty)
            {
                ColorTile(emptyColor);

                if (Input.GetMouseButtonDown(0))
                    PlaceTower();
            } 
            else
            {
                ColorTile(fullColor);
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn == null && Input.GetMouseButtonDown(0))
        {
            if (myTower != null)
            {
                GameManager.Instance.SelectTower(myTower);
            }
            else
            {
                GameManager.Instance.DeselectTower();
            }
        }
    }

    private void OnMouseExit()
    {
        ColorTile(Color.white);
    }

    private void PlaceTower()
    {
        if (GameManager.Instance.BuyTower())  // can only place tower if you can buy it first
        {
            // Debug.Log(GridPosition.X + ", " + GridPosition.Y);
            GameObject t = GameManager.Instance.ClickedBtn.TowerPrefab; 
            string dir = LevelManager.Instance.getDirection(GridPosition.X, GridPosition.Y);
            string ogSpriteName = t.GetComponent<SpriteRenderer>().sprite.name;
            string spriteBaseName = ogSpriteName.Substring(0, ogSpriteName.Length - 1);
            // Debug.Log(spriteBaseName + ", " + dir);
            // Debug.Log("Before: " + t);
            if (spriteBaseName == "Base") {
                if (dir == "up") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/BaseU");
                } else if (dir == "down") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/BaseD");
                } else if (dir == "left") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/BaseL");
                } else {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/BaseR");
                }
            } else if (spriteBaseName == "SBL") {
                if (dir == "up") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/SBLU");
                } else if (dir == "down") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/SBLD");
                } else if (dir == "left") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/SBLL");
                } else {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/SBLR");
                }
            } else if (spriteBaseName == "Wizard") {
                if (dir == "up") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/WizardU");
                } else if (dir == "down") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/WizardD");
                } else if (dir == "left") {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/WizardL");
                } else {
                    t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Towers/WizardR");
                }
            }
            // Debug.Log("After: " + t);
            GameObject tower = Instantiate(t, WorldPosition, Quaternion.identity);
            
            //tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y;

            tower.transform.SetParent(transform);

            this.myTower = tower.GetComponentInChildren<Tower>();

            IsEmpty = false;

            ColorTile(Color.white);

            Hover.Instance.Deactivate();
        }
        

        
    }

    private void ColorTile(Color newColor)
    {
        spriteRenderer.color = newColor;
    }
}
