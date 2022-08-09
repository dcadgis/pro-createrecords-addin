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

namespace DCAD.GIS
{
    #region Class Library Documentation

    /*
     * ***************************************************************************************************************************************************************
     * Project:         Database Class Library
     * Class:           Database Library
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
     * PURPOSE:     This class contains properties and methods for interfacing with database servers.
     *
     * CLASS
     * DESCRIPTION: Class members provide interactions with database servers.
     *
     *
     * CLASS
     * PROPERTIES:   Describe the properties (protected or otherwise) for this class.
     *              (example- layer internal variable from AnotherClass.cs)
     *
     * CLASS 
     * METHODS:     
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

    public class Database
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

        #region Properties

        #region Environment Property

        /// <summary>
        /// Gets or sets the current database server environment. 
        /// </summary>
        public int Environment { get; set; }

        #endregion

        #region Publishing Database Servers

        /// <summary>
        /// Development publishing database server name
        /// private variable.
        /// </summary>
        private string _devPubDBServer = "GSDVSQL01";

        /// <summary>
        /// Development publishing database server name
        /// property.
        /// </summary>
        public string DevPubDBServer
        {
            get { return _devPubDBServer; }
        }


        /// <summary>
        /// Staging publishing database server name
        /// private variable.
        /// </summary>
        private string _stgPubDBServer = "DCADSQLVM01";

        /// <summary>
        /// Staging publishing database server name
        /// property.
        /// </summary>
        public string StgPubDBServer
        {
            get { return _stgPubDBServer; }
        }

        /// <summary>
        /// Production publishing database server name
        /// private variable.
        /// </summary>
        private string _prdPubDBServer = "DCADGIS";

        /// <summary>
        /// Production publishing database server name
        /// property.
        /// </summary>
        public string prdPubDBServer
        {
            get { return _prdPubDBServer; }
        }

        #endregion

        #region Transactional Database Servers

        /// <summary>
        /// Development transactional database server name
        /// private variable.
        /// </summary>
        private string _devTransDBServer = "DCADSQLVM02";

        /// <summary>
        /// Development transactional database server name
        /// property.
        /// </summary>
        public string DevTransDBServer
        {
            get { return _devTransDBServer; }
        }


        /// <summary>
        /// Staging transactional database server name
        /// private variable.
        /// </summary>
        private string _stgTransDBServer = "DCADZOEY";

        /// <summary>
        /// Staging transactional database server name
        /// property.
        /// </summary>
        public string StgTransDBServer
        {
            get { return _stgTransDBServer; }
        }

        /// <summary>
        /// Production transactional database server name
        /// private variable.
        /// </summary>
        private string _prdTransDBServer = "DCADGDT";

        /// <summary>
        /// Production transactional database server name
        /// property.
        /// </summary>
        public string prdTransDBServer
        {
            get { return _prdTransDBServer; }
        }

        #endregion


        #endregion

        #region Public Constants


        // Development

        public const string DevelopmentPubServer = "GSDVSQL01";

        public const string DevelopmentTransServer = "DCADSQLVM02";

        // Staging

        public const string StagingPubServer = "DCADSQLVM01";

        public const string StatingTransServer = "DCADZOEY";

        // Production

        public const string ProductionPubServer = "DCADGIS";

        public const string ProductionTransServer = "DCADGDT";

        #endregion


    }
}