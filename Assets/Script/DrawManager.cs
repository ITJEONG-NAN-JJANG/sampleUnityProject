using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    // file link
    public static string fileLink = "C:\\Temp\\data.dat";

    // circle palette info
    public ArrayList ArrayofCirclePalette;
    public GameObject CirclePalettePrefab;
    private GameObject circlePalette;
    private DrawCircle drawCircle;

    // line palette info
    public ArrayList ArrayofLinePalette;
    public GameObject LinePalettePrefab;
    private GameObject linePalette;
    private DrawLine drawLine;

    // line info
    private ArrayList InfoOfLines;
    private Line line;

    // bool
    private bool drawingMode;
    private bool lineRendererStatus;

    public void SetOnDrawingMode()
    {
        drawingMode = true;
        line = new Line();

        ArrayofLinePalette = new ArrayList();
        ArrayofCirclePalette = new ArrayList();

        SetLinePalette();
    }
    public void SetOffDrawingMode()
    {
        drawingMode = false;
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
                SetOnDrawingMode();

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
    public void SetOnOffLineRendererStatus()
    {
        if(lineRendererStatus)
        {
            lineRendererStatus = false;
        }
        else
        {
            lineRendererStatus = true;
        }

        SetShowHideLineRenderer();
    }
    public void SetShowHideLineRenderer()
    {
        Renderer renderer;

        foreach( GameObject linePaletteObject in ArrayofLinePalette )
        {
            renderer = linePaletteObject.GetComponent<Renderer>();
            renderer.enabled = lineRendererStatus;
        }
        foreach( GameObject circlePaletteObject in ArrayofCirclePalette )
        {
            renderer = circlePaletteObject.GetComponent<Renderer>();
            renderer.enabled = lineRendererStatus;

        }
    }

    public void DrawCircle( int index )
    {
        //GameObject circlePalette = transform.Find("CirclePalette").gameObject;
        circlePalette = Instantiate(CirclePalettePrefab, this.transform);
        ArrayofCirclePalette.Add(circlePalette);
        drawCircle = (DrawCircle)circlePalette.GetComponent(typeof(DrawCircle));
        drawCircle.Draw( line.GetNode(index-1) );
    }
    public void SetLinePalette()
    {
        linePalette = Instantiate(LinePalettePrefab, this.transform);
        ArrayofLinePalette.Add(linePalette);
        drawLine = (DrawLine)linePalette.GetComponent(typeof(DrawLine));
        drawLine.SetLineRenderer();
    }
    public void DrawLine( int index )
    {
        drawLine.AppendLine(index, line.GetNode(index-1) );
    }

    void Start ()
    {
        InfoOfLines = new ArrayList();

        drawingMode = false;
        lineRendererStatus = true;

	}
	void Update ()
    {
        if ( drawingMode )
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
                }
            }
        }
	}
}
