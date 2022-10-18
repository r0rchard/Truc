using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeName : MonoBehaviour
{
    public TMP_Text nodeName;
    // Start is called before the first frame update
    public void SetName(string name)
    {
        nodeName.text = name;
    }
}
