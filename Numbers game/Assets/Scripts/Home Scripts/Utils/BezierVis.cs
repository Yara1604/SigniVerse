using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class BezierVis : MonoBehaviour
{
    // Some control points for testing
    // We will allow inserting new control points by double clicking
    // on the screen
    public List<Vector2> controlPoints = new List<Vector2>()
    {
        new Vector2(-4.0f, -4.0f),
        new Vector2(0.0f, 2.0f),
        new Vector2(5.0f, 2.0f),
    };

    // Reference to the point prefab to represent EACH control point
    public GameObject PointPrefab;

    // LineRenderer is used to show the straight lines connecting
    // the control points, and also the bezier curve itself.
    LineRenderer[] mLineRenderers = null;

    // A game object to represent each control point, so we can move them around
    List<GameObject> mControlPointObjects = new List<GameObject>();
    
    // Store properties of the line
    public float LineWidth;
    public float LineWidthBezier;
    public Color LineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color BezierCurveColor = new Color(0.5f, 0.6f, 0.8f, 0.8f);

    // Create the line renderer
    private LineRenderer createLine()
    {
        GameObject obj = new GameObject();
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = LineColor;
        lr.endColor = LineColor;
        lr.startWidth = LineWidth;
        lr.endWidth = LineWidth;
        return lr;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mLineRenderers = new LineRenderer[2];
        mLineRenderers[0] = createLine();
        mLineRenderers[1] = createLine();

        mLineRenderers[0].gameObject.name = "LineRenderer_obj_0";
        mLineRenderers[1].gameObject.name = "LineRenderer_obj_1";

        // create the instances of control points
        for (int i = 0; i < controlPoints.Count; i++)
        {
            GameObject obj = Instantiate(PointPrefab, controlPoints[i], Quaternion.identity);
            obj.name = $"ControlPoint_{i}";
            mControlPointObjects.Add(obj);
        }

    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = mLineRenderers[0];
        LineRenderer curveRenderer = mLineRenderers[1];

        List<Vector2> pts = new List<Vector2>();
        for (int i = 0; i < mControlPointObjects.Count; i++)
        {
            pts.Add(mControlPointObjects[i].transform.position);
        }

        // set the linerenderer for showing straight lines
        // between the control points
        lineRenderer.positionCount = pts.Count;
        for (int i = 0; i < pts.Count; i++)
        {
            lineRenderer.SetPosition(i, pts[i]);
        }

        // Now for drawing the bezier curves between the lines
        List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
        curveRenderer.startColor = BezierCurveColor;
        curveRenderer.endColor = BezierCurveColor;
        curveRenderer.startWidth = LineWidthBezier;
        curveRenderer.endWidth = LineWidthBezier;
        curveRenderer.positionCount = curve.Count;

        for(int i=0; i < curve.Count; i++)
        {
            curveRenderer.SetPosition(i, curve[i]);
        }
    }

    // Now adding functionality to add new control points
    // by double clicking on the screen
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isMouse)
        {
            if (e.clickCount == 2 && e.button == 0)
            {
                Vector3 screenPos = new Vector3(e.mousePosition.x, Screen.height - e.mousePosition.y, 0);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                Vector2 rayPos = new Vector2(worldPos.x, worldPos.y);
                InsertNewControlPoint(rayPos);

            }
        }
    }

    void InsertNewControlPoint(Vector2 p)
    {
        if(mControlPointObjects.Count >= 18)
        {
            Debug.Log("You have used more than 18 control points.");
            return;
        }

        GameObject obj = Instantiate(PointPrefab, p, Quaternion.identity);
        obj.name = $"ControlPoint_{mControlPointObjects.Count}";
        mControlPointObjects.Add(obj);
    }
}
