using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI_FoodBar : MonoBehaviour {
    
    public Image foodSpriteTilePrefab;
    
    public float tileSeparation;

    public float tileStart;

    private float imageWidth;

    private float panelBottom;
    private float panelHeight;
    private float panelWidth;

    private Vector2 labelTextStart;

    private Image foodSpriteTile;

    private int tileElement;

    private RectTransform foodPanelRectTransform;

    private RectTransform foodTileRectTransform;

    private Text labelText;

    private RectTransform labelTextRectTransform;

    private Vector2 imageSize;

    private Vector3 offset;

    private Vector3 tilePosition;

    private List<Image> foodTileList = new List<Image>();

    void Start()
    {
        tileElement = 0;

        panelWidth = GetComponent<RectTransform>().rect.width;
        panelHeight = GetComponent<RectTransform>().rect.height;

        imageSize = new Vector2(panelHeight, panelHeight);

        labelText = GetComponentInChildren<Text>();
        labelTextRectTransform = labelText.GetComponent<RectTransform>();

        labelTextStart = new Vector2(labelTextRectTransform.position.x, labelTextRectTransform.position.y);

        foodPanelRectTransform = GetComponent<RectTransform>();

        foodSpriteTile = foodSpriteTilePrefab;
        foodTileRectTransform = foodSpriteTile.GetComponent<RectTransform>();

        SetPanelSize();

        SetImageSize();
        AddInitialFoodTile();      
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddNewFoodTile();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            RemoveFoodTile();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {

            ScalePanelSize();

            labelTextRectTransform.position = labelTextStart;
        }
    }

    public void AddInitialFoodTile()
    {
        offset = new Vector3(tileStart, 0f, 0f);

        Vector3 imagePosition = transform.position + offset;

        Image foodImage = Instantiate(foodSpriteTile, imagePosition, Quaternion.identity, transform) as Image;

        foodTileList.Add(foodImage);

        tileElement++;
    }

    public void AddNewFoodTile()
    {       
        offset += TileOffset();

        Vector3 imagePosition = transform.position + offset;

        Image foodImage = Instantiate(foodSpriteTile, imagePosition, Quaternion.identity, transform) as Image;

        foodTileList.Add(foodImage);

        tileElement++;
    }

    public void RemoveFoodTile()
    {
        offset -= TileOffset();

        DestroyObject(foodTileList[tileElement - 1].gameObject);

        foodTileList.RemoveAt(tileElement-1);

        tileElement--;
    }

    void SetImageSize()
    {
        foodSpriteTile.GetComponent<RectTransform>().sizeDelta = imageSize;
    }

    void SetPanelSize()
    {
        Vector2 textSize = labelTextRectTransform.sizeDelta;
        Vector2 tileSize = foodTileRectTransform.sizeDelta;


    }

    void ScalePanelSize()
    {    
        foodPanelRectTransform.sizeDelta += Vector2.right;
        foodPanelRectTransform.localPosition += new Vector3(0.5f,0f,0f);


    }
    
    Vector3 TileOffset()
    {
        return new Vector3(imageWidth + tileSeparation, 0f, 0f);
    }
}