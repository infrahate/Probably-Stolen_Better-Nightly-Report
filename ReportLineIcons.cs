using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ReportLineIcons
{
    const string IconName = "ReportLineIcon";
    const float GameFontSize = 11f;
    const float IconScale = 2.5f;
    const float IconSizeMax = 32f;
    const float TextGap = 4f;

    const string DataKey = "report_line_kinds";

    static readonly Stack<string> callers = new Stack<string>();
    static readonly List<string> captured = new List<string>();
    static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    static List<(string kind, string text)> templates;

    public static void Push(string kind)
    {
        callers.Push(kind);
    }

    public static void Pop()
    {
        if (callers.Count > 0)
        {
            callers.Pop();
        }
    }

    public static void Capture(object[] args)
    {
        string kind = callers.Count > 0 ? callers.Peek() : "info";
        string text = args != null && args.Length > 0 ? args[0] as string : null;
        string specific = ReportLines.KindForLoggedText(text);
        if (specific != null)
        {
            kind = specific;
        }

        captured.Add(kind);
        Remember();
    }

    public static List<string> Snapshot()
    {
        if (captured.Count == 0)
        {
            return LoadSaved();
        }

        var copy = new List<string>(captured);
        captured.Clear();
        return copy;
    }

    public static bool ShowLine(string kind, string text)
    {
        StartOfDayUIManager panel = StartOfDayUIManager.Instance;
        if (panel == null || panel.contentGroupObject == null || panel.startOfDayTMPPrefab == null)
        {
            return false;
        }

        if (panel.UIPanel != null && !panel.UIPanel.activeInHierarchy)
        {
            return false;
        }

        var tmp = UnityEngine.Object.Instantiate(panel.startOfDayTMPPrefab, panel.contentGroupObject.transform).GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            return false;
        }

        tmp.fontSizeMin = GameFontSize;
        tmp.text = text;
        ApplyFontSize(tmp);
        Attach(tmp, kind);
        var rect = panel.contentGroupObject.transform as RectTransform;
        if (rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        return true;
    }

    public static void ShowPrepared(List<string> kinds, List<string> texts)
    {
        StartOfDayUIManager panel = StartOfDayUIManager.Instance;
        if (panel == null || panel.contentGroupObject == null || panel.startOfDayTMPPrefab == null || kinds == null || texts == null)
        {
            return;
        }

        Transform content = panel.contentGroupObject.transform;
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(content.GetChild(i).gameObject);
        }

        int count = Math.Min(kinds.Count, texts.Count);
        for (int i = 0; i < count; i++)
        {
            var tmp = UnityEngine.Object.Instantiate(panel.startOfDayTMPPrefab, content).GetComponent<TextMeshProUGUI>();
            if (tmp == null)
            {
                continue;
            }

            tmp.fontSizeMin = GameFontSize;
            tmp.text = texts[i];
            ApplyFontSize(tmp);
            Attach(tmp, kinds[i]);
        }

        var rect = content as RectTransform;
        if (rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    public static void ShowBuffer()
    {
        StartOfDayUIManager panel = StartOfDayUIManager.Instance;
        if (panel == null || panel.contentGroupObject == null || panel.startOfDayTMPPrefab == null)
        {
            return;
        }

        Transform content = panel.contentGroupObject.transform;
        List<string> logs = PlayerStore.Instance != null ? PlayerStore.Instance.nightLogs : null;
        if (logs != null && logs.Count > 0)
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(content.GetChild(i).gameObject);
            }

            for (int i = 0; i < logs.Count; i++)
            {
                var tmp = UnityEngine.Object.Instantiate(panel.startOfDayTMPPrefab, content).GetComponent<TextMeshProUGUI>();
                if (tmp == null)
                {
                    continue;
                }

                tmp.fontSizeMin = GameFontSize;
                tmp.text = "-" + logs[i];
                ApplyFontSize(tmp);
            }
        }
        else
        {
            for (int i = 0; i < content.childCount; i++)
            {
                Transform icon = content.GetChild(i).Find(IconName);
                if (icon != null)
                {
                    UnityEngine.Object.DestroyImmediate(icon.gameObject);
                }
            }
        }

        DecorateLog(panel, new List<string>());
        var rect = content as RectTransform;
        if (rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    public static void ClearBuffer()
    {
        if (PlayerStore.Instance != null && PlayerStore.Instance.nightLogs != null)
        {
            PlayerStore.Instance.nightLogs.Clear();
        }

        captured.Clear();
        Remember();
        StartOfDayUIManager panel = StartOfDayUIManager.Instance;
        if (panel == null || panel.contentGroupObject == null)
        {
            return;
        }

        Transform content = panel.contentGroupObject.transform;
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(content.GetChild(i).gameObject);
        }
    }

    public static void DecorateLog(StartOfDayUIManager panel, List<string> kinds)
    {
        if (panel == null || panel.contentGroupObject == null || kinds == null)
        {
            return;
        }

        Transform content = panel.contentGroupObject.transform;
        int lines = content.childCount;
        int stored = kinds.Count;
        int useStored = Math.Min(stored, lines);
        int storedOffset = stored - useStored;
        int fromText = lines - useStored;
        int marked = 0;
        for (int i = 0; i < lines; i++)
        {
            var line = content.GetChild(i).GetComponent<TextMeshProUGUI>();
            if (line == null)
            {
                continue;
            }

            if (line.text.StartsWith("-"))
            {
                line.text = line.text.Substring(1).TrimStart();
            }

            string kind = i < fromText ? Classify(line.text) : kinds[storedOffset + (i - fromText)];
            ApplyFontSize(line);
            Attach(line, kind);
            marked++;
        }

        if (marked > 0)
        {
            ModEntry.Log?.Log("Marked " + marked + " report lines.");
        }
    }

    static void Remember()
    {
        if (string.IsNullOrEmpty(ModEntry.Id))
        {
            return;
        }

        ModHelper.SetModData(ModEntry.Id, DataKey, string.Join("\n", captured));
    }

    static List<string> LoadSaved()
    {
        var loaded = new List<string>();
        if (string.IsNullOrEmpty(ModEntry.Id))
        {
            return loaded;
        }

        string saved = ModHelper.GetModData(ModEntry.Id, DataKey, "");
        if (string.IsNullOrEmpty(saved))
        {
            return loaded;
        }

        string[] parts = saved.Split('\n');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
            {
                loaded.Add(parts[i]);
            }
        }

        return loaded;
    }

    static string Classify(string line)
    {
        string plain = Tags.Replace(line ?? "", "").Trim();
        if (plain.Length == 0)
        {
            return "info";
        }

        EnsureTemplates();
        string bestKind = "info";
        int best = 0;
        for (int i = 0; i < templates.Count; i++)
        {
            string text = templates[i].text;
            if (string.Equals(plain, text, StringComparison.Ordinal))
            {
                return templates[i].kind;
            }

            int score = LiteralScore(plain, text);
            if (score > best)
            {
                best = score;
                bestKind = templates[i].kind;
            }
        }

        return best >= 12 ? bestKind : "info";
    }

    static int LiteralScore(string line, string template)
    {
        string[] parts = Placeholder.Split(template);
        int score = 0;
        bool any = false;
        for (int i = 0; i < parts.Length; i++)
        {
            string chunk = parts[i].Trim();
            if (chunk.Length < 4)
            {
                continue;
            }

            any = true;
            if (line.IndexOf(chunk, StringComparison.Ordinal) < 0)
            {
                return 0;
            }

            score += chunk.Length;
        }

        return any ? score : 0;
    }

    static readonly Regex Tags = new Regex("<[^>]+>", RegexOptions.Compiled);
    static readonly Regex Placeholder = new Regex(@"\{\d+\}", RegexOptions.Compiled);

    static void EnsureTemplates()
    {
        if (templates != null)
        {
            return;
        }

        templates = new List<(string kind, string text)>();
        ReportLines.Line[] lines = ReportLines.All;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] keys = lines[i].Keys;
            for (int k = 0; k < keys.Length; k++)
            {
                AddTemplate(lines[i].Kind, keys[k]);
            }
        }

        ReportLines.AddTrashLevelTemplates(AddRawTemplate);
    }

    static void AddRawTemplate(string kind, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        templates.Add((kind, Tags.Replace(text, "").Trim()));
    }

    static void AddTemplate(string kind, string key)
    {
        string text = ReportLines.Localized(key);
        if (string.IsNullOrEmpty(text) || text == key)
        {
            return;
        }

        templates.Add((kind, Tags.Replace(text, "").Trim()));
    }

    public static void ApplyFontSize()
    {
        StartOfDayUIManager panel = StartOfDayUIManager.Instance;
        if (panel == null || panel.contentGroupObject == null)
        {
            return;
        }

        Transform content = panel.contentGroupObject.transform;
        for (int i = 0; i < content.childCount; i++)
        {
            ApplyFontSize(content.GetChild(i).GetComponent<TextMeshProUGUI>());
        }

        var rect = content as RectTransform;
        if (rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    static void ApplyFontSize(TextMeshProUGUI tmp)
    {
        if (tmp == null)
        {
            return;
        }

        int size = ChosenFontSize();
        if (size <= 0)
        {
            tmp.fontSizeMin = GameFontSize;
            tmp.enableAutoSizing = true;
        }
        else
        {
            tmp.enableAutoSizing = false;
            tmp.fontSize = size;
        }

        FitIcon(tmp);
    }

    static float IconPixels()
    {
        int size = ChosenFontSize();
        return IconPixelsFor(size > 0 ? size : (int)GameFontSize);
    }

    public static int IconPixelsFor(int fontSize)
    {
        float basis = fontSize > 0 ? fontSize : GameFontSize;
        return Mathf.Min(Mathf.RoundToInt(IconSizeMax), Mathf.RoundToInt(IconScale * basis));
    }

    static float InsetFor(float icon)
    {
        return icon + TextGap;
    }

    static void FitIcon(TextMeshProUGUI tmp)
    {
        float icon = IconPixels();
        Transform existing = tmp.transform.Find(IconName);
        if (existing is RectTransform rect)
        {
            rect.sizeDelta = new Vector2(icon, icon);
        }

        float inset = InsetFor(icon);
        Vector4 margin = tmp.margin;
        tmp.margin = new Vector4(inset, margin.y, margin.z, margin.w);
    }

    static int ChosenFontSize()
    {
        if (ModEntry.UseReportFontSize == null || !ModEntry.UseReportFontSize.Value || ModEntry.ReportFontSize == null)
        {
            return 0;
        }

        return ModEntry.ReportFontSize.Value;
    }

    static void Attach(TextMeshProUGUI tmp, string kind)
    {
        if (tmp == null || tmp.transform.Find(IconName) != null)
        {
            return;
        }

        var iconObject = new GameObject(IconName, typeof(RectTransform));
        iconObject.transform.SetParent(tmp.transform, false);
        var rect = iconObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(1f, 0f);
        float icon = IconPixels();
        rect.sizeDelta = new Vector2(icon, icon);

        Sprite sprite = SpriteFor(kind);
        if (sprite == null)
        {
            UnityEngine.Object.DestroyImmediate(iconObject);
            return;
        }

        var image = iconObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.raycastTarget = false;

        Vector4 margin = tmp.margin;
        float inset = InsetFor(icon);
        if (margin.x < inset)
        {
            tmp.margin = new Vector4(inset, margin.y, margin.z, margin.w);
        }
    }

    static readonly HashSet<string> missingIcons = new HashSet<string>();

    static Sprite SpriteFor(string kind)
    {
        if (string.IsNullOrEmpty(kind))
        {
            kind = "info";
        }

        if (sprites.TryGetValue(kind, out Sprite cached))
        {
            return cached;
        }

        if (string.IsNullOrEmpty(ModEntry.Id))
        {
            return null;
        }

        byte[] bytes = ModHelper.ReadModFileBytes(ModEntry.Id, "Icons/" + kind + ".png");
        if (bytes == null || bytes.Length == 0)
        {
            if (missingIcons.Add(kind))
            {
                ModEntry.Log?.Log("Missing Icons/" + kind + ".png");
            }

            return null;
        }

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(bytes))
        {
            UnityEngine.Object.Destroy(texture);
            if (missingIcons.Add(kind))
            {
                ModEntry.Log?.Log("Icons/" + kind + ".png is not a readable PNG.");
            }

            return null;
        }

        texture.name = "ReportIcon_" + kind;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.hideFlags = HideFlags.HideAndDontSave;

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        sprite.name = texture.name;
        sprite.hideFlags = HideFlags.HideAndDontSave;
        sprites[kind] = sprite;
        return sprite;
    }
}
