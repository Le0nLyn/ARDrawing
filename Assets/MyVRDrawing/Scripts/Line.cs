using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	public LineRenderer lineRenderer;

	public List<Vector3> points;

	void Awake()
	{	
		//链接lineRenderer。
		lineRenderer = this.GetComponent<LineRenderer>();
		points = new List<Vector3>();	
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.L))
		{
			AddPoint(Random.insideUnitCircle*Random.Range(1, 10));
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			SetColor(Random.ColorHSV());			
		}
	}

	public void AddPoint(Vector3 newPoint)
	{
		points.Add(newPoint);
		updateLineRendererPoints();
	}

	public void SetColor(Color color)
	{
		Material mat = new Material(lineRenderer.material);
		mat.SetColor("_Color", color);
		lineRenderer.material = mat;
	}

	private void updateLineRendererPoints()
	{
		Vector3[] newPoints = points.ToArray();
		lineRenderer.positionCount = newPoints.Length;
		lineRenderer.SetPositions(newPoints);

	}
}
