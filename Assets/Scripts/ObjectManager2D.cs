using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager2D : MonoBehaviour, IObjectManager
{
    private Tasks studyTask;
    private Material baseMaterial;
    private TextAsset textFile;
    private Curve mainCurve;
    private Curve redCurve;
    private Curve blueCurve;
    private GameObject blueObject;
    private GameObject redObject;
    private bool showUnderstanding = false;
    private string understandingString = "";

    public float threshold = 0.7f;
    private Texture2D mainTexture;
    private Texture2D redTexture;
    private Texture2D blueTexture;
    private Sprite mainSprite;
    private SpriteRenderer mainSpriteRenderer;
    private Sprite redSprite;
    private SpriteRenderer redSpriteRenderer;
    private Sprite blueSprite;
    private SpriteRenderer blueSpriteRenderer;
    private float scale = 1.0f;
    private float scaleMin = 0.1f;
    private float scaleMax = 10.0f;
    private string guiText = "";
    private string[] files = { "1", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "X" };
    private string chrtype = "sen";
    private int current_file = 7;
    private List<Range> sortedRanges;
    private Vector2 mousePosition = new Vector2(0, 0);
    private Dictionary<Range, Dictionary<Range, float>> interactions;

    public Material BaseMaterial
    {
        get
        {
            return baseMaterial;
        }
    }

    public TextAsset TextFile
    {
        get
        {
            return textFile;
        }
    }

    public Curve MainCurve
    {
        get
        {
            return mainCurve;
        }
    }

    public Curve RedCurve
    {
        get
        {
            return redCurve;
        }
    }

    public Curve BlueCurve
    {
        get
        {
            return blueCurve;
        }
    }

    public GameObject BlueObject
    {
        get
        {
            return blueObject;
        }
    }

    public GameObject RedObject
    {
        get
        {
            return redObject;
        }
    }

    public bool ShowUnderstanding
    {
        get
        {
            return showUnderstanding;
        }
    }

    public string UnderstandingString
    {
        get
        {
            return understandingString;
        }
    }

    private Texture2D ReadInFile(string filename, float red = 1.0f, float green = 0.0f, float blue = 0.0f, Range redA = null, Range redB = null, Range blueA = null, Range blueB = null, bool isRange = false, bool attributeUnderstanding = false)
    {
        TextAsset file = Resources.Load(filename) as TextAsset;
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
        List<Range> uniqueRanges = new HashSet<Range>(ranges).ToList();
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
                Color color = new Color(red, green, blue, 0);
                if (studyTask == Tasks.PointDistance || studyTask == Tasks.SegmentDistance)
                {
                    if (isRange)
                    {
                        if (uniqueRanges[itrcx] >= redA && uniqueRanges[itrcx] <= redB)
                        {
                            color = new Color(1, 0, 0);
                        }
                        else if (uniqueRanges[itrcx] >= blueA && uniqueRanges[itrcx] <= blueB)
                        {
                            color = new Color(0, 0, 1);
                        }
                        else if (col > 0)
                        {
                            color = new Color(red, green, blue, threshold * (1 - threshold * (col / maxCol)));
                        }
                    }
                    else
                    {
                        if (uniqueRanges[itrcx] == redA || uniqueRanges[itrcx] == redB)
                        {
                            color = new Color(1, 0, 0);
                        }
                        else if (uniqueRanges[itrcx] == blueA || uniqueRanges[itrcx] == blueB)
                        {
                            color = new Color(0, 0, 1);
                        }
                        else if (col > 0)
                        {
                            color = new Color(red, green, blue, threshold * (1 - threshold * (col / maxCol)));
                        }
                    }
                }
                else if (attributeUnderstanding)
                {
                    if (col > 0)
                        color = new Color((float)itrcx / buckets, (float)itrcy / buckets, 1 - ((float)itrcx / buckets), threshold * (1 - threshold * (col / maxCol)));
                }
                else if (col > 0)
                {
                    color = new Color(red, green, blue, threshold * (1 - threshold * (col / maxCol)));
                }
                tex.SetPixel(x, y, color);
                y--;
            }
            x++;
        }
        tex.Apply();
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

    private void Awake()
    {
        mainSpriteRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        redSpriteRenderer = (new GameObject()).AddComponent<SpriteRenderer>() as SpriteRenderer;
        redSpriteRenderer.gameObject.transform.position = new Vector2(-3, 0);
        blueSpriteRenderer = (new GameObject()).AddComponent<SpriteRenderer>() as SpriteRenderer;
        blueSpriteRenderer.gameObject.transform.position = new Vector2(3, 0);
    }

    private void Start()
    {
        //LoadNextFile();
    }

    public void LoadNextFile(string filename = "")
    {
        if ("" == filename)
            filename = files[current_file] + "_formatted.bed." + chrtype;
        Debug.Log("Loading file: " + filename);
        mainTexture = CreateTexture(filename);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    private Texture2D CreateTexture(string filename = "", float red = 1.0f, float green = 0.0f, float blue = 0.0f, Range redA = null, Range redB = null, Range blueA = null, Range blueB = null, bool isRange = false, bool attributeUnderstanding = false)
    {
        if ("" == filename)
            filename = files[current_file] + "_formatted.bed." + chrtype;
        return ReadInFile(filename, red, green, blue, redA, redB, blueA, blueB, isRange, attributeUnderstanding);
    }

    private SpriteRenderer DisplayTexture(Texture2D tex, Sprite sprite, SpriteRenderer spriteRenderer, int scale)
    {
        sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width / scale, tex.height / scale), new Vector2(0.5f, 0.5f), 100.0f);
        spriteRenderer.sprite = sprite;
        spriteRenderer.material.mainTexture = tex;
        return spriteRenderer;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Camera c = Camera.main;
            Vector3 mousePos = new Vector3();
            mousePos = Input.mousePosition;
            Vector3 worldBottomLeft = Camera.main.WorldToScreenPoint(new Vector3(-mainSpriteRenderer.sprite.bounds.extents.x, -mainSpriteRenderer.sprite.bounds.extents.y, 0f));
            Vector3 worldTopRight = Camera.main.WorldToScreenPoint(new Vector3(mainSpriteRenderer.sprite.bounds.extents.x, mainSpriteRenderer.sprite.bounds.extents.y, 0f));
            if (mousePos.x <= worldTopRight.x && mousePos.x >= worldBottomLeft.x && mousePos.y <= worldTopRight.y && mousePos.y >= worldBottomLeft.y)
            {
                float xRange = mousePos.x - worldBottomLeft.x;
                float yRange = worldTopRight.y - mousePos.y;//mousePos.y - worldBottomLeft.y;
                float unit = (worldTopRight.x - worldBottomLeft.x) / sortedRanges.Count;
                int xIndex = (int)Math.Round(xRange / unit);
                int yIndex = (int)Math.Round(yRange / unit);
                Range xPosition = sortedRanges[xIndex];
                Range yPosition = sortedRanges[yIndex];
                string strength = "0";
                if (interactions.ContainsKey(xPosition) && interactions[xPosition].ContainsKey(yPosition))
                    strength = interactions[xPosition][yPosition].ToString();
                guiText = "chr" + files[current_file] + "-" + xPosition.ToString() + "\nchr" + files[current_file] + "-" + yPosition.ToString() + '\n' + strength;
                if (chrtype == "sen")
                {
                    guiText += "\nSenescent";
                }
                else
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
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            current_file = (current_file + 1) % files.Count();
            LoadNextFile();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (chrtype == "sen")
            {
                chrtype = "pro";
            }
            else
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
        if (showUnderstanding)
            GUI.Box(new Rect(0, 0, 400, 40), understandingString);
        GUI.Box(new Rect(mousePosition.x + 15, Screen.height - mousePosition.y + 15, 200, 70), guiText);
    }

    public void SetupCurveComparisonTrial(CurveComparisonTrial curveComparisonTrial)
    {
        studyTask = Tasks.CurveComparison;
        mainTexture = CreateTexture(curveComparisonTrial.ReferenceChromosome, 0.0f, 0.0f, 0.0f);
        redTexture = CreateTexture(curveComparisonTrial.RedChromosome, 1.0f, 0.0f, 0.0f);
        blueTexture = CreateTexture(curveComparisonTrial.BlueChromosome, 0.0f, 0.0f, 1.0f);

        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 3);
        redSpriteRenderer = DisplayTexture(redTexture, redSprite, redSpriteRenderer, 3);
        blueSpriteRenderer = DisplayTexture(blueTexture, blueSprite, blueSpriteRenderer, 3);
    }

    public void SetupPointDistanceTrial(PointDistanceTrial pdt, string chrfn)
    {
        studyTask = Tasks.PointDistance;
        Range redA = new Range(pdt.RedA);
        Range redB = new Range(pdt.RedB);
        Range blueA = new Range(pdt.BlueA);
        Range blueB = new Range(pdt.BlueB);
        mainTexture = CreateTexture(pdt.Chromosome, 0.0f, 0.0f, 0.0f, redA, redB, blueA, blueB);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    public void SetupAttributeUnderstandingTrial(AttributeUnderstandingTrial adt)
    {
        studyTask = Tasks.AttributeUnderstanding;
        //mainCurve = new Curve(adt.Chromosome, false, true);
        mainTexture = CreateTexture(adt.Chromosome, attributeUnderstanding: true);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
        understandingString = adt.Question;
        showUnderstanding = true;
    }

    public void SetupSegmentDistanceTrial(SegmentDistanceTrial sdt, string chrfn)
    {
        studyTask = Tasks.SegmentDistance;
        Range redA = new Range(sdt.RedA);
        Range redB = new Range(sdt.RedB);
        Range blueA = new Range(sdt.BlueA);
        Range blueB = new Range(sdt.BlueB);
        mainTexture = CreateTexture(sdt.Chromosome, 0.0f, 0.0f, 0.0f, redA, redB, blueA, blueB, true);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    public void SetupTouchingSegments(TouchingSegmentsTrial touchingSegmentsTrial)
    {
        throw new NotImplementedException();
    }
}