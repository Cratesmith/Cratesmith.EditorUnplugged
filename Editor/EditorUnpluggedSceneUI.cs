using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace EditorUnplugged
{
    [InitializeOnLoad]
    public class EditorUnpluggedSceneUI 
    {
        static GUIStyle s_LabelStyle;
        static GUIStyle s_ToggleStyle;
        static GUIStyle s_TextFieldStyle;
        static GUIStyle s_TimeLabelStyle;

        static double s_LastRepaint = -1;
        static bool s_MousePanning;

        static Texture2D s_TimeIcon;
        static Texture2D s_GreenLightIcon;
        static Texture2D s_YellowLightIcon;
        static Texture2D s_RedLightIcon;
        static Texture2D s_ScriptIcon;
        static Texture2D s_CompileIcon;
        static Texture2D s_TextureIcon;
        static Texture2D s_LightingIcon;

        static EditorUnpluggedSceneUI()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
            CompilationPipeline.assemblyCompilationStarted += OnCompileStart;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (EditorApplication.timeSinceStartup - s_LastRepaint > 60)
            {
                SceneView.RepaintAll();
            }
        }

        static void OnCompileStart(string _obj)
        {
            SceneView.RepaintAll();
        }

        static void OnSceneGUI(SceneView _sceneview)
        {
            // don't repaint if we're panning
            if (Tools.current == Tool.View || Tools.current== Tool.Custom)
            {
                return;
            }
            
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                s_MousePanning = true;
            }

            if (s_MousePanning)
            {
                if (Event.current.type == EventType.MouseLeaveWindow || Event.current.type==EventType.MouseUp && Event.current.button == 1)
                {
                    EditorApplication.delayCall += () => s_MousePanning = false; // just to do this outside of gui events
                }
                return;
            }

            if (s_LabelStyle==null)
            {
                s_LabelStyle = new GUIStyle("Label");
            }

            if (s_ToggleStyle == null)
            {
                s_ToggleStyle = new GUIStyle("Toggle");
            }
            
            if (s_TextFieldStyle == null)
            {
                s_TextFieldStyle = new GUIStyle("MiniTextField");
            }

            if (s_TimeLabelStyle == null)
            {
                s_TimeLabelStyle = new GUIStyle("helpbox");
                s_TimeLabelStyle.alignment = TextAnchor.MiddleCenter;
                s_TimeLabelStyle.padding.top = 1;
                s_TimeLabelStyle.padding.bottom = 1;
            }

            if (s_TimeIcon == null)
            {
                s_TimeIcon = EditorGUIUtility.FindTexture("TestStopwatch");
            }

            if (s_GreenLightIcon == null)
            {
                s_GreenLightIcon = EditorGUIUtility.FindTexture("winbtn_mac_max");
            }
            
            if (s_YellowLightIcon == null)
            {
                s_YellowLightIcon = EditorGUIUtility.FindTexture("winbtn_mac_min");
            }
            
            if (s_RedLightIcon == null)
            {
                s_RedLightIcon = EditorGUIUtility.FindTexture("winbtn_mac_close");
            }

            if (s_ScriptIcon == null)
            {
                s_ScriptIcon = EditorGUIUtility.FindTexture("cs Script Icon");
            }

            if (s_CompileIcon == null)
            {
                s_CompileIcon = EditorGUIUtility.FindTexture("WaitSpin00");
            }

            if (s_TextureIcon == null)
            {
                s_TextureIcon = EditorGUIUtility.FindTexture("animationdopesheetkeyframe");
            }
            
            if (s_LightingIcon == null)
            {
                s_LightingIcon = EditorGUIUtility.FindTexture("Main Light Gizmo");
            }
            
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(5, 5, Screen.width, 22));
            using (new GUILayout.HorizontalScope( GUILayout.ExpandWidth(false), GUILayout.Height(20)))
            {
                using (new GUILayout.HorizontalScope("HelpBox", GUILayout.ExpandWidth(false), GUILayout.Width(1)))
                {
                    {
                        var r = GUILayoutUtility.GetRect(12,12);
                        GUI.Label(new Rect(r.xMin-4, r.yMin-1, r.height+8, r.height+8), new GUIContent(s_TimeIcon, "Session duration"));
                    }
                    var timespan = TimeSpan.FromSeconds(EditorApplication.timeSinceStartup);
                    GUILayout.Label($"{timespan.Hours.ToString("00")}h {timespan.Minutes.ToString("00")}m", s_LabelStyle, GUILayout.ExpandWidth(false));
                }

                if (SystemInfo.batteryStatus != BatteryStatus.Unknown)
                {
                    using (new GUILayout.HorizontalScope("HelpBox", GUILayout.ExpandWidth(false), GUILayout.Width(1)))
                    {
                        {
                            var r = GUILayoutUtility.GetRect(12,12); r.yMin = r.yMin + 4;
                            if (SystemInfo.batteryLevel > .6f)
                            {
                                GUI.DrawTexture(r, s_GreenLightIcon, ScaleMode.ScaleToFit, true, 1);
                            }else if (SystemInfo.batteryLevel > .3f)
                            {
                                GUI.DrawTexture(r, s_YellowLightIcon,ScaleMode.ScaleToFit, true, 1);
                            }
                            else
                            {
                                GUI.DrawTexture(r, s_RedLightIcon,ScaleMode.ScaleToFit, true, 1);
                            }
                        }
                        var current = SystemInfo.batteryLevel * 100f;
                        var used = EditorUnpluggedUtil.BatteryUsed * 100f;
                        GUILayout.Label(SystemInfo.batteryStatus.ToString(), s_LabelStyle, GUILayout.ExpandWidth(false));
                        GUILayout.Label($"{current.ToString("F1")}%", s_LabelStyle, GUILayout.ExpandWidth(false));
                        GUILayout.Label(new GUIContent($"{(used < 0 ? "+" : "-")}{Mathf.Abs(used).ToString("F1")}%","Percent used this session"), s_TimeLabelStyle, GUILayout.Width(55));

                        var timeRemainingA = EditorUnpluggedUtil.BatteryTimeRemainingShortSample;
                        var timeRemainingB = EditorUnpluggedUtil.BatteryTimeRemainingLongSample;
                        var minRemaining = timeRemainingA < timeRemainingB ? timeRemainingA : timeRemainingB;
                        var maxRemaining = timeRemainingA >= timeRemainingB ? timeRemainingA : timeRemainingB;
                        
                        if (!float.IsInfinity(minRemaining))
                        {
                            var timespan = TimeSpan.FromSeconds(minRemaining);
                            if(timespan.Days < 1) GUILayout.Label(new GUIContent($"{timespan.Hours.ToString("00")}h {timespan.Minutes.ToString("00")}m", "Estimated Remaining (Min)"), s_TimeLabelStyle, GUILayout.Width(60));

                            if (!float.IsInfinity(maxRemaining) && !Mathf.Approximately(maxRemaining, minRemaining))
                            {
                                GUILayout.Label("->");
                                var maxtimespan = TimeSpan.FromSeconds(maxRemaining);
                                if (maxtimespan.Days < 1)
                                {
                                    GUILayout.Label(new GUIContent($"{maxtimespan.Hours.ToString("00")}h {maxtimespan.Minutes.ToString("00")}m","Estimated Remaining (Max)"), s_TimeLabelStyle, GUILayout.Width(60));
                                }
                            }
                            GUILayout.Label("@");

                            var rate1h = EditorUnpluggedUtil.BatteryRateShortSample*100f*60f*60f;
                            if(rate1h < 1000f) GUILayout.Label(new GUIContent($"{rate1h.ToString("F1")}/h", "Recent usage percent per hour"), s_TimeLabelStyle, GUILayout.Width(55));
                        }
                    }
                }

                using (new GUILayout.HorizontalScope("HelpBox",GUILayout.ExpandWidth(false), GUILayout.Width(1)))
                {
                    EditorUnpluggedUtil.FrameLimiter = GUILayout.Toggle(EditorUnpluggedUtil.FrameLimiter, "Fps limit", s_ToggleStyle, GUILayout.ExpandWidth(false));
                    if (int.TryParse(EditorGUILayout.DelayedTextField(EditorUnpluggedUtil.FrameLimiterFPS.ToString(), s_TextFieldStyle, GUILayout.Width(20)),
                        out var newTargetFps))
                    {
                        EditorUnpluggedUtil.FrameLimiterFPS = newTargetFps;
                    }                    
                }
                
                using (new GUILayout.HorizontalScope("HelpBox",GUILayout.ExpandWidth(false), GUILayout.Width(1)))
                {
                    {
                        var r = GUILayoutUtility.GetRect(12,12);
                        GUI.Label(new Rect(r.xMin-4, r.yMin-1, r.height+8, r.height+8), new GUIContent(s_ScriptIcon, "Asset Reload/Script Compile"));
                    }
                    GUILayout.Label(EditorUnpluggedUtil.CompileCount.ToString(), s_LabelStyle, GUILayout.ExpandWidth(false));
                    EditorUnpluggedUtil.AutoRefresh = GUILayout.Toggle(EditorUnpluggedUtil.AutoRefresh, "Auto", s_ToggleStyle, GUILayout.ExpandWidth(false));
                    if (!EditorUnpluggedUtil.AutoRefresh)
                    {
                        if (GUILayout.Button(new GUIContent("Ctrl-R", "Reimport assets/compile"), "minibutton"))
                        {
                            AssetDatabase.Refresh();
                        }
                    }
                    
                    if (EditorApplication.isCompiling)
                    {     
                        var r = GUILayoutUtility.GetRect(12,12);
                        GUI.Label(new Rect(r.xMin-4, r.yMin-1, r.height+8, r.height+8), new GUIContent(s_CompileIcon, "Compiling! (how did you see this tooltip!?)"));
                    }

                    if (EditorUnpluggedUtil.LastCompileDuration > 0 && EditorUnpluggedUtil.CompileCount > 0)
                    { 
                        GUILayout.Label(new GUIContent($"{EditorUnpluggedUtil.LastCompileDuration.ToString("00.0")}s", "Last Script Reload Duration"), s_TimeLabelStyle, GUILayout.Width(40));
                    }
                }

                using (new GUILayout.HorizontalScope("HelpBox", GUILayout.ExpandWidth(false), GUILayout.Width(1)))
                {
                    var r = GUILayoutUtility.GetRect(12,12);
                    GUI.Label(new Rect(r.xMin-4, r.yMin-1, r.height+8, r.height+8), new GUIContent(s_TextureIcon, "When to compress assets"));
                    EditorUnpluggedUtil.CompressOnImport = EditorGUILayout.Popup(EditorUnpluggedUtil.CompressOnImport ? 0 : 1,
                                                         new[] {"On Build", "On Import"}, GUILayout.Width(70))==1;
                }
                
                using (new GUILayout.HorizontalScope("HelpBox", GUILayout.ExpandWidth(false), GUILayout.Width(1)))
                {
                    var r = GUILayoutUtility.GetRect(12,12);
                    GUI.Label(new Rect(r.xMin-4, r.yMin-1, r.height+8, r.height+8), new GUIContent(s_LightingIcon, "Lightmapping workflow"));
                    Lightmapping.giWorkflowMode = (Lightmapping.GIWorkflowMode)EditorGUILayout.EnumPopup(Lightmapping.giWorkflowMode,GUILayout.Width(75));
                    if (Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.OnDemand)
                    {
                        if (GUILayout.Button(new GUIContent("Bake", "Bake lightmaps"), "minibutton"))
                        {
                            Lightmapping.BakeAsync();
                        }
                    }
                }
            }
            GUILayout.EndArea();
            Handles.EndGUI();
            
            s_LastRepaint = EditorApplication.timeSinceStartup;
        }
    }
}