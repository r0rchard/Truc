using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cellule
{
    public int Time;
    public string Source;
    public string Destination;
    public int Count;
}

public class Datas
{
    public List<Cellule> data;
}

[System.Serializable]
public class Informations
{
    public List<int> Times;
    public List<string> Sources;
    public List<string> Destinations;
}