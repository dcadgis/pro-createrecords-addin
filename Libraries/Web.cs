using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace DCAD.GIS
{
    #region Class Library Documentation

    /*
     * ***************************************************************************************************************************************************************
     * Project:         Web Class Library
     * Class:           Web Library
     * Version:         1.0
     * Author:          John W. Fell
     * Date Created:    08/09/22
     * Date Launched:   Date Project launched for general use (mm/dd/yyyy)
     * Dept:            GIS Division
     * Location:        https://github.com/dcadgis/pro-createrecords-addin/
     * Revisions:       mm/dd/yyyy -programmer-: Summary of modifications to code or Docked Window
     * ***************************************************************************************************************************************************************
     * 
     * CLASS
     * PURPOSE:     This class contains properties and methods for interfacing with the ArcGIS Server,
     *              Portal, and other internet resources. Some methods are still in development.
     *
     * CLASS
     * DESCRIPTION: Class members provide interactions with arcgis and portal servers.
     *
     *
     * CLASS
     * PROPERTIES:  PortalUri   - string variable representing the uri of the active portal
     *              Environment - integer variable representing a specific geodatabase environment
     *                            Value | Description
     *                                0 | Development
     *                                1 | Staging
     *                                2 | Production
     *                                3 | Unknown
     *
     * CLASS 
     * METHODS:     GetActivePortalUri() - obtains the active portal uri for the current ArcGIS Pro session.
     *              SetEnvironment()     - Applies the appropriate environment flag based on the portal uri's structure.
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

    public class Web
    {
        #region Enumerations

        #region Environments

        /// <summary>
        /// An enumeration that specifies the arcgis server or portal environment in use.
        /// </summary>
        public enum Environments
        {

            Development,
            Staging,
            Production,
            Unknown

        }

        #endregion

        #endregion

        #region Exceptions




        #endregion

        #region Public Constants




        public const string DevelopmentIdentifier = "DV";

        public const string StagingIdentifier = "SG";

        public const string ProductionIdentifier = "PD";

        public const string SmtpServer   = "<enter fully qualified smtp server name here>";    // Host name of exchange server.

        public const char BACK_SLASH    = '\\';

        #endregion

        #region Properties


        #region Environment Property

        /// <summary>
        /// Gets or sets the current arcgis server or portal environment. 
        /// </summary>
        public int Environment { get; set; }

        #endregion

        #region PortalUri Property

        /// <summary>
        /// Contains the uri for the currently active portal.
        /// </summary>
        private string _portalURI = String.Empty;
        public string PortalURI
        {
            get { return _portalURI; }
            set { _portalURI = value; }
        }


        #endregion

        #endregion

        #region Methods

        #region Portal Methods

        #region Get Active Portal Uri

        /// <summary>
        /// Asynchronous method that gets the active portal's uniform resource identifier (Uri).
        /// **IMPORTANT**: This method is only valid when used with the ArcGIS PRO SDK.
        /// </summary>
        /// <returns>uri - String representing the active portal's uri.</returns>
        public async void GetActivePortalUri()
        {
            try
            {

                // Setup the async portal object
                ArcGISPortal active_portal = ArcGISPortalManager.Current.GetActivePortal();

                await QueuedTask.Run(() =>
                {
                    if (active_portal == null) // no currently active portal
                    {
                        PortalURI = "NO VALID PORTAL URI";
                        return;
                    }

                    else  // Successful portal connection; return portal uri
                    {
                        PortalURI = active_portal.PortalUri.ToString();
                    }
                });


            }
            catch (Exception ex)
            {

                OS.LogException(ex, "DCAD.GIS.Web Class Library");
            }



        }

        #endregion



        #endregion

        #region Environment Methods

        #region Set Environment
        /// <summary>
        /// Captures the current arcgis server or portal uri and interrogates this value to determine
        /// the environment.
        /// </summary>
        /// <returns>returnEnv - Integer value representing the current arcgis server or portal environment. </returns>
        public void SetEnvironment()
        {
            // TODO: Add a logical statement that determines what to do if arcgis.com is the active portal

            // Get the Active Portal
            GetActivePortalUri();

            if (PortalURI.ToUpper().Contains(DevelopmentIdentifier))
            {
                Environment = (int)Environments.Development;
            }

            else if (PortalURI.ToUpper().Contains(StagingIdentifier))
            {
                Environment = (int)Environments.Staging;
            }

            else if (PortalURI.ToUpper().Contains(ProductionIdentifier))
            {
                Environment = (int)Environments.Production;
            }

            else
            {
                Environment = (int)Environments.Unknown;
            }


        }

        #endregion

        #endregion

        #endregion




    }
}