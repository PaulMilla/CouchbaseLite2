﻿using System;
using System.Reflection;
using Couchbase.Lite.Util;
using log4net;

namespace CouchbaseLite2
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog log;

        public Log4NetLogger()
        {
            log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void V(string tag, string msg)
        {
            var message = $"{tag}|{msg}";
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
                           log4net.Core.Level.Verbose,
                           message,
                           null);
        }

        public void V(string tag, string msg, Exception tr)
        {
            var message = $"{tag}|{msg}";
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
                           log4net.Core.Level.Verbose,
                           message,
                           tr);
        }

        public void V(string tag, string format, params object[] args)
        {
            var message = string.Format($"{tag}|{format}", args);
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
                           log4net.Core.Level.Verbose,
                           message,
                           null);
        }

        public void D(string tag, string msg)
        {
            log.Debug($"{tag}|{msg}");
        }

        public void D(string tag, string msg, Exception tr)
        {
            log.Debug($"{tag}|{msg}", tr);
        }

        public void D(string tag, string format, params object[] args)
        {
            log.DebugFormat($"{tag}|{format}", args);
        }

        public void I(string tag, string msg)
        {
            log.Info($"{tag}|{msg}");
        }

        public void I(string tag, string msg, Exception tr)
        {
            log.Info($"{tag}|{msg}", tr);
        }

        public void I(string tag, string format, params object[] args)
        {
            log.InfoFormat($"{tag}|{format}", args);
        }

        public void W(string tag, string msg)
        {
            log.Warn($"{tag}|{msg}");
        }

        public void W(string tag, Exception tr)
        {
            log.Warn($"{tag}", tr);
        }

        public void W(string tag, string msg, Exception tr)
        {
            log.Warn($"{tag}|{msg}", tr);
        }

        public void W(string tag, string format, params object[] args)
        {
            log.WarnFormat($"{tag}|{format}", args);
        }

        public void E(string tag, string msg)
        {
            log.Error($"{tag}|{msg}");
        }

        public void E(string tag, string msg, Exception tr)
        {
            log.Error($"{tag}|{msg}", tr);
        }

        public void E(string tag, string format, params object[] args)
        {
            log.ErrorFormat($"{tag}|{format}", args);
        }
    }
}