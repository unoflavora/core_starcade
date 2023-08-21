using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDraw : MonoBehaviour 
{
    Coroutine drawing;
    [SerializeField] GameObject lineObject;

    private GameObject currentLine;

    private void Update() 
    {
       if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }

        if(Input.GetMouseButtonUp(0))
        {
            FinishLine();
        }
    }

    void StartLine()
    {
        if(drawing != null)
        {
            StopCoroutine(drawing);
        }
        drawing = StartCoroutine(DrawLine());
    }

    void FinishLine()
    {
        StopCoroutine(drawing);
        Destroy(currentLine);
    }

    IEnumerator DrawLine()
    {
        currentLine = Instantiate(lineObject, Vector3.zero, Quaternion.identity, transform);
        LineRenderer line = currentLine.GetComponent<LineRenderer>();
        line.positionCount = 0;

        while(true)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, pos);
            yield return null;
        }
    }
}