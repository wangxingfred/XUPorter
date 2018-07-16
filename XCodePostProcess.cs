using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{

    [MenuItem("Tools/测试XUPorter")]
    public static void Test()
    {
        OnPostProcessBuild(BuildTarget.iOS, Path.Combine(Application.dataPath, "..\\XCode\\XCodeProj"));
    }

#if UNITY_EDITOR
    [PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
        if (target != BuildTarget.iOS) {
			Debug.Log("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string[] files = Directory.GetFiles( Application.dataPath, "*.projmods", SearchOption.AllDirectories );
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}

        //TODO disable the bitcode for iOS 9
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");

        var developmentTeam = "xxxx";
        var devProvision = "xxxx";
        var devProvisionName = "xxx";
        var inHouseProvision = "xxxx";
        var inHouseProvisionName = "xxx";

        //TODO implement generic settings as a module option

        // XCode -> 'General/Sigining'
        var pbxProject = project.project;
        pbxProject.OverwriteTargetAttrs(true, developmentTeam);

        // development
        var devConfigs = new string[] { "Debug", "ReleaseForProfiling", "ReleaseForRunning" };
        foreach (var config in devConfigs)
        {
            project.overwriteBuildSetting("CODE_SIGN_STYLE", "Manual", config);
            project.overwriteBuildSetting("DEVELOPMENT_TEAM", developmentTeam, config);
            project.overwriteBuildSetting("PROVISIONING_PROFILE", devProvision, config);
            project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", devProvisionName, config);
        }

        // in-house distribution
        var distConfigs = new string[] { "Release" };
        foreach (var config in distConfigs)
        {
            project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Distribution", config);
            project.overwriteBuildSetting("CODE_SIGN_STYLE", "Manual", config);
            project.overwriteBuildSetting("DEVELOPMENT_TEAM", developmentTeam, config);
            project.overwriteBuildSetting("PROVISIONING_PROFILE", inHouseProvision,config);
            project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", inHouseProvisionName, config);
        }


        // Finally save the xcode project
        project.Save();

	}
#endif

	public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
