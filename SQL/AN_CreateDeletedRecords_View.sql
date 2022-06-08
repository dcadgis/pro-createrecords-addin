/*********************************************************************************************************************************************************************
       == Author:             John W. Fell
       == Date:               05/30/22
       == File Name:          AN_CreateDeletedRecords_View.sql
       == Environment:        Development
       == Exec Location:      GEDT.ADM.DELETED_RECORDS_VW
       == Code Location:      https://www.github.com/dcadgis/pro-createrecords-addin
       == Purpose:            To define a view that displays only the deleted records within a parcel fabric
	   ==                     feature service.
       == Algorithm:          Database archiving is required for feature service publishing, so duplicate rows may result
	   ==                     for deleted records. Standard queries do not work in this scenario and therefore, this approach
	   ==                     first eliminates any deleted records (that display in the business table with archiving enabled)
	   ==                     joining to a separate view named ADM.DELETED_RECORDS_VW.
       == Usage:              The ACTIVE_RECORDS_VW will include this view to determine which records are active.
       == Dependencies:       There must be a parcel fabric named PARCELFABRIC.
       == Permissions:        The authenticated user must have read permission. 
       == Resources:          (1) 
       == Revision History:
     *********************************************************************************************************************************************************************/



/****** Object:  View [ADM].[DELETED_RECORDS_VW]    Script Date: 6/8/2022 3:23:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER VIEW [ADM].[DELETED_RECORDS_VW]

AS 

SELECT R.OBJECTID AS DR_OBJECTID
	  ,R.Name                  AS DR_NAME
      ,R.created_user		   AS DR_CREATED_USER
	  ,R.created_date		   AS DR_CREATED_DATE
	  ,R.AFCType			   AS DR_AFCTYPE
	  ,R.EffectiveDate		   AS DR_EFFECTIVEDATE
	  ,R.GDB_ARCHIVE_OID	   AS DR_GDB_ARCHIVE_OID
	  ,R.GDB_BRANCH_ID		   AS DR_GDB_BRANCH_ID
	  ,R.GDB_DELETED_AT		   AS DR_GDB_DELETED_AT
	  ,R.GDB_DELETED_BY		   AS DR_GDB_DELETED_BY
	  ,R.GDB_FROM_DATE		   AS DR_GDB_FROM_DATE
	  ,R.GDB_GEOMATTR_DATA	   AS DR_GDB_GEOMATTR_DATA
	  ,R.GDB_IS_DELETE		   AS DR_GDB_IS_DELETE
	  ,R.GlobalID			   AS DR_GLOBAL_ID
	  ,R.last_edited_date	   AS DR_LAST_EDITED_DATE
	  ,R.last_edited_user	   AS DR_LAST_EDITED_USER
	  ,R.OriginalFeatureOID	   AS DR_ORIGINALFEATUREOID
	  ,R.ParcelCount		   AS DR_PARCELCOUNT
	  ,R.RecordedDate		   AS DR_RECORDEDDATE
	  ,R.RecordStatus		   AS DR_RECORDSTATUS
	  ,R.RecordType			   AS DR_RECORDTYPE
	  ,R.RetiredParcelCount	   AS DR_RETIREDPARCELCOUNT

FROM ADM.PARCELFABRIC_RECORDS R 

WHERE R.GDB_IS_DELETE = 1
GO


