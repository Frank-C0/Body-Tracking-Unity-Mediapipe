using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

public class AnimationCode : MonoBehaviour
{
    public bool isActivated = false;
    public GameObject[] Body;
    List<string> lines;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        lines = System.IO.File.ReadLines("Assets/AnimationFile.txt").ToList();
    }

    private Vector3 offset = new Vector3(0.278629512f, -3.55589509f, 0.14224191f) - new Vector3(6.1500001f, 0.510000014f, 1.15999997f);

    // Update is called once per frame
    void Update()
    {
        if (!isActivated) { return; }
        string[] points = lines[counter].Split(',');

        for (int i = 0; i <= 32; i++)
        {
            float x = float.Parse(points[0 + (i * 3)]) / 100;
            float y = float.Parse(points[1 + (i * 3)]) / 100;
            float z = float.Parse(points[2 + (i * 3)]) / 300;
            Body[i].transform.localPosition = new Vector3(x, y, z) + offset;
        }


        counter += 1;
        if (counter == lines.Count) { counter = 0; }
        Thread.Sleep(30);
    }
}