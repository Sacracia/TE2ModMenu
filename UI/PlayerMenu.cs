﻿using HarmonyLib;
using UnityEngine;

namespace TE2ModMenu
{
    internal class PlayerMenu : MonoBehaviour
    {
        private bool _visible = true;
        private bool _infEnergy = false;
        private bool _zeroHeat = false;
        private bool _teleport = false;
        private bool _godmode = false;
        private bool _noCollide = false;
        private bool _oneHitKill = false;
        private bool _fov = false;
        private bool _invisible = false;
        private bool _fury = false;
        private bool _maxItemHealth = false;
        private float _lastCacheTime = Time.time + 3f;
        private Rect window = new Rect(10f, 10f, 250f, 400f);
        internal static float fov = 1f;
        internal static Player player = null;

        private static PlayerMenu s_instance;

        public static PlayerMenu Instance
        {
            get => s_instance;
        }

        void Awake() => s_instance = this;

        int Strength
        {
            get
            {
                if (player == null)
                    return 0;
                return Mathf.RoundToInt(player.m_CharacterStats.Strength);
            }
            set
            {
                if (player == null)
                    return;
                player.m_CharacterStats.Strength = value;
            }
        }

        int Cardio
        {
            get
            {
                if (player == null)
                    return 0;
                return Mathf.RoundToInt(player.m_CharacterStats.Cardio);
            }
            set
            {
                if (player == null)
                    return;
                player.m_CharacterStats.Cardio = value;
            }
        }

        int Intellect
        {
            get
            {
                if (player == null)
                    return 0;
                return Mathf.RoundToInt(player.m_CharacterStats.Intellect);
            }
            set
            {
                if (player == null)
                    return;
                player.m_CharacterStats.Intellect = value;
            }
        }

        float Speed
        {
            get
            {
                if (player == null)
                    return 1f;
                return player.m_CharacterMovement.m_fMaxSpeed / 5f;
            }
            set
            {
                if (player == null)
                    return;
                player.m_CharacterMovement.m_fMaxSpeed = 5f * value;
                player.m_CharacterMovement.m_fMaxSpeedBlocking = 1f * value * value;
                player.m_CharacterMovement.m_fMaxSpeedDashing = 8f * value > 32f ? 32f : 8f * value;
            }
        }

        void OnGUI()
        {
            if (!_visible)
                return;
            window = GUILayout.Window(0, window, OnWindow, "Player", new GUILayoutOption[0]);
        }

        void OnWindow(int windowID)
        {
            DrawElements();
            GUI.DragWindow();
        }

        void DrawElements()
        {
            bool flag = GUILayout.Toggle(_godmode, "Godmode", new GUILayoutOption[0]);
            if (flag != _godmode)
            {
                _godmode = flag;
                if (_godmode)
                {
                    bool res = true;
                    var original = AccessTools.Method(typeof(Player), "TakeDamage");
                    var mPrefix = SymbolExtensions.GetMethodInfo(() => Patches.Godmode(ref res));
                    Loader.harmony.Patch(original, new HarmonyMethod(mPrefix));
                }
                else
                {
                    var original = AccessTools.Method(typeof(Player), "TakeDamage");
                    Loader.harmony.Unpatch(original, HarmonyPatchType.Prefix);
                }
            }

            flag = GUILayout.Toggle(_oneHitKill, "One hit kills", new GUILayoutOption[0]);
            if (flag != _oneHitKill)
            {
                _oneHitKill = flag;
                if (flag)
                {
                    float dmg = 0f;
                    var original = AccessTools.Method(typeof(Player), "DamageCharacter");
                    var mPrefix = SymbolExtensions.GetMethodInfo(() => Patches.OneHitKill(null, null, ref dmg));
                    Loader.harmony.Patch(original, new HarmonyMethod(mPrefix));
                }
                else
                {
                    var original = AccessTools.Method(typeof(Player), "DamageCharacter");
                    Loader.harmony.Unpatch(original, HarmonyPatchType.Prefix);
                }
            }

            flag = GUILayout.Toggle(_noCollide, "No collision", new GUILayoutOption[0]);
            if (flag != _noCollide && player)
            {
                _noCollide = flag;
                PlayerMenu.player.m_PhysicsSphereCol.enabled = !flag;
            }

            _infEnergy = GUILayout.Toggle(_infEnergy, "Max stamina", new GUILayoutOption[0]);
            _zeroHeat = GUILayout.Toggle(_zeroHeat, "No heat", new GUILayoutOption[0]);
            _teleport = GUILayout.Toggle(_teleport, "Enable teleport (F1)", new GUILayoutOption[0]);

            if (GUILayout.Button("Add 100$", new GUILayoutOption[0]) && player != null)
                player.m_CharacterStats.IncreaseMoney(100f);
            if (GUILayout.Button("Regain Consciousness", new GUILayoutOption[0]) && player != null)
                player.RegainConsciousness();
            GUILayout.Label($"Strength {Strength}", new GUILayoutOption[0]);
            Strength = Mathf.RoundToInt(GUILayout.HorizontalSlider(Strength, 0f, CharacterStats.MaxStrength, new GUILayoutOption[0]));
            GUILayout.Label($"Cardio {Cardio}", new GUILayoutOption[0]);
            Cardio = Mathf.RoundToInt(GUILayout.HorizontalSlider(Cardio, 0f, CharacterStats.MaxCardio, new GUILayoutOption[0]));
            GUILayout.Label($"Intellect {Intellect}", new GUILayoutOption[0]);
            Intellect = Mathf.RoundToInt(GUILayout.HorizontalSlider(Intellect, 0f, CharacterStats.MaxIntellect, new GUILayoutOption[0]));
            GUILayout.Label($"Speed {Speed:f3}", new GUILayoutOption[0]);
            Speed = GUILayout.HorizontalSlider(Speed, 1f, 4f, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal();
            flag = GUILayout.Toggle(_fov, "Change fov");
            if (flag != _fov)
            {
                _fov = flag;
                if (flag)
                {
                    float temp = 0f;
                    var original = AccessTools.Method(typeof(CameraManager), "CalculatePixelPerfectOffset");
                    var mPostfix = SymbolExtensions.GetMethodInfo(() => Patches.FOV(ref temp));
                    Loader.harmony.Patch(original, postfix: new HarmonyMethod(mPostfix));
                }
                else
                {
                    var original = AccessTools.Method(typeof(CameraManager), "CalculatePixelPerfectOffset");
                    Loader.harmony.Unpatch(original, HarmonyPatchType.Postfix);
                }
            }
            fov = GUILayout.HorizontalSlider(fov, 0.1f, 2f, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();

            flag = GUILayout.Toggle(_invisible, "Invisible");
            if (flag != _invisible)
            {
                _invisible = flag;
                if (player != null)
                    player.m_bIsHidden = flag;
            }

            flag = GUILayout.Toggle(_maxItemHealth, "Infinite item health");
            if (flag != _maxItemHealth)
            {
                _maxItemHealth = flag;
                if (flag)
                {
                    var original = AccessTools.Method(typeof(Item), "DecreaseHealth");
                    var mPrefix = SymbolExtensions.GetMethodInfo(() => Patches.DecreaseHealth(null));
                    Loader.harmony.Patch(original, new HarmonyMethod(mPrefix));
                }
                else
                {
                    var original = AccessTools.Method(typeof(Item), "DecreaseHealth");
                    Loader.harmony.Unpatch(original, HarmonyPatchType.Prefix);
                }
            }

            flag = GUILayout.Toggle(_fury, "Fists of fury");
            if (flag != _fury)
            {
                _fury = flag;
                if (flag)
                {
                    Item_Combat temp = null;
                    var original = AccessTools.Method(typeof(Character), "GetItemCombat");
                    var mPrefix = SymbolExtensions.GetMethodInfo(() => Patches.FistsOfFury(null, ref temp));
                    Loader.harmony.Patch(original, new HarmonyMethod(mPrefix));
                }
                else
                {
                    var original = AccessTools.Method(typeof(Character), "GetItemCombat");
                    Loader.harmony.Unpatch(original, HarmonyPatchType.Prefix);
                }
            }
        }

        void Update()
        {
            if (Time.time >= _lastCacheTime)
            {
                _lastCacheTime = Time.time + 3f;
                player = Gamer.GetPrimaryGamer().m_PlayerObject;
            }
            if (_godmode)
            {
                if (!player)
                    return;
                player.m_CharacterStats.Health = CharacterStats.MaxHealth;
            }
            if (_infEnergy)
            {
                if (!player)
                    return;
                player.m_CharacterStats.Energy = CharacterStats.MaxEnergy;
            }
            if (_zeroHeat)
            {
                if (!player)
                    return;
                player.m_CharacterStats.Heat = 0f;
            }
            if (Input.GetKeyDown(KeyCode.F1) && _teleport && player)
            {
                Camera camera = CameraManager.GetInstance().GetCamera(player.m_PlayerCameraManagerBindingID);
                if (camera != null)
                {
                    Vector2 a = Input.mousePosition;
                    Vector2 b = default;
                    MouseDetector.GetMouseToCameraOffset(camera, ref b);
                    a += b;
                    Vector3 vector = new Vector3(a.x, a.y, camera.nearClipPlane);
                    vector = camera.ScreenToWorldPoint(vector);
                    vector.z = player.CurrentFloor.m_zPos;
                    player.Teleport(vector);
                }
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _visible = !_visible;
            }
        }
    }
}
