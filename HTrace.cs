using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leestar54.WeChat.WebAPI
{
    public static class HTrace
    {
        /// <summary>
        /// Defines the level of the tracing message.  Depending on the framework version this is translated
        /// to an equivalent logging level in System.Diagnostices (if available)
        /// </summary>
        public enum HTraceLevel
        {
            /// <summary>
            /// Used for logging Debug or Verbose level messages
            /// </summary>
            Verbose,
            /// <summary>
            /// Used for logging Informational messages
            /// </summary>
            Info,
            /// <summary>
            /// Used for logging non-fatal or ignorable error messages
            /// </summary>
            Warn,
            /// <summary>
            /// Used for logging Error messages that may need investigation 
            /// </summary>
            Error
        }


        private static volatile TraceSource m_traceSource = new TraceSource("HTrace")
        {
            Switch = new SourceSwitch("sourceSwitch", "Verbose") { Level = SourceLevels.All }
        };


        static bool m_flushOnWrite = true;

        /// <summary>
        /// Should the trace listeners be flushed immediately after writing to them?
        /// </summary>
        public static bool FlushOnWrite
        {
            get { return m_flushOnWrite; }
            set { m_flushOnWrite = value; }
        }


        static bool m_prefix = false;

        /// <summary>
        /// Should the log entries be written with a prefix of "HTrace"?
        /// Useful if you have a single TraceListener shared across multiple libraries.
        /// </summary>
        public static bool LogPrefix
        {
            get { return m_prefix; }
            set { m_prefix = value; }
        }


        /// <summary>
        /// Add a TraceListner to the collection. You can use one of the predefined
        /// TraceListeners in the System.Diagnostics namespace, such as ConsoleTraceListener
        /// for logging to the console, or you can write your own deriving from 
        /// System.Diagnostics.TraceListener.
        /// </summary>
        /// <param name="listener">The TraceListener to add to the collection</param>
        public static void AddListener(TraceListener listener)
        {
            lock (m_traceSource)
            {
                m_traceSource.Listeners.Add(listener);
            }
        }

        /// <summary>
        /// Remove the specified TraceListener from the collection
        /// </summary>
        /// <param name="listener">The TraceListener to remove from the collection.</param>
        public static void RemoveListener(TraceListener listener)
        {
            lock (m_traceSource)
            {
                m_traceSource.Listeners.Remove(listener);
            }
        }


        static bool m_functions = true;

        /// <summary>
        /// Should the function calls be logged in Verbose mode?
        /// </summary>
        public static bool LogFunctions
        {
            get { return m_functions; }
            set { m_functions = value; }
        }


        static bool m_tracing = true;

        /// <summary>
        /// Should we trace at all?
        /// </summary>
        public static bool EnableTracing
        {
            get { return m_tracing; }
            set { m_tracing = value; }
        }


        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="message">The message to write</param>
        //[Obsolete("Use overloads with HTraceLevel")]
        public static void Write(string message)
        {
            Write(HTraceLevel.Verbose, message);
        }

        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="message">The message to write</param>
        //[Obsolete("Use overloads with HTraceLevel")]
        public static void WriteLine(object message)
        {
            Write(HTraceLevel.Verbose, message.ToString());
        }

        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="eventType">The type of tracing event</param>
        /// <param name="message">The message to write</param>
        public static void WriteLine(HTraceLevel eventType, object message)
        {
            Write(eventType, message.ToString());
        }

        /// <summary>
        /// Write to the TraceListeners, adding an automatic prefix to the message based on the `eventType`
        /// </summary>
        /// <param name="eventType">The type of tracing event</param>
        /// <param name="message">The message to write</param>
        public static void WriteStatus(HTraceLevel eventType, object message)
        {
            Write(eventType, TraceLevelPrefix(eventType) + message.ToString());
        }

        /// <summary>
        /// Write to the TraceListeners, for the purpose of logging a API function call
        /// </summary>
        /// <param name="function">The name of the API function</param>
        /// <param name="args">The args passed to the function</param>
        public static void WriteFunc(string function, object[] args = null)
        {
            if (m_functions)
            {
                Write(HTraceLevel.Verbose, "");
                Write(HTraceLevel.Verbose, "# " + function + "(" + args.ItemsToString().Join(", ") + ")");
            }
        }

        /// <summary>
		/// Join the given strings by a delimiter.
		/// </summary>
		public static string Join(this List<string> values, string delimiter)
        {
#if NET20 || NET35
			return string.Join(delimiter, values.ToArray());
#else
            return string.Join(delimiter, values);
#endif
        }

        private static List<string> ItemsToString(this object[] args)
        {
            List<string> results = new List<string>();
            if (args == null)
            {
                return results;
            }
            foreach (object v in args)
            {
                string txt;
                if (v == null)
                {
                    txt = "null";
                }
                else if (v is string)
                {
                    txt = ("\"" + v as string + "\"");
                }
                else
                {
                    txt = v.ToString();
                }
                results.Add(txt);
            }
            return results;
        }

        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="eventType">The type of tracing event</param>
        /// <param name="message">A formattable string to write</param>
        public static void Write(HTraceLevel eventType, string message)
        {
            if (!EnableTracing)
                return;

            if (m_prefix)
            {
                // if prefix is wanted then use TraceEvent()
                m_traceSource.TraceEvent(TraceLevelTranslation(eventType), 0, message);
            }
            else
            {
                // if prefix is NOT wanted then write manually
                EmitEvent(m_traceSource, TraceLevelTranslation(eventType), message);
            }
            if (m_flushOnWrite)
            {
                m_traceSource.Flush();
            }
        }

        private static string TraceLevelPrefix(HTraceLevel level)
        {
            switch (level)
            {
                case HTraceLevel.Verbose:
                    return "Status:   ";
                case HTraceLevel.Info:
                    return "Info:   ";
                case HTraceLevel.Warn:
                    return "Warning:  ";
                case HTraceLevel.Error:
                    return "Error:    ";
            }
            return "Status:   ";
        }

        private static TraceEventType TraceLevelTranslation(HTraceLevel level)
        {
            switch (level)
            {
                case HTraceLevel.Verbose:
                    return TraceEventType.Verbose;
                case HTraceLevel.Info:
                    return TraceEventType.Information;
                case HTraceLevel.Warn:
                    return TraceEventType.Warning;
                case HTraceLevel.Error:
                    return TraceEventType.Error;
                default:
                    return TraceEventType.Verbose;
            }
        }

        static object traceSync = new object();
        private static void EmitEvent(TraceSource traceSource, TraceEventType eventType, string message)
        {
            try
            {
                lock (traceSync)
                {
                    if (traceSource.Switch.ShouldTrace(eventType))
                    {
                        foreach (TraceListener listener in traceSource.Listeners)
                        {
                            try
                            {
                                listener.WriteLine(message);
                                listener.Flush();
                            }
                            catch { }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void Test()
        {
            Write("Hello");
        }
    }
}
