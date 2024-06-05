﻿using LogicPOS.Data.XPO.Settings;
using LogicPOS.DTOs.Printing;
using LogicPOS.DTOs.Reporting;
using LogicPOS.Globalization;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Printing.Tickets;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.Printing.Documents
{
    public class FinanceDocumentPayment : BaseFinanceTemplate
    {
        private readonly PrintingFinancePaymentDto _financePaymentDto = null;
        private readonly List<FinancePaymentViewReportDto> _financePaymentViewReportDtos;
        private readonly List<FinancePaymentDocumentViewReportDto> _financePaymentDocumentViewReportDtos;

        public FinanceDocumentPayment(
            PrintingPrinterDto printer,
            PrintingFinancePaymentDto financePayment,
            List<int> copyNumbers,
            bool secondCopy,
            List<FinancePaymentViewReportDto> financePaymentViewReportDtos)
            : base(
                  printer,
                  financePayment.DocumentType,
                  copyNumbers,
                  secondCopy)
        {
            _financePaymentDto = financePayment;
            _financePaymentViewReportDtos = financePaymentViewReportDtos;
            _financePaymentDocumentViewReportDtos = financePaymentViewReportDtos[0].DocumentFinancePaymentDocument;
        }

        public override void PrintContent()
        {
            try
            {
                //Call base PrintDocumentMaster();
                PrintDocumentMaster(_financePaymentViewReportDtos[0].DocumentTypeResourceString, _financePaymentViewReportDtos[0].PaymentRefNo, _financePaymentViewReportDtos[0].DocumentDate);

                //Call base PrintCustomer();
                PrintCustomer(
                    _financePaymentViewReportDtos[0].EntityName,
                    _financePaymentViewReportDtos[0].EntityAddress,
                    _financePaymentViewReportDtos[0].EntityZipCode,
                    _financePaymentViewReportDtos[0].EntityCity,
                    _financePaymentViewReportDtos[0].EntityCountry,
                    _financePaymentViewReportDtos[0].EntityFiscalNumber
                );

                PrintDocumentDetails();
                PrintMasterTotals();
                PrintExtendedValue();

                //Call base PrintDocumentPaymentDetails();
                PrintDocumentPaymentDetails(_financePaymentViewReportDtos[0].PaymentMethodDesignation, _financePaymentViewReportDtos[0].CurrencyAcronym);

                //Call base PrintNotes();
                if (!string.IsNullOrEmpty(_financePaymentViewReportDtos[0].Notes)) PrintNotes(_financePaymentViewReportDtos[0].Notes.ToString());

                //Call base PrintDocumentTypeFooterString();
                PrintDocumentTypeFooterString(_financePaymentViewReportDtos[0].DocumentTypeResourceStringReport);

                //Call Base CertificationText Without Hash
                PrintCertificationText();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("DocumentDate", CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_date"), 11, TicketColumnsAlignment.Left),
                    new TicketColumn("DocumentNumber", CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_document_number_acronym"), 0, TicketColumnsAlignment.Left),
                    new TicketColumn("DocumentTotal", CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_document_total"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"),
                    new TicketColumn("TotalPayed", CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_total_payed_acronym"), 10, TicketColumnsAlignment.Right, typeof(decimal), "{0:00.00}"),
                    new TicketColumn("Payed", "L", 1, TicketColumnsAlignment.Right)
                };
            //Prepare Table with Padding
            DataTable dataTable = TicketTable.InitDataTableFromTicketColumns(columns);
            TicketTable ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
            string paddingLeftFormat = "  {0,-" + ticketTable.TableWidth + "}";//"  {0,-TableWidth}"
                                                                               //Print Table Headers
            ticketTable.Print(_genericThermalPrinter, paddingLeftFormat);

            foreach (var item in _financePaymentDocumentViewReportDtos)
            {
                //Recreate/Reset Table for Item Details Loop
                ticketTable = new TicketTable(dataTable, columns, _maxCharsPerLineNormal - _ticketTablePaddingLeftLength);
                PrintDocumentDetail(ticketTable, item, paddingLeftFormat);
            }

            //Line Feed
            _genericThermalPrinter.LineFeed();
        }

        public void PrintDocumentDetail(
            TicketTable ticketTable,
            FinancePaymentDocumentViewReportDto financePaymentDocumentViewReportDto,
            string paddingLeftFormat)
        {
            try
            {
                //Trim Data
                string documentNumber = (financePaymentDocumentViewReportDto.DocumentNumber.Length <= _maxCharsPerLineNormalBold)
                    ? financePaymentDocumentViewReportDto.DocumentNumber
                    : financePaymentDocumentViewReportDto.DocumentNumber.Substring(0, _maxCharsPerLineNormalBold)
                ;
                //Print Document Number : Bold
                _genericThermalPrinter.WriteLine(documentNumber, WriteLineTextMode.Bold);

                //Document Details
                DataRow dataRow;
                dataRow = ticketTable.NewRow();
                dataRow[0] = financePaymentDocumentViewReportDto.DocumentDate;
                dataRow[1] = financePaymentDocumentViewReportDto.DocumentNumber;
                //dataRow[2] = (item.CreditAmount > 0 && item.DocumentTotal > item.CreditAmount) 
                //    ? LogicPOS.Utility.DataConversionUtils.DecimalToString((item.DocumentTotal - item.CreditAmount) * _documentFinancePayment.ExchangeRate)
                //    : string.Empty;
                dataRow[2] = financePaymentDocumentViewReportDto.DocumentTotal * _financePaymentDto.ExchangeRate;
                dataRow[3] = financePaymentDocumentViewReportDto.CreditAmount * _financePaymentDto.ExchangeRate;
                dataRow[4] = (financePaymentDocumentViewReportDto.Payed) ? "*" : string.Empty;

                //Add DataRow to Table, Ready for Print
                ticketTable.Rows.Add(dataRow);
                //Print Table Rows
                ticketTable.Print(_genericThermalPrinter, WriteLineTextMode.Normal, true, paddingLeftFormat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintMasterTotals()
        {
            try
            {
                //Init DataTable
                DataRow dataRow = null;
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Value", typeof(string)));

                //Add Row : TotalFinal
                dataRow = dataTable.NewRow();
                dataRow[0] = CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_total");
                dataRow[1] = DataConversionUtils.DecimalToString(_financePaymentViewReportDtos[0].PaymentAmount * _financePaymentViewReportDtos[0].ExchangeRate);
                dataTable.Rows.Add(dataRow);

                //Configure Ticket Column Properties
                List<TicketColumn> columns = new List<TicketColumn>
                {
                    new TicketColumn("Label", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Left),
                    new TicketColumn("Value", "", Convert.ToInt16(_maxCharsPerLineNormal / 2) - 2, TicketColumnsAlignment.Right)
                };

                //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
                TicketTable ticketTable = new TicketTable(dataTable, columns, _genericThermalPrinter.MaxCharsPerLineNormalBold);
                ticketTable.Print(_genericThermalPrinter, WriteLineTextMode.DoubleHeightBold);

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintExtendedValue()
        {
            try
            {
                string extended = _financePaymentDto.ExtendedValue;

                //Require to generated/override default Exchanged with document ExchangeRate Extended Value (Foreign Curency)
                if (_financePaymentViewReportDtos[0].CurrencyAcronym != XPOSettings.ConfigurationSystemCurrency.Acronym)
                {
                    //Get ExtendedValue
                    NumberToWordsUtility extendValue = new NumberToWordsUtility();
                    extended = extendValue.GetExtendedValue(_financePaymentViewReportDtos[0].PaymentAmount * _financePaymentViewReportDtos[0].ExchangeRate, _financePaymentViewReportDtos[0].CurrencyDesignation);
                }

                //ExtendedValue
                _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_total_extended_label"), WriteLineTextMode.Bold);
                _genericThermalPrinter.WriteLine(extended);

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}