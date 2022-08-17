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
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using System.Threading.Tasks;

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

    public class Database : Messaging
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

        #region Database Name

        /// <summary>
        /// The name of the database.
        /// </summary>
        public string DatabaseName { get; set; }
        #endregion

        #region Authentication Modes

        /// <summary>
        /// Windows authentication mode
        /// for geodatabase connections
        /// </summary>
        public AuthenticationMode OSAAuthMode
        {
            get { return AuthenticationMode.OSA; }
        }

        /// <summary>
        /// Database authentication mode
        /// for geodatabase connections
        /// </summary>
        public AuthenticationMode DBAuthMode
        {
            get { return AuthenticationMode.DBMS; }
        }



        #endregion

        #region Traditional Version Name

        /// <summary>
        /// The traditional version name
        /// for traditional versioning
        /// scenarios.
        /// </summary>
        private string _traditionalVersionName;

        public string TraditionalVersionName
        {
            get { return _traditionalVersionName; }
            set { _traditionalVersionName = value; }
        }

        #endregion

        #region Branch Version Name

        /// <summary>
        /// Branch version name for
        /// branch versioning scenarios.
        /// </summary>
        private string _branchVersionName;

        public string BranchVersionName
        {
            get { return _branchVersionName; }
            set { _branchVersionName = value; }
        }

        #endregion

        #region Target View Exists

        /// <summary>
        /// A property that reports
        /// back to the calling
        /// thread if the target
        /// view exists.
        /// </summary>
        private bool _viewExists;

        public bool ViewExists
        {
            get { return _viewExists; }
            set { _viewExists = value; }
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

        #region Methods

        #region Geodatabase Methods

        #region Checking for the existence of a Table

        /// <summary>
        /// Returns a boolean value that validates
        /// the existence of a geodatabase table.
        /// IMPORTANT: Must be called within QueuedTask.Run())
        /// </summary>
        /// <param name="geodatabase"></param>
        /// <param name="tableName"></param>
        /// <returns>bool</returns>
        public async Task TableExists(Geodatabase geodatabase, string tableName)
        {

            try
            {
                await QueuedTask.Run(() =>
                {

                    using (TableDefinition tableDefinition = geodatabase.GetDefinition<TableDefinition>(tableName))
                    {
                        tableDefinition.Dispose();
                        ViewExists = true;
                        
                    }
                });

            }
            catch (Exception ex)
            {
                // GetDefinition throws an exception if the definition doesn't exist
                OS.LogException(ex, OS.Source);
                ViewExists = false;
            }

        }

        #endregion


        #region Checking for the existence of a Feature Class
        // Must be called within QueuedTask.Run()
        /// <summary>
        /// Returns a boolean value that validates
        /// the existence of a geodatabase feature class.
        /// cref: ARCGIS.CORE.DATA.GEODATABASE.GETDEFINITION
        /// cref: ArcGIS.Core.CoreObjectsBase.Dispose
        /// </summary>
        /// <param name="geodatabase"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        public bool FeatureClassExists(Geodatabase geodatabase, string featureClassName)
        {
            try
            {
                FeatureClassDefinition featureClassDefinition = geodatabase.GetDefinition<FeatureClassDefinition>(featureClassName);
                featureClassDefinition.Dispose();
                return true;
            }
            catch
            {
                // GetDefinition throws an exception if the definition doesn't exist
                return false;
            }
        }
        #endregion



        #endregion

        #region Database Methods

        #region Return Database Properties

        /// <summary>
        /// Returns a DatabaseConnectionProperties object
        /// based on the server instance supplied as a 
        /// parameter and properties
        /// assigned in the instance of the database
        /// object.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>DatabaseConnectionProperties</returns>
        public DatabaseConnectionProperties GetDatabaseConnProperties(string instance)
        {
            try
            {
                // Create SQl Server Database Connections object
                DatabaseConnectionProperties databaseConnectionProperties =
                    new DatabaseConnectionProperties(EnterpriseDatabaseType.SQLServer);

                // Determine if database name and version type supplied
                if ((DatabaseName == null && TraditionalVersionName == null) ||
                    (DatabaseName == null && BranchVersionName == null))
                {
                    OS.LogError(this.ErrorMessages[3003], OS.Source);
                }

                else
                {

                    // Supply the properties of the object
                    databaseConnectionProperties.AuthenticationMode = OSAAuthMode;
                    databaseConnectionProperties.Database = DatabaseName;
                    databaseConnectionProperties.Instance = instance;
                    if (TraditionalVersionName == null)
                    {
                        databaseConnectionProperties.Branch = BranchVersionName;
                    }

                    else
                    {
                        databaseConnectionProperties.Version = TraditionalVersionName;

                    }

                }

                return databaseConnectionProperties;
            }

            catch (Exception ex)
            {

                OS.LogException(ex, OS.Source);
                OS.LogError(this.ErrorMessages[3004], OS.Source);
                DatabaseConnectionProperties returnDatabaseConnProperties = 
                    new DatabaseConnectionProperties(EnterpriseDatabaseType.SQLServer);
                return returnDatabaseConnProperties;
            }
   


            }

        }

        #endregion

        #endregion

        #endregion


    }
