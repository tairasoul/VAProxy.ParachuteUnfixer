using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Invector.vCharacterController;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ParachuteCooldownFixRemover
{
    [BepInPlugin("tairasoul.vaproxy.jumpfixreverter", "JumpFixReverter", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {

        public static ManualLogSource log;
        public static vThirdPersonInput tpInput;
        private static bool usingParachute = false;
        private static float previousY = 0f;
        private void Awake()
        {
            log = Logger;
            Harmony harmony = new Harmony("tairasoul.vaproxy.jumpfixreverter");
            harmony.PatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(vParachuteController), "get_openParachuteConditions")]
        [HarmonyPrefix]
        public static bool ConditionPrefix(ref bool __result, ref vParachuteController __instance)
        {
            if (!tpInput)
            {
                tpInput = (vThirdPersonInput)typeof(vParachuteController).GetField("tpInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(__instance);
                tpInput.cc.jumpStamina = 0f;
            }
            __result = __instance.canUseParachute && !Object.FindObjectOfType<Inventory>().ancked && !usingParachute && !tpInput.cc.ragdolled && tpInput.cc.groundDistance > __instance.minHeightToOpenParachute;
            usingParachute = __result;
            return false;
        }
        [HarmonyPatch(typeof(vParachuteController), "get_openParachuteConditions")]
        [HarmonyPostfix]
        public static void ConditionPostfix(ref bool __result)
        {
            if (!__result)
            {
                Object.FindObjectOfType<Inventory>().JumpMid();
            }
        }
    }
}
 