using BepInEx;
using HarmonyLib;
using Receiver2;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace WhiskeyPatch
{
    [BepInPlugin("org.szikaka.plugins.whiskey", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static readonly int gun_model = 1002;    

        private static MethodInfo tryFireBullet;

        private void Start()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            tryFireBullet = typeof(GunScript).GetMethod("TryFireBullet", BindingFlags.NonPublic | BindingFlags.Instance);

            Harmony.CreateAndPatchAll(this.GetType());
        }

        [HarmonyPatch(typeof(CartridgeSpec), "SetFromPreset")]
        [HarmonyPrefix]
        private static void PatchSetFromPreset(ref CartridgeSpec __instance, CartridgeSpec.Preset preset) {
            if ((int) preset == gun_model) {
                __instance.extra_mass = 25f;
				__instance.mass = 40f;
				__instance.speed = 350f;
				__instance.diameter = 0.0165f;
            }
        }

        [HarmonyPatch(typeof(ReceiverCoreScript), "Awake")]
        [HarmonyPostfix]
        private static void PatchCoreAwake(ref ReceiverCoreScript __instance, ref GameObject[] ___gun_prefabs_all, ref List<MagazineScript> ___magazine_prefabs_all) {
            GameObject whiskey = null;
            MagazineScript whiskeyMag = null;

            try { 
                whiskey = ___gun_prefabs_all.Single(go => {
                    GunScript gs = go.GetComponent<GunScript>();
                    return ((int) gs.gun_model) == gun_model;
                });
            } catch (Exception e) {
                Debug.Log(e.StackTrace);
                Debug.LogError("Couldn't load gun \"Whiskey\"");
                return;
            }

            try { 
                whiskeyMag = ___magazine_prefabs_all.Single(ms => { return ((int) ms.gun_model) == gun_model; });
            } catch (Exception e) {
                Debug.Log(e.StackTrace);
                Debug.LogError("Couldn't load magazine for gun \"Whiskey\"");
                return;
            }

            ShellCasingScript whiskeyRound = whiskey.GetComponent<GunScript>().loaded_cartridge_prefab.GetComponent<ShellCasingScript>();

            __instance.generic_prefabs = new List<InventoryItem>(__instance.generic_prefabs) {
                whiskey.GetComponent<GunScript>(),
                whiskeyMag,
                whiskeyRound
            }.ToArray();

            LocaleTactics lt = new LocaleTactics();

            lt.title = "Whiskey";
            lt.gun_internal_name = "szikaka.whiskey";
            lt.text = "A modded rifle, firing powerful .650\" caliber rounds.\nHowever, immense damage it can cause is counteracted by high recoil, low magazine capacity and mediocre ergonomics, making this weapon very situational, but still deadly";

            Locale.active_locale_tactics.Add("szikaka.whiskey", lt);
        }

        [HarmonyPatch(typeof(AmmoBoxScript), "Start")]
        [HarmonyPostfix]
        private static void PatchAmmoBoxStart(ref AmmoBoxScript __instance) {
            if (!((int) __instance.round_prefab.GetComponent<ShellCasingScript>().cartridge_type == gun_model)) return;
            
            foreach (ShellCasingScript shell in __instance.transform.GetComponentsInChildren<ShellCasingScript>()) {
                shell.transform.localEulerAngles = new Vector3(0, 90, 0);
            }
        }

        //[HarmonyPatch(typeof(GunScript), "Awake")]
        //[HarmonyPrefix]
        private static void PatchGunAwake(ref GunScript __instance) {

        }

        [HarmonyPatch(typeof(GunScript), "Update")]
        [HarmonyPostfix]
        private static void PatchGunUpdate(ref GunScript __instance, ref int ___hammer_state, ref bool ___disconnector_needs_reset) {
            if ((int) __instance.gun_model != gun_model) return;

            __instance.ApplyTransform("bolt_lock", __instance.slide.amount, __instance.transform.Find("slide/point_bolt_rotate"));

            if (__instance.IsSafetyOn() && __instance.trigger.amount != 0f)
	        {
		        __instance.trigger.amount = 0f;
	        }
	        if (__instance.slide.amount > 0.2f)
	        {
		        __instance.hammer.target_amount = 1f;
		        __instance.hammer.vel = 1f * ReceiverCoreScript.Instance().player_stats.animation_speed;
		        ___hammer_state = 2;
	        }
	        if (__instance.slide.amount == 0f && __instance.trigger.amount == 0f)
	        {
		        ___disconnector_needs_reset = false;
	        }
	        if (__instance.trigger.amount == 1f && __instance.hammer.amount == 1f && !___disconnector_needs_reset && !__instance.IsSafetyOn())
	        {
		        if (__instance.slide.amount == 0f)
		        {
			        __instance.hammer.target_amount = 0f;
			        __instance.hammer.vel = -0.1f * ReceiverCoreScript.Instance().player_stats.animation_speed;
		        }
		        ___disconnector_needs_reset = true;
	        }
	        float vel = __instance.hammer.vel;

            if (__instance.hammer.transform == null || __instance.safety.transform == null) return;

	        __instance.hammer.TimeStep(Time.deltaTime);

	        if (__instance.hammer.amount == 0f && ___hammer_state == 2 && vel < 0)
	        {
                tryFireBullet.Invoke(__instance, new object[] { 0.5f });
		        ___hammer_state = 0;
	        }
	        __instance.hammer.UpdateDisplay();
            __instance.trigger.UpdateDisplay();
            __instance.safety.UpdateDisplay();
        }
    }
}
