﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Utility;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPlaceMovementType : BOBaseDialog
    {
        public DialogConfigurationPlaceMovementType(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            /* IN009039 */

            SetSizeRequest(500, 353);
            this.Title = logicpos.Utils.GetWindowTitle(GeneralUtils.GetResourceByName("window_title_edit_configurationplacemovementtype"), pDialogMode);
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
                BOWidgetBox boxLabel = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(GeneralUtils.GetResourceByName("global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", RegexUtils.RegexAlfaNumericExtended, true));

                //VatDirectSelling
                //In POS Mode add VatDirectSelling
                if (AppOperationModeSettings.IsDefaultTheme) /* IN008024 */
                {
                    CheckButton checkButtonVatDirectSelling = new CheckButton(GeneralUtils.GetResourceByName("global_vat_direct_selling"));
                    vboxTab1.PackStart(checkButtonVatDirectSelling, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonVatDirectSelling, _dataSourceRow, "VatDirectSelling"));
                }

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(GeneralUtils.GetResourceByName("global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
