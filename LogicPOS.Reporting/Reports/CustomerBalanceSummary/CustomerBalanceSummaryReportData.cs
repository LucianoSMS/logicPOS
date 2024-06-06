﻿/* 
VIEW 
		[dbo].[view_documentfinancecustomerbalancesummary] 
	AS
		SELECT
			[EntityOid],
			MIN([Date]) as CustomerSinceDate,
			SUM([Credit]) AS TotalCredit,
			SUM([Debit]) AS TotalDebit,
			SUM([Credit]) - SUM([Debit]) AS Balance
		FROM
			view_documentfinancecustomerbalancedetails
		GROUP BY
			[EntityOid]
*/
using LogicPOS.Reporting.Common;
using System;

namespace LogicPOS.Reporting.Reports.CustomerBalanceSummary
{
    /* IN008018 and IN009010 */
    [Report(Entity = "view_documentfinancecustomerbalancesummary")]
    internal class CustomerBalanceSummaryReportData : ReportData
    {
        [Report(Field = "EntityOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }        //DocumentTypeOid AS Oid,

        public string EntityOid { get; set; }
        public DateTime CustomerSinceDate { get; set; } // MIN([DocumentDate]) as CustomerSinceDate
        public decimal TotalCredit { get; set; }        // SUM([Credit]) AS TotalCredit
        public decimal TotalDebit { get; set; }         // SUM([Debit]) AS TotalDebit
        public decimal Balance { get; set; }            // SUM(Credit) - SUM(Debit) AS BALANCE

        [Report(Hide = true)]
        public string EntityName { get; set; }
        [Report(Hide = true)]
        public string EntityFiscalNumber { get; set; }
    }
}