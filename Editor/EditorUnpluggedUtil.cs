using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

namespace EditorUnplugged
{
    public static class EditorUnpluggedUtil
    {
        public static double LastCompileDuration => LastCompileTimeEnd - LastCompileTimeStart;
        
        const string EditorPrefs_CrunchOnImport = "kCompressTexturesOnImport";
        
        public static bool CompressOnImport
        {
            get => EditorPrefs.GetBool(EditorPrefs_CrunchOnImport, false);
            set => EditorPrefs.SetBool(EditorPrefs_CrunchOnImport, value);
        }
        
        const string EditorPrefs_AutoRefresh = "kAutoRefresh";

        public static bool AutoRefresh
        {
            get => EditorPrefs.GetBool(EditorPrefs_AutoRefresh, false);
            set => EditorPrefs.SetBool(EditorPrefs_AutoRefresh, value);
        }
        
        const string EditorPrefs_CompileCount = "EditorUnpluggedUtil.CompileCount";
        public static int CompileCount
        {
            get => EditorPrefs.GetInt(EditorPrefs_CompileCount, 0);
            private set => EditorPrefs.SetInt(EditorPrefs_CompileCount, value);
        }
        
        const string EditorPrefs_LastReloadTime = "EditorUnpluggedUtil.LastReloadTime";
        public static double LastReloadTime
        {
            get => EditorPrefs_GetDouble(EditorPrefs_LastReloadTime, 0);
            private set => EditorPrefs_SetDouble(EditorPrefs_LastReloadTime, value);
        }

        static double EditorPrefs_GetDouble(string _editorPrefsLastReloadTime, int _default)
        {
            return double.TryParse(EditorPrefs.GetString(_editorPrefsLastReloadTime), out var result)
                ? result
                : _default;
        }

        static void EditorPrefs_SetDouble(string _editorPrefsLastReloadTime, double _value)
        {
            EditorPrefs.SetString(_editorPrefsLastReloadTime, _value.ToString());
        }

        const string EDITORPREFS_LastCompileTimeStart = "ScriptExecutionOrder.LastCompileTimeStart";
        public static double LastCompileTimeStart
        {
            get => EditorPrefs_GetDouble(EDITORPREFS_LastCompileTimeStart, 0);
            private set { EditorPrefs_SetDouble(EDITORPREFS_LastCompileTimeStart, value);}
        }
        
        const string EDITORPREFS_LastCompileTimeEnd = "ScriptExecutionOrder.LastCompileTimeEnd";
        public static double LastCompileTimeEnd
        {
            get => EditorPrefs_GetDouble(EDITORPREFS_LastCompileTimeEnd, 0);
            private set { EditorPrefs_SetDouble(EDITORPREFS_LastCompileTimeEnd, value);}
        }
        
        const string EditorPrefs_BatterySample1Time = "EditorUnpluggedUtil.BatterySample1Time";
        static double BatterySample1Time
        {
            get => EditorPrefs_GetDouble(EditorPrefs_BatterySample1Time, 0);
            set => EditorPrefs_SetDouble(EditorPrefs_BatterySample1Time, value);
        }
        
        const string EditorPrefs_BatterySample2Time = "EditorUnpluggedUtil.BatterySample2Time";
        static double BatterySample2Time
        {
            get => EditorPrefs_GetDouble(EditorPrefs_BatterySample2Time, 0);
            set => EditorPrefs_SetDouble(EditorPrefs_BatterySample2Time, value);
        }
        
        const string EditorPrefs_BatterySample1 = "EditorUnpluggedUtil.BatterySample1";
        static float BatterySample1
        {
            get => EditorPrefs.GetFloat(EditorPrefs_BatterySample1, 0);
            set => EditorPrefs.SetFloat(EditorPrefs_BatterySample1, value);
        }

        const string EditorPrefs_BatterySample2 = "EditorUnpluggedUtil.BatterySample2";
        static float BatterySample2
        {
            get => EditorPrefs.GetFloat(EditorPrefs_BatterySample2, 0);
            set => EditorPrefs.SetFloat(EditorPrefs_BatterySample2, value);
        }
        
        const string EditorPrefs_BatteryRateLongSample = "EditorUnpluggedUtil.BatteryRateLongSample";
        public static double BatteryRateLongSample
        {
            get => EditorPrefs_GetDouble(EditorPrefs_BatteryRateLongSample, 0);
            private set => EditorPrefs_SetDouble(EditorPrefs_BatteryRateLongSample, value);
        }
        
        const string EditorPrefs_StartBattery = "EditorUnpluggedUtil.StartBattery";
        public static float StartBattery
        {
            get => EditorPrefs.GetFloat(EditorPrefs_StartBattery, 1);
            private set => EditorPrefs.SetFloat(EditorPrefs_StartBattery, value);
        }

        const string EditorPrefs_WasCompiling = "EditorUnpluggedUtil.WasCompiling";
        public static bool WasCompiling
        {
            get => EditorPrefs.GetBool(EditorPrefs_WasCompiling, false);
            private set => EditorPrefs.SetBool(EditorPrefs_WasCompiling, value);
        }

        public static float CurrentBattery => SystemInfo.batteryLevel >= 0f ? SystemInfo.batteryLevel : 1f;
        public static float BatteryUsed => StartBattery - SystemInfo.batteryLevel;
        

        public static bool FrameLimiter
        {
            get => QualitySettings.vSyncCount == 0;
            set => QualitySettings.vSyncCount = value?0:1;
        }

        public static int FrameLimiterFPS
        {
            get => Application.targetFrameRate;
            set => Application.targetFrameRate = value;
        }
        
        public static double BatteryRateShortSample => (CurrentBattery - BatterySample2) / (EditorApplication.timeSinceStartup - BatterySample2Time);

        public static float BatteryTimeRemainingShortSample => BatteryRateShortSample < -0.00001 ? Mathf.Max(0,(float)(-SystemInfo.batteryLevel/BatteryRateShortSample)):float.PositiveInfinity;
        
        public static float BatteryTimeRemainingLongSample => BatteryRateLongSample < -0.00001 ? Mathf.Max(0,(float)(-SystemInfo.batteryLevel/BatteryRateLongSample)):float.PositiveInfinity;

        [InitializeOnLoadMethod()]
        static void InitializeOnLoad()
        {
            if (LastReloadTime > 0 && EditorApplication.timeSinceStartup < LastReloadTime)
            { 
                OnEditorLaunch();
            }
            LastReloadTime = EditorApplication.timeSinceStartup;
            
            EditorApplication.update += OnUpdate;
        }

        [DidReloadScripts(9999)]
        static void OnReloadScripts()
        {
            if (WasCompiling)
            {
                WasCompiling = false;
                ++CompileCount;
                LastCompileTimeEnd =  EditorApplication.timeSinceStartup;
//                Debug.Log($"Complication ended at {EditorApplication.timeSinceStartup} duration={LastCompileDuration}");
            }
            CompilationPipeline.assemblyCompilationStarted += OnCompileStarted;
        }

        const float BatterySampleDelay = 30f;
        
        static void OnUpdate()
        {
            if (EditorApplication.timeSinceStartup - BatterySample1Time >= BatterySampleDelay && !Mathf.Approximately(BatterySample1,CurrentBattery))
            {
                BatterySample2Time = BatterySample1Time;
                BatterySample2 = BatterySample1;

                BatterySample1Time = EditorApplication.timeSinceStartup;
                BatterySample1 = CurrentBattery;

                BatteryRateLongSample = BatteryRateShortSample*0.9 + BatteryRateLongSample*0.1;
            }
        }

        static void OnCompileStarted(string _)
        {
            if (!WasCompiling)
            {
                LastCompileTimeStart = EditorApplication.timeSinceStartup;
//                Debug.Log($"Complication started at {EditorApplication.timeSinceStartup}");
                WasCompiling = true;
                
                ClosePackageManagerWindows();
            }
        }

        static void ClosePackageManagerWindows()
        {
            // close the package manager to save battery and 10s of compile time
            var pmwt = GetType("UnityEditor.PackageManager.UI.PackageManagerWindow");
            var pmw = (EditorWindow[]) Resources.FindObjectsOfTypeAll(pmwt);
            if (pmw.Length <= 0)
            {
                return;
            }
            
            Debug.Log("Closing package manager windows to avoid excessive compile time");
            foreach (var window in pmw)
            {
                window.Close();
            }
        }

        static Type GetType(string name)
        { 
            Type type = null;
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                type = assemblies[i].GetType(name);
                if (type != null) break;
            }
            return type;
        }
        
        static void OnEditorLaunch()
        {
            Debug.Log("Editor Launch Detected");
            CompileCount = 0;
            BatterySample1 = BatterySample2 = StartBattery = SystemInfo.batteryLevel;
            LastCompileTimeStart = LastCompileTimeStart = 0;
            BatterySample1Time = 0;
            FrameLimiterFPS = 30;
            Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
            Lightmapping.ForceStop();
        }
    }
}