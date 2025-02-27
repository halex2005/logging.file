﻿using System;
using System.Text;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.File.Configuration
{
    /// <summary>
    /// <para>Settings of a <see cref="FileLog"/> instance.</para>
    /// <para><see cref="FileLogSettings"/> instances should be treated as immutable after construction:
    /// to reconfigure a <see cref="FileLog"/> on the fly, always create a new one instead of modifying the properties directly.</para>
    /// </summary>
    [PublicAPI]
    public class FileLogSettings
    {
        /// <summary>
        /// <para>Path to the log file. If a <see cref="RollingStrategy"/> is specified, this path serves as base path and gets combined with suffixes.</para>
        /// <para>Here are some examples of how rolling may transform file paths:</para>
        /// <list type="bullet">
        ///     <item><description><see cref="RollingStrategyType.None"/>: <c>log</c> --> <c>log</c></description></item>
        ///     <item><description><see cref="RollingStrategyType.BySize"/>: <c>log</c> --> <c>log-2</c></description></item>
        ///     <item><description><see cref="RollingStrategyType.ByTime"/>: <c>log</c> --> <c>log-2018-09-05</c></description></item>
        ///     <item><description><see cref="RollingStrategyType.Hybrid"/>: <c>log</c> --> <c>log-2018-09-05-2</c></description></item>
        /// </list>
        /// <para>The path is relative to current working directory (<see cref="Environment.CurrentDirectory"/>).</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: <see cref="FileLog"/> will switch to a new file on change.</para>
        /// </summary>
        [NotNull]
        public string FilePath { get; set; } = "logs/log";

        /// <summary>
        /// <para>The template used to render log messages. See <see cref="Formatting.OutputTemplate"/> for details.</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: it's accessed for each <see cref="LogEvent"/>.</para>
        /// </summary>
        [NotNull]
        public OutputTemplate OutputTemplate { get; set; } = OutputTemplate.Default;

        /// <summary>
        /// <para>If specified, this <see cref="IFormatProvider"/> will be used when formatting log property values.</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: it's accessed for each <see cref="LogEvent"/>.</para>
        /// </summary>
        [CanBeNull]
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// <para>Specifies the way to treat an existing log file: append (default) or rewrite.</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: <see cref="FileLog"/> will reopen the file on change.</para>
        /// </summary>
        public FileOpenMode FileOpenMode { get; set; } = FileOpenMode.Append;

        /// <summary>
        /// <para>An optional rolling strategy (disabled by default).</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: <see cref="FileLog"/> will react to its changes.</para>
        /// </summary>
        [NotNull]
        public RollingStrategyOptions RollingStrategy { get; set; } = new RollingStrategyOptions();

        /// <summary>
        /// <para>Output text encoding (UTF-8 by default).</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: <see cref="FileLog"/> will reopen the file on change.</para>
        /// </summary>
        [NotNull]
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// <para>Output buffer size, in bytes (64K by default).</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: <see cref="FileLog"/> will reopen the file on change.</para>
        /// </summary>
        public int OutputBufferSize { get; set; } = 65536;

        /// <summary>
        /// <para>A whitelist of enabled <see cref="LogLevel"/>s (contains all levels by default). Only log events with levels contained in this array will be logged.</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: it's accessed for each <see cref="LogEvent"/>.</para>
        /// </summary>
        [NotNull]
        public LogLevel[] EnabledLogLevels { get; set; } = (LogLevel[])Enum.GetValues(typeof(LogLevel));

        /// <summary>
        /// <para>Capacity of the internal log events queue.</para>
        /// <para>This parameter has a per-file scope.</para>
        /// <para>Dynamic reconfiguration is not supported for this parameter: a snapshot will be taken on first usage attempt.</para>
        /// </summary>
        public int EventsQueueCapacity { get; set; } = 50 * 1000;

        /// <summary>
        /// <para>Specifies how many log events are processed in one iteration for each file.</para>
        /// <para>This parameter has a per-file scope.</para>
        /// <para>Dynamic reconfiguration is not supported for this parameter: a snapshot will be taken on first usage attempt.</para>
        /// </summary>
        public int EventsBufferCapacity { get; set; } = 10 * 1000;

        /// <summary>
        /// <para>Cooldown for enforcing file-related settings (name, rolling strategy, buffer size, etc.). This means that when conditions are met to switch to the next part of log file or reopen the file with another name/options due to change in settings, the switching may be delayed for up to <see cref="FileSettingsUpdateCooldown"/>.</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: <see cref="FileLog"/> will react to its changes.</para>
        /// </summary>
        public TimeSpan FileSettingsUpdateCooldown { get; set; } = TimeSpan.FromSeconds(1);
    }
}