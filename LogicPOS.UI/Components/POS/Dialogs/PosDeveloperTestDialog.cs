﻿using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosDeveloperTestDialog : PosBaseDialog
    {
        //private Fixed _fixedContent;
        private readonly ScrolledWindow _scrolledWindow;
        private readonly VBox _vbox;
        private readonly uint _padding = 0;
        private EntryBoxValidation _entryBoxValidationCustomButton1;
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _xPOEntryBoxSelectRecordValidationTextMode;

        public PosDeveloperTestDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_template");
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_default.png";
            _windowSize = new Size(595, 740);

            //Init VBox
            _vbox = new VBox(false, 2) { WidthRequest = _windowSize.Width - 44 };

            //Call InitUI
            InitUI1();
            //InitUI_FilePicker();
            //InitUI_LittleAdds();
            //InitUI2();
            //InitUI3();

            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.ResizeMode = ResizeMode.Parent;
            viewport.Add(_vbox);

            _scrolledWindow = new ScrolledWindow();
            _scrolledWindow.ShadowType = ShadowType.EtchedIn;
            _scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
            _scrolledWindow.ResizeMode = ResizeMode.Parent;
            _scrolledWindow.Add(viewport);

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, _scrolledWindow, actionAreaButtons);
        }

        private void InitUI1()
        {
            //EntryBoxValidation with KeyBoard Input
            EntryBoxValidation entryBoxValidation = new EntryBoxValidation(this, "EntryBoxValidation", KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true);
            entryBoxValidation.EntryValidation.Sensitive = false;
            entryBoxValidation.ButtonKeyBoard.Sensitive = false;
            _vbox.PackStart(entryBoxValidation, true, true, _padding);

            //EntryBoxValidation with KeyBoard Input and Custom Buttons : Start without KeyBoard, and KeyBoard Button After all Others
            _entryBoxValidationCustomButton1 = new EntryBoxValidation(this, "EntryBoxValidationCustomButton", KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            TouchButtonIcon customButton1 = _entryBoxValidationCustomButton1.AddButton("CustomButton1", @"Icons/Windows/icon_window_orders.png");
            TouchButtonIcon customButton2 = _entryBoxValidationCustomButton1.AddButton("CustomButton2", @"Icons/Windows/icon_window_pay_invoice.png");
            TouchButtonIcon customButton3 = _entryBoxValidationCustomButton1.AddButton("CustomButton3", @"Icons/Windows/icon_window_orders.png");
            //Now we manually Init Keyboard
            _entryBoxValidationCustomButton1.EntryValidation.KeyboardMode = KeyboardMode.AlfaNumeric;
            _entryBoxValidationCustomButton1.InitKeyboard(_entryBoxValidationCustomButton1.EntryValidation);
            //Test Required Rule
            customButton1.Clicked += customButton1_Clicked;
            customButton2.Clicked += customButton2_Clicked;
            customButton3.Clicked += customSharedButton_Clicked;
            _vbox.PackStart(_entryBoxValidationCustomButton1, true, true, _padding);

            //EntryBoxValidationButton
            EntryBoxValidationButton entryBoxValidationButton = new EntryBoxValidationButton(this, "EntryBoxValidationButton", KeyboardMode.AlfaNumeric, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true);
            entryBoxValidationButton.Button.Clicked += customSharedButton_Clicked;
            _vbox.PackStart(entryBoxValidationButton, true, true, _padding);

            //Test XPOEntryBoxSelectRecordValidation without KeyBoard Input
            fin_documentfinancetype defaultValueDocumentFinanceType = XPOUtility.GetEntityById<fin_documentfinancetype>(InvoiceSettings.InvoiceId);
            CriteriaOperator criteriaOperatorDocumentFinanceType = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            XPOEntryBoxSelectRecordValidation<fin_documentfinancetype, TreeViewDocumentFinanceType> entryBoxSelectDocumentFinanceType = new XPOEntryBoxSelectRecordValidation<fin_documentfinancetype, TreeViewDocumentFinanceType>(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinanceseries_documenttype"), "Designation", "Oid", defaultValueDocumentFinanceType, criteriaOperatorDocumentFinanceType, LogicPOS.Utility.RegexUtils.RegexGuid, true);
            //entryBoxSelectDocumentFinanceType.EntryValidation.IsEditable = false;
            entryBoxSelectDocumentFinanceType.ClosePopup += delegate { };
            _vbox.PackStart(entryBoxSelectDocumentFinanceType, true, true, _padding);

            //Test XPOEntryBoxSelectRecordValidation with KeyBoard Input
            CriteriaOperator criteriaOperatorXPOEntryBoxSelectRecordValidationTextMode = null;
            _xPOEntryBoxSelectRecordValidationTextMode = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, "XPOEntryBoxSelectRecordValidationTextMode", "Name", "Name", null, criteriaOperatorXPOEntryBoxSelectRecordValidationTextMode, KeyboardMode.AlfaNumeric, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            //_xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Sensitive = false;
            //Start Disabled
            //_xPOEntryBoxSelectRecordValidationTextMode.ButtonKeyBoard.Sensitive = false;
            _xPOEntryBoxSelectRecordValidationTextMode.ClosePopup += delegate { };
            _vbox.PackStart(_xPOEntryBoxSelectRecordValidationTextMode, true, true, _padding);

            //Test XPOEntryBoxSelectRecordValidation without KeyBoard Input / Guid
            CriteriaOperator criteriaOperatorXPOEntryBoxSelectRecordValidationGuidMode = null;
            XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> xPOEntryBoxSelectRecordValidationGuidMode = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, "XPOEntryBoxSelectRecordValidationGuidMode", "Name", "Oid", null, criteriaOperatorXPOEntryBoxSelectRecordValidationGuidMode, KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexGuid, true);
            _xPOEntryBoxSelectRecordValidationTextMode.ClosePopup += delegate { };
            _vbox.PackStart(xPOEntryBoxSelectRecordValidationGuidMode, true, true, _padding);

            //Test DateTime Picker
            DateTime initalDateTime = DateTime.Now;
            EntryBoxValidationDatePickerDialog entryBoxShipToDeliveryDate = new EntryBoxValidationDatePickerDialog(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_to_delivery_date"), "dateFormat", DateTime.Now, LogicPOS.Utility.RegexUtils.RegexDate, true, CultureSettings.DateFormat);
            //entryBoxShipToDeliveryDate.EntryValidation.Sensitive = true;
            entryBoxShipToDeliveryDate.EntryValidation.Text = initalDateTime.ToString(CultureSettings.DateFormat);

            //entryBoxShipToDeliveryDate.EntryValidation.Validate();
            //entryBoxShipToDeliveryDate.ClosePopup += delegate { };
            _vbox.PackStart(entryBoxShipToDeliveryDate, true, true, _padding);

            //Test DateTime Picker with KeyBoard
            EntryBoxValidationDatePickerDialog entryBoxShipToDeliveryDateKeyboard = new EntryBoxValidationDatePickerDialog(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_ship_to_delivery_date"), CultureSettings.DateTimeFormat, DateTime.Now, KeyboardMode.AlfaNumeric, LogicPOS.Utility.RegexUtils.RegexDateTime, true, CultureSettings.DateTimeFormat);
            entryBoxShipToDeliveryDateKeyboard.EntryValidation.Sensitive = false;
            entryBoxShipToDeliveryDateKeyboard.ButtonKeyBoard.Sensitive = false;
            //entryBoxShipToDeliveryDate.EntryValidation.Sensitive = true;
            entryBoxShipToDeliveryDateKeyboard.EntryValidation.Text = initalDateTime.ToString(CultureSettings.DateTimeFormat);
            _vbox.PackStart(entryBoxShipToDeliveryDateKeyboard, true, true, _padding);

            //Simple ListView
            List<string> itemList = new List<string>
            {
                "Looking for Kiosk mode in Android Lollipop 5.0",
                "Think of a hypothetical ATM machine that is running Android",
                "In this article we provide a brief overview of how",
                "Kiosk Mode can be implemented without any modifications",
                "The Home key brings you back to the Home screen"
            };

            //ListComboBox
            ListComboBox listComboBox = new ListComboBox(itemList, itemList[3]);
            _vbox.PackStart(listComboBox, true, true, _padding);

            //ListComboBoxTouch
            ListComboBoxTouch listComboBoxTouch = new ListComboBoxTouch(this, "ListComboBoxTouch (Todo: Highlight Validation in Component)", itemList, itemList[4]);
            _vbox.PackStart(listComboBoxTouch, true, true, _padding);

            //EntryMultiline entryTouchMultiline = new EntryMultiline(this, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true, 100, 10);
            //vbox.PackStart(entryTouchMultiline, true, true, padding);
            EntryBoxValidationMultiLine entryBoxMultiLine = new EntryBoxValidationMultiLine(this, "EntryBoxMultiLine", KeyboardMode.AlfaNumeric, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true, 18, 6) { HeightRequest = 200 };

            //Start Disabled
            entryBoxMultiLine.EntryMultiline.Sensitive = false;
            entryBoxMultiLine.ButtonKeyBoard.Sensitive = false;
            _vbox.PackStart(entryBoxMultiLine, true, true, _padding);


            /*
            ListRadioButtonTouch listRadioButtonTouch = new ListRadioButtonTouch(this, "Label", itemList, itemList[4]);
            _fixedContent.Put(listRadioButtonTouch, 100, 320);

            string initialShipFromDeliveryDate = FrameworkUtils.CurrentDateTimeAtomic().ToString(SettingsApp.DateFormat);
            //EntryBoxValidationButton entryBoxDate = new EntryBoxValidationButton(this, CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_from_delivery_date, KeyboardModes.Alfa, regexDate, false);
            //entryBoxDate.EntryValidation.Text = initialShipFromDeliveryDate;
            //entryBoxDate.EntryValidation.Validate();

            EntryBoxValidationDatePickerDialog entryBoxDate = new EntryBoxValidationDatePickerDialog(this, CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ship_from_delivery_date, SettingsApp.RegexDate, false);
            entryBoxDate.EntryValidation.Text = initialShipFromDeliveryDate;
            entryBoxDate.EntryValidation.Validate();
            entryBoxDate.ClosePopup += delegate
            {
                _logger.Debug(string.Format("entryBoxDate.Value: [{0}]", entryBoxDate.Value));
            };
            vbox.PackStart(entryBoxDate, true, true, padding);
            */
        }

        private void customButton1_Clicked(object sender, EventArgs e)
        {
            _entryBoxValidationCustomButton1.EntryValidation.Required = true;
            _entryBoxValidationCustomButton1.EntryValidation.Validate();

            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Required = true;
            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Validate();
            _logger.Debug(string.Format("Validated: [{0}]", _entryBoxValidationCustomButton1.EntryValidation.Validated));
        }

        private void customButton2_Clicked(object sender, EventArgs e)
        {
            _entryBoxValidationCustomButton1.EntryValidation.Required = false;
            _entryBoxValidationCustomButton1.EntryValidation.Validate();

            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Required = false;
            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Validate();
            _logger.Debug(string.Format("Validated: [{0}]", _entryBoxValidationCustomButton1.EntryValidation.Validated));
        }

        private void buttonTestDocumentMasterCreatePDF_Clicked(object sender, EventArgs e)
        {
            Guid guidOid = new Guid("099EF525-FCEC-48D8-9EE8-FA0F34A34ED4");
            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), guidOid);
            string fileName = LogicPOS.Reporting.Common.FastReport.DocumentMasterCreatePDF(documentFinanceMaster);
            _logger.Debug(string.Format("fileName: [{0}]", fileName));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void customSharedButton_Clicked(object sender, EventArgs e)
        {
            _logger.Debug("customSharedButton_Clicked");
        }
    }
}
