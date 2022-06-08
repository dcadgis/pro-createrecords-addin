/*********************************************************************************************************************************************************************
       == Author:             John W. Fell
       == Date:               05/30/22
       == File Name:          AN_CreateActiveRecords_View.sql
       == Environment:        Development
       == Exec Location:      GEDT.ADM.ACTIVE_RECORDS_VW
       == Code Location:      https://www.github.com/dcadgis/pro-createrecords-addin
       == Purpose:            To define a view that displays only the active records within a parcel fabric
	   ==                     feature service.
       == Algorithm:          Database archiving is required for feature service publishing, so duplicate rows may result
	   ==                     for deleted records. Standard queries do not work in this scenario and therefore, this approach
	   ==                     first eliminates any deleted records (that display in the business table with archiving enabled)
	   ==                     joining to a separate view named ADM.DELETED_RECORDS_VW.
       == Usage:              The AFC_LOG_VW_SDE will include this view instead of the PARCEL FABRIC RECORDS feature class.
       == Dependencies:       There must be a view named ADM.DELETED_RECORDS_VW and a parcel fabric named PARCELFABRIC.
       == Permissions:        The authenticated user must have read permission. 
       == Resources:          (1) 
       == Revision History:
     *********************************************************************************************************************************************************************/



/****** Object:  View [ADM].[ACTIVE_RECORDS_VW]    Script Date: 6/8/2022 3:23:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER   VIEW [ADM].[ACTIVE_RECORDS_VW]

AS 

SELECT R.OBJECTID
	  ,R.GlobalID
      ,R.Name
	  ,R.created_user
	  ,R.created_date
	  ,R.RecordType
	  ,R.RecordStatus
	  ,R.AFCType
	  ,R.COGOAccuracy
	  ,R.RecordedDate
	  ,R.EffectiveDate
	  ,R.ParcelCount
	  ,R.last_edited_user
	  ,R.last_edited_date
	  ,R.OriginalFeatureOID
	  ,R.RetiredParcelCount
	  ,R.GDB_ARCHIVE_OID
	  ,R.GDB_BRANCH_ID
	  ,R.GDB_DELETED_AT
	  ,R.GDB_DELETED_BY
	  ,R.GDB_FROM_DATE
	  ,R.GDB_GEOMATTR_DATA
	  ,R.GDB_IS_DELETE

FROM ADM.PARCELFABRIC_RECORDS R 

LEFT OUTER JOIN

ADM.DELETED_RECORDS DR

ON R.OBJECTID             = DR.DR_OBJECTID

WHERE DR.DR_OBJECTID IS NULL
GO


