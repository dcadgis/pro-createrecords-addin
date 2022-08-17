#region "CLASS DOCUMENTATION"
/*
 * ***************************************************************************************************************************************************************
 * Project:         Create AFC Records
 * Class:           CreateRecordsPaneViewModel.cs
 * Version:         0.1.0
 * Author:          John W. Fell
 * Date Created:    06/21/2021
 * Date Launched:   TBD
 * Dept:            GIS Deparment
 * Location:        https://github.com/dcadgis/pro-createrecords-addin/
 * Revisions:       
 * ***************************************************************************************************************************************************************
 * 
 * CLASS
 * PURPOSE:     Business logic for MVVM pattern dockpane used to create parcel fabric
 *              records in ArcGIS Pro.
 *              
 *
 * CLASS
 * DESCRIPTION: This class generates the list of AFC logs to display in the 
 *              dock pane as well as other features such as onclick button
 *              events.
 * CLASS
 * PROPERTIES:   AFCLogs  - The ReadOnlyObservableCollection object that is
 *                          bound to the XAML ListBox object in the 
 *                          CreateRecordsPane.xaml file.
 *               _afclogs - The ObservableCollection object that is manipulated
 *                          in the SearchForAFCLogs() method. The database view
 *                          GEDT.ADM.AFC_LOG_VW is read using a QueryFilter and
 *                          the resulting RowCursor object populates AFC Log class
 *                          objects and adds them to this collection. This property
 *                          is manipulated in the business logic but is not the ultimate
 *                          data source. This property is bounds to the _afclogsRO property
 *                          which is returned by the AFCLogs property bound to the dock pane
 *                          control in the xaml.
 *                          

 *
 * CLASS 
 * METHODS:     AsyncSearchForAFCLogs() - Initiates a pull from the data source in order to populate the dockpane.
 *              PopulateAFCLogCollection() - Reads the database view and populates the collection with AFC log objects.
 *
 * CLASS
 * EVENTS:      Provide a list of the Events available in this class and a brief explanation of the actions they represent.
 *              (example- DatabaseConnected event is triggered when a database connection is attempted along with the result.)
 *              (example- RecordRetrieved event is triggered when a database record was returned from a retrieve operation.)
 *
 * CLASS
 * USER
 * INTERFACE:   If the class provides a user interface, describe what the user can do and should expect 
 *              from the interface, otherwise if no user interface provided, leave blank.
 *              (example- This class provides a button control that, when activated, initiates the query.  Results returned are provided in a message box.)
 *
 * SUPPORTING
 * CLASSES
 * AND
 * INTERFACES:  AFCLog.cs - Properties and methods for AFC log objects created in the AFC log collection.
 *              OS.cs     - Properties and methods for logging messages to the event log.
 *              (example- b). ErrorLog.cs ==> User Event Log controls.)
 *
 * SOURCE
 * DATA
 * CONDITIONS:  The user must assign an AFC log to themselves before it will be visible in the dockpane.
 *
 *
 * SUPPORTING
 * ONLINE
 * DOCUMENTATION:  
 *                 
 *
 *
 *
 * APPLICABLE
 * STANDARDS: If standards were considered as part of the development they should be listed here with a link if available.
 *            (example- (1) C# Coding Standards - https://bit.ly/r398u779. DCAD standards for C# development.
 *
 *
 * MODIFICATIONS:
 * 
 *                 06/16/22 -jwf- Altered the name of the database view acting as AFC log data source.
 *                 08/11/22 -jwf- Added logic to change the database server name connection based on the
 *                                active portal url (development, staging, production) environments.
 * ***************************************************************************************************************************************************************
 * ***************************************************************************************************************************************************************
 */
#endregion

using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Forms;
using DCAD.GIS;
using System.Threading;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Mapping;

namespace pro_createrecords_addin
{

    internal class CreateRecordsPaneViewModel : DockPane, INotifyPropertyChanged
    {

        #region Constants

        private const string _dockPaneID  = "pro_createrecords_addin_CreateRecordsPane";
        private const string _database = "GEDT";
        private const AuthenticationMode _authentication = AuthenticationMode.OSA;
        private const string _version = "sde.DEFAULT";
        private const string _stagingInstance = "DCADZOEY";
        private const string _afcView = "ADM.AFC_LOG_SDE_VW";
        private const string _actView = "ADM.ACTIVE_RECORDS_VW";
        private const string _dltView = "ADM.DELETED_RECORDS_VW";
        private const string _yes = "Y";
        private const string _blank = "";
        private const string _eventLogSourceName = "ArcGIS Pro: Create Records AddIn";

        #endregion

        #region Private Fields

        // Fields related to the observable collection
        private ObservableCollection<AFCLog> _afclogs = new ObservableCollection<AFCLog>();
        private ObservableCollection<AFCLog> _templogs = new ObservableCollection<AFCLog>();
        private ObservableCollection<AFCLog> _records = new ObservableCollection<AFCLog>();
        private ReadOnlyObservableCollection<AFCLog> _afclogsRO;
        private Object _lockObj = new object();
        
        // Fields related to instances of class libraries
        private Web _web = new Web();
        private string _instance;
        private Layers _lyrs = new Layers();
        private Messaging _msg = new Messaging();
        private DCAD.GIS.Database _db = new DCAD.GIS.Database();
        private bool _afcViewExists = false;
        private bool _actViewExists = false;
        private bool _dltViewExists = false;



        #endregion

        #region ICommand Implementations

        /**************************************************************************************************
          * Public ICommand Implementations for Custom WPF Buttons. This allows the application to call    *
          * existing methods in the ViewModel from the button using AsyncRelayCommand.                     *
          * (1) RefreshListCommand - Refreshes the afc log list.                                           *
          * (2) CreateRecordCommand - Creates a new record based on selected AFC Log information.          *
          * ************************************************************************************************
          * THE COMMAND BELOW IS NOT CURRENTLY USED. THIS WAS NOT RECOMMENDED BY ESRI.                     *
          * ************************************************************************************************
          * (3) CreateCleanupRecordCommand - Creates a new parcel fabric records of the cleanup type.      *
          *     This is a custom record with specific attributes applied automatically when the workflow   *
          *     involves  cleaning up GIS data only and no legal document is triggering a parcel change.   *
          *************************************************************************************************/
        public ICommand RefreshListCommand { get; set; }

        //public ICommand CreateCleanupRecordCommand { get; set; }



        #endregion



        public CreateRecordsPaneViewModel() 
        {

            // Assign the event source for this 
            // application

            OS.Source = _eventLogSourceName;

            // Assing the instance name
            ServerInstance = _stagingInstance;

            #region Uncomment When Server Instance, Database View, Admin User, and Parcel Fabric Checks are Working
            /*****************************************
            * Get the server instance by returning
            * the active portal in the arcgis pro
            * portal manager
            *****************************************/
            //GetServerInstanceByEnvAsync();


            /*******************************************************************
            * Check to ensure that the AFC Log View, Active Records View, and  *
            * Delted Records Views exist in the target database                *
            *******************************************************************/
            //RequiredViewsExistAsync(_afcView);
            //RequiredViewsExistAsync(_actView);
            //RequiredViewsExistAsync(_dltView);

            //if (AFCViewExists && ACTViewExists && DLTViewExists)
            //{



            /* Check to ensure that a parcel fabric is
             * included in the current map
             */

            //_lyrs.ParcelFabricInMap = CheckParcelFabricExists();


            /* Check to ensure that the authenticated
             * user is an administrator on the machine
             */

            //if (!OS.IsAdministrator)
            //       OS.LogWarning(Messages.WarningMessages[2002], OS.Source);
            #endregion


            /******************************************************************
            * ReadOnlyObservableCollection for AFC Logs binding:
            * This variable is assigned a new ReadOnlyObservableCollection
            * bound to the public ObservableCollection object _afclogs.
            * The _afclogs variable is a collection of AFCLog objects and 
            * is manipulated based on the contents of the ADM.AFC_LOG_VW
            * database view. To update the list of AFC logs in the
            * wrap panel properly, a lock object must be used to add
            * items to the _afclogs list. However, the AFC logs list only
            * updates as changes occur to the database view when bound to
            * a ReadOnlyObservableCollection, hence this approach is used.
            ******************************************************************/
            
            _afclogsRO = new ReadOnlyObservableCollection<AFCLog>(_afclogs);
            BindingOperations.EnableCollectionSynchronization(_afclogsRO, _lockObj);


            /*******************************************
            * Determine if there are assigned AFC logs
            * for the authenticated user
            *******************************************/ 

            SearchForAFCLogsAsync();
            
            
            //DisplayTestMessage();

            /*******************************************************************************
             * Hook RefreshList and CreateCleanupRecord commands                           *
             * The AsyncRelayCommand is part of the Microsoft.Toolkit.Mvvm.Input namespace *
             * and allows developers to pass class methods to ICommand implementations to  *
             * be called from custom button controls on the xaml UI.                       *
             * *****************************************************************************/

            RefreshListCommand = new AsyncRelayCommand(func => SearchForAFCLogsAsync());

            #region For Create Cleanup Record Command

            //CreateCleanupRecord = new AsyncRelayCommand(func => AsyncCreateCleanupRecord());

            #endregion


            #region Uncomment When Admin User Check Working
            //}
            //else
            //{
            //    OS.LogError(Messages.ErrorMessages[3007], OS.Source);
            //    return;
            //}
            #endregion



        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();

            
        }

        #region Properties

        /// <summary>
        /// The name of the geodatabase
        /// server that stores the view
        /// containing AFC log information
        /// acting as the data source for
        /// the Create Recrods pane.
        /// </summary>
        public String ServerInstance
        {
            get { return _instance; }
            set { _instance = value;  }
        }

        /// <summary>
        /// Property containing list of AFC logs
        /// that is bounds to the MVVM xaml dock pane.
        /// </summary>
        public ReadOnlyObservableCollection<AFCLog> AFCLogs
        {

            get { return _afclogsRO; }


        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Create a Record from an Existing AFC Log";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        /// <summary>
        /// Search string used to limit the returned symbols.
        /// </summary>
        private string _searchString = "";
        public string SearchString
        {
            get
            {
                return _searchString;
            }
            set
            {
                SetProperty(ref _searchString, value, () => SearchString);

                //Call SearchForAFCLogs
                SearchForAFCLogsAsync(_searchString);


            }
        }




        private AFCLog _selectedAFCLog;


        public AFCLog SelectedAFCLog
        {
            get { return _selectedAFCLog; }
            set { _selectedAFCLog = value; }

        }

        /// <summary>
        /// The messaging object for this
        /// class
        /// </summary>
        public Messaging Messages
        {
            get { return _msg; }
            set { _msg= value; }

        }


        /// <summary>
        /// Reports back to the UI
        /// that the AFC view exists
        /// or that the user has permission
        /// to select on the view. This
        /// property is set by the
        /// RequiredViewExistsAsync
        /// method.
        /// </summary>
        public bool AFCViewExists 
        {
            get { return _afcViewExists;  }
            set { _afcViewExists = value; }
        }

        /// <summary>
        /// Reports back to the UI
        /// that the Active Records view exists
        /// or that the user has permission
        /// to select on the view. This
        /// property is set by the
        /// RequiredViewExistsAsync
        /// method.
        /// </summary>
        public bool ACTViewExists
        {
            get { return _actViewExists; }
            set { _actViewExists = value; }

        }

        /// <summary>
        /// Reports back to the UI
        /// that the Deleted Records view exists
        /// or that the user has permission
        /// to select on the view. This
        /// property is set by the
        /// RequiredViewExistsAsync
        /// method.
        /// </summary>
        public bool DLTViewExists
        {
            get { return _dltViewExists; }
            set { _dltViewExists = value; }

        }


        #endregion

        #region Methods

        #region Help Button Override

        /// <summary>
        /// Override the default behavior when the dockpane's help icon is clicked
        /// or the F1 key is pressed.
        /// </summary>
        protected override void OnHelpRequested()
        {
            System.Diagnostics.Process.Start(@"http://dcadwiki.dcad.org/dcadwiki/ArcGISPro-CreateAFCRecords");
        }

        #endregion

        #region Clear AFC Logs Collections

        /// <summary>
        /// Remove all items
        /// from the AFCLogs 
        /// collection.
        /// </summary>
        private void ClearAFCLogsCollection()
        {
            _afclogs.Clear();
        }

        #endregion

        #region Search for AFC Logs

        /// <summary>
        /// Update the list of AFC Logs given the current search text.
        /// If the AFC Log has already had a record created during the 
        /// current session, then skip this AFC log and do not add it
        /// to the Create Records Pane collection. 
        /// </summary>
        public async Task SearchForAFCLogsAsync(string _searchString = _blank)
        {
            if (AFCLogs.Count > 0)
            {
                foreach (var afclog in AFCLogs)
                {
                    if (afclog.RECORD_CREATED)
                    {
                        _records.Add(afclog);
                    }
                }
                ClearAFCLogsCollection();

            }

            await QueuedTask.Run(() =>
            {
                // Get a list of AFC Logs
                PopulateAFCLogCollectionAsync(_searchString);

                // Search for AFC Logs
                // and apply search string
                // if provided
                IEnumerable<AFCLog> linqResults;


                if (_searchString != _blank)
                {
                    linqResults = _afclogs.Where(afc => afc.DOC_NUM.Contains(_searchString));
                    //_templogs = _afclogs.FirstOrDefault(afc => afc.DOC_NUM.Contains(_searchString));

                }
                else
                {
                    linqResults = _afclogs.Where(afc => afc.AFC_LOG_ID > 0);
                }

                    // Create a temporary observable collection
                    // for filtering
                    ObservableCollection<AFCLog>_tempafclogs;

                    // Filter the items in the existing observable collection
                    
                    _tempafclogs = new ObservableCollection<AFCLog>(linqResults);

                    // Compare temporary collection with the original.
                    // Remove any items from the original collection
                    // that do not appear in the temporary collection.
                    for (int i = _afclogs.Count - 1; i >= 0; i--)
                    {
                        var item = _afclogs[i];
                        if (!_tempafclogs.Contains(item))
                        {
                            lock (_lockObj)
                            {
                                _afclogs.Remove(item);
                            }

                        }
                    }

                    // Now add any items that are included in
                    // the temporary collection that are not in
                    // the original collection in the case of a
                    // backspace
                    foreach (var item in _tempafclogs)
                    {
                        if (!_afclogs.Contains(item))
                        {
                            lock (_lockObj)
                            {
                                _afclogs.Add(item);
                            }

                        }
                    }

                    /**********************************************
                     * Remove any items that are included in
                     * the records collection because these
                     * have already had a record created
                     * during this session.
                     * *******************************************/

                 // Remove temporary observable collection
                 _tempafclogs = null;
                


            });

            // Call NotifyPropertyChanged and pass in the AFCLogs property
            NotifyPropertyChanged(() => AFCLogs);


        }

        #endregion

        #region Populates AFCLog Collection

        public async Task PopulateAFCLogCollectionAsync(string _searchString)
        {
            // Define columns to be included in
            // query filter
            string _instNum = "INSTRUMENT_NUM";
            string _seqNum = "SEQ_NUM";
            string _afcLogID = "AFC_LOG_ID";
            int _afcCount = 0;
            string _whereClause = _blank;
            bool _acctNumBlank = false;
            int _docNumType = 1;         // Helps the SetForegroundColor method know the color type for DOC_NUM.
            int _acctNumTYpe = 2;        // Helps the SetForegroundColor method know the color type for ACCOUNT_NUM.

            // Define where clause based
            // on search string contents
            if (_searchString != _blank)
            {
                _whereClause = String.Format("{0} LIKE '%{1}%' OR {2} LIKE '%{1}%'", _instNum, _searchString, _seqNum);
            }

            // Multi-threaded synchronization
            //private Object _lockObj = new object();
            //BindingOperations.EnableCollectionSynchronization(_afclogsRO, _lockObj);

            try
            {
                await QueuedTask.Run(() => {

                    // Opening a Non-Versioned SQL Server instance.
                    DatabaseConnectionProperties connectionProperties = new DatabaseConnectionProperties(EnterpriseDatabaseType.SQLServer)
                    {
                        AuthenticationMode = _authentication,
                                                
                        Instance = ServerInstance,
                                                
                        Database = _database,
                                                
                        //Version = _version

                        Branch = _version
                    };


                    using (Geodatabase geodatabase = new Geodatabase(connectionProperties))
                    using (Table table = geodatabase.OpenDataset<Table>(_afcView))
                    {

                        QueryFilter queryFilter = new QueryFilter
                        {
                            WhereClause = _whereClause,
                            SubFields = "*",
                            PostfixClause = String.Format("ORDER BY {0} ASC", _afcLogID)
                        };

                        using (RowCursor rowCursor = table.Search(queryFilter, false))
                        {


                            /* ***********************************
                             * Search through returned rows from *
                             * query filter and create a new     *
                             * AFC Log object and bind to the    *
                             * AFCLogs observable collection.    *
                             * ***********************************/
            while (rowCursor.MoveNext())
                            {
                                using (Row row = rowCursor.Current)
                                {
                                    AFCLog afcLog = new AFCLog();

                                    // Determine AFC NOTE length and truncate if longer than 35 chars
                                    int _afcNoteLength = Convert.ToString(row["AFC_NOTE"]).Length;
                                    if (_afcNoteLength > 35) _afcNoteLength = 35;

                                    afcLog.AFC_LOG_ID = Convert.ToInt32(row["AFC_LOG_ID"]);
                                    afcLog.AFC_STATUS_CD = Convert.ToInt32(row["AFC_STATUS_CD"]);
                                    afcLog.AFC_TYPE_CD = Convert.ToInt32(row["AFC_TYPE_CD"]);
                                    afcLog.AFC_YEAR = Convert.ToInt32(row["AFC_YEAR"]);
                                    afcLog.AFC_NOTE = Convert.ToString(row["AFC_NOTE"]).Substring(0, _afcNoteLength);
                                    afcLog.TILE_NO = Convert.ToInt32(row["TILE_NO"]);
                                    afcLog.DRAFTER_EMPL_ID = Convert.ToString(row["DRAFTER_EMPL_ID"]);
                                    afcLog.DRAFTER_COMP_DT = Convert.ToDateTime(row["DRAFTER_COMP_DT"]);
                                    afcLog.FILE_DATE = Convert.ToDateTime(row["FILE_DATE"]);
                                    afcLog.EFFECTIVE_DT = Convert.ToDateTime(row["EFFECTIVE_DT"]);
                                    afcLog.INSTRUMENT_NUM = Convert.ToString(row["INSTRUMENT_NUM"]);
                                    afcLog.SEQ_NUM = Convert.ToString(row["SEQ_NUM"]);
                                    afcLog.RUSH_IND = Convert.ToString(row["RUSH_IND"]) == _yes ? true : false;

                                    // Determine if Account Number is provided
                                    if (Convert.ToString(row["ACCOUNT_NUM"]).Equals(_blank)) _acctNumBlank = true;
                                    if (!_acctNumBlank) afcLog.ACCOUNT_NUM = Convert.ToString(row["ACCOUNT_NUM"]);

                                    //afcLog.ACCT_LIST = Convert.ToString(row["ACCT_LIST"]);
                                    afcLog.DOC_TYPE = Convert.ToString(row["DOC_TYPE"]);
                                    afcLog.SetImageSource();    // Method sets the image source for the afc log type
                                    afcLog.SetDocumentNumber(); // Method sets the document number for the afc log type
                                    afcLog.SetRecordType();     // Method sets the record type for the afc log

                                    // Set the record status based on
                                    // the AFC status code
                                    afcLog.SetRecordStatus();   // Method that sets the record status for the afc log

                                    /***************************************
                                    * Subscribe to RecordCreated Event in *
                                    * the AFCLog class.                   *
                                    * ************************************/
                                    afcLog.RecordCreatedEvent += OnRecordCreated;

                                    /***********************************************
                                    * Set the foreground color for the document    *
                                    * and account number properties based on the   *
                                    * RUSH_IND (if yes == RED else Black/Gray      *
                                    * *********************************************/
                                    afcLog.SetForegroundColor(_docNumType);
                                    afcLog.SetForegroundColor(_acctNumTYpe);

                                    _afcCount += 1;             // Increment afc count variable
                                    // Reads and Writes should be made from within the lock
                                    lock (_lockObj)
                                    {
                                        _afclogs.Add(afcLog);
                                    }
                                }
                            }
                        }
                    }

                    // If completed, add psuedo afc log if count is zero
                    AddEmptyListAFCLog(_afcCount);
                });
            }
            catch (GeodatabaseFieldException fieldException)
            {
                // One of the fields in the where clause might not exist. 
                // There are multiple ways this can be handled:
                // Handle error appropriately
               OS.LogWarning(fieldException.Message, OS.Source);
            }
            catch (Exception ex)
            {
                OS.LogException(ex, OS.Source);
            }
        }



        /// <summary>
        /// When a record is created, the dock pane should
        /// refresh and remove the newly created AFC log
        /// from the observable collection based on
        /// the database view. The database view involves
        /// a join on the records feature class and removes
        /// any AFC Logs whose document number matches an
        /// existing record in the feature class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordCreated(object sender, RecordCreatedEventArgs e)
        {
            try
            {

                SearchForAFCLogsAsync();

            }
            catch (Exception ex)
            {

                OS.LogException(ex, OS.Source);
            }

        }



        #region Add Empty Result List AFC Log
        /// <summary>
        /// Adds a psuedo afc log entry to the
        /// observable collection to send a message
        /// to the user to assign and afc log.
        /// </summary>
        /// <param name="_afcCount"></param>
        private void AddEmptyListAFCLog(int _afcCount)
                {
        
                    /***********************************************
                    * DEFAULT BEHAVIOR WHEN NO AFC LOG ASSIGNED   *
                    * *********************************************
                    * Displays a default afc log row in the       *
                    * observable collection. See details in       *
                    * the AFCLog class.                           *
                    * This type of afc log is not valid, but just *
                    * displays an empty marker with message.      *
                    **********************************************/
        
                    if (_afcCount == 0)
                    {
                        AFCLog afcLog = new AFCLog();
                        afcLog.AFC_LOG_ID = 1;
                        afcLog.AFC_STATUS_CD = 99;
                        afcLog.VALID_AFC_LOG = false;
                        afcLog.SetImageSource();
                        afcLog.SetDocumentNumber();


                lock (_lockObj)
                        {
                            _afclogs.Add(afcLog);
                        }

                
                    }
        
                    /***************************************
                     * Disable the create record button    *
                     * for this type of entry.             *
                     * ************************************/
                     
        
                }
        #endregion



        #region Get Server Instance Based on Environment
        /// <summary>
        /// This method returns the database server
        /// name based on the environment derived from
        /// the active portal.
        /// </summary>
        public async Task GetServerInstanceByEnvAsync()
        {

            // Set the environment
            await _web.SetEnvironmentAsync();

            switch (_web.Environment)
                {
                    case 0: // Development database server
                        ServerInstance = _db.DevTransDBServer;
                        break;

                    case 1: // Staging database server
                        ServerInstance = _db.StgTransDBServer;
                        break;

                    case 2: // Production database server
                        ServerInstance = _db.prdTransDBServer;
                        break;

                    case 3: // Database server unknown

                        OS.LogError(Messages.ErrorMessages[3001], OS.Source);

                        // TODO: Correct dispatch error with these - MessageBox.Show("The target database server is UNKNOWN.", "Create Records Add-In", MessageBoxButtons.OK);

                        break;

                    default: // Something else happened

                        OS.LogError(Messages.ErrorMessages[3002], OS.Source);

                    // TODO: Correct dispatch error with these - MessageBox.Show("Created Records Add-In: There was a problem setting the target database environment.");

                        break;
                }


        }

        #endregion

        #region Check if required views exist
        /// <summary>
        /// This asynchronous method returns a
        /// boolean value verifying the existance
        /// of a view name entered as a parameter.
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns>bool</returns>
        public async Task RequiredViewsExistAsync(string viewName)
        {
            try
            {

                _db.DatabaseName = _database;
                _db.BranchVersionName = _version;


               await QueuedTask.Run(() =>
               {
                   using (Geodatabase geodatabase =
                   new Geodatabase(_db.GetDatabaseConnProperties(ServerInstance)))

                   {
                      _db.TableExists(geodatabase, viewName).Wait();

                   }
               });


                // Update the property based on the
                // view name
                switch (viewName)
                {
                    case _afcView:
                        AFCViewExists = _db.ViewExists;
                        break;
                    case _actView:
                        ACTViewExists = _db.ViewExists;
                        break;
                    case _dltView:
                        DLTViewExists = _db.ViewExists;
                        break;
                    default:
                        break;
                }

            }

            catch (Exception ex)
            {
                OS.LogException(ex, OS.Source);
            }
        }

        #endregion

        #region Check if Parcel fabric exists

        public bool CheckParcelFabricExists()
        {
            try
            {
                bool fabricExists = false;

                fabricExists = _lyrs.ParcelFabricExists().Result;

                return fabricExists;

            }
            catch (Exception ex)
            {

                OS.LogException(ex, OS.Source);
                return false;
            }
        }
        #endregion

        #endregion


        #region Delegates




        #endregion

        #endregion


    }


    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class CreateRecordsPane_ShowButton : ArcGIS.Desktop.Framework.Contracts.Button
    {
        protected override void OnClick()
        {
            CreateRecordsPaneViewModel.Show();
        }
    }
}
