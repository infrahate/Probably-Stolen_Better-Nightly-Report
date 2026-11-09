using System;
using System.Collections.Generic;
using System.Reflection;

public static class ReportLines
{
    public readonly struct Line
    {
        public readonly string Kind;
        public readonly Type Owner;
        public readonly string Method;
        public readonly string[] Keys;

        public Line(string kind, Type owner, string method, params string[] keys)
        {
            Kind = kind;
            Owner = owner;
            Method = method;
            Keys = keys ?? Array.Empty<string>();
        }
    }

    public static readonly Line[] All =
    {
        new Line("service_clean_unpaid", typeof(NightLog), nameof(NightLog.StorefrontNotCleaned), "mech_nightlog_storefront_not_cleaned"),
        new Line("service_clean_paid", typeof(NightLog), nameof(NightLog.StorefrontClean), "mech_nightlog_storefront_clean"),
        new Line("service_clean_paid", typeof(NightLog), nameof(NightLog.VandalismCleaned), "mech_nightlog_vandalism_cleaned"),
        new Line("event_trash_lot", typeof(DebrisHelper), nameof(DebrisHelper.OnNight),
            "mech_nightlog_debris_rest_progress",
            "mech_nightlog_debris_crew_progress"),
        new Line("service_clean_paid", typeof(DebrisHelper), "ClearPile", "mech_nightlog_debris_cleared"),
        new Line("service_power_unpaid", typeof(NightLog), nameof(NightLog.PowerOff), "mech_nightlog_power_off"),
        new Line("service_power_paid", typeof(NightLog), nameof(NightLog.PowerOn), "mech_nightlog_power_on"),
        new Line("event_security_bad_2", typeof(NightLog), nameof(NightLog.InsuranceUnpaid), "mech_nightlog_insurance_unpaid"),
        new Line("event_security_protection", typeof(NightLog), nameof(NightLog.InsurancePaid), "mech_nightlog_insurance_paid"),
        new Line("event_security_protection", typeof(NightLog), nameof(NightLog.BurglaryGrace), "mech_nightlog_burglary_grace"),
        new Line("event_security_bad_1", typeof(NightLog), nameof(NightLog.BurglaryAlarmFailed), "mech_nightlog_burglary_alarm_failed"),
        new Line("event_security_bad_1", typeof(NightLog), nameof(NightLog.BurglaryNoAlarm), "mech_nightlog_burglary_no_alarm"),
        new Line("event_security_good_2", typeof(NightLog), nameof(NightLog.BurglaryAlarmStopped), "mech_nightlog_burglary_alarm_stopped"),
        new Line("event_security_good_2", typeof(NightLog), nameof(NightLog.BurglaryInsured), "mech_nightlog_burglary_insured"),
        new Line("effect_afterhour_debuff", typeof(NightLog), nameof(NightLog.AfterhourDebuff), "mech_nightlog_afterhour_debuff"),
        new Line("event_aug", typeof(NightLog), nameof(NightLog.AugLeftAlive), "mech_nightlog_aug_left_alive"),
        new Line("event_aug", typeof(NightLog), nameof(NightLog.AugIncoming), "mech_nightlog_aug_incoming"),
        new Line("event_wanted_visited", typeof(NightLog), nameof(NightLog.WantedPersonVisited), "mech_nightlog_wanted_visited"),
        new Line("event_promo_offer", typeof(NightLog), nameof(NightLog.AddNewPromotionalOffer), "mech_nightlog_promo_offer"),
        new Line("service_referral_buyer", typeof(NightLog), nameof(NightLog.BuyerReferral), "mech_nightlog_buyer_referral"),
        new Line("service_referral_seller", typeof(NightLog), nameof(NightLog.SellerReferral), "mech_nightlog_seller_referral"),
        new Line("service_referral_buyer", typeof(NightLog), nameof(NightLog.CriminalNetwork), "mech_nightlog_criminal_network"),
        new Line("event_inspection_soon", typeof(NightLog), nameof(NightLog.InspectionWarningIn2Days), "mech_nightlog_inspection_2days"),
        new Line("event_inspection_tommorow", typeof(NightLog), nameof(NightLog.InspectionWarningTomorrow), "mech_nightlog_inspection_tomorrow"),
        new Line("event_inspection_now", typeof(NightLog), nameof(NightLog.InspectionWarningImminent), "mech_nightlog_inspection_imminent"),
        new Line("event_upgrade_tip", typeof(NightLog), nameof(NightLog.UpgradeTip), "mech_nightlog_upgrade_tip"),
        new Line("event_security_bad_2", typeof(NightLog), nameof(NightLog.StingOperationWarning), "mech_nightlog_sting_warning"),
        new Line("event_security_bad_1", typeof(NightLog), nameof(NightLog.SecurityInvestigationNearComplete), "mech_nightlog_investigation_near"),
        new Line("health_state_positive", typeof(NightLog), nameof(NightLog.AgoraphobiaReady), "mech_nightlog_agoraphobia_ready"),
        new Line("service_fixer", typeof(NightLog), nameof(NightLog.FixerUsed), "mech_nightlog_fixer_used"),
        new Line("event_security_protection", typeof(NightLog), nameof(NightLog.CartelProtectionActive), "mech_nightlog_cartel_protection"),
        new Line("event_trash_extreme", typeof(NightLog), nameof(NightLog.VandalizedByFaction),
            "mech_nightlog_vandalized_lower",
            "mech_nightlog_vandalized_security",
            "mech_nightlog_vandalized_upper",
            "mech_nightlog_vandalized_revolution",
            "mech_nightlog_vandalized_cartel",
            "mech_nightlog_vandalized_generic"),
        new Line("event_trash_extreme", typeof(PlayerStore), nameof(PlayerStore.HandleVandalism),
            "mech_nightlog_vandalized_john_smith",
            "mech_nightlog_vandalism_butcher"),
        new Line("event_trash_extreme", typeof(NightLog), nameof(NightLog.StoreStillVandalized), "mech_nightlog_store_vandalized"),
        new Line("operation", typeof(NightLog), nameof(NightLog.FactionOperationDateShared), "mech_nightlog_faction_op_date"),
        new Line("event_contact_ready", typeof(NightLog), nameof(NightLog.ContactReady), "mech_nightlog_contact_ready"),
        new Line("service_power_paid", typeof(NightLog), nameof(NightLog.BlackoutBypassed), "mech_nightlog_blackout_bypassed"),
        new Line("service_power_unpaid", typeof(NightLog), nameof(NightLog.BlackoutActive), "mech_nightlog_blackout_active"),
        new Line("health_state_better", typeof(HealthData), nameof(HealthData.HandleNightlyWound), "health_wound_stabilized"),
        new Line("health_state_positive", typeof(HealthData), nameof(HealthData.HandleNightlyWound), "health_wound_healed"),
        new Line("health_state_positive", typeof(HealthData), nameof(HealthData.HandleNightlyWound), "health_wound_mobility_regained"),
        new Line("health_state_better", typeof(HealthData), nameof(HealthData.HandleNightlyWound), "health_wound_minor_healing"),
        new Line("health_state_negative", typeof(HealthData), nameof(HealthData.HandleNightlyWound), "health_wound_major_lost_mobility"),
        new Line("health_state_worse", typeof(HealthData), nameof(HealthData.HandleNightlyWound), "health_wound_major_worsened"),
        new Line("health_state_worse", typeof(HealthData), nameof(HealthData.HandleNightlyWound), "health_wound_minor_worsened"),
        new Line("health_addiction_smoking_overcome", typeof(HealthData), nameof(HealthData.OnSleep), "health_addiction_overcome_nicotine"),
        new Line("health_addiction_smoking_withdrawal", typeof(HealthData), nameof(HealthData.OnSleep), "health_addiction_withdrawal_nicotine"),
        new Line("health_addiction_drinking_overcome", typeof(HealthData), nameof(HealthData.OnSleep), "health_addiction_overcome_alcohol"),
        new Line("health_addiction_drinking_withdrawal", typeof(HealthData), nameof(HealthData.OnSleep), "health_addiction_withdrawal_alcohol"),
        new Line("health_addiction_narcotic_overcome", typeof(HealthData), nameof(HealthData.OnSleep), "health_addiction_overcome_narcotic"),
        new Line("health_addiction_narcotic_withdrawal", typeof(HealthData), nameof(HealthData.OnSleep), "health_addiction_withdrawal_narcotic"),
        new Line("health_state_negative", typeof(HealthData), nameof(HealthData.OnSleep), "health_sentinel_critical"),
        new Line("health_state_worse", typeof(HealthData), nameof(HealthData.OnSleep), "health_sentinel_severe"),
        new Line("health_state_worse", typeof(HealthData), nameof(HealthData.OnSleep), "health_sentinel_moderate"),
        new Line("health_state_worse", typeof(HealthData), nameof(HealthData.OnSleep), "health_sentinel_mild"),
        new Line("sentinel_stage_1", typeof(HealthData), "AddSentinelDamageLog", "health_sentinel_collapse_light"),
        new Line("sentinel_stage_2", typeof(HealthData), "AddSentinelDamageLog", "health_sentinel_collapse_advanced"),
        new Line("sentinel_stage_3", typeof(HealthData), "AddSentinelDamageLog", "health_sentinel_collapse_final"),
        new Line("sentinel_stage_4", typeof(HealthData), "AddSentinelDamageLog", "health_sentinel_collapse_lethal"),
        new Line("investigation_stage_1", typeof(SecData), nameof(SecData.OnNewDay), "sec_data_investigation_fast"),
        new Line("investigation_stage_2", typeof(SecData), nameof(SecData.OnNewDay), "sec_data_investigation_very_fast"),
        new Line("luck_gone", typeof(LiquidLuckHelper), nameof(LiquidLuckHelper.OnSleep), "mech_liquid_luck_wears_off"),
        new Line("luck_good", typeof(LotteryHelper), nameof(LotteryHelper.DrawWinningNumberWithLuck), "mech_lottery_luck_good"),
        new Line("luck_bad", typeof(LotteryHelper), nameof(LotteryHelper.DrawWinningNumberWithLuck), "mech_lottery_luck_bad"),
        new Line("reactor_damaged", typeof(ReactorHelper), nameof(ReactorHelper.OnNight), "mech_nightlog_reactor_no_power"),
        new Line("reactor_critical", typeof(ReactorHelper), nameof(ReactorHelper.OnNight), "mech_nightlog_reactor_critical"),
        new Line("mislabel", typeof(VendingMachineHelper), "SettleRequestMislabels", "mech_nightlog_request_mislabel"),
        new Line("mislabel", typeof(VendingMachineHelper), "SettleVendingMislabels", "mech_nightlog_vending_mislabel"),
        new Line("reputation_lost", typeof(VendingMachineHelper), "SettleMislabelRep", "ui_rep_lost"),
        new Line("event_butcher_hunted", typeof(PlayerStore), nameof(PlayerStore.HandleButcherHunting), "mech_nightlog_butcher_hunted"),
        new Line("event_dividend", typeof(ShareholderHelper), nameof(ShareholderHelper.HandleDividends), "mech_nightlog_dividend"),
        new Line("story_good", typeof(SerpicoStoryline), nameof(SerpicoStoryline.OnLeaveDumpingGround),
            "mech_nightlog_serpico_food",
            "mech_nightlog_serpico_food_extra_day"),
        new Line("story_good", typeof(SerpicoStoryline), "DeliverGeorgioFood", "mech_nightlog_serpico_food_georgio"),
    };

    static readonly (string kind, string key)[] TrashLevels =
    {
        ("event_trash_clean", "mech_nightlog_garbage_clean"),
        ("event_trash_some", "mech_nightlog_garbage_some"),
        ("event_trash_lot", "mech_nightlog_garbage_lot"),
        ("event_trash_extreme", "mech_nightlog_garbage_extreme"),
    };

    static bool SplitByLoggedText(Type owner, string method)
    {
        if (owner == typeof(HealthData))
        {
            return method == nameof(HealthData.HandleNightlyWound) || method == nameof(HealthData.OnSleep) || method == "AddSentinelDamageLog";
        }

        if (owner == typeof(LotteryHelper))
        {
            return method == nameof(LotteryHelper.DrawWinningNumberWithLuck);
        }

        if (owner == typeof(ReactorHelper))
        {
            return method == nameof(ReactorHelper.OnNight);
        }

        return owner == typeof(SecData) && method == nameof(SecData.OnNewDay);
    }

    static string SplitFallback(Type owner)
    {
        if (owner == typeof(LotteryHelper))
        {
            return "luck";
        }

        if (owner == typeof(ReactorHelper))
        {
            return "reactor";
        }

        if (owner == typeof(SecData))
        {
            return "event_security_bad_2";
        }

        return "health";
    }

    public static string KindForLoggedText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        for (int i = 0; i < All.Length; i++)
        {
            if (!SplitByLoggedText(All[i].Owner, All[i].Method))
            {
                continue;
            }

            string[] keys = All[i].Keys;
            for (int k = 0; k < keys.Length; k++)
            {
                string localized = LocHelper.GetLocalizedMechanic(keys[k]);
                if (!string.IsNullOrEmpty(localized) && localized != keys[k] && text == localized)
                {
                    return All[i].Kind;
                }
            }
        }

        return null;
    }

    public static string KindFor(MethodBase method)
    {
        return KindFor(method, null);
    }

    public static string KindFor(MethodBase method, object[] args)
    {
        if (method == null)
        {
            return "info";
        }

        if (method.DeclaringType == typeof(NightLog) && method.Name == nameof(NightLog.StorefrontNotCleaned))
        {
            string level = TrashLevel(args);
            if (level != null)
            {
                return level;
            }
        }

        if (SplitByLoggedText(method.DeclaringType, method.Name))
        {
            return SplitFallback(method.DeclaringType);
        }

        for (int i = 0; i < All.Length; i++)
        {
            if (All[i].Owner == method.DeclaringType && All[i].Method == method.Name)
            {
                return All[i].Kind;
            }
        }

        return "info";
    }

    public static void AddTrashLevelTemplates(Action<string, string> add)
    {
        for (int i = 0; i < TrashLevels.Length; i++)
        {
            add(TrashLevels[i].kind, TrashLevelText(TrashLevels[i].key));
        }
    }

    static string TrashLevel(object[] args)
    {
        if (args == null || args.Length == 0)
        {
            return null;
        }

        string detail = args[0] as string;
        if (string.IsNullOrEmpty(detail))
        {
            return null;
        }

        for (int i = 0; i < TrashLevels.Length; i++)
        {
            if (detail == LocHelper.GetLocalizedMechanic(TrashLevels[i].key))
            {
                return TrashLevels[i].kind;
            }
        }

        return null;
    }

    static string TrashLevelText(string key)
    {
        return LocHelper.GetLocalizedMechanic("mech_nightlog_storefront_not_cleaned", LocHelper.GetLocalizedMechanic(key));
    }

    public readonly struct Sample
    {
        public readonly string Kind;
        public readonly string Label;
        public readonly string Text;

        public Sample(string kind, string label, string text)
        {
            Kind = kind;
            Label = label;
            Text = text;
        }
    }

    static List<Sample> samples;

    public static List<Sample> Samples()
    {
        if (samples != null)
        {
            return samples;
        }

        samples = new List<Sample>();
        for (int i = 0; i < All.Length; i++)
        {
            if (All[i].Owner == typeof(NightLog) && All[i].Method == nameof(NightLog.StorefrontNotCleaned))
            {
                for (int level = 0; level < TrashLevels.Length; level++)
                {
                    samples.Add(new Sample(TrashLevels[level].kind, TrashLevels[level].key, TrashLevelText(TrashLevels[level].key)));
                }

                continue;
            }

            string[] keys = All[i].Keys;
            if (keys.Length == 0)
            {
                string name = All[i].Owner.Name + "." + All[i].Method;
                samples.Add(new Sample(All[i].Kind, name, name));
                continue;
            }

            for (int k = 0; k < keys.Length; k++)
            {
                string text = Localized(keys[k], "1", "2", "3");
                if (keys[k] == "ui_rep_lost")
                {
                    text += " (-1)";
                }

                samples.Add(new Sample(All[i].Kind, keys[k], text));
            }
        }

        return samples;
    }

    public static void AddSample(int index)
    {
        List<Sample> list = Samples();
        if (index < 0 || index >= list.Count)
        {
            return;
        }

        Sample sample = list[index];
        if (ReportLineIcons.ShowLine(sample.Kind, sample.Text))
        {
            return;
        }

        Write(sample.Kind, sample.Text);
    }

    public static int FeedPage(int page, int pageSize)
    {
        List<Sample> list = Samples();
        if (list.Count == 0 || pageSize < 1)
        {
            return 0;
        }

        int pages = (list.Count + pageSize - 1) / pageSize;
        if (page < 0 || page >= pages)
        {
            page = 0;
        }

        int start = page * pageSize;
        int count = Math.Min(pageSize, list.Count - start);
        var kinds = new List<string>(count);
        var texts = new List<string>(count);
        for (int i = 0; i < count; i++)
        {
            Sample sample = list[start + i];
            kinds.Add(sample.Kind);
            texts.Add(sample.Text);
            Write(sample.Kind, sample.Text);
        }

        ReportLineIcons.ShowPrepared(kinds, texts);
        ModEntry.Log?.Log("Fed report page " + (page + 1) + " of " + pages + " (" + count + " lines).");
        return page + 1 >= pages ? 0 : page + 1;
    }

    public static string Localized(string key, params object[] arguments)
    {
        string text = LocHelper.GetLocalizedMechanic(key, arguments);
        if (IsTranslation(text, key))
        {
            return text;
        }

        string ui = LocHelper.GetLocalizedUI(key, arguments);
        if (IsTranslation(ui, key))
        {
            return ui;
        }

        return text;
    }

    static bool IsTranslation(string text, string key)
    {
        string table;
        string missing;
        return !string.IsNullOrEmpty(text) && text != key && !LocHelper.TryParseMissingTranslation(text, out table, out missing);
    }

    static void Write(string kind, string text)
    {
        ReportLineIcons.Push(kind);
        try
        {
            PlayerStore.Instance.AddNightLog(text);
        }
        finally
        {
            ReportLineIcons.Pop();
        }
    }
}
