using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

#endif

public class BuildPostprocessor
{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414
#pragma warning restore CS0414
#pragma warning restore IDE0051 // Remove unused private members


/// <summary>
/// Handles post-processing after building a project for iOS.
/// </summary>
/// <param name="target">The build target platform.</param>
/// <param name="pathToBuiltProject">The path to the built project.</param>
/// <returns>No expected outputs.</returns>
    [PostProcessBuild(500)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_IOS
        if (target == BuildTarget.iOS)
        {
            //string plistPath = pathToBuiltProject + "/Info.plist";

            //Main
            string _projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject _pbxProject = new PBXProject();
            _pbxProject.ReadFromFile(_projPath);
            string _targetPBX = _pbxProject.GetUnityMainTargetGuid();
            _pbxProject.SetBuildProperty(_targetPBX, "ENABLE_BITCODE", "NO");
            //Unity Framework
            _targetPBX = _pbxProject.GetUnityFrameworkTargetGuid();
            _pbxProject.SetBuildProperty(_targetPBX, "ENABLE_BITCODE", "NO");
            _pbxProject.WriteToFile(_projPath);
        }
#endif
    }
}