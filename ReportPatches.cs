using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch]
static class ReportPatches
{
    const BindingFlags Any = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    static IEnumerable<MethodBase> TargetMethods()
    {
        var methods = new List<MethodBase>();
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.StorefrontNotCleaned)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.StorefrontClean)));
        Add(methods, typeof(DebrisHelper).GetMethod(nameof(DebrisHelper.OnNight)));
        Add(methods, typeof(DebrisHelper).GetMethod("ClearPile", Any));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.PowerOff)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.PowerOn)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.InsuranceUnpaid)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.InsurancePaid)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BurglaryGrace)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BurglaryAlarmFailed)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BurglaryNoAlarm)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BurglaryAlarmStopped)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BurglaryInsured)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.AugLeftAlive)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.AddNewPromotionalOffer)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BuyerReferral)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.SellerReferral)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.CriminalNetwork)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.InspectionWarningIn2Days)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.InspectionWarningTomorrow)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.InspectionWarningImminent)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.AugIncoming)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.WantedPersonVisited)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.UpgradeTip)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.StingOperationWarning)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.SecurityInvestigationNearComplete)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.AgoraphobiaReady)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.AfterhourDebuff)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.FixerUsed)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.VandalismCleaned)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.CartelProtectionActive)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.VandalizedByFaction)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.StoreStillVandalized)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.FactionOperationDateShared)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.ContactReady)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BlackoutBypassed)));
        Add(methods, typeof(NightLog).GetMethod(nameof(NightLog.BlackoutActive)));
        Add(methods, typeof(HealthData).GetMethod(nameof(HealthData.HandleNightlyWound)));
        Add(methods, typeof(HealthData).GetMethod(nameof(HealthData.OnSleep)));
        Add(methods, typeof(HealthData).GetMethod("AddSentinelDamageLog", Any));
        Add(methods, typeof(SecData).GetMethod(nameof(SecData.OnNewDay)));
        Add(methods, typeof(LiquidLuckHelper).GetMethod(nameof(LiquidLuckHelper.OnSleep)));
        Add(methods, typeof(LotteryHelper).GetMethod(nameof(LotteryHelper.DrawWinningNumberWithLuck)));
        Add(methods, typeof(ReactorHelper).GetMethod(nameof(ReactorHelper.OnNight)));
        Add(methods, typeof(VendingMachineHelper).GetMethod("SettleRequestMislabels", Any));
        Add(methods, typeof(VendingMachineHelper).GetMethod("SettleVendingMislabels", Any));
        Add(methods, typeof(VendingMachineHelper).GetMethod("SettleMislabelRep", Any));
        Add(methods, typeof(PlayerStore).GetMethod(nameof(PlayerStore.HandleVandalism)));
        Add(methods, typeof(PlayerStore).GetMethod(nameof(PlayerStore.HandleButcherHunting)));
        Add(methods, typeof(ShareholderHelper).GetMethod(nameof(ShareholderHelper.HandleDividends)));
        Add(methods, typeof(SerpicoStoryline).GetMethod(nameof(SerpicoStoryline.OnLeaveDumpingGround)));
        Add(methods, typeof(SerpicoStoryline).GetMethod("DeliverGeorgioFood", Any));
        return methods;
    }

    static void Add(List<MethodBase> methods, MethodInfo method)
    {
        if (method == null)
        {
            ModEntry.Log?.Log("A night-log method is missing and was not patched.");
            return;
        }

        methods.Add(method);
    }

    static void Prefix(MethodBase __originalMethod, object[] __args)
    {
        ReportLineIcons.Push(ReportLines.KindFor(__originalMethod, __args));
    }

    static Exception Finalizer()
    {
        ReportLineIcons.Pop();
        return null;
    }
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.AddNightLog), typeof(string))]
static class CapturePlainNightLog
{
    static void Prefix(object[] __args)
    {
        ReportLineIcons.Capture(__args);
    }
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.AddNightLog), typeof(string), typeof(string))]
static class CaptureColoredNightLog
{
    static void Prefix(object[] __args)
    {
        ReportLineIcons.Capture(__args);
    }
}

[HarmonyPatch(typeof(StartOfDayUIManager), nameof(StartOfDayUIManager.InitPanel))]
static class DecorateMorningReport
{
    static System.Collections.Generic.List<string> kinds;

    static void Prefix()
    {
        kinds = ReportLineIcons.Snapshot();
    }

    static void Postfix(StartOfDayUIManager __instance)
    {
        ReportLineIcons.DecorateLog(__instance, kinds);
    }
}

[HarmonyPatch(typeof(AdvCalendarUIManager), nameof(AdvCalendarUIManager.UpdateAttractivenessSubsection))]
static class ReplaceAttractivenessSprites
{
    static void Postfix(AdvCalendarUIManager __instance)
    {
        AttractivenessStages.Apply(__instance);
    }
}

[HarmonyPatch(typeof(ModConfigWindow), "BuildEntry")]
static class ShowReportFontPixels
{
    const string Key = "report_font_size";

    static void Postfix(CustomUIBuilder b, ConfigEntry entry)
    {
        if (b == null || entry == null || entry.Key != Key || entry.Config == null || entry.Config.ModId != ModEntry.Id)
        {
            return;
        }

        CustomUIWindow window = b.Handle;
        if (window == null)
        {
            return;
        }

        CustomUIElement slider = window.Get(Key);
        CustomUIElement label = window.Get("label_" + Key);
        if (slider == null || slider.slider == null || label == null)
        {
            return;
        }

        void Show(float value)
        {
            label.SetText(Label(entry, Mathf.RoundToInt(value)));
        }

        Show(slider.slider.value);
        slider.slider.onValueChanged.AddListener(Show);
    }

    static string Label(ConfigEntry entry, int size)
    {
        string title = entry.Label != null ? entry.Label() : "Report font size";
        string text = title + "  " + size + " pt";
        if (entry.Tooltip == null || entry.Config == null)
        {
            return text;
        }

        string link = "modcfg:" + entry.Config.ModId + ":" + entry.Key;
        return text + " <link=\"" + link + "\"><sprite name=\"info\" tint=1></link>";
    }
}
