using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    private ArrayList infoOfNodes;

    public Line()
    {
        infoOfNodes = new ArrayList();
    }

    public void AddNodes( Vector3 newNode )
    {
        infoOfNodes.Add(newNode);

        Debug.Log( GetLineSize() + " node : " + newNode.x + ", " + newNode.y + ", " + newNode.z );
    }
    public Vector3 GetNode(int index)
    {
        return (Vector3)infoOfNodes[index];
    }
    public ArrayList GetNodes()
    {
        return infoOfNodes;
    }
    public int GetLineSize()
    {
        return infoOfNodes.Count; 
    }
}
