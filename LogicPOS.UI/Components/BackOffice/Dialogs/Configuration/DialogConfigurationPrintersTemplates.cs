﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPrintersTemplates : BOBaseDialog
    {
        public DialogConfigurationPrintersTemplates(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_dialogconfigurationprinterstype"));

            SetSizeRequest(500, 500);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //FileTemplate
                FileChooserButton fileChooserFileTemplate = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
                Image fileChooserImagePreviewFileTemplate = new Image() { WidthRequest = _sizefileChooserPreview.Width, HeightRequest = _sizefileChooserPreview.Height };
                Frame fileChooserFrameImagePreviewFileTemplate = new Frame();
                fileChooserFrameImagePreviewFileTemplate.ShadowType = ShadowType.None;
                fileChooserFrameImagePreviewFileTemplate.Add(fileChooserImagePreviewFileTemplate);
                fileChooserFileTemplate.SetFilename(((sys_configurationprinterstemplates)DataSourceRow).FileTemplate);
                fileChooserFileTemplate.Filter = logicpos.Utils.GetFileFilterTemplates();
                fileChooserFileTemplate.SelectionChanged += (sender, eventArgs) => fileChooserImagePreviewFileTemplate.Pixbuf = logicpos.Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(fileChooserImagePreviewFileTemplate.WidthRequest, fileChooserImagePreviewFileTemplate.HeightRequest));
                BOWidgetBox boxfileChooserFileTemplate = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_file"), fileChooserFileTemplate);
                HBox hboxfileChooserAndimagePreviewFileTemplate = new HBox(false, _boxSpacing);
                hboxfileChooserAndimagePreviewFileTemplate.PackStart(boxfileChooserFileTemplate, true, true, 0);
                hboxfileChooserAndimagePreviewFileTemplate.PackStart(fileChooserFrameImagePreviewFileTemplate, false, false, 0);
                vboxTab1.PackStart(hboxfileChooserAndimagePreviewFileTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxfileChooserFileTemplate, _dataSourceRow, "FileTemplate", string.Empty, false));

                //FinancialTemplate
                CheckButton checkButtonFinancialTemplate = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_financialtemplate"));
                vboxTab1.PackStart(checkButtonFinancialTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonFinancialTemplate, _dataSourceRow, "FinancialTemplate"));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}


