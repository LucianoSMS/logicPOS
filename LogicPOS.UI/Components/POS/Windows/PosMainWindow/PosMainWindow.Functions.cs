﻿using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Hardware;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Printing.Utility;
using LogicPOS.Settings;
using LogicPOS.Settings.Extensions;
using LogicPOS.Shared;
using LogicPOS.Shared.Orders;
using LogicPOS.Utility;
using System;

namespace logicpos
{
    public partial class PosMainWindow
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Guid pTableOid = Guid.Empty;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Window

        private void PosMainWindow_WindowStateEvent(object o, WindowStateEventArgs args)
        {
            // Activate Pos Window, ex Activated From BackOffice
            if (args.Event.NewWindowState == Gdk.WindowState.Fullscreen)
            {
                //Always Reset tablePadFamily before Refresh, to Prevent Keep old selected when we delete articles and create a new Familiy 
                //This way we always have the first family when we come from backoffice
                TablePadFamily.SelectedButtonOid = Guid.Empty;
                TablePadFamily.Refresh();
                //Always Apply Filter to prevent error when work in a clean database, and we create first Records
                //Filter is "  AND (Family = '00000000-0000-0000-0000-000000000000')"
                TablePadSubFamily.Filter = string.Format("  AND (Family = '{0}')", TablePadFamily.SelectedButtonOid);
                TablePadSubFamily.Refresh();
                TablePadArticle.Refresh();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: KeyRelease

        private void PosMainWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //Redirect Event to BarCodeReader.KeyReleaseEvent
            if (GlobalApp.BarCodeReader != null)
            {
                GlobalApp.BarCodeReader.KeyReleaseEvent(this, o, args);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarApplicationClose

        private void touchButtonPosToolbarApplicationClose_Clicked(object sender, EventArgs e)
        {
            LogicPOSApp.Quit(this);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarBackOffice

        private void touchButtonPosToolbarBackOffice_Clicked(object sender, EventArgs e)
        {
            Utils.ShowBackOffice(this);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarReports

        // Deprecated
        private void touchButtonPosToolbarReports_Clicked(object sender, EventArgs e)
        {
            //Temporary Disable
            //Utils.ShowReportsWinForms(this);

            //Old GTK Report Window, Not Used Anymore
            //Utils.ShowReports(this);

            //PosSystemDialog dialog = new PosSystemDialog(this, Gtk.DialogFlags.DestroyWithParent);
            //int response = dialog.Run();
            //dialog.Destroy();

            try
            {
                PosReportsDialog dialog = new PosReportsDialog(this, DialogFlags.DestroyWithParent);
                int response = dialog.Run();
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            //Test Report With DocumentFinanceMaster Documents
            //Guid documentFinanceMasterOid = new Guid("'73ca9f06-dda9-44a6-a4d9-e7d9d21b042e'");
            //CustomReport.ProcessReportFinanceDocument(CustomReportDisplayMode.Design, "aQpO", 4, documentFinanceMasterOid);

            //Test Report With DocumentFinancePayment Documents
            //Guid documentFinancePaymentOid = new Guid("88f082ce-da52-48b5-a31d-32bfa87f119d");
            //CustomReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.Design, 4, documentFinancePaymentOid);

            //Test Printer
            //ConfigurationPrintersTemplates configurationPrintersTemplates = (ConfigurationPrintersTemplates)XPOUtility.GetXPGuidObjectFromSession(typeof(ConfigurationPrintersTemplates), new Guid("5409255A-3741-411C-B05B-056CBD470226"));
            //DocumentFinancePayment documentFinancePayment = (DocumentFinancePayment)XPOUtility.GetXPGuidObjectFromSession(typeof(DocumentFinancePayment), new Guid("88F082CE-DA52-48B5-A31D-32BFA87F119D"));
            //FrameworkCalls.PrintFinanceDocumentPayment(this, TerminalSettings.LoggedTerminal.Printer, configurationPrintersTemplates, documentFinancePayment);

            //Test WorkSession
            //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarLogoutUser

        private void touchButtonPosToolbarLogoutUser_Clicked(object sender, EventArgs e)
        {
            Hide();
            //Call Shared WindowStartup LogOutUser, and Show WindowStartup
            GlobalApp.StartupWindow.LogOutUser(true);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarShowSystemDialog

        // System Dialog
        private void touchButtonPosToolbarShowSystemDialog_Clicked(object sender, EventArgs e)
        {
            PosSystemDialog dialog = new PosSystemDialog(this, DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarCashDrawer

        private void touchButtonPosToolbarCashDrawer_Clicked(object sender, EventArgs e)
        {
            ShowCashDialog();
        }

        private void ShowCashDialog()
        {
            PosCashDialog dialog = new PosCashDialog(this, DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarNewFinanceDocument

        private void touchButtonPosToolbarNewFinanceDocument_Clicked(object sender, EventArgs e)
        {
            //Call New DocumentFinance Dialog
            PosDocumentFinanceDialog dialogNewDocument = new PosDocumentFinanceDialog(this, DialogFlags.DestroyWithParent);
            ResponseType responseNewDocument = (ResponseType)dialogNewDocument.Run();
            if (responseNewDocument == ResponseType.Ok)
            {
            }
            dialogNewDocument.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarFinanceDocuments

        private void touchButtonPosToolbarFinanceDocuments_Clicked(object sender, EventArgs e)
        {
            PosDocumentFinanceSelectRecordDialog dialog = new PosDocumentFinanceSelectRecordDialog(this, DialogFlags.DestroyWithParent, 0);
            ResponseType response = (ResponseType)dialog.Run();
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: touchButtonPosToolbarShowChangeUser

        private void touchButtonPosToolbarShowChangeUserDialog_Clicked(object sender, EventArgs e)
        {
            PosChangeUserDialog dialogChangeUser = new PosChangeUserDialog(this, DialogFlags.DestroyWithParent);

            try
            {
                string terminalInfo = string.Empty;

                int responseChangeUser = dialogChangeUser.Run();
                if (responseChangeUser == (int)ResponseType.Ok)
                {
                    //Already logged
                    if (POSSession.CurrentSession.LoggedUsers.ContainsKey(dialogChangeUser.UserDetail.Oid))
                    {
                        XPOSettings.LoggedUser = XPOUtility.GetEntityById<sys_userdetail>(dialogChangeUser.UserDetail.Oid);
                        GeneralSettings.LoggedUserPermissions = XPOUtility.GetUserPermissions();
                        TicketList.UpdateTicketListButtons();
                        XPOUtility.Audit("USER_CHANGE", string.Format(GeneralUtils.GetResourceByName("audit_message_user_change"), XPOSettings.LoggedUser.Name));
                        terminalInfo = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);
                        if (LabelTerminalInfo.Text != terminalInfo) LabelTerminalInfo.Text = terminalInfo;
                    }
                    //Not Logged, Request Pin Login
                    else
                    {
                        PosPinPadDialog dialogPinPad = new PosPinPadDialog(dialogChangeUser, DialogFlags.DestroyWithParent, dialogChangeUser.UserDetail);
                        int responsePinPad = dialogPinPad.Run();
                        if (responsePinPad == (int)ResponseType.Ok)
                        {
                            if (!POSSession.CurrentSession.LoggedUsers.ContainsKey(dialogChangeUser.UserDetail.Oid))
                            {
                                POSSession.CurrentSession.LoggedUsers.Add(dialogChangeUser.UserDetail.Oid, XPOUtility.CurrentDateTimeAtomic());
                                POSSession.CurrentSession.Save();
                                XPOSettings.LoggedUser = XPOUtility.GetEntityById<sys_userdetail>(dialogChangeUser.UserDetail.Oid);
                                GeneralSettings.LoggedUserPermissions = XPOUtility.GetUserPermissions();
                                TicketList.UpdateTicketListButtons();
                                XPOUtility.Audit("USER_loggerIN", string.Format(GeneralUtils.GetResourceByName("audit_message_user_loggerin"), XPOSettings.LoggedUser.Name));
                                terminalInfo = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);
                                if (LabelTerminalInfo.Text != terminalInfo) LabelTerminalInfo.Text = terminalInfo;
                                //After First time Login ShowNotifications
                                Utils.ShowNotifications(dialogPinPad);
                            }
                        };

                        dialogPinPad.Destroy();
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                dialogChangeUser.Destroy();
            }

        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Favorites Button

        private void buttonFavorites_Clicked(object sender, EventArgs e)
        {
            TablePadArticle.Filter = " AND (Favorite = 1)";
            //Enable Buttons, else when we have only a Family or Subfamily, the buttons will never be enabled
            if (TablePadFamily.SelectedButton != null && !TablePadFamily.SelectedButton.Sensitive) TablePadFamily.SelectedButton.Sensitive = true;
            if (TablePadSubFamily.SelectedButton != null && !TablePadSubFamily.SelectedButton.Sensitive) TablePadSubFamily.SelectedButton.Sensitive = true;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Families Button Clicks

        private void _tablePadFamily_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            TablePadFamily.SelectedButtonOid = button.CurrentButtonOid;
            //SubFamily Filter
            TablePadSubFamily.Filter = string.Format(" AND (Family = '{0}')", button.CurrentButtonOid);

            //IN009277
            string getFirstSubFamily = "0";
            if (DatabaseSettings.DatabaseType.IsMySql() || DatabaseSettings.DatabaseType.IsSQLite())
            {
                string mysql = string.Format("SELECT Oid FROM fin_articlesubfamily WHERE Family = '{0}' Order by CODE Asc LIMIT 1", TablePadFamily.SelectedButtonOid);
                getFirstSubFamily = XPOSettings.Session.ExecuteScalar(mysql).ToString();
            }
            else if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
            {
                string mssqlServer = string.Format("SELECT TOP 1 Oid FROM fin_articlesubfamily WHERE Family = '{0}' Order by CODE Asc", TablePadFamily.SelectedButtonOid);
                getFirstSubFamily = XPOSettings.Session.ExecuteScalar(mssqlServer).ToString();
            }

            //Article Filter : When Change Family always change Article too
            TablePadArticle.Filter = " AND (SubFamily = '" + getFirstSubFamily + "')";
            //IN009277ENDS

            //Debug
            //_logger.Debug(string.Format("_tablePadFamily_Clicked(): F:CurrentId: [{0}], Name: [{1}]", button.CurrentId, button.Name));
            //_logger.Debug(string.Format("_tablePadFamily_Clicked(): SubFamily.Sql:[{0}{1}{2}]", _tablePadSubFamily.Sql, _tablePadSubFamily.Filter, _tablePadSubFamily.Order));
            //_logger.Debug(string.Format("_tablePadFamily_Clicked(): Article.Sql:[{0}{1}{2}]", _tablePadArticle.Sql, _tablePadArticle.Filter, _tablePadArticle.Order));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: SubFamilies Button Clicks

        private void _tablePadSubFamily_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            TablePadSubFamily.SelectedButtonOid = button.CurrentButtonOid;

            //Article Filter
            TablePadArticle.Filter = string.Format(" AND (SubFamily = '{0}')", button.CurrentButtonOid);

            //Debug
            //_logger.Debug(string.Format("_tablePadSubFamily_Clicked(): S:CurrentId:[{0}], Name:[{1}]", button.CurrentId, button.Name));
            //_logger.Debug(string.Format("_tablePadSubFamily_Clicked(): Article.Sql:[{0}{1}{2}]", _tablePadArticle.Sql, _tablePadArticle.Filter, _tablePadArticle.Order));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Articles Button Clicks

        private void _tablePadArticle_Clicked(object sender, EventArgs e)
        {
            try
            {
                TouchButtonBase button = (TouchButtonBase)sender;
                //_logger.Debug(string.Format("_tablePadArticle_Clicked(): A:CurrentId:[{0}], Name:[{1}]", button.CurrentButtonOid, button.Name));

                //Change Mode
                if (TicketList.ListMode != TicketListMode.Ticket)
                {
                    TicketList.ListMode = TicketListMode.Ticket;
                    TicketList.UpdateModel();
                }

                //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
                TablePadArticle.SelectedButtonOid = button.CurrentButtonOid;

                //Send to TicketList
                TicketList.InsertOrUpdate(button.CurrentButtonOid);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: Open Cash Drawer

        private void eventBoxImageLogo_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (TerminalSettings.HasLoggedTerminal)
            {
                PosPinPadDialog dialogPinPad = new PosPinPadDialog(this, DialogFlags.Modal, XPOSettings.LoggedUser, true);
                int responsePinPad = dialogPinPad.Run();
                if (responsePinPad == (int)ResponseType.Ok)
                {
                    var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                    if (!resultOpenDoor)
                    {
                        Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, GeneralUtils.GetResourceByName("global_information"), string.Format(GeneralUtils.GetResourceByName("open_cash_draw_permissions")));
                    }
                    else
                    {
                        //Audit
                        XPOUtility.Audit("CASHDRAWER_OUT", string.Format(
                             GeneralUtils.GetResourceByName("audit_message_cashdrawer_out"),
                             TerminalSettings.LoggedTerminal.Designation,
                             "Button Open Door"));
                    }

                };

                dialogPinPad.Destroy();


            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event: BarCodeReader

        private void HWBarCodeReader_Captured(object sender, EventArgs e)
        {
            //_logger.Debug(String.Format("Window: [{0}] Device: [{1}] Captured: [{2}] Length: [{3}]", GlobalApp.HWBarCodeReader.Window, GlobalApp.HWBarCodeReader.Device, GlobalApp.HWBarCodeReader.Buffer, GlobalApp.HWBarCodeReader.Buffer.Length));
            /* 
             * TK013134 - Parking Ticket 
             * Check for cases that a table has not been opened yet
             */
            if (GlobalApp.PosMainWindow.TicketList.CurrentOrderDetails != null)
            {
                switch (GlobalApp.BarCodeReader.Device)
                {
                    case InputReaderDevice.None:
                        break;
                    case InputReaderDevice.BarCodeReader:
                    case InputReaderDevice.CardReader:
                        /* TK013134 - Parking Ticket */
                        // TODO implement a message dialog for UX purposes informing user that needs to select a table before scan a barcode
                        if (GeneralSettings.AppUseParkingTicketModule)
                        {
                            GlobalApp.ParkingTicket.GetTicketDetailFromWS(GlobalApp.BarCodeReader.Buffer);
                            //TicketList.InsertOrUpdate(GlobalApp.BarCodeReader.Buffer);
                        }
                        // Default Mode : Articles
                        else
                        {
                            TicketList.InsertOrUpdate(GlobalApp.BarCodeReader.Buffer);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Public Methods

        //Update UI, Required to Reflect BO or Outside Changes, Like Database Changes
        public void UpdateUI()
        {
            LabelTerminalInfo.Text = string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name);
            TablePadFamily.UpdateSql();
            TablePadSubFamily.UpdateSql();
            TablePadArticle.UpdateSql();

            TicketList.UpdateTicketListButtons();
        }

        public void UpdateWorkSessionUI()
        {
            //_logger.Debug("void UpdateWorkSessionUI() :: Starting..."); /* IN009008 */

            //Update Toolbar UI Buttons After ToolBox and ToolBar
            if (XPOSettings.WorkSessionPeriodDay != null)
            {
                //With Valid WorkSessionPeriodDay
                if (XPOSettings.WorkSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    //if (_touchButtonPosToolbarCashDrawer.LabelText != CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_close_day)
                    //  _touchButtonPosToolbarCashDrawer.LabelText = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_close_day;
                    //if (!_touchButtonPosToolbarCashDrawer.Sensitive == true)
                    //  _touchButtonPosToolbarCashDrawer.Sensitive = true;

                    //With Valid WorkSessionPeriodTerminal
                    if (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
                    {
                        bool isTableOpened = POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
                          && POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].Table != null;
                        //IN009231
                        //Abrir ordem na abertura
                        SelectedData xpoSelectedData = null;
                        if (DatabaseSettings.DatabaseType.ToString() == "MySql" || DatabaseSettings.DatabaseType.ToString() == "SQLite")
                        {
                            string sqlQuery = @"SELECT Oid FROM pos_configurationplacetable WHERE (Disabled IS NULL or Disabled  <> 1) ORDER BY Code asc LIMIT 1";

                            xpoSelectedData = XPOSettings.Session.ExecuteQueryWithMetadata(sqlQuery);
                        }
                        else if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
                        {
                            string sqlQuery = @"SELECT TOP 1 Oid FROM pos_configurationplacetable WHERE (Disabled IS NULL or Disabled  <> 1) ORDER BY Code asc";
                            xpoSelectedData = XPOSettings.Session.ExecuteQueryWithMetadata(sqlQuery);
                        }

                        SelectStatementResultRow[] selectStatementResultMeta = xpoSelectedData.ResultSet[0].Rows;
                        SelectStatementResultRow[] selectStatementResultData = xpoSelectedData.ResultSet[1].Rows;
                        if (!isTableOpened && !GeneralSettings.AppUseBackOfficeMode)
                        {

                            Guid currentTableOid = Guid.Parse(selectStatementResultData[0].Values[0].ToString());

                            //string filterCriteria = string.Format("Oid = '{0}'", SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity.ToString());

                            Guid newOrderMainOid = Guid.NewGuid();
                            POSSession.CurrentSession.OrderMains.Add(newOrderMainOid, new OrderMain(newOrderMainOid, currentTableOid));
                            OrderMain newOrderMain = POSSession.CurrentSession.OrderMains[newOrderMainOid];
                            OrderTicket orderTicket = new OrderTicket(newOrderMain, (PriceType)newOrderMain.Table.PriceType);
                            //Create Reference to SessionApp OrderMain with Open Ticket, Ready to Add Details
                            newOrderMain.OrderTickets.Add(1, orderTicket);
                            //Table TableOID = null;
                            //Create Reference to be used in Shared Code
                            OrderMain currentOrderMain = newOrderMain;

                            TicketList.UpdateArticleBag();
                            TicketList.UpdateTicketListOrderButtons();
                            TicketList.UpdateOrderStatusBar();

                            //ALWAYS Update current PersistentOid and Status from database
                            currentOrderMain.PersistentOid = currentOrderMain.GetOpenTableFieldValueGuid(pTableOid, "Oid");
                            currentOrderMain.OrderStatus = (OrderStatus)currentOrderMain.GetOpenTableFieldValue(pTableOid, "OrderStatus");

                            //Shared Code
                            POSSession.CurrentSession.CurrentOrderMainId = currentOrderMain.Table.OrderMainOid;
                            POSSession.CurrentSession.Save();
                            TicketList.UpdateModel();

                            //GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid].Table.OrderMainOid = currentTableOid;
                            _ticketPad.Sensitive = true;
                        }
                        if (!GeneralSettings.AppUseBackOfficeMode)
                            TablePadArticle.Sensitive = true;

                        if (!_ticketPad.Sensitive == true && !GeneralSettings.AppUseBackOfficeMode)
                            _ticketPad.Sensitive = true;
                    }
                    //With No WorkSessionPeriodTerminal
                    else if (!GeneralSettings.AppUseBackOfficeMode)
                    {
                        if (!_ticketPad.Sensitive == false)
                            _ticketPad.Sensitive = false;
                        if (!TablePadArticle.Sensitive == false)
                            TablePadArticle.Sensitive = false;
                    }
                }
            }
            //No WorkSessionPeriodDay
            else if (!GeneralSettings.AppUseBackOfficeMode)
            {
                //if (_touchButtonPosToolbarCashDrawer.LabelText != CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_open_day)
                //  _touchButtonPosToolbarCashDrawer.LabelText = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_open_day;
                //if (!_touchButtonPosToolbarCashDrawer.Sensitive == false)
                //  _touchButtonPosToolbarCashDrawer.Sensitive = false;
                if (!_ticketPad.Sensitive == false)
                    _ticketPad.Sensitive = false;
                if (!TablePadArticle.Sensitive == false)
                    TablePadArticle.Sensitive = false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Timer/Clock/UpdateUI

        private void StartClock()
        {
            // Every second call update_status' (1000 milliseconds)
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            if (GlobalApp.PosMainWindow.Visible)
            {
                _labelClock.Text = XPOUtility.CurrentDateTime(_clockFormat);

                //Call Current OrderMain Update Status
                if (POSSession.CurrentSession.CurrentOrderMainId != Guid.Empty && POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId))
                {
                    UpdateGUITimer(POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId], TicketList);
                }

                //Update UI Button and Get WorkSessionPeriodDay if is Opened by Other Terminal
                if (XPOSettings.WorkSessionPeriodTerminal == null
                  || (XPOSettings.WorkSessionPeriodTerminal != null && 
                  XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Close))
                {
                    pos_worksessionperiod workSessionPeriodDay = WorkSessionProcessor.GetSessionPeriod(WorkSessionPeriodType.Day);

                    if (workSessionPeriodDay == null)
                    {
                        if(XPOSettings.WorkSessionPeriodDay != null)
                        {
                            XPOUtility.WorkSession.SaveCurrentWorkSessionPeriodDayDto();
                        }

                        XPOSettings.WorkSessionPeriodDay = null;
                        UpdateWorkSessionUI();
                    }
                    else
                    {
                        if (workSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                        {
                            XPOSettings.WorkSessionPeriodDay = workSessionPeriodDay;
                            UpdateWorkSessionUI();
                        }
                    }
                }
            }
            // returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return true;
        }

        private void UpdateGUITimer(OrderMain orderMain, TicketList pTicketList)
        {
            bool debug = false;

            string sqlOrderMainUpdatedAt = string.Format("SELECT UpdatedAt FROM fin_documentordermain WHERE (PlaceTable = '{0}' AND OrderStatus = {1}) ORDER BY UpdatedAt DESC", orderMain.Table.Oid, Convert.ToInt16(OrderStatus.Open));
            var oResultUpdatedAt = XPOSettings.Session.ExecuteScalar(sqlOrderMainUpdatedAt);

            if (oResultUpdatedAt != null)
            {
                DateTime dateLastDBUpdate = Convert.ToDateTime(oResultUpdatedAt);

                if (orderMain.UpdatedAt < dateLastDBUpdate)
                {
                    orderMain.PersistentOid = orderMain.GetOpenTableFieldValueGuid(orderMain.Table.Oid, "Oid");
                    orderMain.OrderStatus = (OrderStatus)orderMain.GetOpenTableFieldValue(orderMain.Table.Oid, "OrderStatus");
                    orderMain.UpdatedAt = dateLastDBUpdate;

                    //If is in OrderMain List Mode Update Model
                    if (pTicketList.ListMode == TicketListMode.OrderMain) pTicketList.UpdateModel();

                    pTicketList.UpdateOrderStatusBar();
                    pTicketList.UpdateTicketListOrderButtons();

                    //Debug
                    if (debug) _logger.Debug(string.Format("UpdateGUITimer(): Table Status Updated [{0}], _persistentOid [{1}], _orderStatus [{2}], _UpdatedAt [{3}], dateLastDBUpdate [{4}]", orderMain.Table.Name, orderMain.PersistentOid, Convert.ToInt16(OrderStatus.Open), orderMain.UpdatedAt, dateLastDBUpdate));

                    POSSession.CurrentSession.Save();
                }
            }
            //Cant Get Table Open Status
            else
            {
                orderMain.PersistentOid = orderMain.GetOpenTableFieldValueGuid(orderMain.Table.Oid, "Oid");
                orderMain.OrderStatus = (OrderStatus)orderMain.GetOpenTableFieldValue(orderMain.Table.Oid, "OrderStatus");

                //If is in OrderMain List Mode Update Model
                if (pTicketList.ListMode == TicketListMode.OrderMain) pTicketList.UpdateModel();

                pTicketList.UpdateOrderStatusBar();
                pTicketList.UpdateTicketListOrderButtons();

                POSSession.CurrentSession.Save();

                //Debug
                //if (debug) _logger.Debug(string.Format("UpdateGUITimer(): Cant Get Table Status [{0}], sql:[{1}]", orderMain.Table.Name, sqlOrderMainUpdatedAt));
            }
        }

        //UpdateUI if detect open Orders
        private void UpdateUIIfHasWorkingOrder()
        {
            if (POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
              && POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].Table != null)
            {
                //Update Order Summary Status Bar
                TicketList.UpdateOrderStatusBar();
            }
            else
            {
                //Force Start With Default Table
                _ticketPad.SelectTableOrder(POSSettings.XpoOidConfigurationPlaceTableDefaultOpenTable);
                TicketList.UpdateArticleBag();
                TicketList.UpdateTicketListOrderButtons();
                TicketList.UpdateOrderStatusBar();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        // Automatically scroll to bottom of _textviewLog
        private void ScrollTextViewLog(object o, SizeAllocatedArgs args)
        {
            _textviewLog.ScrollToIter(_textviewLog.Buffer.EndIter, 0, false, 0, 0);
        }
    }
}
