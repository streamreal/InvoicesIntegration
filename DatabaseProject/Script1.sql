/****** Script for SelectTopNRows command from SSMS  ******/
SELECT [CustomerINN], ca.*
FROM (SELECT DISTINCT [CustomerINN] FROM [personalcabinets].[dbo].[vCabinets]) vc
CROSS APPLY (
SELECT
CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND  COALESCE(v.[ShipperName], '') = '') THEN 1 ELSE 0 END isEmptyShipperName
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[No_], '') = '') THEN 1 ELSE 0 END isEmptyNo_
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DeliveryDocumentDate], '') = '') THEN 1 ELSE 0 END isEmptyDeliveryDocumentDate
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[State], '') = '') THEN 1 ELSE 0 END isEmptyState
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[oper_station_name], '') = '') THEN 1 ELSE 0 END isEmptyoper_station_name
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[ExecDepartmentName], '') = '') THEN 1 ELSE 0 END isEmptyExecDepartmentName
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[STLSalesPersonName], '') = '') THEN 1 ELSE 0 END isEmptySTLSalesPersonName
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DCLDepartmentCode], '') = '') THEN 1 ELSE 0 END isEmptyDCLDepartmentCode
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[STOSalesPersonName], '') = '') THEN 1 ELSE 0 END isEmptySTOSalesPersonName
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[Add_Documents], '') = '') THEN 1 ELSE 0 END isEmptyAdd_Documents
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DeliveryObjects], '') = '') THEN 1 ELSE 0 END isEmptyDeliveryObjects
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[TrainTransportDesc], '') = '') THEN 1 ELSE 0 END isEmptyTrainTransportDesc
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DRLFirstActDate], '') = '') THEN 1 ELSE 0 END isEmptyDRLFirstActDate
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[TransportMethod], '') = '') THEN 1 ELSE 0 END isEmptyTransportMethod
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[TransitDir], '') = '') THEN 1 ELSE 0 END isEmptyTransitDir
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[TERMINAL_NAME], '') = '') THEN 1 ELSE 0 END isEmptyTERMINAL_NAME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DRLTermExpInDate], '') = '') THEN 1 ELSE 0 END isEmptyDRLTermExpInDate
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DELIVERY_SVX_DATE_TIME], '') = '') THEN 1 ELSE 0 END isEmptyDELIVERY_SVX_DATE_TIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[TCD_DATE_TIME], '') = '') THEN 1 ELSE 0 END isEmptyTCD_DATE_TIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DTCreateDate], '') = '') THEN 1 ELSE 0 END isEmptyDTCreateDate
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[ExecutorUSERNAMEKey], 0) = 0) THEN 1 ELSE 0 END isEmptyExecutorUSERNAMEKey
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[REGNUM_DT_DATE_TIME], '') = '') THEN 1 ELSE 0 END isEmptyREGNUM_DT_DATE_TIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[ReleaserUSERNAMEKey], 0) = 0) THEN 1 ELSE 0 END isEmptyReleaserUSERNAMEKey
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[RegUsername_Full], '') = '') THEN 1 ELSE 0 END isEmptyRegUsername_Full
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[CreaterUsername_Full], '') = '') THEN 1 ELSE 0 END isEmptyCreaterUsername_Full
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[RegNumDT], '') = '') THEN 1 ELSE 0 END isEmptyRegNumDT
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[EDSTATUS_DT_Description], '') = '') THEN 1 ELSE 0 END isEmptyEDSTATUS_DT_Description
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[WriteOffAmount], 0) = 0) THEN 1 ELSE 0 END isEmptyWriteOffAmount
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DOSMOTR_DATE_TIME], '') = '') THEN 1 ELSE 0 END isEmptyDOSMOTR_DATE_TIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[AKT_DOSMOTRA_DATE_TIME], '') = '') THEN 1 ELSE 0 END isEmptyAKT_DOSMOTRA_DATE_TIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[FINISH_DATETIME], '') = '') THEN 1 ELSE 0 END isEmptyFINISH_DATETIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DEPARTURE_SVX_DATE_TIME], '') = '') THEN 1 ELSE 0 END isEmptyDEPARTURE_SVX_DATE_TIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DRLLastExpDate], '') = '') THEN 1 ELSE 0 END isEmptyDRLLastExpDate
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DRLLastFactDate], '') = '') THEN 1 ELSE 0 END isEmptyDRLLastFactDate
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[NAV_TotalBilledAmountRUR_STO], 0) = 0) THEN 1 ELSE 0 END isEmptyNAV_TotalBilledAmountRUR_STO
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[BilledSTL], 0) = 0) THEN 1 ELSE 0 END isEmptyBilledSTL
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[Billed1C_STO], 0) = 0) THEN 1 ELSE 0 END isEmptyBilled1C_STO
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[Billed1C_GMBH], 0) = 0) THEN 1 ELSE 0 END isEmptyBilled1C_GMBH
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[CargoID], '') = '') THEN 1 ELSE 0 END isEmptyCargoID
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[CargoCommonDiscription], '') = '') THEN 1 ELSE 0 END isEmptyCargoCommonDiscription
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[id_request], '') = '') THEN 1 ELSE 0 END isEmptyid_request
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[Arhive], 0) = 0) THEN 1 ELSE 0 END isEmptyArhive
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[TypeofObject], '') = '') THEN 1 ELSE 0 END isEmptyTypeofObject
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DeliverPointLoading], '') = '') THEN 1 ELSE 0 END isEmptyDeliverPointLoading
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[FirstRailwayStation], '') = '') THEN 1 ELSE 0 END isEmptyFirstRailwayStation
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[LastRailwayStation], '') = '') THEN 1 ELSE 0 END isEmptyLastRailwayStation
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[DeliverPointUnloading], '') = '') THEN 1 ELSE 0 END isEmptyDeliverPointUnloading
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[car_number], '') = '') THEN 1 ELSE 0 END isEmptycar_number
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[rest_distance], '') = '') THEN 1 ELSE 0 END isEmptyrest_distance
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[rash_date], '') = '') THEN 1 ELSE 0 END isEmptyrash_date
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[PickupCurCode], '') = '') THEN 1 ELSE 0 END isEmptyPickupCurCode
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[PickupAmount], 0) = 0) THEN 1 ELSE 0 END isEmptyPickupAmount
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[man_TCD_DATE_TIME], '') = '') THEN 1 ELSE 0 END isEmptyman_TCD_DATE_TIME
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[AmountOfCustomsPayments], '') = '') THEN 1 ELSE 0 END isEmptyAmountOfCustomsPayments
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[man_TransportDeliveryCloseDateTimeCaption], '') = '') THEN 1 ELSE 0 END isEmptyman_TransportDeliveryCloseDateTimeCaption
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[payment_check], '') = '') THEN 1 ELSE 0 END isEmptypayment_check
,CASE WHEN EXISTS(SELECT 0 FROM [personalcabinets].[dbo].[vCabinets] v WHERE v.customerINN = vc.CustomerINN AND COALESCE(v.[Mode], '') = '') THEN 1 ELSE 0 END isEmptyMode) AS ca