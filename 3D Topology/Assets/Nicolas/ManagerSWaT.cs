using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModelSWaT;
using System;
using System.Linq;

public class ManagerSWaT : MonoBehaviour
{
    [HideInInspector]
    public bool isDataTreated;
    [HideInInspector]
    public ListSensors listSensors;

    public bool fiveSensors;

    public SpawnManagerScriptableObject spawnManagerValues;
    public GameObject entityToSpawn;

    private List<string> namesSensors;
    private List<Sensor> sensors;

    private int instanceNumber = 1;

    // Start is called before the first frame update
    private void Start()
    {
        print("start");
        isDataTreated = false;
        StartCoroutine(LoadingData());
        print("done");
        isDataTreated = true;

        sensors = new List<Sensor>();

        namesSensors = new List<string> { "DPIT301", "LIT401", "FIT504", "FIT201", "MV201" };
        if (fiveSensors)
        {
            CreateSensors(namesSensors);
        }
        else
        {
            sensors = listSensors.Sensors;
        }
        print("time avant spawn: " + Time.realtimeSinceStartup);
        SpawnSensors();
        print("time après spawn: " + Time.realtimeSinceStartup);
        //SpawnOneSensor(sensors[0]);
        //InitiateSensors();

    }

    private void InitiateSensors()
    {
        foreach(Sensor s in sensors)
        {
            GameObject currentSensor = GameObject.Find(s.Name);
            currentSensor.GetComponent<HelixSensor>().SetSensor(s);
            currentSensor.GetComponent<HelixSensor>().SetHelix(50, 27, 10000, 10000);
            currentSensor.GetComponent<HelixSensor>().SetTime(0,10000);
        }
    }

    private void CreateSensors(List<string> sensorsNames)
    {

        IEnumerable<Sensor> query = listSensors.Sensors.Where(sensor => sensorsNames.Contains(sensor.Name));
        List<Sensor> lists = query.ToList();
        sensors = lists;
        //foreach (Sensor s in sensors) { if (s.Name == " MV201") { s.Name = "MV201"; } }
    }

    private void SpawnSensors()
    {
        int currentSpawnPointIndex = 0;
        float x = -.75f;
        float y = 1f; 
        float z = 1.25f; 

        foreach (Sensor s in sensors)
        {


            print(s.Name);


            Vector3 position = new Vector3(x, y, z);
            GameObject currentEntity = Instantiate(entityToSpawn, position, Quaternion.identity);
            currentEntity.GetComponentInChildren<baseHelice>().sensor = s;

            if (x > 2.5f)
            {
                x = 0;
                y += .75f;
            }
            else
            {

                x += .25f;
            }
            // Creates an instance of the prefab at the current spawn point.
            //GameObject currentEntity = Instantiate(entityToSpawn, spawnManagerValues.spawnPoints[currentSpawnPointIndex], Quaternion.identity);

            //// Sets the name of the instantiated entity to be the string defined in the ScriptableObject and then appends it with a unique number. 
            //currentEntity.name = spawnManagerValues.prefabName + instanceNumber;

            //// Moves to the next spawn point index. If it goes out of range, it wraps back to the start.
            //currentSpawnPointIndex = (currentSpawnPointIndex + 1) % spawnManagerValues.spawnPoints.Length;

            //instanceNumber++;

            //currentEntity.GetComponentInChildren<baseHelice>().sensor = s;
        }

        

    }

    private void SpawnOneSensor(Sensor s)
    {
        int currentSpawnPointIndex = 0;
        // Creates an instance of the prefab at the current spawn point.
        GameObject currentEntity = Instantiate(entityToSpawn, spawnManagerValues.spawnPoints[currentSpawnPointIndex], Quaternion.identity);

        // Sets the name of the instantiated entity to be the string defined in the ScriptableObject and then appends it with a unique number. 
        currentEntity.name = spawnManagerValues.prefabName + instanceNumber;

        // Moves to the next spawn point index. If it goes out of range, it wraps back to the start.
        currentSpawnPointIndex = (currentSpawnPointIndex + 1) % spawnManagerValues.spawnPoints.Length;

        instanceNumber++;

        currentEntity.GetComponent<baseHelice>().sensor = s;
    }

    public IEnumerator LoadingData()
    {
        if (fiveSensors)
        { 
            yield return StartCoroutine(LoadJsonData("SWaT3000"));
        }
        else
        {
            yield return StartCoroutine(LoadJsonData("SWaT3000"));

        }
    }

    private T ReadJson<T>(string path) where T : class
    {
        T records;
        TextAsset jsonTextFile = Resources.Load<TextAsset>(path);
        string jsonString = jsonTextFile.text;
        records = JsonUtility.FromJson<T>(jsonString);
        return records;
    }

    public IEnumerator LoadJsonData(string path)
    {

        listSensors = ReadJson<ListSensors>(path);

        while (!isDataTreated)
        {
            yield return null;
        }
        yield return null;
    }

    //public IEnumerator CorPutDataIntoStructures()
    //{
    //List<int> items = new List<int>();
    //List<string> zipCodes = new List<string>(); // Pour compter le nbr de zip différents
    //List<string> occupations = new List<string>(); // Pour compter le nbr d'occupations différentes
    //foreach (Rating r in listeRatings.ratings)
    //{
    //    if (!items.Contains(r.itemID))
    //    {
    //        items.Add(r.itemID);
    //    }
    //}
    //foreach (User u in listeUsers.users)
    //{
    //    if (!zipCodes.Contains(u.zip))
    //    {
    //        zipCodes.Add(u.zip);
    //    }
    //    if (!occupations.Contains(u.occupation))
    //    {
    //        occupations.Add(u.occupation);
    //    }
    //}
    //yield return null;
    //for (int i = 0; i < items.Count; i++)
    //{
    //    List<Rating> m_ratings = new List<Rating>();
    //    List<User> m_users = new List<User>();
    //    foreach (Rating data in listeRatings.ratings)
    //    {
    //        if (data.itemID == items[i])
    //        {
    //            m_ratings.Add(data);
    //        }
    //    }


    //    foreach (Rating r in m_ratings)
    //    {
    //        foreach (User data in listeUsers.users)
    //        {
    //            if (r.userID == data.userID)
    //            {
    //                m_users.Add(data);
    //                break;
    //            }
    //        }
    //    }


    //    movies.Add(new Movie(items[i], m_ratings.ToList(), m_users.ToList()));

    //    yield return null;

    //}

    //isDataTreated = true;
    //testLoadingBar = 1;
    //yield return null;
    //}
}
