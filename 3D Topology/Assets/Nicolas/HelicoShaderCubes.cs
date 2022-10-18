using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;

public class HelicoShaderCubes : MonoBehaviour
{

    public Shader shader;
    public float[] sensorValues;
    public string name;
    public Gradient g;

    public Slider sliderPeriod;
    /*public Slider sliderStart;
    public Slider sliderLenght;*/

    public Text textPeriod;

    public float rayon = .5f;
    public float period = 2000;
    public float nbrPoints = 36000;
    public float height = 2;
    void OnEnable()
    {
        StartCoroutine(WaitForSensorValues());
    }

    public void UpdateMesh()
    {
        MoveMeshHelice(rayon, sliderPeriod.value, nbrPoints, height, 0, 0) ;
        textPeriod.text = sliderPeriod.value.ToString();
    }
    Mesh CreateMesh()
    {
        Vector3[] points;
        points = new Vector3[2];
        points[0] = new Vector3(0, 0, 0);
        points[1] = new Vector3(10, 0, 0);

        int[] indices = new int[2]; 
        indices[0] = 0;
        indices[1] = 1;
        Color[] colors = new Color[2];
        colors[0] = Color.red;
        colors[1] = Color.blue;


        Mesh mesh = new Mesh();
        mesh.vertices = points;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        mesh.colors = colors;
        return mesh;
    }

    void MoveMeshHelice(float R, float p, float nb, float h, float startTime, float lenght)
    {
        Vector3[] points = new Vector3[(int)nb];
        int[] indices = new int[(int)nb];
        Color[] colors = new Color[(int)nb];

        int latency = (int)(startTime * 60); // time en minutes
        int plage = (int)(lenght * 60); // time en minutes

        for (int k = 0; k < nb; k++)
        {
            Vector3 position = ToCartesian(((k / nb) * h), R, -2 * Mathf.PI * (k / p));
            points[k] = position;
        }
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = points;
       
    }
    Mesh CreateMeshHelice(float R, float p, float nb, float h)
    {
        Vector3[] points = new Vector3[(int)nb];
        int[] indices = new int[(int)nb];
        Color[] colors = new Color[(int)nb];
       
        float Max = sensorValues.Max();
        if (Max > 0)
        {
            for (int k = 0; k < nb; k++)
            {
                Vector3 position = ToCartesian(((k / nb) * h), R, -2 * Mathf.PI * (k / p));
                points[k] = position;
                indices[k] = k;

                colors[k] = g.Evaluate((sensorValues[k]) / (Max));
            }
        }
       
        Mesh mesh = new Mesh();
        mesh.vertices = points;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        mesh.colors = colors;
        //AssetDatabase.CreateAsset( mesh, "Assets/_CyberShader/"+name+".asset");
        //AssetDatabase.SaveAssets();
        return mesh;
    }
    private float ColorPicker(float value, float max)
    {
        return (value / max);
    }

    private Vector3 ToCartesian(float height, float radius, float theta)
    {
        Vector3 position = new Vector3(radius * Mathf.Cos(theta), height, radius * Mathf.Sin(theta));
        return position;
    }

    private Vector3 ToCylinder(Vector3 position)
    {
        float radius;
        float theta;
        float height;

        radius = Mathf.Sqrt(position.x * position.x + position.z * position.z);
        height = position.y;

        if (position.x == 0 && position.z == 0)
        {
            theta = 0;
        }
        else if (position.x >= 0)
        {
            theta = Mathf.Asin(position.z / radius);
        }
        else if (position.x < 0)
        {
            theta = -Mathf.Asin(position.z / radius) + Mathf.PI;
        }
        else { theta = 0; }

        Vector3 positionCyl = new Vector3(radius, theta, height);

        return positionCyl;
    }


    IEnumerator WaitForSensorValues()
    {

        yield return new WaitUntil(() => sensorValues != null);
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshFilter>().mesh = CreateMeshHelice(rayon, period, nbrPoints, height);
        Renderer rend = gameObject.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Unlit/CubeShader");

        /*sliderLenght = GameObject.Find("SliderLenght").GetComponent<Slider>();
        sliderStart = GameObject.Find("SliderStart").GetComponent<Slider>();*/

        sliderPeriod.onValueChanged.AddListener(delegate { UpdateMesh(); });
        /*sliderStart.onValueChanged.AddListener(delegate { UpdateMesh(); });
        sliderLenght.onValueChanged.AddListener(delegate { UpdateMesh(); });*/
    }
}
