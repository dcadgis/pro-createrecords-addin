using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace DCAD.GIS
{
    #region Class Library Documentation

    /*
     * ***************************************************************************************************************************************************************
     * Project:         OS Class Library
     * Class:           OS Library
     * Version:         1.0
     * Author:          John W. Fell
     * Date Created:    01/03/22
     * Date Launched:   Date Project launched for general use (mm/dd/yyyy)
     * Dept:            GIS Division
     * Location:        https://github.com/dcadgis/pro-createrecords-addin/
     * Revisions:       mm/dd/yyyy -programmer-: Summary of modifications to code or Docked Window
     * ***************************************************************************************************************************************************************
     * 
     * CLASS
     * PURPOSE:     This class contains properties and methods for interfacing with the local
     *              operating system. Some methods are still in development.
     *
     * CLASS
     * DESCRIPTION: Class members provide logging and other interactions with the local windows
     *              operating system. 
     *
     * CLASS
     * PROPERTIES:   Describe the properties (protected or otherwise) for this class.
     *              (example- layer internal variable from AnotherClass.cs)
     *
     * CLASS 
     * METHODS:     GetCurrentUser()     - obtains the users login id. Still in development. 
     *              WriteEventLogEntry() - writes to the Application log with the application's event source name.
     *              SendEventLogEmail()  - emails the application team and user with the event information. Still in development.
     *
     * CLASS
     * EVENTS:      
     *              
     *              
     *
     * CLASS
     * USER
     * INTERFACE:   
     *              
     *              
     *
     * SUPPORTING
     * CLASSES
     * AND
     * INTERFACES:  
     *              
     *              
     *
     * SOURCE
     * DATA
     * CONDITIONS:  
     *              
     *
     *
     * SUPPORTING
     * ONLINE
     * DOCUMENTATION: (1) EventLog.WriteEntry - https://bit.ly/3N9poze. Writes an entry in the event log.
     *                (2) Log - Method used from Bob Horn. See   
     *
     *
     * APPLICABLE
     * STANDARDS: (1) C# Coding Standards - http://dcadwiki.dcad.org/dcadwiki/CSharp-CodingStandards. DCAD standards for C# development.
     *
     *
     * ***************************************************************************************************************************************************************
     * ***************************************************************************************************************************************************************
     */

    #endregion

    public class OS
    {

        #region Private Fields

        private static bool _isAdmin = false;

        #endregion

        #region Constructor

        public OS()
        {
            _isAdmin = IsCurrentProcessAdmin();
        }

        #endregion

        #region Properties

        #region Source Property

        /// <summary>
        /// Gets or sets the source/caller. When logging, this logger class will attempt to get the
        /// name of the executing/entry assembly and use that as the source when writing to a log.
        /// In some cases, this class can't get the name of the executing assembly. This only seems
        /// to happen though when the caller is in a separate domain created by its caller. So,
        /// unless you're in that situation, there is no reason to set this. However, if there is
        /// any reason that the source isn't being correctly logged, just set it here when your
        /// process starts.
        /// </summary>
        public static string Source { get; set; }

        #endregion

        #region IsAdministrator Property

        /// <summary>
        /// Boolean property that
        /// describes the permission
        /// level of the authenticated user.
        /// If the authenticated user is an
        /// administrator the value is true,
        /// otherwise it is false.
        /// </summary>


        public static bool IsAdministrator
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }

        #endregion


        #endregion

        #region Public Constants




        /// <summary>
        /// Eventy Log Entry Length Limit
        /// This constants provides a limit the length of the event log.
        /// Note: The actual limit is higher than this, but different Microsoft operating systems actually have
        /// different limits. So just use 30,000 to be safe.
        /// </summary>
        private const int MaxEventLogEntryLength = 30000;



        public const string EventLogName = "Application"; // Name of event viewer Application log.

        public const string SmtpServer   = "<enter fully qualified smtp server name here>";    // Host name of exchange server.

        public const char BACK_SLASH    = '\\';

        #endregion

        #region Custom Exceptions

        #region Exception: Operating System Not Supported


        /// <summary>
        /// Operating system not supported exception. 
        /// </summary>
        public class OSNotSupported : Exception
        {
            public OSNotSupported()
            {
            }

            public OSNotSupported(string message)
                : base(message)
            {
            }

            public OSNotSupported(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        #endregion



        #endregion

        #region Methods

        #region Permission Methods


        #region Is Current User an Administrator
        /// <summary>
        /// Returns a boolean value
        /// that determines if the
        /// authenticated user is
        /// an administrator.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsCurrentProcessAdmin()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        #endregion

        #endregion

        #region Event Logging Methods

        #region Get Current Authenticated User
        /// <summary>
        /// Identifies the authenticated user.
        /// </summary>
        /// <returns>The string containing the user name without the domain.</returns>

        public static string GetCurrentUser()
        {
            // Get the logged in user
            string authenticatedUser = string.Empty;

            string returnedUser = string.Empty;

            // First check if it is a windows operating system
            bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            try
            {

                if (IsWindows)
                {


                    authenticatedUser = WindowsIdentity.GetCurrent().Name;

                    returnedUser = authenticatedUser.Split(BACK_SLASH)[1];

                }

                else
                {
                    throw new OSNotSupported(@"[This operating system is not supported.]");
                }

                return returnedUser;

            }



            catch (OSNotSupported oex)
            {
                EventLog.WriteEntry("Get Current User", oex.Message, EventLogEntryType.Error);
                return null;
            }


        }
        #endregion

        #region Log Information

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogInformation(string message, string source = "")
        {
            Log(message, EventLogEntryType.Information, source);
        }

        #endregion

        #region Log Warning

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogWarning(string message, string source = "")
        {
            Log(message, EventLogEntryType.Warning, source);
        }

        #endregion

        #region Log Error

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogError(string message, string source = "")
        {
            Log(message, EventLogEntryType.Error, source);
        }

        #endregion

        #region Log Exception

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogException(Exception ex, string source = "")
        {
            if (ex == null) { throw new ArgumentNullException("ex"); }

            if (Environment.UserInteractive)
            {
                Console.WriteLine(ex.ToString());
            }

            Log(ex.ToString(), EventLogEntryType.Error, source);
        }

        #endregion

        #region Log Object Dump

        ///// <summary>
        ///// Recursively gets the properties and values of an object and dumps that to the log.
        ///// </summary>
        ///// <param name="theObject">The object to log</param>
        //[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Xanico.Core.Logger.Log(System.String,System.Diagnostics.EventLogEntryType,System.String)")]
        //[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object")]
        //public static void LogObjectDump(object theObject, string objectName, string source = "")
        //{
        //    const int objectDepth = 5;
        //    string objectDump = ObjectDumper.GetObjectDump(theObject, objectDepth);

        //    string prefix = string.Format(CultureInfo.CurrentCulture,
        //                                  "{0} object dump:{1}",
        //                                  objectName,
        //                                  Environment.NewLine);

        //    Log(prefix + objectDump, EventLogEntryType.Warning, source);
        //}

        #endregion

        #region Log Static Method

        /// <summary>
        /// Log static method.
        /// This method writes the message to the
        /// event log.
        /// More info available <see cref="https://bit.ly/3N9poze"/>here</see>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="entryType"></param>
        /// <param name="source"></param>
        private static void Log(string message, EventLogEntryType entryType, string source)
        {
            // Note: I got an error that the security log was inaccessible. To get around it, I ran the app as administrator
            //       just once, then I could run it from within VS.

            if (string.IsNullOrWhiteSpace(source))
            {
                source = GetSource();
            }

            string possiblyTruncatedMessage = EnsureLogMessageLimit(message);
            EventLog.WriteEntry(source, possiblyTruncatedMessage, entryType);

            // If we're running a console app, also write the message to the console window.
            if (Environment.UserInteractive)
            {
                Console.WriteLine(message);
            }
        }

        #endregion

        #region Get Source

        /// <summary>
        /// Gets the source for the event log.
        /// </summary>
        /// <returns></returns>
        private static string GetSource()
        {
            // If the caller has explicitly set a source value, just use it.
            if (!string.IsNullOrWhiteSpace(Source)) { return Source; }

            try
            {
                var assembly = Assembly.GetEntryAssembly();

                // GetEntryAssembly() can return null when called in the context of a unit test project.
                // That can also happen when called from an app hosted in IIS, or even a windows service.

                if (assembly == null)
                {
                    assembly = Assembly.GetExecutingAssembly();
                }


                if (assembly == null)
                {
                    // From http://stackoverflow.com/a/14165787/279516:
                    assembly = new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
                }

                if (assembly == null) { return "Unknown"; }

                return assembly.GetName().Name;
            }
            catch
            {
                return "Unknown";
            }
        }

        #endregion

        #region Ensure Log Message Limit


        /// <summary>
        /// Ensure Log Message Limit
        /// Ensures that the log message entry text length does not exceed 
        /// the event log viewer maximum length of 32766 characters.
        /// If a message exceeds the limit, it will be truncated.
        /// </summary>
        /// <param name="logMessage">The string representing the log message.</param>
        /// <returns>The log message and a truncated representation if the message exceeds the limit.</returns>
        private static string EnsureLogMessageLimit(string logMessage)
        {
            if (logMessage.Length > MaxEventLogEntryLength)
            {
                string truncateWarningText = string.Format(CultureInfo.CurrentCulture, "... | Log Message Truncated [ Limit: {0} ]", MaxEventLogEntryLength);

                // Set the message to the max minus enough room to add the truncate warning.
                logMessage = logMessage.Substring(0, MaxEventLogEntryLength - truncateWarningText.Length);

                logMessage = string.Format(CultureInfo.CurrentCulture, "{0}{1}", logMessage, truncateWarningText);
            }

            return logMessage;
        }

        #endregion

        #region SendEventLogEmail (In Development)
        //public static void SendEventLogEmail(String procName, String exception)
        //{

        //    try
        //    {

        //        // Required for the application to assign the username to My.User.Name properly
        //        //this.User.InitializeWithWindowsUser()


        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}
        #endregion

        #endregion

        #endregion




    }
}