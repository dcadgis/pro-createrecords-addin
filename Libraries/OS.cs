using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Threading;

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
     *  
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

        #region Public Constants

        /*********************************************************************** 
        * EVENT LOG SOURCE NAME                                                *
        * **********************************************************************
        * Enter the event log source name here.                                *
        * This will be used for the application event log's source name.       *
        * Important: This will need to be changed for each application         *
        * where it is used.                                                    *
        *                                                                      *
        ***********************************************************************/
        public const string EventLogSourceName = "Create Records Add-In";



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

        #region Event Logging Procedures and Functions

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
                WriteLogEntry("Get Current User", oex.Message, EventLogEntryType.Error);
                return null;
            }


        }
        #endregion

        #region Write Message to Event Log

        /// <summary>
        /// Writes an entry to the event log.
        /// More info available <see cref="https://bit.ly/3N9poze"/>here</see>.
        /// </summary>
        /// <param name="entry">The string to write to the event log.</param>
        /// <param name="entryType">One of the EventLogEntryType values.</param>
        public static void WriteLogEntry(String appSourceName, String entry, EventLogEntryType entryType)
        {


            try

            {

                // Check if event log exists

                if (!EventLog.SourceExists(appSourceName))

                {

                    // Create the event log source

                    EventLog.CreateEventSource(appSourceName, EventLogName);

                    Thread.Sleep(2000);


                }

                if (EventLog.SourceExists(appSourceName))

                {

                    EventLog eventLog = new EventLog();                     // Reference event log.

                    eventLog.Source = appSourceName;                        // Define app source.

                    eventLog.WriteEntry(entry, entryType);                  // Write log entry. 

                }






            }

            catch (Exception ex)

            {

                Console.WriteLine(ex.Message);

            }

            #endregion


        }


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
    }
}