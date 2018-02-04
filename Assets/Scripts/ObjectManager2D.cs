using System;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectManager2D : MonoBehaviour {
    public Material mat;
    public float threshold = 0.7f;
    private Texture2D tex;
    private Sprite mySprite;
    private SpriteRenderer sr;
    private float scale = 1.0f;
    private float scaleMin = 0.1f;
    private float scaleMax = 10.0f;
    string guiText = "";
    string[] files = { "1", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "X" };
    string chrtype = "sen";
    int current_file = 7;
    List<Range> sortedRanges;
    Vector2 mousePosition = new Vector2(0, 0);
    Dictionary<Range, Dictionary<Range, float>> interactions;
    Texture2D ReadInFile(string filename)
    {
        Debug.Log("About to load in the file");
        
        TextAsset file = Resources.Load(filename) as TextAsset;
        Debug.Log("Loaded in the file");
        int counter = 0;
    
   
        List<Range> ranges = new List<Range>();
        interactions = new Dictionary<Range, Dictionary<Range, float>>();
        List<float> colorsIn = new List<float>();
        float maxCol = 0.0f;
        foreach (string line in file.text.Split('\n'))
        {
            if (line != "")
            {
                Square sq = new Square(line);
                SafelyAdd2D(ref interactions, sq);
                ranges.Add(sq.A);
                ranges.Add(sq.B);
                if (sq.Strength > maxCol)
                    maxCol = sq.Strength;
                colorsIn.Add(sq.Strength);
                counter++;
            }
        }
        Resources.UnloadAsset(file);
        Debug.Log("Initialisation complete");
        Debug.Log(ranges.Count);
        List<Range> uniqueRanges = new HashSet<Range>(ranges).ToList();
        Debug.Log(uniqueRanges.Count);
        uniqueRanges.Sort();
        int items_per_bucket = 1;
        int buckets = uniqueRanges.Count / items_per_bucket;
        Texture2D tex = new Texture2D(buckets, buckets);
        int x = 0;
        int y = uniqueRanges.Count;
        for (int itrcx = 0; itrcx < buckets; itrcx++)
        {
            for (int itrcy = 0; itrcy < buckets; itrcy++)
            {
                float col = 0f;
                if (interactions.ContainsKey(uniqueRanges[itrcx]) && interactions[uniqueRanges[itrcx]].ContainsKey(uniqueRanges[itrcy]))
                    col += interactions[uniqueRanges[itrcx]][uniqueRanges[itrcy]];
                Color color = new Color(1, 0, 0, 0);
                if (col > 0)
                {
                    color = new Color(1, 0, 0, threshold * (1-threshold * (col/maxCol)));
                }
                tex.SetPixel(x, y, color);
                y--;
            }
            x++;
        }
        tex.Apply();
        Debug.Log("Rendering complete.");
        sortedRanges = uniqueRanges;
        return tex;
    }

    private static void SafelyAdd2D(ref Dictionary<Range, Dictionary<Range, float>> interactions, Square x)
    {
        if (interactions.ContainsKey(x.A))
        {
            interactions[x.A].Add(x.B, x.Strength);
        }
        else
        {
            var b = new Dictionary<Range, float>
                {
                    { x.B, x.Strength }
                };
            interactions.Add(x.A, b);
        }
        if (interactions.ContainsKey(x.B))
        {
            interactions[x.B].Add(x.A, x.Strength);
        }
        else
        {
            var a = new Dictionary<Range, float>
                {
                    { x.A, x.Strength }
                };
            interactions.Add(x.B, a);
        }
    }

    void Awake()
    {
        sr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

    void Start()
    {
        LoadNextFile();
    }

    private void LoadNextFile()
    {
        Debug.Log("Starting...");
        var filename = files[current_file] + "_formatted.bed." + chrtype;
        tex = ReadInFile(filename);
        mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        sr.sprite = mySprite;
        sr.material.mainTexture = tex;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Camera c = Camera.main;
            Vector3 mousePos = new Vector3();
            mousePos = Input.mousePosition;
            Vector3 worldBottomLeft = Camera.main.WorldToScreenPoint(new Vector3(-sr.sprite.bounds.extents.x, -sr.sprite.bounds.extents.y, 0f));
            Vector3 worldTopRight = Camera.main.WorldToScreenPoint(new Vector3(sr.sprite.bounds.extents.x, sr.sprite.bounds.extents.y, 0f));
            if (mousePos.x <= worldTopRight.x && mousePos.x >= worldBottomLeft.x && mousePos.y <= worldTopRight.y && mousePos.y >= worldBottomLeft.y)
            {
                float xRange = mousePos.x - worldBottomLeft.x;
                float yRange = worldTopRight.y - mousePos.y;//mousePos.y - worldBottomLeft.y;
                float unit = (worldTopRight.x - worldBottomLeft.x)/sortedRanges.Count;
                int xIndex = (int) Math.Round(xRange / unit);
                int yIndex = (int) Math.Round(yRange / unit);
                Range xPosition = sortedRanges[xIndex];
                Range yPosition = sortedRanges[yIndex];
                string strength = "0";
                if (interactions.ContainsKey(xPosition) && interactions[xPosition].ContainsKey(yPosition))
                    strength = interactions[xPosition][yPosition].ToString();
                guiText = "chr" + files[current_file] + "-" + xPosition.ToString() + "\nchr" + files[current_file] + "-" + yPosition.ToString() + '\n' + strength;
                if (chrtype == "sen")
                {
                    guiText += "\nSenescent";
                }else
                {
                    guiText += "\nProliferating";
                }
            }
            mousePosition = mousePos;
        }
        else
        {
            guiText = "";
            mousePosition = new Vector2(Screen.width + 10, Screen.height + 10);
        }
        scale = Mathf.Clamp(scale - Input.GetAxis("Mouse ScrollWheel") * 5, scaleMin, scaleMax);
        Vector3 old_scale = transform.localScale;

        old_scale.Set(scale, scale, scale);

        transform.localScale = old_scale;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            current_file--;
            current_file = ((current_file %= files.Count()) < 0) ? current_file + files.Count() : current_file;
            LoadNextFile();
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            current_file = (current_file + 1) % files.Count();
            LoadNextFile();
        } else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (chrtype == "sen")
            {
                chrtype = "pro";
            } else
            {
                chrtype = "sen";
            }
            LoadNextFile();
        }
    }

    //void LateUpdate()
    //{
    //    scale = Mathf.Clamp(scale - Input.GetAxis("Mouse ScrollWheel") * 5, scaleMin, scaleMax);
    //    transform.lossyScale.Set(scale, scale, scale);
    //}

    private void OnGUI()
    {
        GUI.Box(new Rect(mousePosition.x + 15, Screen.height - mousePosition.y + 15, 200, 70), guiText);
    }
}

