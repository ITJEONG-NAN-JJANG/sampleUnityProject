using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawManager : MonoBehaviour
{
    // file link
    public static string fileLink = "C:\\Temp\\data.dat";
    public static int MAX_LAYER = 4;

    // circle palette info
    public ArrayList []ArrayofCirclePalette;
    public GameObject CirclePalettePrefab;
    private GameObject circlePalette;
    private DrawCircle drawCircle;

    // line palette info
    public ArrayList []ArrayofLinePalette;
    public GameObject LinePalettePrefab;
    private GameObject linePalette;
    private DrawLine drawLine;

    // line info
    private ArrayList []InfoOfLines;
    private Line line;

    // status value
    private int drawingMode;
    private bool []lineRendererStatus;
    private bool popUpStatus;

    // popUp
    private GameObject popUpCircle;
    private GameObject popUpLine;
    private ArrayList selectInfo;

    public void OnClickLayer1()
    {
        SetOnOffLineRendererStatus(1);
    }
    public void OnClickLayer2()
    {
        SetOnOffLineRendererStatus(2);
    }
    public void OnClickLayer3()
    {
        SetOnOffLineRendererStatus(3);
    }
    public void OnClickLayer4()
    {
        SetOnOffLineRendererStatus(4);
    }

    public void OnClickDraw1()
    {
        if (drawingMode != 0)
            SetOffDrawingMode();

        SetOnDrawingMode(1);
    }
    public void OnClickDraw2()
    {
        if (drawingMode != 0)
            SetOffDrawingMode();

        SetOnDrawingMode(2);
    }
    public void OnClickDraw3()
    {
        if (drawingMode != 0)
            SetOffDrawingMode();

        SetOnDrawingMode(3);
    }
    public void OnClickDraw4()
    {
        if (drawingMode != 0)
            SetOffDrawingMode();

        SetOnDrawingMode(4);
    }

    private void SetOnDrawingMode(int index)
    {
        drawingMode = index;
        line = new Line();

        SetLinePalette();
    }
    public void SetOffDrawingMode()
    {
        if (line == null || drawingMode == 0) return;

        if ( line.GetLineSize() > 1 ) InfoOfLines[drawingMode-1].Add(line);

        drawingMode = 0;

    }
    public void DoSaveLineInfo()
    {
        using (StreamWriter streamWriter = new StreamWriter(fileLink)) 
        {
            for (int k = 0; k < MAX_LAYER; k++)
            {
                streamWriter.WriteLine("Layer");

                foreach (Line i in InfoOfLines[k])
                {
                    string saveLine = "";
                    for (int j = 0; j < i.GetLineSize(); j++)
                    {
                        saveLine += i.GetNode(j).x + " " + i.GetNode(j).y + " " + i.GetNode(j).z + " ";
                    }

                    //Debug.Log(saveLine);
                    streamWriter.WriteLine(saveLine);
                }
            }
        }
    }
    public void DoLoadLineInfo()
    {
        string readLine;

        int indexer = 0;

        using (StreamReader streamReader = new StreamReader(fileLink))
        {
            while ( ( readLine = streamReader.ReadLine() ) != null )
            {
                // Debug.Log("read data : " + readLine);

                //split data
                string[] subStrings = readLine.Split();

                // check its validity
                if (subStrings[0].Contains("Layer"))
                {
                    indexer++;
                    continue;
                }
                else if (subStrings.Length < 2) continue;

                // convert string type to float type
                float[] subFloats = new float[subStrings.Length];

                int i = 0; 
                foreach( string subString in subStrings )
                {
                    if( ! subString.Equals("") )
                    {
                        subFloats[i++] = System.Convert.ToSingle(subString);
                    }
                }

                // draw using imported float values
                SetOffDrawingMode();
                SetOnDrawingMode(indexer);

                for( int j = 0; j < i/3; j++ )
                {
                    line.AddNodes(new Vector3(subFloats[j * 3], subFloats[j * 3 + 1], subFloats[j * 3 + 2]));

                    DrawCircle(j+1);
                    DrawLine(j+1);
                }
            }

            SetOffDrawingMode();
        }
    }
    private void SetOnOffLineRendererStatus(int index)
    {
        index--;

        //Debug.Log("SetOnOffLineRendererStatus's index : " + index);

        if(lineRendererStatus[index])
        {
            lineRendererStatus[index] = false;
        }
        else
        {
            lineRendererStatus[index] = true;
        }

        SetShowHideLineRenderer(index);
    }
    private void SetShowHideLineRenderer(int index)
    {
        if (ArrayofLinePalette.Length == 0 || ArrayofCirclePalette.Length == 0 ) return;

        Renderer renderer;

        foreach( GameObject linePaletteObject in ArrayofLinePalette[index] )
        {
            renderer = linePaletteObject.GetComponent<Renderer>();
            renderer.enabled = lineRendererStatus[index];
        }
        foreach( GameObject circlePaletteObject in ArrayofCirclePalette[index] )
        {
            renderer = circlePaletteObject.GetComponent<Renderer>();
            renderer.enabled = lineRendererStatus[index];

        }
    }

    public void DrawCircle( int index )
    {
        //GameObject circlePalette = transform.Find("CirclePalette").gameObject;
        circlePalette = Instantiate(CirclePalettePrefab, this.transform);
        ArrayofCirclePalette[drawingMode-1].Add(circlePalette);
        drawCircle = (DrawCircle)circlePalette.GetComponent(typeof(DrawCircle));
        drawCircle.Draw( line.GetNode(index-1) );
    }
    public void SetLinePalette()
    {
        linePalette = Instantiate(LinePalettePrefab, this.transform);
        ArrayofLinePalette[drawingMode-1].Add(linePalette);
        drawLine = (DrawLine)linePalette.GetComponent(typeof(DrawLine));
        drawLine.SetLineRenderer();
    }
    public void DrawLine( int index )
    {
        drawLine.AppendLine(index, line.GetNode(index-1) );
    }

    // pop-up : circle
    public void SetPopUpCircleOn(Vector3 position)
    {
        popUpStatus = true;
        popUpCircle.SetActive(popUpStatus);
        Vector3 popUpStatusPosition = popUpCircle.transform.position;
        popUpStatusPosition.x = position.x;
        popUpStatusPosition.y = position.y;
        popUpStatusPosition.z = 0;
        popUpCircle.transform.position = popUpStatusPosition;
    }
    public void SetPopUpCircleOff( )
    {
        // Debug.Log("[CLICK] CANCEL CIRCLE");
        popUpStatus = false;
        popUpCircle.SetActive(popUpStatus);
        if( selectInfo == null)
            selectInfo.Clear();
    }
    public void OnClickMoveCircle()
    {

    }
    public void OnClickDeleteCircle()
    {

    }
    public void OnClickCancelCircle()
    {
        SetPopUpCircleOff();
    }
    private ArrayList IsOnCircle(Vector3 clickPosition)
    {
        ArrayList answer = new ArrayList();
        for (int k = 0; k < MAX_LAYER; k++)
        {
            if (!lineRendererStatus[k]) continue;

            foreach (Line i in InfoOfLines[k])
            {
                for (int j = 0; j < i.GetLineSize(); j++)
                {
                    // Debug.Log("[VS0] " + clickPosition.x + ", " + clickPosition.y + ", " + clickPosition.z);
                    // Debug.Log("[VS1] " + i.GetNode(j).x + ", " + i.GetNode(j).y + ", " + i.GetNode(j).z);
                    if (IsInCircleLocation(clickPosition, i.GetNode(j)))
                    {
                        // Debug.Log("[TRUE]");
                        answer.Add(k);
                        answer.Add(i);
                        answer.Add(j);
                        return answer;
                    }
                }
            }
        }
        // Debug.Log("[FALSE]");
        return null;
    }
    private bool IsInCircleLocation(Vector3 click, Vector3 pos)
    {
        const float marginError = 2.0f;
        const float realRadius = 1.0f;

        if ( (pos.x - click.x) * (pos.x - click.x) + (pos.y - click.y) * (pos.y - click.y) <= (marginError + realRadius) * (marginError + realRadius) )
            return true;
        else
            return false;
    }

    // pop-up : line
    public void SetPopUpLineOn(Vector3 position)
    {
        popUpStatus = true;
        popUpLine.SetActive(popUpStatus);
        Vector3 popUpStatusPosition = popUpLine.transform.position;
        popUpStatusPosition.x = position.x;
        popUpStatusPosition.y = position.y;
        popUpStatusPosition.z = 0;
        popUpLine.transform.position = popUpStatusPosition;
    }
    public void SetPopUpLineOff()
    {
        // Debug.Log("[CLICK] CANCEL LINE");
        popUpStatus = false;
        popUpLine.SetActive(popUpStatus);
        if (selectInfo == null)
            selectInfo.Clear();
    }
    public void OnClickMoveLine()
    {

    }
    public void OnClickDeleteLine()
    {

    }
    public void OnClickCancelLine()
    {
        SetPopUpLineOff();
    }
    private ArrayList IsOnLine(Vector3 clickPosition)
    {
        ArrayList answer = new ArrayList();

        for (int k = 0; k < MAX_LAYER; k++)
        {
            if (!lineRendererStatus[k]) continue;

            foreach (Line i in InfoOfLines[k])
            {
                for (int j = 0; j < i.GetLineSize()-1; j++)
                {
                    // Debug.Log("[VS0] " + clickPosition.x + ", " + clickPosition.y + ", " + clickPosition.z);
                    // Debug.Log("[VS1] " + i.GetNode(j).x + ", " + i.GetNode(j).y + ", " + i.GetNode(j).z);
                    // Debug.Log("[VS2] " + i.GetNode(j+1).x + ", " + i.GetNode(j+1).y + ", " + i.GetNode(j+1).z);
                    if (IsInLineLocation(clickPosition, i.GetNode(j), i.GetNode(j + 1)))
                    {
                        // Debug.Log("[TRUE]");
                        answer.Add(k);
                        answer.Add(i);
                        answer.Add(j);
                        return answer;
                    }
                }
            }
        }
        // Debug.Log("[FALSE]");
        return null;
    }
    private bool IsInLineLocation(Vector3 click, Vector3 first, Vector3 last)
    {
        const float marginofError = 7.5f;

        if( ( first.x > last.x ? last.x : first.x ) - marginofError <= click.x
          &&( first.x > last.x ? first.x : last.x ) + marginofError >= click.x
          &&( first.z > last.y ? last.z : first.z ) - marginofError <= click.z
          &&( first.z > last.z ? first.z : last.z ) + marginofError >= click.z
          //&&( first.z - last.z - marginofError*2 ) / ( first.x - last.x + marginofError*2 ) >= ( first.z - click.z ) / ( first.x - click.x )
          //&&( first.z - last.z + marginofError*2 ) / ( first.x - last.x - marginofError*2 ) <= ( first.z - click.z ) / ( first.x - click.x )
          //&&( first.z - last.z - marginofError*2 ) / ( first.x - last.x + marginofError*2 ) >= ( last.z - click.z ) / ( last.x - click.x )
          //&&( first.z - last.z + marginofError*2 ) / ( first.x - last.x - marginofError*2 ) <= ( last.z - click.z ) / ( last.x - click.x )
          )
            return true;
        else
            return false;
    }

    void Start ()
    {
        // initial arrayList
        ArrayofCirclePalette = new ArrayList[MAX_LAYER];
        ArrayofLinePalette = new ArrayList[MAX_LAYER];
        InfoOfLines = new ArrayList[MAX_LAYER];

        for (int i = 0; i < MAX_LAYER; i++)
        {
            ArrayofCirclePalette[i] = new ArrayList();
            ArrayofLinePalette[i] = new ArrayList();
            InfoOfLines[i] = new ArrayList();
        }

        // initial status variable
        drawingMode = 0;
        lineRendererStatus = new bool[MAX_LAYER];

        // popUp Set
        popUpStatus = false;
        popUpCircle = GameObject.Find("PopUpCircle");
        popUpLine = GameObject.Find("PopUpLine");

        popUpCircle.SetActive(false);
        popUpLine.SetActive(false);
    }

    void Update ()
    {
        // Debug.Log("[REAL]" + popUpCircle.transform.position.x + ", " + popUpCircle.transform.position.y);
        // Debug.Log("[STATUS] " + popUpCircle.activeSelf );
        if( Input.GetMouseButtonDown(0) )
        {
            Vector3 mousePosition = Input.mousePosition;
            if (drawingMode > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    line.AddNodes(new Vector3(hit.point.x, hit.point.y, hit.point.z));

                    DrawCircle(line.GetLineSize());
                    DrawLine(line.GetLineSize());

                    //Debug.Log("Draw Circle in " + drawingMode + " layer.");
                }
            }
            else if( !popUpStatus )
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    selectInfo = IsOnCircle(new Vector3(hit.point.x, hit.point.y, hit.point.z));

                    if (selectInfo != null)
                    {
                        Camera camera = GetComponent<Camera>();
                        Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(mousePosition);
                        //Debug.Log("[CIRCLE] " + pos.x + ", " + pos.y);
                        SetPopUpCircleOn(new Vector3(pos.x, pos.y, 0));
                    }
                    else
                    {
                        selectInfo = IsOnLine(new Vector3(hit.point.x, hit.point.y, hit.point.z));

                        if (selectInfo != null)
                        {
                            Camera camera = GetComponent<Camera>();
                            Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(mousePosition);
                            //Debug.Log("[LINE] " + pos.x + ", " + pos.y);
                            SetPopUpLineOn(new Vector3(pos.x, pos.y, 0));
                        }
                    }
                    /*
                    if (!popUpStatus)
                    {
                        Camera camera = GetComponent<Camera>();
                        Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                        Debug.Log("[CLICK] " + pos.x + ", " + pos.y); 
                        //SetPopUpOn(new Vector3(pos.x, pos.y, 0));
                    }
                    */
                }
            }
        }
	}
}
