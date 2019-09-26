/*This is not attached to any class. 
 * Safeguard class to overwrite settings. 
 * Some of the settings are set based on the current project's requirements.
 * Author: Aparant Mane
 */ 

using UnityEngine;
public class Settings {

    string unity_version = "2018.2.2f 1";  

    //only for mobile devices
#if UNITY_ANDROID || UNITY_IOS
    private void Start() {

        //we force orientation to portrait
        Screen.orientation = ScreenOrientation.Portrait;

        //don't vsync, since we target 60. Conditional check on FPS to enable it
        QualitySettings.vSyncCount = 0;
    
        Application.targetFrameRate = 60;

        QualitySettings.streamingMipmapsActive = false;
        
        //considering we havn't added our own qualities. In this case: consider using QualitySettings.names
        QualitySettings.SetQualityLevel(5, false);
    }
#endif
}
