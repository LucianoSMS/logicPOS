﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Configuration;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPrinters : BOBaseDialog
    {
        private VBox _vboxTab2;
        private XPOComboBox _xpoComboBoxPrinterType;
        private BOWidgetBox _xpoComboBoxPrinterSelect;
        private Entry entryDesignation;
        private ComboBox xpoComboBoxInputType;
        private Entry _entryThermalEncoding;
        private Entry _entryThermalImageCompanyLogo;
        private Entry _entryThermalMaxCharsPerLineNormal;
        private Entry _entryThermalMaxCharsPerLineNormalBold;
        private Entry _entryThermalMaxCharsPerLineSmall;
        private Entry _entryThermalCutCommand;
        private Entry _entryThermalOpenDrawerValueM;
        private Entry _entryThermalOpenDrawerValueT1;
        private Entry _entryThermalOpenDrawerValueT2;


        private sys_configurationprinters _configurationPrinter;

        public DialogConfigurationPrinters(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(GeneralUtils.GetResourceByName("window_title_edit_dialogconfigurationprinters"));

            SetSizeRequest(500, 448);
            InitUI();
            InitNotes();
            ShowAll();
            //Tab Visibility Require to be after ShowAll, else always is Visible
            _configurationPrinter = (_dataSourceRow as sys_configurationprinters);
            _vboxTab2.Visible = (_configurationPrinter.PrinterType != null && _configurationPrinter.PrinterType.ThermalPrinter);
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                // HBoxs
                HBox hbox1 = new HBox(true, _boxSpacing);
                HBox hbox2 = new HBox(true, _boxSpacing);

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //PrinterType
                _xpoComboBoxPrinterType = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstype), (DataSourceRow as sys_configurationprinters).PrinterType, "Designation", null);
                BOWidgetBox boxPrinterType = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_type"), _xpoComboBoxPrinterType);
                vboxTab1.PackStart(boxPrinterType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinterType, DataSourceRow, "PrinterType", LogicPOS.Utility.RegexUtils.RegexGuid, true));


                //Configurar impressora windows com ESC-POS - LINDOTE
                //Configuração Impressoras Windows TK016310
                //verifica tipo de dialog conforme o tipo de impressora
                _configurationPrinter = (_dataSourceRow as sys_configurationprinters);

                //Verifica tipo de impressora ativo na drop
                string getSelectedType = _xpoComboBoxPrinterType.ActiveText.Substring(0, 1);
                //Preenche o conjunto de strings associado ás diferentes impressoras instaladas no sistema
                var _printersOnSystem = ComboBoxPrinterSelect();
                if (_printersOnSystem.Length != 0 /*&& getSelectedType != "-"*/)
                {
                    //Designação para Windows será a escolha da impressora instalada no sistema
                    entryDesignation = new Entry();

                    //Configuração da selectbox
                    TreeIter iter;
                    TreeStore store = new TreeStore(typeof(string), typeof(string));
                    for (int i = 0; i < System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count; i++)
                    {
                        iter = store.AppendValues(System.Drawing.Printing.PrinterSettings.InstalledPrinters[i].ToString(), System.Drawing.Printing.PrinterSettings.InstalledPrinters[i]);
                    }
                    //Iniciação da comboBox
                    xpoComboBoxInputType = new ComboBox(_printersOnSystem);
                    bool flagSelected = false;

                    //Escolha do valor por defeito
                    xpoComboBoxInputType.Model.GetIterFirst(out iter);
                    int cbox = 0;
                    do
                    {
                        GLib.Value thisRow = new GLib.Value();
                        xpoComboBoxInputType.Model.GetValue(iter, 0, ref thisRow);
                        if ((_dataSourceRow as sys_configurationprinters).Designation != null)
                        {
                            if (_printersOnSystem[cbox] == (_dataSourceRow as sys_configurationprinters).Designation)
                            {
                                xpoComboBoxInputType.SetActiveIter(iter);
                                flagSelected = true;
                                break;
                            }
                        }
                        cbox++;
                    } while (xpoComboBoxInputType.Model.IterNext(ref iter));

                    //Se existir impressora selecionada associa á sua designação
                    if (flagSelected)
                    {
                        entryDesignation.Text = _printersOnSystem[xpoComboBoxInputType.Active];
                    }
                    //Label escondida / Já existe a dropdown list que mostra a escolha da seleção
                    entryDesignation.Visibility = false;

                    //Junção da Drop na WidgetBox do POS
                    _xpoComboBoxPrinterSelect = new BOWidgetBox(GeneralUtils.GetResourceByName("global_designation"), xpoComboBoxInputType);

                    //Mostra Drop List
                    vboxTab1.PackStart(_xpoComboBoxPrinterSelect, false, false, 0);

                    //Verifica designação válida
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                }
                //Se for Linux usa a Designação por defeito, escrita á mão
                else
                {
                    //Designation
                    Entry entryDesignation = new Entry();
                    BOWidgetBox boxDesignation = new BOWidgetBox(GeneralUtils.GetResourceByName("global_designation"), entryDesignation);
                    vboxTab1.PackStart(boxDesignation, false, false, 0);
                    _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));
                }

                //NetworkName
                Entry entryNetworkName = new Entry();
                BOWidgetBox boxNetworkName = new BOWidgetBox(GeneralUtils.GetResourceByName("global_networkname"), entryNetworkName);
                vboxTab1.PackStart(boxNetworkName, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxNetworkName, _dataSourceRow, "NetworkName", "", false));

                //Quando a combobox da drop altera
                if (xpoComboBoxInputType != null)
                {
                    xpoComboBoxInputType.Changed += delegate
                    {
                        try
                        {
                            //Se a Seleção for válida, associa á designação
                            entryDesignation.Text = _printersOnSystem[xpoComboBoxInputType.Active];
                            //Se for impressora de rede associa logo o caminho da rede á impressora associada
                            if (_xpoComboBoxPrinterType.Active == 3)
                            {
                                entryNetworkName.Text = _printersOnSystem[xpoComboBoxInputType.Active];
                            }
                            else
                            {
                                entryNetworkName.Text = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message, ex);
                        }

                    };
                }


                //Tab2
                _vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //ThermalMaxCharsPerLineNormal
                _entryThermalMaxCharsPerLineNormal = new Entry();
                BOWidgetBox boxThermalMaxCharsPerLineNormal = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_max_chars_per_line_normal"), _entryThermalMaxCharsPerLineNormal);
                _vboxTab2.PackStart(boxThermalMaxCharsPerLineNormal, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalMaxCharsPerLineNormal, _dataSourceRow, "ThermalMaxCharsPerLineNormal", LogicPOS.Utility.RegexUtils.RegexInteger, true));

                //ThermalMaxCharsPerLineNormalBold
                _entryThermalMaxCharsPerLineNormalBold = new Entry();
                BOWidgetBox boxThermalMaxCharsPerLineNormalBold = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_max_chars_per_line_normal_bold"), _entryThermalMaxCharsPerLineNormalBold);
                _vboxTab2.PackStart(boxThermalMaxCharsPerLineNormalBold, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalMaxCharsPerLineNormalBold, _dataSourceRow, "ThermalMaxCharsPerLineNormalBold", LogicPOS.Utility.RegexUtils.RegexInteger, true));

                //ThermalMaxCharsPerLineSmall
                _entryThermalMaxCharsPerLineSmall = new Entry();
                BOWidgetBox boxThermalMaxCharsPerLineSmall = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_max_chars_per_line_small"), _entryThermalMaxCharsPerLineSmall);
                _vboxTab2.PackStart(boxThermalMaxCharsPerLineSmall, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalMaxCharsPerLineSmall, _dataSourceRow, "ThermalMaxCharsPerLineSmall", LogicPOS.Utility.RegexUtils.RegexInteger, true));

                //ThermalEncoding
                _entryThermalEncoding = new Entry();
                BOWidgetBox boxThermalEncoding = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_encoding"), _entryThermalEncoding);
                //_vboxTab2.PackStart(boxThermalEncoding, false, false, 0);
                hbox1.PackStart(boxThermalEncoding, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalEncoding, _dataSourceRow, "ThermalEncoding", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //ThermalCutCommand
                _entryThermalCutCommand = new Entry();
                BOWidgetBox boxThermalCutCommand = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_cut_command"), _entryThermalCutCommand);
                //_vboxTab2.PackStart(boxThermalCutCommand, false, false, 0);
                hbox1.PackStart(boxThermalCutCommand, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalCutCommand, _dataSourceRow, "ThermalCutCommand", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                // Pack hbox
                _vboxTab2.PackStart(hbox1, false, false, 0);

                //ThermalOpenDrawerValueM
                _entryThermalOpenDrawerValueM = new Entry();
                BOWidgetBox boxThermalOpenDrawerValueM = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_open_drawer_value_m"), _entryThermalOpenDrawerValueM);
                //_vboxTab2.PackStart(boxThermalOpenDrawerValueM, false, false, 0);
                hbox2.PackStart(boxThermalOpenDrawerValueM, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalOpenDrawerValueM, _dataSourceRow, "ThermalOpenDrawerValueM", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                //ThermalOpenDrawerValueT1
                _entryThermalOpenDrawerValueT1 = new Entry();
                BOWidgetBox boxThermalOpenDrawerValueT1 = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_open_drawer_value_t1"), _entryThermalOpenDrawerValueT1);
                //_vboxTab2.PackStart(boxThermalOpenDrawerValueT1, false, false, 0);
                hbox2.PackStart(boxThermalOpenDrawerValueT1, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalOpenDrawerValueT1, _dataSourceRow, "ThermalOpenDrawerValueT1", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                //ThermalOpenDrawerValueT2
                _entryThermalOpenDrawerValueT2 = new Entry();
                BOWidgetBox boxThermalOpenDrawerValueT2 = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_open_drawer_value_t2"), _entryThermalOpenDrawerValueT2);
                //_vboxTab2.PackStart(boxThermalOpenDrawerValueT2, false, false, 0);
                hbox2.PackStart(boxThermalOpenDrawerValueT2, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalOpenDrawerValueT2, _dataSourceRow, "ThermalOpenDrawerValueT2", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                // Pack hbox
                _vboxTab2.PackStart(hbox2, false, false, 0);

                //ThermalPrintLogo
                _entryThermalImageCompanyLogo = new Entry();
                BOWidgetBox boxThermalImageCompanyLogo = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_image_company_loggero"), _entryThermalImageCompanyLogo);
                _vboxTab2.PackStart(boxThermalImageCompanyLogo, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalImageCompanyLogo, _dataSourceRow, "ThermalImageCompanyLogo", LogicPOS.Utility.RegexUtils.RegexAlfaNumericFilePath, false));

                //ThermalPrintLogo
                CheckButton checkButtonThermalPrintLogo = new CheckButton(GeneralUtils.GetResourceByName("global_printer_thermal_print_loggero"));
                _vboxTab2.PackStart(checkButtonThermalPrintLogo, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonThermalPrintLogo, _dataSourceRow, "ThermalPrintLogo"));

                // Events
                _xpoComboBoxPrinterType.Changed += XpoComboBoxPrinterType_Changed;

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(GeneralUtils.GetResourceByName("global_record_main_detail")));
                _notebook.AppendPage(_vboxTab2, new Label(GeneralUtils.GetResourceByName("global_record_main_properties")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public static void SaveSettings(string fieldName)
        {
            try
            {

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["customCultureResourceDefinition"].Value = fieldName;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

        }

        //Preenche o conjunto de strings associado ás diferentes impressoras instaladas no sistema
        private string[] ComboBoxPrinterSelect()
        {
            int countPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count;
            string[] _printersOnSystem = new string[countPrinters];
            int i = 0;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                _printersOnSystem[i] = printer;
                i++;
            }
            return _printersOnSystem;
        }


        private void XpoComboBoxPrinterType_Changed(object sender, System.EventArgs e)
        {
            try
            {
                _logger.Debug(string.Format("Message: [{0}]", _xpoComboBoxPrinterType.Value));

                //Tab Visibility
                _vboxTab2.Visible = (_xpoComboBoxPrinterType.Value != null && (_xpoComboBoxPrinterType.Value as sys_configurationprinterstype).ThermalPrinter);

                // If Working in ThermalPrinter assign Defaults From Config
                if (_vboxTab2.Visible)
                {
                    if (string.IsNullOrEmpty(_entryThermalEncoding.Text)) _entryThermalEncoding.Text = PrintingSettings.ThermalPrinter.Encoding;
                    if (string.IsNullOrEmpty(_entryThermalImageCompanyLogo.Text)) _entryThermalImageCompanyLogo.Text = PrintingSettings.ThermalPrinter.CompanyLogoLocation;
                    if (_entryThermalMaxCharsPerLineNormal.Text.Equals("0")) _entryThermalMaxCharsPerLineNormal.Text = Convert.ToString(PrintingSettings.ThermalPrinter.MaxCharsPerLineNormal);
                    if (_entryThermalMaxCharsPerLineNormalBold.Text.Equals("0")) _entryThermalMaxCharsPerLineNormalBold.Text = Convert.ToString(PrintingSettings.ThermalPrinter.MaxCharsPerLineNormalBold);
                    if (_entryThermalMaxCharsPerLineSmall.Text.Equals("0")) _entryThermalMaxCharsPerLineSmall.Text = Convert.ToString(PrintingSettings.ThermalPrinter.MaxCharsPerLineSmall);
                    if (string.IsNullOrEmpty(_entryThermalCutCommand.Text)) _entryThermalCutCommand.Text = PrintingSettings.ThermalPrinter.CutCommand;
                    if (_entryThermalOpenDrawerValueM.Text.Equals("0")) _entryThermalOpenDrawerValueM.Text = Convert.ToString(PrintingSettings.ThermalPrinter.OpenDrawerValueM);
                    if (_entryThermalOpenDrawerValueT1.Text.Equals("0")) _entryThermalOpenDrawerValueT1.Text = Convert.ToString(PrintingSettings.ThermalPrinter.OpenDrawerValueT1);
                    if (_entryThermalOpenDrawerValueT2.Text.Equals("0")) _entryThermalOpenDrawerValueT2.Text = Convert.ToString(PrintingSettings.ThermalPrinter.OpenDrawerValueT2);
                }
                // If Not Working in ThermalPrinter reset Values
                else
                {
                    _entryThermalEncoding.Text = null;
                    _entryThermalImageCompanyLogo.Text = null;
                    _entryThermalMaxCharsPerLineNormal.Text = null;
                    _entryThermalMaxCharsPerLineNormalBold.Text = null;
                    _entryThermalMaxCharsPerLineSmall.Text = null;
                    _entryThermalCutCommand.Text = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
