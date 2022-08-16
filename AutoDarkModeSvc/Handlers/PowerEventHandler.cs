﻿using AutoDarkModeConfig;
using AutoDarkModeSvc.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Windows.System.Power;

namespace AutoDarkModeSvc.Handlers
{
    static class PowerEventHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool darkThemeOnBatteryEnabled;
        private static bool resumeEventEnabled;

        public static void RegisterThemeEvent()
        {
            if (!darkThemeOnBatteryEnabled)
            {
                Logger.Info("enabling event handler for dark mode on battery state discharging");
                PowerManager.BatteryStatusChanged += PowerManager_BatteryStatusChanged;
                darkThemeOnBatteryEnabled = true;
            }
        }

        private static void PowerManager_BatteryStatusChanged(object sender, object e)
        {
            AdmConfigBuilder builder = AdmConfigBuilder.Instance();
            if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline)
            {
                Logger.Info("battery discharging, enabling dark mode");
                ThemeManager.UpdateTheme(builder.Config, Theme.Dark, new(SwitchSource.BatteryStatusChanged));
            }
            else
            {
                ThemeManager.RequestSwitch(builder, new(SwitchSource.BatteryStatusChanged));
            }
        }

        public static void DeregisterThemeEvent()
        {
            try
            {
                if (darkThemeOnBatteryEnabled)
                {
                    Logger.Info("disabling event handler for dark mode on battery state discharging");
                    PowerManager.BatteryStatusChanged -= PowerManager_BatteryStatusChanged;
                    darkThemeOnBatteryEnabled = false;
                    ThemeManager.RequestSwitch(AdmConfigBuilder.Instance(), new(SwitchSource.BatteryStatusChanged));
                }
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error(ex, "while deregistering SystemEvents_PowerModeChanged ");
            }
        }

        public static void RegisterResumeEvent()
        {
            if (!resumeEventEnabled)
            {
                Logger.Info("enabling theme refresh at system resume");
                SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
                resumeEventEnabled = true;
            }
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                Logger.Info("system resuming from suspended state, refreshing theme");
                ThemeManager.RequestSwitch(AdmConfigBuilder.Instance(), new(SwitchSource.SystemResume));
            }
        }

        public static void DeregisterResumeEvent()
        {
            try
            {
                if (resumeEventEnabled)
                {
                    Logger.Info("disabling theme refresh at system resume");
                    SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
                    resumeEventEnabled = false;
                }
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error(ex, "while deregistering SystemEvents_PowerModeChanged ");
            }
        }
    }
}
