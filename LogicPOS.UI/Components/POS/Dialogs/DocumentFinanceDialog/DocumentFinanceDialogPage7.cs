﻿using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Finance.Dtos;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Shared.Orders;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    /// <summary>
    /// Test Page : Not Used
    /// </summary>

    internal class DocumentFinanceDialogPage7 : PagePadPage
    {
        private readonly VBox _vboxButtons;
        private readonly HBox _hboxButtons1;
        private readonly HBox _hboxButtons2;
        private readonly OrderMain _orderMain;
        private readonly ArticleBag _articleBag;

        //Constructor
        public DocumentFinanceDialogPage7(Window pSourceWindow, string pPageName) : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage7(Window pSourceWindow, string pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage7(Window pSourceWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            _vboxButtons = new VBox(true, 0);
            _hboxButtons1 = new HBox(true, 0);
            _hboxButtons2 = new HBox(true, 0);

            //Print Invoice
            Button buttonPrintInvoice = new Button("Print Invoice") { Sensitive = false };
            buttonPrintInvoice.Clicked += buttonPrintInvoice_Clicked;

            //Cancel Invoice
            Button buttonCancelInvoice = new Button("Cancel Invoice");
            buttonCancelInvoice.Clicked += buttonCancelInvoice_Clicked;

            //OrderReferences
            Button buttonOrderReferences = new Button("Order References");
            buttonOrderReferences.Clicked += buttonOrderReferences_Clicked;

            //Credit Note
            Button buttonCreditNote = new Button("Credit Note");
            buttonCreditNote.Clicked += buttonCreditNote_Clicked;

            //Print Invoice With Diferent Vat's
            Button buttonPrintInvoiceVat = new Button("Print Invoice Vat");
            buttonPrintInvoiceVat.Clicked += buttonPrintInvoiceVat_Clicked;

            //Print Invoice With Discounts
            Button buttonPrintInvoiceDiscount = new Button("Print Invoice Discount");
            buttonPrintInvoiceDiscount.Clicked += buttonPrintInvoiceDiscount_Clicked;

            //Print Invoice With ExchangeRate
            Button buttonPrintInvoiceExchangeRate = new Button("Print Invoice ExchangeRate");
            buttonPrintInvoiceExchangeRate.Clicked += buttonPrintInvoiceExchangeRate_Clicked;

            //Print Invoice With JohnDoe1
            Button buttonPrintInvoiceJohnDoe1 = new Button("Print Invoice JohnDoe1");
            buttonPrintInvoiceJohnDoe1.Clicked += buttonPrintInvoiceJohnDoe1_Clicked;

            //Print Invoice With JohnDoe2
            Button buttonPrintInvoiceJohnDoe2 = new Button("Print Invoice JohnDoe2");
            buttonPrintInvoiceJohnDoe2.Clicked += buttonPrintInvoiceJohnDoe2_Clicked;

            //Print Invoice Transportation Guide With Totals
            Button buttonPrintTransportationGuideWithTotals = new Button("Print Transportation Guide With Totals");
            buttonPrintTransportationGuideWithTotals.Clicked += buttonPrintTransportationGuideWithTotals_Clicked;

            //Print Invoice Transportation Guide Without Totals
            Button buttonPrintTransportationGuideWithoutTotals = new Button("Print Transportation Guide Without Totals");
            buttonPrintTransportationGuideWithoutTotals.Clicked += buttonPrintTransportationGuideWithoutTotals_Clicked;

            //Pack hboxButtons1
            _hboxButtons1.PackStart(buttonPrintInvoice, true, true, 2);
            _hboxButtons1.PackStart(buttonCancelInvoice, true, true, 2);
            _hboxButtons1.PackStart(buttonOrderReferences, true, true, 2);
            _hboxButtons1.PackStart(buttonCreditNote, true, true, 2);
            _hboxButtons1.PackStart(buttonPrintInvoiceVat, true, true, 2);
            _hboxButtons1.PackStart(buttonPrintInvoiceDiscount, true, true, 2);
            //Pack hboxButtons2
            _hboxButtons2.PackStart(buttonPrintInvoiceExchangeRate, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintInvoiceJohnDoe1, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintInvoiceJohnDoe2, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintTransportationGuideWithTotals, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintTransportationGuideWithoutTotals, true, true, 2);

            //Shared : Prepare ArticleBag
            if (POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId))
            {
                _orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                if (_orderMain != null) _articleBag = ArticleBag.TicketOrderToArticleBag(_orderMain);
                if (_articleBag != null && _articleBag.Count > 0)
                {
                    buttonPrintInvoice.Sensitive = true;
                }
            }

            _vboxButtons.PackStart(_hboxButtons1, true, true, 2);
            _vboxButtons.PackStart(_hboxButtons2, true, true, 2);

            PackStart(_vboxButtons);
        }

        //Override Base Validate
        public override void Validate()
        {
            //_logger.Debug(string.Format("Validate: {0}", this.Name));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        //5.2: FT: Fatura
        private void buttonPrintInvoice_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = InvoiceSettings.InvoiceId;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, _articleBag)
            {
                Customer = customerGuid
            };

            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
            _vboxButtons.Sensitive = false;
        }

        //5.3: FT: Cancel Invoice
        private void buttonCancelInvoice_Clicked(object sender, EventArgs e)
        {
            string dateTimeFormatCombinedDateTime = LogicPOS.Settings.CultureSettings.DateTimeFormatCombinedDateTime;
            Guid documentMasterGuid = new Guid("81fcf207-ff59-4971-90cb-80d2cbdb87dc");//Document To Cancel
            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), documentMasterGuid);

            //Cancel Document
            documentFinanceMaster.DocumentStatusStatus = "A";
            documentFinanceMaster.DocumentStatusDate = XPOUtility.CurrentDateTimeAtomic().ToString(dateTimeFormatCombinedDateTime);
            documentFinanceMaster.DocumentStatusReason = "Erro ao Inserir Artigos";
            documentFinanceMaster.Save();
        }

        //OrderReferences
        private void buttonOrderReferences_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = InvoiceSettings.InvoiceId;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");
            Guid orderReference = new Guid("fbec0056-71a7-4d5b-8bfa-d5e887ec585f");

            //DC DC2015S0001/1
            fin_documentfinancemaster documentOrderReference = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), orderReference);
            //Add Order References
            List<fin_documentfinancemaster> orderReferences = new List<fin_documentfinancemaster>
            {
                documentOrderReference
            };

            //Get ArticleBag from documentFinanceMasterSource
            ArticleBag articleBag = ArticleBag.DocumentFinanceMasterToArticleBag(documentOrderReference);

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                OrderReferences = orderReferences,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                SourceOrderMain = documentOrderReference.SourceOrderMain
            };

            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        //NC : Credit Note
        private void buttonCreditNote_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = CustomDocumentSettings.CreditNoteId;
            Guid reference = new Guid("daecbf1d-6211-4e74-a8cd-81795e347656");

            //FT FT2015S0001/16
            fin_documentfinancemaster documentReference = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), reference);
            //Add Order References
            List<DocumentReference> references = new List<DocumentReference>
            {
                new DocumentReference(documentReference, "Artigo com defeito")
            };

            //Get ArticleBag from documentFinanceMasterSource
            ArticleBag articleBag = ArticleBag.DocumentFinanceMasterToArticleBag(documentReference);

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = documentReference.EntityOid,
                //References = references,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        //FT: Vats
        private void buttonPrintInvoiceVat_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = InvoiceSettings.InvoiceId;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");
            Guid vatExemptionReasonGuid = new Guid("8311ce58-50ee-4115-9cf9-dbca86538fdd");
            fin_configurationvatexemptionreason vatExemptionReason = (fin_configurationvatexemptionreason)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationvatexemptionreason), vatExemptionReasonGuid);

            //Article:Line1
            Guid articleREDGuid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article articleRED = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), articleREDGuid);
            //Article:Line2
            Guid articleISEGuid = new Guid("78638720-e728-4e96-8643-6d6267ff817b");
            fin_article articleISE = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), articleISEGuid);
            //Article:Line3
            Guid articleINTGuid = new Guid("bf99351b-1556-43c4-a85c-90082fb02d05");
            fin_article articleINT = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), articleINTGuid);
            //Article:Line4
            Guid articleNORGuid = new Guid("6b547918-769e-4f5b-bcd6-01af54846f73");
            fin_article articleNOR = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), articleNORGuid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag
            {
                { articleRED, placeGuid, tableGuid, PriceType.Price1, 1.0m },
                { articleISE, placeGuid, tableGuid, PriceType.Price1, 1.0m, vatExemptionReason },
                { articleINT, placeGuid, tableGuid, PriceType.Price1, 1.0m },
                { articleNOR, placeGuid, tableGuid, PriceType.Price1, 1.0m }
            };

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
                documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        private void buttonPrintInvoiceDiscount_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = InvoiceSettings.InvoiceId;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");

            //Article:Line1
            Guid article1Guid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article article1 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article1Guid);
            //Article:Line2
            Guid article2Guid = new Guid("78638720-e728-4e96-8643-6d6267ff817b");
            fin_article article2 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article2Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag
            {
                { article1, placeGuid, tableGuid, PriceType.Price1, 100.0m },
                { article2, placeGuid, tableGuid, PriceType.Price1, 1.0m }
            };

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        private void buttonPrintInvoiceExchangeRate_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = InvoiceSettings.InvoiceId;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");
            Guid currencyGuid = new Guid("28d692ad-0083-11e4-96ce-00ff2353398c");
            cfg_configurationcurrency currency = (cfg_configurationcurrency)XPOSettings.Session.GetObjectByKey(typeof(cfg_configurationcurrency), currencyGuid);

            //Article:Line1
            Guid article1Guid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article article1 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article1Guid);
            //Article:Line2
            Guid article2Guid = new Guid("78638720-e728-4e96-8643-6d6267ff817b");
            fin_article article2 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article2Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag
            {
                { article1, placeGuid, tableGuid, PriceType.Price1, 100.0m },
                { article2, placeGuid, tableGuid, PriceType.Price1, 1.0m }
            };

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                Currency = currencyGuid,
                ExchangeRate = currency.ExchangeRate
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        private void buttonPrintInvoiceJohnDoe1_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = DocumentSettings.SimplifiedInvoiceId;
            Guid customerGuid = new Guid("d8ce6455-e1a4-41dc-a475-223c00de3a91");//John Doe1

            //Article
            Guid article1Guid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article article1 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag
            {
                { article1, placeGuid, tableGuid, PriceType.Price1, 1.0m }
            };

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        private void buttonPrintInvoiceJohnDoe2_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = DocumentSettings.SimplifiedInvoiceId;
            Guid customerGuid = new Guid("f5a382bb-f826-40d8-8910-cfb18df8a41e");//John Doe2

            //Article
            Guid article1Guid = new Guid("32deb30d-ffa2-45e4-bca6-03569b9e8b08");
            fin_article article1 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag
            {
                { article1, placeGuid, tableGuid, PriceType.Price1, 8.0m }
            };

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        private void buttonPrintTransportationGuideWithTotals_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = new Guid("96bcf534-0dab-48bb-a69e-166e81ae6f7b");
            Guid customerGuid = new Guid("d64c5d26-b4f9-4220-bd3c-72ece5e3960a");

            //Article
            Guid article1Guid = new Guid("55892c3f-de10-4076-afde-619c54100c9b");
            fin_article article1 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag
            {
                { article1, placeGuid, tableGuid, PriceType.Price1, 24.0m }
            };

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        private void buttonPrintTransportationGuideWithoutTotals_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = new Guid("96bcf534-0dab-48bb-a69e-166e81ae6f7b");
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");

            //Article
            Guid article1Guid = new Guid("55892c3f-de10-4076-afde-619c54100c9b");
            fin_article article1 = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag
            {
                { article1, placeGuid, tableGuid, PriceType.Price1, 48.0m }
            };

            //Prepare ProcessFinanceDocumentParameter
            DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                ExchangeRate = 0.0m
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }
    }
}
