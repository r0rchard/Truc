using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Matrice : MonoBehaviour
{
 
    public Gradient gradient;
    public Shader shader;
    public Material dataMaterial;
    //public ClippingPlane clipPlane;
    public GameObject dataPoint;
    public GameObject labelPrefab;

    public int pasAffiches;
    public bool attaque;

    private Datas datas;
    private Informations infos;
    private int[,,] matrice;
    
    

    
    // Start is called before the first frame update
    void Start()
    {
        if (attaque)
        {
            datas = ReadJson<Datas>("exportweek2InsideTCP");
            infos = ReadJson<Informations>("Informationsweek2InsideTCP");
        }
        else
        {
            datas = ReadJson<Datas>("exportTCP1Week");
            infos = ReadJson<Informations>("InformationsTCP1Week");
        }
        matrice = MatriceCreation(datas);
        int[] minMax;

        // Find all assets labelled with 'Piano' :
        //var notes = AssetDatabase.FindAssets("l:Piano");

        int affichagepas;
        if (pasAffiches < infos.Times.Count)
        { affichagepas = pasAffiches; }
        else { affichagepas = infos.Times.Count; };

        for (int i = 0; i < affichagepas; i++)
        {
            minMax = MaxMatrice(matrice, i);
            for (int j = 0; j < infos.Sources.Count; j++)
            {
                for (int k = 0; k < infos.Destinations.Count; k++)
                {
                    if (matrice[i, j, k] != 0)
                    {
                        GameObject cube = Instantiate(dataPoint);

                        Material myNewMaterial = new Material(shader);

                        AudioSource audio = cube.GetComponent<AudioSource>();

                        float fraction = ColorPicker((float)matrice[i, j, k] - 1, (float)minMax[1] - 1);

                        cube.GetComponent<Renderer>().material = myNewMaterial;
                        //cube.GetComponentInChildren<Renderer>().material.SetColor("_BaseColor", gradient.Evaluate(fraction));
                        cube.GetComponentInChildren<Renderer>().material.color = gradient.Evaluate(fraction);
                        cube.GetComponentInChildren<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        cube.GetComponentInChildren<Renderer>().receiveShadows = false;
                        //clipPlane.addMaterial(cube.GetComponent<Renderer>().material);
                        cube.transform.position = new Vector3(i * 2, j, k);
                        cube.transform.localScale = new Vector3(fraction, fraction, fraction);
                        cube.transform.SetParent(transform, true);
                        cube.GetComponent<DataPoint>().SetValue(matrice[i, j, k]);

                        int playedSound = Mathf.RoundToInt(fraction * 87); // 88 notes (de 0 à 87)
                        if (playedSound > 87) { playedSound = 87; }

                        /*string path = AssetDatabase.GUIDToAssetPath(notes[playedSound]);
                        path = path.Replace(".mp3", "");
                        path = path.Replace("Assets/Resources/", "");
                        AudioClip audioClip = Resources.Load<AudioClip>(path);
                        audio.clip = audioClip;*/
                    }

                }
            }
        }

        //gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<MeshRenderer>();
        //gameObject.GetComponent<MeshFilter>().mesh = CreateMesh();

        //Renderer rend = gameObject.GetComponent<Renderer>();
        //rend.material.shader = Shader.Find("Unlit/CubeShader");

    }

   Mesh CreateMesh()
    {
        Vector3[] points;
        points = new Vector3[2];
        points[0] = new Vector3(0, 0, 0);
        points[1] = new Vector3(10, 0, 0);

        int[] indices = new int[2]; // 12 triangles à donner ??? <- non 
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

    private int[,,] MatriceCreation(Datas datas)
    {
        int[,,] matrice = new int[infos.Times.Count, infos.Sources.Count, infos.Destinations.Count];
        Debug.Log(infos.Times.Count);
        Debug.Log(infos.Sources.Count);
        Debug.Log(infos.Destinations.Count);
        foreach(Cellule c in datas.data)
        {
            matrice[infos.Times.FindIndex(x => x == c.Time), infos.Sources.FindIndex(x => x == c.Source), infos.Destinations.FindIndex(x=>x==c.Destination)] = c.Count;
            print(c.Count);
        }

        return matrice;
    }

    //private void MatrixDisplay(int[,,] matrice)
    //{
    //    int[] minMax;
    //    for (int i = 0; i < 30; i++)
    //    {
    //        //GameObject go = Instantiate(labelPrefab);
    //        //go.transform.position = new Vector3(i * 2, -1, -1);
    //        //TMP_Text txtMesh = go.transform.GetComponent<TMP_Text>();
    //        //txtMesh.text = i.ToString();

    //        minMax = MaxMatrice(matrice, i);
    //        for (int j = 0; j < matrice.GetLength(1); j++)
    //        {
    //            //GameObject go2 = Instantiate(labelPrefab);
    //            //go2.transform.position = new Vector3(-1, j, -1);
    //            //go2.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);

    //            //TMP_Text txtMesh2 = go2.transform.GetComponent<TMP_Text>();
    //            //txtMesh2.text = listMovieIds[j].ToString();


    //            for (int k = 0; k < matrice.GetLength(2); k++)
    //            {
    //                //GameObject go3 = Instantiate(labelPrefab);
    //                //go3.transform.position = new Vector3(-1, -1, k);
    //                //go3.transform.Rotate(0.0f, 90.0f, 90.0f, Space.Self);
    //                //TMP_Text txtMesh3 = go3.transform.GetComponent<TMP_Text>();
    //                //txtMesh3.text = Enum.GetName(typeof(Occupations), k);

    //                if (matrice[i, j, k] != 0)
    //                {
    //                    GameObject cube = GameObject.Instantiate(dataPoint);
    //                    cube.GetComponent<AlexTooltip>().SetDataPoint(i.ToString(), listMovieIds[j].ToString(), Enum.GetName(typeof(Occupations), k), matrice[i, j, k]);
    //                    cube.transform.position = new Vector3(i * 2, j, k);
    //                    cube.transform.localScale = new Vector3(.8f, .8f, .8f);
    //                    cube.GetComponent<Renderer>().material = newMaterialRef;
    //                    cube.GetComponent<Renderer>().material.color = g.Evaluate(ColorPicker((float)matrice[i, j, k] - 1, (float)minMax[1] - 1));
    //                    cube.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    //                    cube.GetComponent<Renderer>().receiveShadows = false;
    //                }
    //            }
    //        }
    //    }
    //}

    private float ColorPicker(float nbrVote, float max)
    {
        return (nbrVote / max);
    }
    private int[] MaxMatrice(int[,,] matrice, int dimension)
    {

        int max = -1;
        int min = 10000;

        for (int i = 0; i < matrice.GetLength(0); i++)
        {
            for (int j = 0; j < matrice.GetLength(1); j++)
            {
                for (int k = 0; k < matrice.GetLength(2); k++)
                {
                    if (matrice[i, j, k] > max) { max = matrice[i, j, k]; }
                    if (matrice[i, j, k] < min) { min = matrice[i, j, k]; }
                }
            }
        }

        int[] tab = { min, max };

        return tab;
    }
    private T ReadJson<T>(string path) where T : class
    {
        T records;
        TextAsset jsonTextFile = Resources.Load<TextAsset>(path);
        string jsonString = jsonTextFile.text;
        records = JsonUtility.FromJson<T>(jsonString);
        return records;
    }

}
