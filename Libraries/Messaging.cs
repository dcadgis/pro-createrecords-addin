using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCAD.GIS
{
    #region Class Library Documentation

    /*
     * ***************************************************************************************************************************************************************
     * Project:         Class Libraries
     * Class:           Messaging Base Class
     * Version:         1.0
     * Author:          John W. Fell
     * Date Created:    08/16/22
     * Date Launched:   Date Project launched for general use (mm/dd/yyyy)
     * Dept:            GIS Division
     * Location:        https://github.com/dcadgis/pro-createrecords-addin/
     * Revisions:       mm/dd/yyyy -programmer-: Summary of modifications to code or Docked Window
     * ***************************************************************************************************************************************************************
     * 
     * CLASS
     * PURPOSE:     This class contains properties and methods for enabling messaging in derived classes.
     *
     * CLASS
     * DESCRIPTION: Class members provide dictionaries that act as message repositories for reporting information
     *              to the user and the event log.
     *
     *
     * CLASS
     * PROPERTIES:   Describe the properties (protected or otherwise) for this class.
     *              (InfoMessages - dictionary containing information related messages.)
     *              (WarnMessages - dictionary containing warning related messages.)
     *              (ErrMessages - dictionary containing error related messages.)
     *
     * CLASS 
     * METHODS:     IncrementMessageCode - Returns and integer value that increments the code for 
     *                                     the given message dictionary.
     * 
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
     * DOCUMENTATION: (1) 
     *                (2) 
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

    public class Messaging
    {
        #region Fields and Variables

        #region Messaging Fields

        // Private variable infoDictionary
        Dictionary<Int32, string> _infoMessages = new Dictionary<Int32, string>();

        // Private variable warnMessages
        Dictionary<Int32, string> _warnMessages = new Dictionary<Int32, string>();

        // Private variable errMessages
        Dictionary<Int32, string> _errMessages = new Dictionary<Int32, string>();


        #endregion


        #endregion

        #region Constants


        /// <summary>
        /// Portal Messages
        /// </summary>
        public const string InfoConnectedArcGISPro  = "You are now connected to ArcGIS Pro.";
        public const string ErrorNotConnectedPortal = "You are not connected to ArcGIS Portal.";
        public const string WarningDefaultPortal    = "The default portal www.arcgis.com is the active portal.";
        public const string NoActivePortal          = "There is no active portal.";

        /// <summary>
        /// Database Connection Messages
        /// </summary>
        /// 
        public const string TargetDBServerUnknown = "The target database server is UNKNOWN.";
        public const string ProblemDBEnvironment  = "There was a problem setting the target database environment.";
        public const string SpecifyDBVerName      = "Please specify a database name or version name.";
        public const string ProblemDBConnection   = "There was a problem making the database connection.";
 
        /// <summary>
        /// Message Dictionary Codes
        /// </summary>
        public const Int32  InfoMessageCodes         = 1000;
        public const Int32  WarningMessageCodes      = 2000;
        public const Int32  ErrorMessageCodes        = 3000;


        /// <summary>
        /// Data Messages
        /// </summary>
        /// 
        public const string CreatedRecordMessage = "Created Record: {0} - {1}.";
        public const string NoParcelFabric       = "There is no fabric in the map.";
        public const string ViewPermOrDoNotExist = "Either you do not have permission or the required views to not exist.";
        public const string CouldNotCheckViews   = "There was a problem checking for the existence of required views.";


        /// <summary>
        /// Operating System Messages
        /// </summary>
        public const string NotAdministrator = "The currently authenticated user is not an administrator.";

        #endregion

        #region Constructor


        public Messaging()
        {

            /* Information Messages *****************************************************/

            // 1000: Connected to ArcGIS Pro
            _infoMessages.Add(IncrementMessageCode(InfoMessageCodes,_infoMessages.Count), 
                              InfoConnectedArcGISPro);
            
            // 1001: Created record
            _infoMessages.Add(IncrementMessageCode(InfoMessageCodes, _infoMessages.Count),
                              CreatedRecordMessage);

            /* Warning Messages  *********************************************************/
         

            // 2000: The default portal www.arcgis.com is the active portal
            _warnMessages.Add(IncrementMessageCode(WarningMessageCodes, _warnMessages.Count), 
                              WarningDefaultPortal);

            // 2001: Not an administrator
            _warnMessages.Add(IncrementMessageCode(WarningMessageCodes, _warnMessages.Count),
                              NotAdministrator);

            /* Error Messages  ***********************************************************/

            // 3000: You are not connected to ArcGIS Portal
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                             ErrorNotConnectedPortal);

            // 3001: Target database server UNKNOWN
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                             TargetDBServerUnknown);

            // 3002: Problem setting database environment
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                             ProblemDBEnvironment);

            // 3003: Please specify a database name or version name
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                             SpecifyDBVerName);

            // 3004: There was a problem making the database connection
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                             ProblemDBConnection);

            // 3005: There is no parcel fabric in the map
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                             NoParcelFabric);

            // 3006: There is no active portal
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                                         NoActivePortal);

            // 3007: The required views do not exist or you do not have permission to view them
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                                                     ViewPermOrDoNotExist);

            // 3008: There was a problem checking for existence of required views
            _errMessages.Add(IncrementMessageCode(ErrorMessageCodes, _errMessages.Count),
                                                     CouldNotCheckViews);




        }

        #endregion

        #region Properties

        #region Messaging Dictionaries

        /// This region contains different message dictionaries
        /// that will be used to report information to the user.
        /// The dictionaries include the following types:
        ///     - Information Messages
        ///     - Warning Messages
        ///     - Error Messages
        ///     

        #region Info Messages Property
        /// <summary>
        /// This dictionary property contains the information
        /// messages for this base class
        /// </summary>
        public Dictionary<Int32, string> InfoMessages
        {
            get { return _infoMessages; }

        }
        #endregion

        #region Warning Messages Property

        /// <summary>
        /// This dictionary property contains the warning
        /// messages for this base class
        /// </summary>
        public Dictionary<Int32, string> WarningMessages
        {
            get { return _warnMessages; }

        }

        #endregion

        #region Error Messages Property

        /// <summary>
        /// This dictionary property contains the error
        /// messages for this base class
        /// </summary>
        public Dictionary<Int32, string> ErrorMessages
        {
            get { return _errMessages; }

        }

        #endregion


        #endregion





        #endregion


        #region Methods

        #region Increment Message Code

        /// <summary>
        /// Increments the integer value passed
        /// into the method. The intention is to
        /// increment the integer value representing
        /// the current length of the message dictionary.
        /// If the length of the dictionary is zero, then
        /// the starting message code is incremented by 1
        /// and then returned.
        /// </summary>
        /// <param name="dictionaryLength"></param>
        /// <param name="messageCodeInitializer"></param>
        /// <returns>int - The index of the dictionary and the message code.</returns>
        private int IncrementMessageCode(int messageCodeInitializer, int dictionaryLength)
        {
            if (dictionaryLength == 0)
            {
                return messageCodeInitializer++;
            }

            else
            {
                return (messageCodeInitializer + dictionaryLength++);
            }
            

        }

        #endregion

        #endregion



    }
}
