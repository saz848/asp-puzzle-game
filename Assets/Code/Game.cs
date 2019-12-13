using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;




public class Game : MonoBehaviour
{
    public Waypoint[] allWaypoints;
    public float radius = 0.61f;
    public int control;
    public const int VICTORY = 1;
    public const int DEFEAT = 5;
    public Character[] game_chars;
    public static Character player;
    public static RotateTile[] tiles;
    public static Color default_color = new Color();
    public static Color highlighted_color = new Color();
    public static Sprite plus_outlined;
    public static Sprite plus_;
    public static Sprite straight_outlined;
    public static Sprite straight_;
    public static Sprite corner_;
    public static Sprite corner_outlined;
    public static bool player_exists;


    public static Sprite plus_outlined_n;
    public static Sprite plus_n;
    public static Sprite straight_outlined_n;
    public static Sprite straight_n;
    public static Sprite corner_n;
    public static Sprite corner_outlined_n;
    public static Sprite t_outlined_n;
    public static Sprite t_n;

    public GameObject pause_menu;
    public static GameObject restart_text;
    public Canvas restart_canvas;
    public Button restart_button;
    public GameObject button_object;

    public static bool restart_visible;
    public bool menu_visible;
    public static bool change_selection = true;
    RotateTile selected;
    public Canvas menu_canvas;

    public Button levelButton;
    public Button quitButton;

    public bool dead;

    public GameObject cross;
    public GameObject goal;
    public GameObject corner;
    public GameObject line;
    //public GameObject tTile;

    // Use this for initialization
    void Start()
    {
        dead = false;
        player_exists = true;
        pause_menu = Object.Instantiate(Resources.Load("Pause Menu") as GameObject);
        menu_canvas = pause_menu.GetComponent<Canvas>();
        menu_canvas.worldCamera = Camera.main;
        change_selection = true;
        menu_visible = false;

        restart_text = Object.Instantiate(Resources.Load("Restart Text") as GameObject);
        restart_canvas = restart_text.GetComponent<Canvas>();
        button_object = GameObject.FindGameObjectWithTag("Restart");
        restart_button = button_object.GetComponent<Button>();
        restart_button.onClick.AddListener(RestartGame);
        restart_canvas.worldCamera = Camera.main;
        restart_visible = false;

        pause_menu.SetActive(menu_visible);
        restart_text.SetActive(restart_visible);

        highlighted_color = new Color((248f / 255f), (149f / 255f), (43f / 255f));
        default_color = new Color((238f / 255f), (180f / 255f), (119f / 255f));
        allWaypoints = FindObjectsOfType<Waypoint>();
        plus_outlined = Resources.Load<Sprite>("Tiles/PlusLightOutlined");
        plus_ = Resources.Load<Sprite>("Tiles/PlusLight");
        straight_outlined = Resources.Load<Sprite>("Tiles/StraightOutlined");
        straight_ = Resources.Load<Sprite>("Tiles/Straight");
        corner_outlined = Resources.Load<Sprite>("Tiles/CornerOutlined");
        corner_ = Resources.Load<Sprite>("Tiles/Corner");


        plus_outlined_n = Resources.Load<Sprite>("NarrowTiles/PlusNarrowTileOutlined");
        plus_n = Resources.Load<Sprite>("NarrowTiles/PlusSmallerTiles");
        straight_outlined_n = Resources.Load<Sprite>("NarrowTiles/StraightOutlined");
        straight_n = Resources.Load<Sprite>("NarrowTiles/StraightNarrow");
        corner_outlined_n = Resources.Load<Sprite>("NarrowTiles/CornerNarrowOutlined");
        corner_n = Resources.Load<Sprite>("NarrowTiles/CornerNarrow");
        t_n = Resources.Load<Sprite>("NarrowTiles/TNarrow");
        t_outlined_n = Resources.Load<Sprite>("NarrowTiles/TNarrowOutlined");

        Camera.main.GetComponent<Camera>().backgroundColor = new Color((49f / 255f), (77f / 255f), (121f / 255f));

        LoadTilePrefabs();
        LoadLocationJson();

        tiles = FindObjectsOfType<RotateTile>();


        control = 0;
        AdjustInput(control, tiles);

        foreach (Waypoint w in allWaypoints)
        {
            SetNeighbors(w);
        }
        game_chars = FindObjectsOfType<Character>();
        foreach (Character c in game_chars)
        {
            if (c.type == 0)
            {
                player = c;
                break;
            }
        }

    }

    public void LoadTilePrefabs()
    {
        cross = Resources.Load("Prefabs/NarrowTiles/PlusNarrowTile") as GameObject;
        goal = Resources.Load("Prefabs/NarrowTiles/GoalTile") as GameObject;
        corner = Resources.Load("Prefabs/NarrowTiles/CornerNarrow") as GameObject;
        line = Resources.Load("Prefabs/NarrowTiles/StraightNarrow") as GameObject;
        //tTile = Resources.Load("Prefabs/NarrowTiles/TNarrow") as GameObject;

        //GameObject newPlus = GameObject.Instantiate(corner, new Vector3(1.0f, 0f, 0f), transform.rotation);
        //Debug.Log(newPlus);
    }

    public void LoadLocationJson()
    {

        string val = SceneManager.GetActiveScene().name; // Levels 0 to 9
        string fileName = "Assets/Resources/Levels/" + val +".json";
        LoadJson(fileName);
    }

    public void LoadJson(string fileName)
    {
        using (StreamReader r = new StreamReader(fileName))
        {
            string json = r.ReadToEnd();
            Level myLevels = JsonUtility.FromJson<Level>(json);
            // randomly select a level from myLevels
            int randIndex = Random.Range(0, myLevels.Levels.Length);
            Debug.Log(randIndex);

            Locations items = myLevels.Levels[randIndex];

            for (int i = 0; i < items.Value.Length; i++)
            {
                // List of arguments for shape instance
                // instantiates object based on location and type
                List<string> args = ExtractParameters(items.Value[i].ToString());
                GameObject toInstantiate = DetermineObject(args[2]);
                int rtCat = RotateCategory(args[2]);
                if (toInstantiate != null)
                {
                    GameObject newTile = GameObject.Instantiate(toInstantiate, new Vector3(float.Parse(args[0]) / 2, float.Parse(args[1]) / 2, 0f), transform.rotation);
                    var rt = newTile.GetComponent<RotateTile>();
                    // To make "families" of tiles rotate at the same time, set the rt.category to rtCat
                    rt.category = i;
                }
            }
        }
    }

    public GameObject DetermineObject(string myTile)
    {
        GameObject myObj;
        if (myTile == "corner")
        {
            myObj = corner;
        }
        else if (myTile == "cross")
        {
            myObj = cross;
        }
        else if (myTile == "line")
        {
            myObj = line;
        }
        else
        {
            myObj = null;
        }
        //Debug.Log(myObj);
        return myObj;
    }

    public int RotateCategory(string myTile)
    {
        int myObj = -1;
        if (myTile == "corner")
        {
            myObj = 0;
        }
        else if (myTile == "cross")
        {
            myObj = 1;
        }
        else if (myTile == "line")
        {
            myObj = 2;
        }

        //Debug.Log(myObj);
        return myObj;
    }

    public List<string> ExtractParameters(System.String toParse)
    {
        string pattern = @"\b(?<=[\w]*\()[\w,]*(?=\))";
        string input = toParse;

        MatchCollection matches = Regex.Matches(input, pattern);
        //Debug.Log(matches[0]);

        List<string> result = matches[0].Value.Split(',').ToList();
        return result;
    }

    public string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        string projectName = s[s.Length - 2];
        Debug.Log("project = " + projectName);
        return projectName;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Character c in game_chars)
        {

            if (c != player && player_exists == true)
            {
                float distance = Vector3.Distance(player.transform.position, c.transform.position);
                if (distance < (player.radius + c.radius))
                {
                    player_exists = false;
                    DestroyImmediate(player.gameObject);
                    DestroyImmediate(player);
                    dead = true;
                    Camera.main.GetComponent<Camera>().backgroundColor = new Color((38f / 255f), (38f / 255f), (38f / 255f));

                    game_chars = FindObjectsOfType<Character>();
                    RotateTile[] rotate_tiles = FindObjectsOfType<RotateTile>();
                    foreach (RotateTile rt in rotate_tiles)
                    {
                        SpriteRenderer sr = rt.GetComponent<SpriteRenderer>();
                        sr.color = new Color((128f / 255f), (128f / 255f), (128f / 255f));
                        rt.characters = game_chars;
                    }

                    restart_visible = true;
                    restart_text.SetActive(restart_visible);
                }
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            foreach (RotateTile t in tiles)
            {
                if (!menu_visible)
                {
                    if (t.rotatable)
                    {
                        selected = t;
                    }
                }
                t.rotatable = false;
                if (menu_visible)
                {
                    selected.rotatable = true;
                }
            }
            menu_visible = !menu_visible;
            change_selection = !menu_visible;
            foreach (Character c in game_chars)
            {
                c.paused = menu_visible;
            }


            pause_menu.SetActive(menu_visible);
        }



        if (Input.GetKey(KeyCode.Space))
        {
            foreach (Character c in game_chars)
            {
                if (c.timefactor == 1)
                {
                    c.timefactor = 2;
                    c.MaxProgress = c.MaxProgress / 2;
                    c.progress = c.progress / 2;
                }
            }
        }
        else
        {
            foreach (Character c in game_chars)
            {
                if (c.timefactor == 2)
                {
                    c.timefactor = 1;
                    c.MaxProgress = c.MaxProgress * 2;
                    c.progress = c.progress * 2;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            EndGame(DEFEAT);
        }
    }

    public void SetNeighbors(Waypoint w)
    {
        foreach (Waypoint other_w in allWaypoints)
        {

            if (w != other_w)
            {
                float dist = Vector3.Distance(w.transform.position, other_w.transform.position);

                if (dist < 0.51f)
                {
                    double deltaX = Mathf.Abs(w.transform.position.x - other_w.transform.position.x);
                    double deltaY = Mathf.Abs(w.transform.position.y - other_w.transform.position.y);

                    if (deltaY > deltaX)
                    {
                        if (w.transform.position.y > other_w.transform.position.y)
                        {
                            w.neighbors[2] = other_w;
                            other_w.neighbors[0] = w;
                        }
                        else
                        {
                            w.neighbors[0] = other_w;
                            other_w.neighbors[2] = w;
                        }
                    }

                    else
                    {
                        if (w.transform.position.x > other_w.transform.position.x)
                        {
                            w.neighbors[3] = other_w;
                            other_w.neighbors[1] = w;
                        }
                        else
                        {
                            w.neighbors[1] = other_w;
                            other_w.neighbors[3] = w;
                        }
                    }
                }
            }
        }
    }

    public static void EndGame(int result)
    {
        int numScenes = SceneManager.sceneCountInBuildSettings;
        if (result == VICTORY)
        {
            if (SceneManager.GetActiveScene().buildIndex < numScenes - 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                SceneManager.LoadScene("Menu");
            }

        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void RestartGame()
    {
        EndGame(DEFEAT);
    }

    public static void AdjustInput(int cat, RotateTile[] tile_list)
    {
        if (change_selection)
        {
            foreach (RotateTile r in tile_list)
            {
                SpriteRenderer tile_color = r.GetComponent<SpriteRenderer>();
                if (r.category == cat)
                {
                    r.rotatable = true;
                    if (r.type == 0)
                    {
                        tile_color.sprite = plus_outlined;
                    }
                    else if (r.type == 1)
                    {
                        tile_color.sprite = straight_outlined;
                    }
                    else if (r.type == 2)
                    {
                        tile_color.sprite = corner_outlined;
                    }
                    else if (r.type == 3)
                    {
                        tile_color.sprite = plus_outlined_n;
                    }
                    else if (r.type == 4)
                    {
                        tile_color.sprite = straight_outlined_n;
                    }
                    else if (r.type == 5)
                    {
                        tile_color.sprite = corner_outlined_n;
                    }
                    else if (r.type == 6)
                    {
                        tile_color.sprite = t_outlined_n;
                    }
                }
                else
                {
                    r.rotatable = false;
                    if (r.type == 0)
                    {
                        tile_color.sprite = plus_;

                    }
                    else if (r.type == 1)
                    {
                        tile_color.sprite = straight_;
                    }
                    else if (r.type == 2)
                    {
                        tile_color.sprite = corner_;
                    }
                    else if (r.type == 3)
                    {
                        tile_color.sprite = plus_n;
                    }
                    else if (r.type == 4)
                    {
                        tile_color.sprite = straight_n;
                    }
                    else if (r.type == 5)
                    {
                        tile_color.sprite = corner_n;
                    }
                    else if (r.type == 6)
                    {
                        tile_color.sprite = t_n;
                    }
                }
            }
        }

    }
}

[System.Serializable]
public class Level
{
    public Locations[] Levels; 
}

[System.Serializable]
public class Locations
{
    public string[] Value;

    public static Locations CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Locations>(jsonString);
    }
}


