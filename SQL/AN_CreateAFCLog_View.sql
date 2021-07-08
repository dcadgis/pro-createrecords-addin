﻿/***QUERY DOCUMENTATION ***********************************************************************************************************************************************
   Author:             John W. Fell
   Date:               06/28/21
   File Name:          AN_CreateAFCLog_View.sql
   Environment:        Development
   Exec Location:      
   Code Location:      https://github.com/dcadgis/pro-createrecords-addin
   Purpose:            To generate a database view named ADM.AFC_LOG_VW for current AFC Logs assigned to the 
                       authenticated user.
   Algorithm:          Presents a database view for uncompleted and assigned AFC Logs. The SYSTEM_USER keyword
                       helps to present only those assigned logs to the authenticated user. This view ignores
                       certain types of AFC Logs. The only valid AFC log types are those with a status of 
                       'Active' or 'Cert-Hold'.
                              (1) Pending         - These have some kind of issue precluding the AFC from being processed
                              (2) Deleted         - Are no longer valid AFCs
                              (3) Quality Control - Have recently been completed by a specialist and are under review
                              (4) Corrections     - Require corrections before being sent to PRE
                    
   Usage:              The database view can be queried using an IQueryFilter or ArcGIS Pro SDK Snippet.
   Dependencies:       There must be a table named AFC_LOG in the mars database.
   Permissions:        The authenticated user must have read permission. 
   Resources:          (1) SYSTEM_USER  - https://bit.ly/3y0Zfv1
                       (2) 
   Revision History:
                        06/28/21 - Created sql script.  -- jwf --
 *********************************************************************************************************************************************************************/

USE GEDT
GO

/************************************************************** 
*  DELETE VIEW IF IT EXISTS                                   *
*  Note: Only works in MS SQL Server 2016 and later           *
**************************************************************/
DROP VIEW IF EXISTS [ADM].[AFC_LOG_VW]
GO

/* VIEW DEFINITION */
CREATE VIEW [ADM].[AFC_LOG_VW]
AS
SELECT  AFC.AFC_LOG_ID
       ,AFC.AFC_YEAR
	   ,AFC.AFC_TYPE_CD
	   ,AFC.AFC_STATUS_CD
	   ,AFC.AFC_NOTE
	   ,AFC.RUSH_IND
	   ,AFC.TILE_NO
	   ,AFC.ACCOUNT_NUM
	   ,AFC.INSTRUMENT_NUM
	   ,AFC.SEQ_NUM
	   ,AFC.FILE_DATE
	   ,AFC.EFFECTIVE_DT
	   ,AFC.DRAFTER_EMPL_ID
	   ,AFC.DRAFTER_COMP_DT
	   ,AFC.ACCT_LIST
	   ,CASE AFC.AFC_TYPE_CD
	       WHEN 3 THEN 'NO_INST_NUM'
		   ELSE DEED.DOC_TYPE
		END AS DOC_TYPE
	   
	    /**************************************
	    *   Make sure the appropriate path    *
		*   to the mars database is supplied  *
	    **************************************/
	   FROM DCADSQLVM01.DBPROD.DBO.AFC_LOG AFC  

	   INNER JOIN

	   DCADSQLVM01.DBPROD.DBO.DEED_MAIN DEED

	   ON AFC.INSTRUMENT_NUM = DEED.INSTRUMENT_NUM
	   
	   
	   
	   WHERE AFC.DRAFTER_EMPL_ID = CONVERT(CHAR(8), RIGHT(UPPER(RTRIM(SYSTEM_USER)), LEN(SYSTEM_USER) - 5))
	     AND AFC.AFC_YEAR IN (YEAR(GETDATE()) - 1, YEAR(GETDATE()))
		 AND AFC.DRAFTER_COMP_DT = '1900-01-01 00:00:00.000'
		 AND AFC.AFC_STATUS_CD IN (1, 4)