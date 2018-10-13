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
    private ArrayList InfoOfLines;
    private Line line;

    // status value
    private int drawingMode;
    private bool []lineRendererStatus;

    public void onClickLayer1()
    {
        SetOnOffLineRendererStatus(1);
    }
    public void onClickLayer2()
    {
        SetOnOffLineRendererStatus(2);
    }
    public void onClickLayer3()
    {
        SetOnOffLineRendererStatus(3);
    }
    public void onClickLayer4()
    {
        SetOnOffLineRendererStatus(4);
    }

    public void onClickDraw1()
    {
        if (drawingMode != 0)
            SetOffDrawingMode();

        SetOnDrawingMode(1);
    }
    public void onClickDraw2()
    {
        if (drawingMode != 0)
            SetOffDrawingMode();

        SetOnDrawingMode(2);
    }
    public void onClickDraw3()
    {
        if (drawingMode != 0)
            SetOffDrawingMode();

        SetOnDrawingMode(3);
    }
    public void onClickDraw4()
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
        drawingMode = 0;
        if (line == null) return;
        if( line.GetLineSize() > 1 ) InfoOfLines.Add(line);
    }
    public void DoSaveLineInfo()
    {
        using (StreamWriter streamWriter = new StreamWriter(fileLink)) 
        {
            foreach( Line i in InfoOfLines )
            {
                string saveLine = "";
                for( int j = 0; j < i.GetLineSize(); j++ )
                {
                    saveLine += i.GetNode(j).x + " " + i.GetNode(j).y + " " + i.GetNode(j).z + " ";
                }

                //Debug.Log(saveLine);
                streamWriter.WriteLine( saveLine );
            }
        }
    }
    public void DoLoadLineInfo()
    {

        string readLine;

        using (StreamReader streamReader = new StreamReader(fileLink))
        {
            while ( ( readLine = streamReader.ReadLine() ) != null )
            {
                //Debug.Log("read data : " + readLine);

                //split data
                string[] subStrings = readLine.Split();

                // check its validity
                if (subStrings.Length < 2) continue;

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
                //SetOnDrawingMode(index);

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

        Debug.Log("SetOnOffLineRendererStatus's index : " + index);

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

    void Start ()
    {
        // initial arrayList
        ArrayofCirclePalette = new ArrayList[MAX_LAYER];
        ArrayofLinePalette = new ArrayList[MAX_LAYER];
        for (int i = 0; i < MAX_LAYER; i++)
        {
            ArrayofCirclePalette[i] = new ArrayList();
            ArrayofLinePalette[i] = new ArrayList();
        }

        InfoOfLines = new ArrayList();

        // initial status variable
        drawingMode = 0;
        lineRendererStatus = new bool[MAX_LAYER];

	}

    void Update ()
    {
        if ( drawingMode > 0 )
        {
            if( Input.GetMouseButtonUp(0) )
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if ( Physics.Raycast(ray, out hit, Mathf.Infinity) )
                {
                    line.AddNodes( new Vector3( hit.point.x, hit.point.y, hit.point.z ));

                    DrawCircle( line.GetLineSize() );
                    DrawLine( line.GetLineSize() );

                    Debug.Log("Draw Circle in " + drawingMode + " layer.");
                }
            }
        }
	}
}
