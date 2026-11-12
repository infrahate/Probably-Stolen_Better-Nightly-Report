using System.Collections.Generic;
using UnityEngine;

public class ModEntry : IMod
{
    public static ModLog Log;
    public static string Id;
    public static ConfigEntry<bool> UseReportFontSize;
    public static ConfigEntry<int> ReportFontSize;

    public void Init(ModManifest manifest)
    {
        Id = manifest.ID;
        Log = new ModLog(manifest);
        ModConfig config = ModHelper.GetConfig(manifest.ID);
        UseReportFontSize = config.Define("report_font_custom", false, "Report", () => "Custom report font size",
            tooltip: () => "Use the size below for the morning report lines. Off keeps the game's size.");
        ReportFontSize = config.Define("report_font_size", 32, "Report", () => "Report font size",
            min: 8, max: 32,
            tooltip: () => "Point size of the morning report lines.");
        UseReportFontSize.OnChanged += _ => ReportLineIcons.ApplyFontSize();
        ReportFontSize.OnChanged += _ => ReportLineIcons.ApplyFontSize();
        Log.Log("Report tab icons ready.");
    }

    static bool testHooked;
    static bool debugRegistered;

    public void OnEnable()
    {
        if (!testHooked)
        {
            ModHook.OnWakingUpLate += AddDeveloperLines;
            testHooked = true;
        }

        if (!debugRegistered && !string.IsNullOrEmpty(Id))
        {
            ModHelper.AddDebugFunction(Id, "Special", "Report lines", AddDeveloperLines);
            ModHelper.AddDebugFunction(Id, "Special", "Attractiveness 1000", SetAttractivenessToMax);
            debugRegistered = true;
        }
    }

    public void OnDisable()
    {
        if (testHooked)
        {
            ModHook.OnWakingUpLate -= AddDeveloperLines;
            testHooked = false;
        }

        if (debugRegistered)
        {
            ModHelper.RemoveDebugFunctions(Id);
            debugRegistered = false;
        }
    }

    static void AddDeveloperLines()
    {
        ModTrustRecord record = ModTrust.Get(Id);
        if (record == null || !record.developerMode)
        {
            Log?.Log("Report line picker is off until Developer mode is on for this mod.");
            return;
        }

        CustomUIBuilder window = CustomUIManager.Window(Id + ".report-lines", "Report lines");
        if (window == null)
        {
            Log?.Log("Report line picker could not open.");
            return;
        }

        window.SetSize(560f, 640f).SetCloseOnEscape(true);
        window.AddButton("Open nightly report", OpenNightlyReport);
        window.AddButton("Feed next 8 lines", FeedNextPage);
        window.AddButton("Clear night log", ClearNightLog);
        window.AddLabel("Click a line to add it to this morning's report.");
        window.BeginScroll(520f);
        List<ReportLines.Sample> samples = ReportLines.Samples();
        for (int i = 0; i < samples.Count; i++)
        {
            int index = i;
            window.AddButton(samples[i].Label, () => ReportLines.AddSample(index));
        }

        window.End();
        window.Handle.Center();
        window.Handle.BringToFront();
        RaiseAboveReport(window.Handle.root);
        Log?.Log("Report line picker open.");
    }

    static void OpenNightlyReport()
    {
        AdvCalendarUIManager calendar = AdvCalendarUIManager.Instance;
        if (calendar == null)
        {
            Log?.Log("Nightly report is not available.");
            return;
        }

        calendar.OpenUIFromNightlyReport();
        ReportLineIcons.ShowBuffer();
        KeepPickerInFront();
    }

    static int feedPage;

    static void FeedNextPage()
    {
        AdvCalendarUIManager calendar = AdvCalendarUIManager.Instance;
        if (calendar == null)
        {
            Log?.Log("Nightly report is not available.");
            return;
        }

        ReportLineIcons.ClearBuffer();
        feedPage = ReportLines.FeedPage(feedPage, 8);
        calendar.OpenUIFromNightlyReport();
        KeepPickerInFront();
    }

    static void SetAttractivenessToMax()
    {
        PlayerStore store = PlayerStore.Instance;
        if (store == null)
        {
            Log?.Log("Attractiveness can be set once a store is loaded.");
            return;
        }

        store.ClearTrash();
        store.projectorAttractivenessBonus = 0;
        store.baseStoreAttractiveness = 1000;
        AdvCalendarUIManager calendar = AdvCalendarUIManager.Instance;
        if (calendar != null)
        {
            calendar.UpdateAttractivenessSubsection();
        }

        Log?.Log("Store attractiveness set to " + store.GetCurrentStoreAttractiveness() + ".");
    }

    static void ClearNightLog()
    {
        feedPage = 0;
        ReportLineIcons.ClearBuffer();
        Log?.Log("Night log buffer cleared.");
    }

    static void KeepPickerInFront()
    {
        CustomUIWindow picker = CustomUIManager.Instance != null ? CustomUIManager.Instance.GetWindow(Id + ".report-lines") : null;
        if (picker == null)
        {
            return;
        }

        picker.BringToFront();
        RaiseAboveReport(picker.root);
    }

    static void RaiseAboveReport(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        Canvas canvas = root.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }

        canvas.overrideSorting = true;
        if (canvas.sortingOrder < 2000)
        {
            canvas.sortingOrder = 2000;
        }
    }
}
