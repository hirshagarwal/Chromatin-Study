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

    private Texture2D ReadInFile(string filename, Range redA = null, Range redB = null, Range blueA = null, Range blueB = null, bool isRange = false, bool attributeUnderstanding = false, int redCount = -1, int skip = 1, bool triple = false)
    {
        TextAsset file = Resources.Load(filename) as TextAsset;
        int counter = 0;

        List<Range> ranges = new List<Range>();
        interactions = new Dictionary<Range, Dictionary<Range, float>>();
        List<float> colorsIn = new List<float>();
        float maxCol = 0.0f;
        float minCol = 1.0f;
        foreach (string line in file.text.Split('\n'))
        {
            if (line != "")
            {
                Square sq = new Square(line);
                if (sq.Strength > 0.1  ) {
                    sq.Strength = 0.1f;
                }
                sq.Strength = (float) Math.Log(1+sq.Strength,1000);
                SafelyAdd2D(ref interactions, sq);
                ranges.Add(sq.A);
                ranges.Add(sq.B);
                if (sq.Strength > maxCol)
                    maxCol = sq.Strength;
                if (sq.Strength < minCol)
                    minCol = sq.Strength;

                colorsIn.Add(sq.Strength);
                counter++;
            }
        }
//        if (maxCol > 0.1f) {
//           maxCol = 0.1f;
//        }
        Debug.Log("MaxCol=" + maxCol);
        Debug.Log("MaxCol=" + minCol);
        Resources.UnloadAsset(file);
        List<Range> uniqueRanges = new HashSet<Range>(ranges).ToList();
        uniqueRanges.Sort();
        int buckets = uniqueRanges.Count / skip;
        Texture2D tex = new Texture2D(buckets, buckets);
        int x = 0;
        int y = uniqueRanges.Count;
        for (int itrcx = 0; itrcx < buckets; itrcx+=skip)
        {
            for (int itrcy = 0; itrcy < buckets; itrcy+=skip)
            {
                float col = 0f;
                if (interactions.ContainsKey(uniqueRanges[itrcx]) && interactions[uniqueRanges[itrcx]].ContainsKey(uniqueRanges[itrcy]))
                    col = interactions[uniqueRanges[itrcx]][uniqueRanges[itrcy]];
                Color color = Design.GetClosestColor(0f);

                float threshold = 0.1f;
                // float colval = threshold * (1 - threshold * (col / maxCol));
                // float colval = threshold + ((1 - threshold) * (minCol + col / (maxCol - maxCol)));
                float colval = (col - minCol) / (maxCol - minCol);
                colval = threshold + ((1 - threshold) * colval);
                if (studyTask == Tasks.PointDistance || studyTask == Tasks.SegmentDistance)
                {
                    if (isRange)
                    {
                        if (uniqueRanges[itrcx] >= redA && uniqueRanges[itrcx] <= redB)
                        {
                            color = Design.GetClosestColor(0f);
                        }
                        else if (uniqueRanges[itrcx] >= blueA && uniqueRanges[itrcx] <= blueB)
                        {
                            color = Design.GetClosestColor(1f);
                        }
                        else if (col > 0)
                        {
                            color = Design.GetClosestColor(colval);
                        }
                    }
                    else
                    {
                        if (uniqueRanges[Clamp(itrcx + 1, uniqueRanges.Count)] == redA ||
                            uniqueRanges[Clamp(itrcx + 1, uniqueRanges.Count)] == redB ||
                            uniqueRanges[Clamp(itrcx-1, uniqueRanges.Count)] == redA ||
                            uniqueRanges[Clamp(itrcx-1, uniqueRanges.Count)] == redB ||
                            uniqueRanges[Clamp(itrcy + 1, uniqueRanges.Count)] == redA ||
                            uniqueRanges[Clamp(itrcy + 1, uniqueRanges.Count)] == redB ||
                            uniqueRanges[Clamp(itrcy - 1, uniqueRanges.Count)] == redA ||
                            uniqueRanges[Clamp(itrcy - 1, uniqueRanges.Count)] == redB)
                        {
                            color = Design.GetClosestColor(0f);
                        }
                        else if (uniqueRanges[Clamp(itrcx+1, uniqueRanges.Count)] == blueA ||
                                uniqueRanges[Clamp(itrcx+1, uniqueRanges.Count)] == blueB ||
                                uniqueRanges[Clamp(itrcx-1, uniqueRanges.Count)] == blueA ||
                                uniqueRanges[Clamp(itrcx-1, uniqueRanges.Count)] == blueB ||
                                uniqueRanges[Clamp(itrcy + 1, uniqueRanges.Count)] == blueA ||
                                uniqueRanges[Clamp(itrcy + 1, uniqueRanges.Count)] == blueB ||
                                uniqueRanges[Clamp(itrcy - 1, uniqueRanges.Count)] == blueA ||
                                uniqueRanges[Clamp(itrcy - 1, uniqueRanges.Count)] == blueB)
                        {
                            color = Design.GetClosestColor(1f);
                        }
                        else if (col > 0)
                        {
                            color = Design.GetClosestColor(colval);
                        }
                    }
                }
                else if (attributeUnderstanding)
                {
                    if (col > 0)
                        color = Design.GetClosestColor(colval);
                }
                else if (studyTask == Tasks.LargerTad)
                {
                    if (col > 0)
                    {
                        if (itrcx < buckets / 10 &&
                            itrcy < buckets / 10)
                        {
                            color = Design.GetClosestColor(1f - colval);
                        }
                        else if (itrcx > (buckets - (buckets / 10)) &&
                          itrcy > (buckets - (buckets / 10)))
                        {
                            color = Design.GetClosestColor(colval);
                        }
                        else
                        {
                            color = Design.GetClosestColor(0.5f);
                        }
                    }
                    else
                    {
                        color = Color.white;
                    }
                }
                else if (col > 0)
                {
                    color = Design.GetClosestColor(colval);
                } else
                {
                    color = Design.GetClosestColor(0f);
                }
                tex.SetPixel(x, y, color);
                y--;
            }
            x++;
        }
        if (redCount > -1)
        {
            if (triple)
            {
                tex = MutateTextureTriple(tex, buckets, redCount, skip);
            } else
            {
                tex = MutateTexture(tex, buckets, redCount, skip);
            }
        }
            
        tex.Apply();
        sortedRanges = uniqueRanges;
        return tex;
    }

    private int Clamp(int i, int l)
    {
        if (i < 0)
        {
            return 0;
        }
        if (i >= l)
        {
            return l - 1;
        }
        return i;
    }

    private Texture2D MutateTexture(Texture2D tex, int buckets, int redCount, int skip)
    {
        int currentReds = 0;
        System.Random rnd = new System.Random(0);
        List<Tuple<int, int>> modifiedLocations = new List<Tuple<int,int>>();
        while (currentReds < redCount)
        {
            for (int itrcx = 0; itrcx < buckets; itrcx+=skip)
            {
                for (int itrcy = 0; itrcy < buckets; itrcy+=skip)
                {
                    int r = rnd.Next(25000);
                    if (r == 2)
                    {
                        if (
                            !modifiedLocations.Contains(new Tuple<int, int>(itrcx, itrcy)) &&
                            itrcx > 0 &&
                            itrcx < buckets - 2 &&
                            !modifiedLocations.Contains(new Tuple<int, int>(itrcx - 1, itrcy)) &&
                            !modifiedLocations.Contains(new Tuple<int, int>(itrcx + 1, itrcy))
                            )
                        {
                            if (currentReds < redCount)
                            {
                                Debug.Log("Setting non-decoy point");
                                tex.SetPixel(itrcx - 1, itrcy, tex.GetPixel(itrcx - 1, itrcy + 1));
                                tex.SetPixel(itrcx, itrcy + 1, tex.GetPixel(itrcx - 1, itrcy + 1));
                                for (int i = itrcx - 1; i <= itrcx + 1; i++)
                                {
                                    for (int j = itrcy - 1; j <= itrcy + 1; j++)
                                    {
                                        tex.SetPixel(i, j, GetRecolored(i, j, tex));
                                        modifiedLocations.Add(new Tuple<int, int>(i, j));

                                    }
                                }
                                currentReds++;
                            }
                            else
                            {
                                Debug.Log("Setting decoy point");
                                tex.SetPixel(itrcx, itrcy, GetRecolored(itrcx, itrcy, tex));
                            }
                        }
                    }
                }
            }
        }
        tex.Apply();
        return tex;
    }

    private Texture2D MutateTextureTriple(Texture2D tex, int buckets, int redCount, int skip)
    {
        int currentReds = 0;
        System.Random rnd = new System.Random(0);
        List<Tuple<int, int>> modifiedLocations = new List<Tuple<int, int>>();
        while (currentReds < redCount)
        {
            for (int itrcx = 0; itrcx < buckets; itrcx += skip)
            {
                for (int itrcy = 0; itrcy < buckets; itrcy += skip)
                {
                    int r = rnd.Next(25000);
                    if (r == 2)
                    {
                        bool modifiedBefore = false;
                        for (int i = itrcx - 3; i <= itrcx + 3; i++)
                        {
                            for (int j = itrcy - 3; j <= itrcy + 3; j++)
                            {
                                if (modifiedLocations.Contains(new Tuple<int, int>(i, j)))
                                {
                                    modifiedBefore = true;
                                }
                            }
                        }
                        if (
                            !modifiedBefore &&
                            itrcx > 2 &&
                            itrcx < buckets - 4
                            )
                        {
                            if (currentReds < redCount)
                            {
                                Debug.Log("Setting non-decoy point");
                                tex.SetPixel(itrcx - 2, itrcy - 3, GetRecolored(itrcx - 2, itrcy - 3, tex, 1.5f));
                                tex.SetPixel(itrcx, itrcy - 3, GetRecolored(itrcx, itrcy - 3, tex, 0.5f));
                                tex.SetPixel(itrcx + 1, itrcy - 3, GetRecolored(itrcx + 1, itrcy - 3, tex, 0.8f));
                                tex.SetPixel(itrcx + 2, itrcy - 3, GetRecolored(itrcx + 2, itrcy - 3, tex, 0.8f));
                                tex.SetPixel(itrcx + 3, itrcy - 3, GetRecolored(itrcx + 3, itrcy - 3, tex, 0.3f));
                                tex.SetPixel(itrcx - 3, itrcy - 2, GetRecolored(itrcx - 3, itrcy - 2, tex, 1.5f));
                                tex.SetPixel(itrcx - 1, itrcy - 2, GetRecolored(itrcx - 1, itrcy - 2, tex, 1.5f));
                                tex.SetPixel(itrcx + 1, itrcy - 2, GetRecolored(itrcx + 1, itrcy - 2, tex, 1.3f));
                                tex.SetPixel(itrcx + 2, itrcy - 2, GetRecolored(itrcx + 2, itrcy - 2, tex, 1.2f));
                                tex.SetPixel(itrcx + 3, itrcy - 2, GetRecolored(itrcx + 3, itrcy - 2, tex, 0.5f));
                                tex.SetPixel(itrcx - 2, itrcy - 1, GetRecolored(itrcx - 2, itrcy - 1, tex, 1.5f));
                                tex.SetPixel(itrcx, itrcy - 1, GetRecolored(itrcx, itrcy - 1, tex, 1.5f));
                                tex.SetPixel(itrcx + 3, itrcy - 1, GetRecolored(itrcx + 3, itrcy - 1, tex, 0.6f));
                                tex.SetPixel(itrcx - 3, itrcy, GetRecolored(itrcx - 3, itrcy, tex, 0.5f));
                                tex.SetPixel(itrcx - 1, itrcy, GetRecolored(itrcx - 1, itrcy, tex, 1.5f));
                                tex.SetPixel(itrcx + 1, itrcy, GetRecolored(itrcx + 1, itrcy, tex, 1.5f));
                                tex.SetPixel(itrcx + 3, itrcy, GetRecolored(itrcx + 3, itrcy, tex, 0.5f));
                                tex.SetPixel(itrcx - 3, itrcy + 1, GetRecolored(itrcx - 3, itrcy + 1, tex, 0.8f));
                                tex.SetPixel(itrcx - 2, itrcy + 1, GetRecolored(itrcx - 2, itrcy + 1, tex, 1.3f));
                                tex.SetPixel(itrcx, itrcy + 1, GetRecolored(itrcx, itrcy + 1, tex, 1.5f));
                                tex.SetPixel(itrcx + 2, itrcy + 1, GetRecolored(itrcx + 2, itrcy + 1, tex, 1.5f));
                                tex.SetPixel(itrcx - 3, itrcy + 2, GetRecolored(itrcx - 3, itrcy + 2, tex, 0.8f));
                                tex.SetPixel(itrcx - 2, itrcy + 2, GetRecolored(itrcx - 2, itrcy + 2, tex, 1.2f));
                                tex.SetPixel(itrcx + 1, itrcy + 2, GetRecolored(itrcx + 1, itrcy + 2, tex, 1.5f));
                                tex.SetPixel(itrcx + 3, itrcy + 2, GetRecolored(itrcx + 3, itrcy + 2, tex, 1.5f));
                                tex.SetPixel(itrcx - 3, itrcy + 3, GetRecolored(itrcx - 3, itrcy + 3, tex, 0.3f));
                                tex.SetPixel(itrcx - 2, itrcy + 3, GetRecolored(itrcx - 2, itrcy + 3, tex, 0.5f));
                                tex.SetPixel(itrcx - 1, itrcy + 3, GetRecolored(itrcx - 1, itrcy + 3, tex, 0.6f));
                                tex.SetPixel(itrcx, itrcy + 3, GetRecolored(itrcx, itrcy + 3, tex, 0.5f));
                                tex.SetPixel(itrcx + 2, itrcy + 3, GetRecolored(itrcx + 2, itrcy + 3, tex, 1.5f));
                                currentReds++;
                                for (int i = itrcx - 3; i <= itrcx + 3; i++)
                                {
                                    for (int j = itrcy - 3; j <= itrcy + 3; j++)
                                    {
                                        modifiedLocations.Add(new Tuple<int, int>(i, j));
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("Setting decoy point");
                                tex.SetPixel(itrcx, itrcy, GetRecolored(itrcx, itrcy, tex));
                            }
                        }
                    }
                }
            }
        }
        tex.Apply();
        return tex;
    }

    Color GetRecolored(int a, int b, Texture2D tex) => Recolor(tex.GetPixel(a, b));

    private Color Recolor(Color oldColor)
    {
        Color c = Design.GetClosestColor(1f);
        c.a = oldColor.a;
        return c;
    }

    Color GetRecolored(int a, int b, Texture2D tex, float factor) => Recolor(tex.GetPixel(a, b), factor);

    private Color Recolor(Color oldColor, float factor)
    {
        Color c = Design.GetClosestColor(1f);
        c.a = oldColor.a * factor;
        return c;
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
        mainSpriteRenderer.enabled = true;
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    private Texture2D CreateTexture(string filename = "", Range redA = null, Range redB = null, Range blueA = null, Range blueB = null, bool isRange = false, bool attributeUnderstanding = false)
    {
        if ("" == filename)
            filename = files[current_file] + "_formatted.bed." + chrtype;
        return ReadInFile(filename, redA, redB, blueA, blueB, isRange, attributeUnderstanding);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainSpriteRenderer.enabled = false;
        }
    }

    private void OnGUI()
    {
        if (showUnderstanding)
            GUI.Box(new Rect(0, 0, 400, 40), understandingString);
        GUI.Box(new Rect(mousePosition.x + 15, Screen.height - mousePosition.y + 15, 200, 70), guiText);
    }

    public void SetupCurveComparisonTrial(CurveComparisonTrial curveComparisonTrial)
    {
        mainSpriteRenderer.enabled = true;
        studyTask = Tasks.CurveComparison;
        mainTexture = CreateTexture(curveComparisonTrial.ReferenceChromosome);
        redTexture = CreateTexture(curveComparisonTrial.RedChromosome);
        blueTexture = CreateTexture(curveComparisonTrial.BlueChromosome);

        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 3);
        redSpriteRenderer = DisplayTexture(redTexture, redSprite, redSpriteRenderer, 3);
        blueSpriteRenderer = DisplayTexture(blueTexture, blueSprite, blueSpriteRenderer, 3);
    }

    public void SetupPointDistanceTrial(PointDistanceTrial pdt)
    {
        mainSpriteRenderer.enabled = true;
        studyTask = Tasks.PointDistance;
        Range redA = new Range(pdt.RedA);
        Range redB = new Range(pdt.RedB);
        Range blueA = new Range(pdt.BlueA);
        Range blueB = new Range(pdt.BlueB);
        mainTexture = CreateTexture(pdt.Filenametwodim, redA, redB, blueA, blueB);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    public void SetupAttributeUnderstandingTrial(AttributeUnderstandingTrial adt)
    {
        mainSpriteRenderer.enabled = true;
        studyTask = Tasks.AttributeUnderstanding;
        mainTexture = CreateTexture(adt.Filenametwodim, attributeUnderstanding: true);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
        understandingString = adt.Question;
        showUnderstanding = true;
    }

    public void SetupSegmentDistanceTrial(SegmentDistanceTrial sdt)
    {
        mainSpriteRenderer.enabled = true;
        studyTask = Tasks.SegmentDistance;
        Range redA = new Range(sdt.RedA);
        Range redB = new Range(sdt.RedB);
        Range blueA = new Range(sdt.BlueA);
        Range blueB = new Range(sdt.BlueB);
        mainTexture = CreateTexture(sdt.Filenametwodim, redA, redB, blueA, blueB, true);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    public void SetupTouchingSegments(TouchingPointsTrial tst)
    {
        mainSpriteRenderer.enabled = true;
        studyTask = Tasks.TouchingSegments;
        mainTexture = ReadInFile(tst.Filenametwodim, redCount: tst.Count, skip:1);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    public void SetupLargerTadTrial(LargerTadTrial ltt)
    {
        mainSpriteRenderer.enabled = true;
        studyTask = Tasks.LargerTad;
        mainTexture = CreateTexture(ltt.Filenametwodim);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }

    public void SetupTripleTrial(TripleTrial tt)
    {
        mainSpriteRenderer.enabled = true;
        studyTask = Tasks.Triple;
        mainTexture = ReadInFile(tt.Filenametwodim, redCount: tt.Count, skip: 1, triple: true);
        mainSpriteRenderer = DisplayTexture(mainTexture, mainSprite, mainSpriteRenderer, 1);
    }
}