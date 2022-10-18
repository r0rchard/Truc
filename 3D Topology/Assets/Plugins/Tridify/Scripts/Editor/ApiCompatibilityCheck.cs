using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ApiCompatibilityCheck
{
    private const string TridifyDoNotRemindApiCompatibilityLevel = "TridifyDoNotRemindApiCompatibilityLevel";
    static ApiCompatibilityCheck()
    {
        ShowApiCompatibilityWarningIfNeeded();
    }

    static void ShowApiCompatibilityWarningIfNeeded()
    {
        var doNotRemind = EditorPrefs.GetBool(TridifyDoNotRemindApiCompatibilityLevel);

        if (!doNotRemind && PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone) != ApiCompatibilityLevel.NET_4_6)
        {
            var result = EditorUtility.DisplayDialogComplex("Tridify BIM Tools requires Api Compatibility Level .NET 4.x",
                "Use .NET 4.x Api Compatibility Level", "Use", "Cancel", "Do not remind me again");
            if (result == 0)
            {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
                EditorUtility.DisplayDialog("", "Api Compatibility Level changed.", "Ok");
            } else if (result == 2)
            {
                EditorPrefs.SetBool(TridifyDoNotRemindApiCompatibilityLevel, true);
            }
        }
    }
}
