using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    public float thetaInterval = .1f;
    public float rad = 1f;
    
    public void Draw( Vector3 firstPoint )
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3 initPos = Vector3.zero;

        int index = 0;

        lineRenderer.positionCount = ((int)(2 * Mathf.PI / thetaInterval) + 2);

        for( float theta = 0f; theta < ( 2 * Mathf.PI); theta += thetaInterval )
        {
            float x = (rad * 5) * Mathf.Cos(theta);
            float z = (rad * 5) * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x + firstPoint.x, 1 + firstPoint.y, z + firstPoint.z );

            lineRenderer.SetPosition( index, pos );

            if (index++ == 0) initPos = pos; 

        }

        lineRenderer.SetPosition(index, initPos);

        // Debug.Log("circle created in " + firstPoint.x + ", " + firstPoint.y + ", " + firstPoint.z);
    }

}
