using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Parsing
{
    /// <summary>
    /// Namespace containing the classes obtained from parsing a Cyrber Range topology
    /// </summary>
    namespace CyberRangeSerialization
    {
        [System.Serializable]
        public class BundleAsset
        {
            public TopologyAsset[] topologies;
            public string name = "";

            public string printStuff()
            {
                string s = "";
                foreach (TopologyAsset t in topologies)
                {
                    s += "\n" + t.name;
                }
                return s;
            }
        }
        [System.Serializable]
        public class TopologyAsset
        {
            public object[] labels;
            public object icon;
            public object attributes;
            public string name = "name";
            public string description = "descr";
            public string tag = "tag";
            public string category = "cat";
            public object[] topologies;
            public int size = 0;
            public HostsSpecsAsset[] hosts_specs;
            public NetworksSpecsAsset[] networks_specs;
            public object[] shapes_specs;
            public object parameters;

        }

        [System.Serializable]
        public class WorkzoneAsset
        {
            public object[] workflows;
            public NetworksSpecsAsset[] networks;
            public HostsSpecsAsset[] hosts;
            public object[] shapes;
        }

        [System.Serializable]
        public class HostWrapper
        {
            public HostsSpecsAsset[] hosts_specs;
        }

        [System.Serializable]
        public class NetworkWrapper
        {
            public NetworksSpecsAsset[] networks_specs;
        }

        [System.Serializable]
        public class HostsSpecsAsset
        {
            public object[] labels;
            public IconHostSpecAsset icon;
            public AttributesAsset attributes;
            public string hardware_type = "";
            public string hostname = "";
            public string username = "";
            public string password = "";
            public string os;
            public int ram_mb = 0;
            public object default_gateway;
            public bool headless;
            public bool agent;
            public object hooks;
            public int cpu_cores;
            public int priority;
            public bool immutable;
            public int video_memory;
            public object backing;
            public int size;
            public int disk_size;
            public NicsAsset[] nics;
            public object[] images;
            public string identifier = "";
            public string status = "";
            public Metrics metrics;
        }

        [System.Serializable]
        public class NetworksSpecsAsset
        {
            public object[] labels;
            public object icon;
            public AttributesAsset attributes;
            public string name;
            public string ip;
            public string mask;
            public bool promiscuous;
            public string identifier = "";
        }

        [System.Serializable]
        public class IconHostSpecAsset
        {
            public int height = 0;
            public int width = 0;
            public string data = "";
        }

        [System.Serializable]
        public class NicsAsset
        {
            public object[] labels;
            public object icon;
            public AttributesAsset attributes;
            public string network_name;
            public string mask;
            public string ip;
            public string mac;
            public bool enabled;
            public object backing;
            public object[] images;
            public string identifier = "";
            public string host_identifier = "";
            public string network_identifier = "";
        }

        [System.Serializable]
        public class AttributesAsset
        {
            public string host_shape_index;
            public int label_rotation_angle;
            public string label_text;
            public float label_x;
            public float label_y;
            public int network_shape_index;
            public string network_type;
            public int image_height;
            public string image_source;
            public int image_rotation_angle;
            public int image_width;
            public float x;
            public float y;
            public bool preserve_mac;
            public string image_edgeleft_height;
            public string image_edgeleft_width;
            public string image_edgeleft_source;
            public string image_edgeright_height;
            public string image_edgeright_width;
            public string image_edgeright_source;
            public string image_segment_height;
            public string image_segment_width;
            public string image_segment_source;
        }

        [System.Serializable]
        public class Metrics
        {
            public float cpu_capacity;
            public float cpu_usage;
            public string firmware;
            public string guest_status;
            public float host_memory_usage;
            public bool kernel_crashed;
            public float memory_capacity;
            public float memory_usage;
            public string power_state;
            public string status;
            public string tool_version;
        }
    }
}