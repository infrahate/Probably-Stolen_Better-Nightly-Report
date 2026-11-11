using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class AttractivenessStages
{
    const float IconSize = 32f;

    static readonly int[] Thresholds = { 100, 200, 250, 300, 400, 450, 500, 650, 750, 900 };

    static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    static readonly HashSet<string> missing = new HashSet<string>();
    static int lastReport = int.MinValue;

    public static void Apply(AdvCalendarUIManager ui)
    {
        if (ui == null)
        {
            return;
        }

        GameObject root = ui.UIPanel != null ? ui.UIPanel : ui.gameObject;
        if (root == null)
        {
            return;
        }

        int current = PlayerStore.Instance != null ? PlayerStore.Instance.GetCurrentStoreAttractiveness() : 0;
        int replaced = 0;
        int reached = 0;
        var seen = new HashSet<int>();
        TooltipContainer[] containers = root.GetComponentsInChildren<TooltipContainer>(true);
        for (int i = 0; i < containers.Length; i++)
        {
            int stage = ApplyTo(ui, containers[i], containers[i].content, current, seen);
            if (stage < 0)
            {
                continue;
            }

            replaced++;
            if (IsLit(stage, current))
            {
                reached++;
            }
        }

        CustomUITooltipTrigger[] triggers = root.GetComponentsInChildren<CustomUITooltipTrigger>(true);
        for (int i = 0; i < triggers.Length; i++)
        {
            int stage = ApplyTo(ui, triggers[i], triggers[i].tooltipText, current, seen);
            if (stage < 0)
            {
                continue;
            }

            replaced++;
            if (IsLit(stage, current))
            {
                reached++;
            }
        }

        int report = replaced * 1000 + reached;
        if (report == lastReport)
        {
            return;
        }

        lastReport = report;
        if (replaced > 0)
        {
            ModEntry.Log?.Log("Attractiveness " + current + ": " + reached + " of " + replaced + " stage icons lit.");
        }
        else
        {
            ModEntry.Log?.Log("No attractiveness stage icons found on the report.");
        }
    }

    static int ApplyTo(AdvCalendarUIManager ui, Component host, string text, int current, HashSet<int> seen)
    {
        int stage = StageOf(text);
        if (stage < 0 || host == null)
        {
            return -1;
        }

        Image image = ImageFor(ui, host);
        if (image != null && !seen.Add(image.GetInstanceID()))
        {
            return -1;
        }
        bool on = IsLit(stage, current);
        Sprite enabled = SpriteFor(stage, true);
        Sprite disabled = SpriteFor(stage, false);
        Sprite shown = on ? enabled : disabled;
        if (image == null || shown == null)
        {
            return -1;
        }

        image.sprite = enabled != null ? enabled : shown;
        image.color = Color.white;
        image.preserveAspect = true;
        SetSize(image.rectTransform);
        Button button = image.GetComponent<Button>();
        if (button == null)
        {
            button = host.GetComponent<Button>();
        }

        if (button == null && image.transform.parent != null)
        {
            button = image.transform.parent.GetComponent<Button>();
        }

        if (button != null)
        {
            button.transition = Selectable.Transition.SpriteSwap;
            SpriteState state = button.spriteState;
            state.highlightedSprite = shown;
            state.pressedSprite = shown;
            state.selectedSprite = shown;
            state.disabledSprite = disabled != null ? disabled : shown;
            button.spriteState = state;
            button.interactable = on;
            if (button.targetGraphic == image)
            {
                SetSize(button.transform as RectTransform);
            }
        }

        image.overrideSprite = on ? null : disabled;
        return stage;
    }

    static void SetSize(RectTransform rect)
    {
        if (rect == null)
        {
            return;
        }

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, IconSize);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, IconSize);
    }

    static bool IsLit(int stage, int current)
    {
        if (stage == Thresholds[0])
        {
            return current <= stage;
        }

        return current >= stage;
    }

    static Image ImageFor(AdvCalendarUIManager ui, Component host)
    {
        Image image = host.GetComponent<Image>();
        if (IsMeter(ui, image))
        {
            image = null;
        }

        if (image == null && host.transform.parent != null)
        {
            Image parent = host.transform.parent.GetComponent<Image>();
            if (!IsMeter(ui, parent))
            {
                image = parent;
            }
        }

        if (image != null)
        {
            return image;
        }

        Image[] images = host.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            if (!IsMeter(ui, images[i]))
            {
                return images[i];
            }
        }

        return null;
    }

    static bool IsMeter(AdvCalendarUIManager ui, Image image)
    {
        if (image == null)
        {
            return true;
        }

        return image == ui.baseMeter || image == ui.trashMeter || image == ui.projectorMeter;
    }

    static int StageOf(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return -1;
        }

        int i = 0;
        while (i < text.Length && char.IsWhiteSpace(text[i]))
        {
            i++;
        }

        int start = i;
        while (i < text.Length && char.IsDigit(text[i]))
        {
            i++;
        }

        if (i == start || i >= text.Length || text[i] != ':')
        {
            return -1;
        }

        if (!int.TryParse(text.Substring(start, i - start), out int value))
        {
            return -1;
        }

        for (int s = 0; s < Thresholds.Length; s++)
        {
            if (Thresholds[s] == value)
            {
                return value;
            }
        }

        return -1;
    }

    static Sprite SpriteFor(int stage, bool enabled)
    {
        string key = stage + (enabled ? "" : "_off");
        if (sprites.TryGetValue(key, out Sprite cached))
        {
            return cached;
        }

        if (string.IsNullOrEmpty(ModEntry.Id))
        {
            return null;
        }

        string file = "Icons/stage_" + key + ".png";
        byte[] bytes = ModHelper.ReadModFileBytes(ModEntry.Id, file);
        if (bytes == null || bytes.Length == 0)
        {
            if (missing.Add(file))
            {
                ModEntry.Log?.Log("Missing " + file);
            }

            return null;
        }

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(bytes))
        {
            UnityEngine.Object.Destroy(texture);
            if (missing.Add(file))
            {
                ModEntry.Log?.Log(file + " is not a readable PNG.");
            }

            return null;
        }

        texture.name = "AttractivenessStage_" + key;
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.hideFlags = HideFlags.HideAndDontSave;

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        sprite.name = texture.name;
        sprite.hideFlags = HideFlags.HideAndDontSave;
        sprites[key] = sprite;
        return sprite;
    }
}
