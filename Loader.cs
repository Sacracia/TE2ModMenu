using HarmonyLib;
using UnityEngine;

namespace TE2ModMenu
{
    public class Loader
    {
        private static GameObject Load;
        internal static Harmony harmony;

        public static void Init()
        {
            harmony = new Harmony("te2.mod.sacracia");
            Load = new GameObject();
            Load.AddComponent<PlayerMenu>();
            Load.AddComponent<GeneralMenu>();
            Load.AddComponent<PrisonMenu>();
            Load.AddComponent<JobMenu>();
            Load.AddComponent<OutfitMenu>();
            Load.AddComponent<WeaponMenu>();
            UnityEngine.Object.DontDestroyOnLoad(Load);
        }

        public static void Unload()
        {
            harmony.UnpatchAll();
            UnityEngine.Object.Destroy(Load);
        }
    }
}
