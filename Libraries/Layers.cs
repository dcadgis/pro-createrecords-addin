

using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
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
     * Project:         Layers Class Library
     * Class:           Layers Library
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
     * PURPOSE:     This class contains properties and methods for interfacing with ArcGIS Pro map layers.
     *
     * CLASS
     * DESCRIPTION: Class members provide interactions with map layers.
     *
     *
     * CLASS
     * PROPERTIES:   Describe the properties (protected or otherwise) for this class.
     *              (ParcelFabricInMap - parcel fabric layer exists in current arcgis pro map.)
     *
     * CLASS 
     * METHODS:     ParcelFabricExists() - Asynchronous method that retrieves the parcel fabric layer
     *                                     if it exists and reports whether it exists.
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

    public class Layers : Messaging
    {

        #region Properties

        #region Parcel Fabric In Map Property
        /// <summary>
        /// Boolean property
        /// verifying if a
        /// parcel fabric exists
        /// in the current map.
        /// </summary>
        private bool _fabricExists;

        public bool ParcelFabricInMap
        {
            get { return _fabricExists; }
            set { _fabricExists = value; }
        }


        #endregion

        #endregion

        #region Layer Methods

        #region Parcel Fabric Layers

        #region Check if parcel fabric layer exists

        /// <summary>
        /// Asynchronous operation that
        /// returns a boolean result for 
        /// locating the parcel fabric 
        /// layer in the current map view.
        /// </summary>
        /// <returns>bool</returns>
        //public async Task<bool> ParcelFabricExists()
        public async Task<bool> ParcelFabricExists()
        {
            try
            {
                ParcelLayer parcelFabricLayer = null;

                await QueuedTask.Run(() =>
                {

                    parcelFabricLayer =
                                        MapView.Active.Map.GetLayersAsFlattenedList().OfType<ParcelLayer>().FirstOrDefault();


                });

                
                

                // if there is no fabric in the map then bail
                if (parcelFabricLayer == null)
                {
                    OS.LogError(this.ErrorMessages[3006], OS.Source);
                    return false;
                }

                else
                {
                    return true;
                }

                
            }
            catch (Exception ex)
            {

                OS.LogException(ex, OS.Source);
                return false;
            }


        }


        #endregion

        #endregion

        #endregion

    }
}
