using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{
    public GameObject startingBox;
    public GameObject currentBox;
    public Color unselectedColor;
    public Color selectedColor;
    public Color startColor;
    private GameObject[] tiles;
    private LevelManager levelManager;
    public bool gameWon = false;
    private Camera mainCamera;
    private AudioSource audioSource;
    public GameObject fadeCanvasPrefab;
    void Start()
    {
        setRandomColours();

        audioSource = GetComponent<AudioSource>();
        levelManager = GameObject.FindWithTag("LevelManager")?.GetComponent<LevelManager>();

        if (levelManager == null)
        {
            GameObject manager = new GameObject("LevelLoader");
            manager.AddComponent<LevelManager>();
            manager.GetComponent<LevelManager>().endGameSceneName = "End Game";
            manager.GetComponent<LevelManager>().levelIndex = SceneManager.GetActiveScene().buildIndex;
            manager.tag = "LevelManager";
            // instantiate FadeCanvas as a child of manager
            if (fadeCanvasPrefab != null)
            {
                GameObject fadeCanvas = Instantiate(fadeCanvasPrefab, manager.transform);
                fadeCanvas.name = "FadeCanvas";
            }

            manager.GetComponent<LevelManager>().StartInitialFadeIn();
            levelManager = manager.GetComponent<LevelManager>();

        }

        tiles = GameObject.FindGameObjectsWithTag("Tile");

        startingBox = GameObject.FindWithTag("StartTile");
        startingBox.GetComponent<TileController>().ChangeColour(startColor);
        startingBox.GetComponent<TileController>().Bounce();
    
        ResetAllTiles();
    }

    void setRandomColours()
    {
        // Ensure mainCamera is assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Generate random colors
        unselectedColor = new Color(Random.value, Random.value, Random.value);
        selectedColor = new Color(Random.value, Random.value, Random.value);
        startColor = new Color(Random.value, Random.value, Random.value);


        // Set the background color of the main camera
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = new Color(Random.value, Random.value, Random.value); // Set to one of the random colors or whichever you prefer
        }
    }

    void FixedUpdate()
    {
        if (WinCondition() && !gameWon && SceneManager.GetActiveScene().name != levelManager.endGameSceneName && SceneManager.GetActiveScene().buildIndex != 0)
        {
            gameWon = true;
            levelManager.LoadNextLevel();
        }
    }

    void Update()
    {
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {

            if (currentBox == null && startingBox.GetComponent<Collider2D>().OverlapPoint(mousepos))
            {
                Debug.Log("start");
                currentBox = startingBox;
                startingBox.GetComponent<TileController>().ChangeColour(new Color(startColor.r, startColor.g, startColor.b, 0.8f));
            }

            
            foreach (var tile in tiles)
            {
            
                if (currentBox != null && AdjacentTile(tile) && tile.GetComponent<Collider2D>().OverlapPoint(mousepos) && tile.GetComponent<TileController>().GetSelect() == false)
                {
                    if (!audioSource.isPlaying){audioSource.Play();}
                    currentBox = tile;
                    tile.GetComponent<TileController>().Select();
                    tile.GetComponent<TileController>().ChangeColour(selectedColor);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetAllTiles();
        }

    }
    bool AdjacentTile(GameObject tile)
    {
        if (currentBox == null) return true; // If there's no currentBox selected, allow selection

        // Get the positions of the current tile and the tile we're checking
        Vector3 currentPos = currentBox.transform.position;
        Vector3 tilePos = tile.transform.position;

        // Calculate the distance between the tiles
        float deltaX = Mathf.Abs(currentPos.x - tilePos.x);
        float deltaY = Mathf.Abs(currentPos.y - tilePos.y);

        // Check if the tile is adjacent
        // We assume the tiles are placed on a grid, and deltaX or deltaY should be approximately equal to the width or height of a tile (i.e., 1 unit)
        float tileWidth = 1f; // Assume each tile is 1 unit in width and height. Adjust this if your tiles are of different size.

        if ((deltaX == tileWidth && deltaY == 0) || (deltaX == 0 && deltaY == tileWidth))
        {
            return true;
        }

        return false;
    }

    bool WinCondition()
    {
        foreach(var tile in tiles)
        {
            if (tile.GetComponent<TileController>().GetSelect() == false)
            {
                return false;
            }
        }
        return true;
    }


    void ResetAllTiles()
    {
        if (!gameWon)
        {
            currentBox = null;
            startingBox.GetComponent<TileController>().ChangeColour(startColor);

            foreach (GameObject tile in tiles)
            {
                tile.GetComponent<TileController>().Deselect();
                tile.GetComponent<TileController>().ChangeColour(unselectedColor);
            }
        }
    }
}
