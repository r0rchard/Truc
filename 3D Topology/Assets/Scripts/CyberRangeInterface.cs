using DataTransmission;
using Parsing.CyberRangeSerialization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CyberRangeInterface
{
    static string _fileName = Application.streamingAssetsPath + "/lade-2.8.5.exe";
    static string _staticArguments = "-c .\\lade_cli.conf --workzone wz-14 --auth-username anthony --base-url https://10.35.114.253 -k --json --auth-password "
        + System.IO.File.ReadAllText(Application.streamingAssetsPath + "\\password.txt");

    static CyberRangeInterface _instance;
    public bool OnlineMode = false;

    public static CyberRangeInterface Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new CyberRangeInterface();
            }
            return _instance;
        }
    }

    public HostsSpecsAsset[] GetAllHosts()
    {
        Process hostGetter = ProcessCreator("host list");
        hostGetter.Start();
        string hostsString = hostGetter.StandardOutput.ReadToEnd();
        hostsString = "{ \"hosts_specs\":" + hostsString + "}";
        HostWrapper hostWrapper = JsonUtility.FromJson<HostWrapper>(hostsString);

        return hostWrapper.hosts_specs;
    }

    public NetworksSpecsAsset[] GetAllNetworks()
    {
        Process networkGetter = ProcessCreator("network list");
        networkGetter.Start();
        string networksString = networkGetter.StandardOutput.ReadToEnd();
        networksString = "{ \"networks_specs\":" + networksString + "}";
        NetworkWrapper networkWrapper = JsonUtility.FromJson<NetworkWrapper>(networksString);

        return networkWrapper.networks_specs;
    }

    public bool ToggleHost(string identifier)
    {
        string status = GetHostStatus(identifier);
        if(status == "running")
        {
            StopHost(identifier);
            return false;
        }
        else if(status == "stopped")
        {
            StartHost(identifier);
            return true;
        }
        return false;
    }

    public void StartHost(string identifier)
    {
        Process starter = ProcessCreator("host start " + identifier);
        starter.Start();
    }

    public void StopHost(string identifier)
    {
        Process stopper = ProcessCreator("host stop " + identifier);
        stopper.Start();
    }

    public Metrics GetHostMetrics(string identifier)
    {
        Process hostGetter = ProcessCreator("host get " + identifier);
        hostGetter.Start();
        string hostString = hostGetter.StandardOutput.ReadToEnd();
        HostsSpecsAsset asset = JsonUtility.FromJson<HostsSpecsAsset>(hostString);

        return asset.metrics;
    }

    public string GetHostStatus(string identifier)
    {
        Process hostGetter = ProcessCreator("host get " + identifier);
        hostGetter.Start();
        string hostString = hostGetter.StandardOutput.ReadToEnd();
        HostsSpecsAsset asset = JsonUtility.FromJson<HostsSpecsAsset>(hostString);

        return asset.status;
    }

    public void DeleteConnection(Pipeline pipeline)
    {
        string hostIdentifier = pipeline.Host.Identifier;
        string nicIdentifier = pipeline.Identifier;
        Process deleter = ProcessCreator("host disconnect " + hostIdentifier + " " + nicIdentifier);
        deleter.Start();
    }

    Process ProcessCreator(string command)
    {
        Process process = new Process();
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = _fileName;
        process.StartInfo.Arguments = command + " " + _staticArguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        return process;
    }
}
