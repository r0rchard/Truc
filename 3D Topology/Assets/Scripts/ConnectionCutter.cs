using DataTransmission;
using Networking;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ConnectionCutter : InteractionTool
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody)
        {
            Pipeline pipeline = other.attachedRigidbody.GetComponent<Pipeline>();
            if (pipeline)
            {
                StartCoroutine(DeleteAndDestroy(pipeline));
            }
        }
    }

    IEnumerator DeleteAndDestroy(Pipeline pipeline)
    {
        if (CyberRangeInterface.Instance.OnlineMode)
        {
            Thread thread = new Thread(delegate ()
            {
                CyberRangeInterface.Instance.DeleteConnection(pipeline);
            });
            thread.Start();

            yield return new WaitUntil(() => !thread.IsAlive);
        }

        PipelineManager.Instance.NetworkRemovePipeline(pipeline.Identifier);
    }
}
