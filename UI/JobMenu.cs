using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TE2ModMenu
{
    internal class JobMenu : MonoBehaviour
    {
        internal bool visible = false;
        private List<BaseJob> _jobs;
        private float _lastCacheTime = Time.time + 3f;
        internal Rect window = new Rect(300f,300f,150f,300f);

        private static JobMenu s_instance;

        public static JobMenu Instance
        {
            get => s_instance;
        }

        void Awake() => s_instance = this;

        void OnGUI()
        {
            if (!visible || !PrisonMenu.Instance.visible)
                return;
            window = GUILayout.Window(3, window, OnWindow, "Jobs", new GUILayoutOption[0]);
        }

        void OnWindow(int windowID)
        {
            DrawElements();
            GUI.DragWindow();
        }

        void DrawElements()
        {
            if (!PlayerMenu.player || _jobs == null)
                return;
            foreach (BaseJob job in _jobs)
            {
                if (GUILayout.Button(job.m_Type.ToString(), new GUILayoutOption[0]))
                {
                    JobsManager jobsManager = JobsManager.GetInstance();
                    jobsManager?.RemoveCharacterFromJob(job.m_Type);
                    jobsManager?.AssignCharacterToJob(PlayerMenu.player, job.m_Type);
                }
            }
        }

        void Update()
        {
            if (Time.time >= _lastCacheTime && visible)
            {
                _lastCacheTime = Time.time + 3f;
                _jobs = Traverse.Create(JobsManager.GetInstance()).Field("m_Jobs").GetValue() as List<BaseJob>;
            }
        }
    }
}
