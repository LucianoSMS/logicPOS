﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Extensions;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal abstract class BOBaseDialog : Dialog
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected Members
        protected GenericTreeViewXPO _treeView = null;
        protected DialogMode _dialogMode;
        protected Notebook _notebook;
        protected HBox _hboxStatus;
        protected Dictionary<int, Widget> _panels = new Dictionary<int, Widget>();
        //Other Shared Protected Members
        protected int _widgetMaxWidth;
        //Protected Members, Defaults
        protected int _dialogPadding = 10;
        protected int _dialogLabelDistance = 20;
        protected System.Drawing.Size _sizefileChooserPreview = new System.Drawing.Size(37, 37);
        //Used to control rows on page  
        protected int _rowCurrent = 0;
        protected int _rowHeight = 0;
        //DEPRECATED : Protected ReadOnly Records, always initialized : Now use protections in TreeView Events
        protected List<Guid> _protectedRecords = new List<Guid>();
        protected bool _protectRecord = false;
        //VBox
        protected int _boxSpacing = 5;
        //Public Properties, to have access to/from TreeView EventHandlers
        protected Entity _dataSourceRow;
        public Entity DataSourceRow
        {
            get { return _dataSourceRow; }
            set { _dataSourceRow = value; }
        }
        protected GenericCRUDWidgetListXPO _crudWidgetList;
        public GenericCRUDWidgetListXPO CrudWidgetList
        {
            get { return _crudWidgetList; }
            set { _crudWidgetList = value; }
        }
        public ICollection<fin_articlecomposition> _articlecompositions;

        protected TouchButtonIconWithText _buttonOk;
        public TouchButtonIconWithText buttonOk
        {
            get { return _buttonOk; }
            set { _buttonOk = value; }
        }

        protected TouchButtonIconWithText _buttonCancel;
        public TouchButtonIconWithText buttonCancel
        {
            get { return _buttonCancel; }
            set { _buttonCancel = value; }
        }

        public BOBaseDialog(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pDataSourceRow)
            : base("", pSourceWindow, pFlags)
        {
            //Parameters
            _treeView = pTreeView;
            _dialogMode = pDialogMode;
            if (pDataSourceRow != null) _dataSourceRow = pDataSourceRow;

            //TODO: try to prevent NULL Error
            //_dataSourceRow = XPOSettings.Session.GetObjectByKey<XPGuidObject>(_dataSourceRow.Oid);
            //TODO: Validar se o erro de editar dá erro de acesso objecto eliminado.
            //APPEAR when we Try to ReEdit Terminal, after assign Printer
            //An exception of type 'System.NullReferenceException' occurred in logicpos.exe but was not handled in user code
            if (pDataSourceRow != null) _dataSourceRow.Reload();

            //Defaults
            //Modal = true; //< Problems in Ubuntu, TitleBar Disapear
            WindowPosition = WindowPosition.CenterAlways;
            GrabFocus();
            SetSize(400, 400);
            _widgetMaxWidth = WidthRequest - (_dialogPadding * 2) - 16;

            //Grey Window : Luis|Muga
            //this.Decorated = false;
            //White Window : Mario
            this.Decorated = true;
            this.Resizable = false;
            this.WindowPosition = WindowPosition.Center;
            //Grey Window : Luis|Muga
            //this.ModifyBg(StateType.Normal, Utils.StringToGTKColor(LogicPOS.Settings.GeneralSettings.Settings["colorBackOfficeContentBackground"]));

            //Accelerators
            AccelGroup accelGroup = new AccelGroup();
            AddAccelGroup(accelGroup);

            //Init WidgetList
            if (pDataSourceRow != null) _crudWidgetList = new GenericCRUDWidgetListXPO(_dataSourceRow.Session);

            //Icon
            string fileImageAppIcon = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, POSSettings.AppIcon);
            if (File.Exists(fileImageAppIcon)) Icon = logicpos.Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));

            //Init StatusBar
            InitStatusBar();
            //InitButtons
            InitButtons();
            //InitUi
            InitUI();
        }

        protected void SetSize(int pWidth, int pHeight)
        {
            SetSizeRequest(pWidth, pHeight);
            _widgetMaxWidth = WidthRequest - (_dialogPadding * 2) - 16;
        }

        private void InitButtons()
        {
            //Settings
            string fontBaseDialogActionAreaButton = GeneralSettings.Settings["fontBaseDialogActionAreaButton"];
            string tmpFileActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
            string tmpFileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
            System.Drawing.Size sizeBaseDialogActionAreaButtonIcon = logicpos.Utils.StringToSize(GeneralSettings.Settings["sizeBaseDialogActionAreaButtonIcon"]);
            System.Drawing.Size sizeBaseDialogActionAreaButton = logicpos.Utils.StringToSize(GeneralSettings.Settings["sizeBaseDialogActionAreaButton"]);
            System.Drawing.Color colorBaseDialogActionAreaButtonBackground = GeneralSettings.Settings["colorBaseDialogActionAreaButtonBackground"].StringToColor();
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = GeneralSettings.Settings["colorBaseDialogActionAreaButtonFont"].StringToColor();

            //TODO:THEME
            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                sizeBaseDialogActionAreaButton.Height -= 10;
                sizeBaseDialogActionAreaButtonIcon.Width -= 10;
                sizeBaseDialogActionAreaButtonIcon.Height -= 10;
            };

            _buttonOk = new TouchButtonIconWithText("touchButtonOk_DialogActionArea", colorBaseDialogActionAreaButtonBackground, GeneralUtils.GetResourceByName("global_button_label_ok"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, tmpFileActionOK, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);
            _buttonCancel = new TouchButtonIconWithText("touchButtonCancel_DialogActionArea", colorBaseDialogActionAreaButtonBackground, GeneralUtils.GetResourceByName("global_button_label_cancel"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, tmpFileActionCancel, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);

            //If DialogMode in View Mode, dont Show Ok Button
            if (_dialogMode != DialogMode.View)
            {
                this.AddActionWidget(_buttonOk, ResponseType.Ok);
            }
            this.AddActionWidget(_buttonCancel, ResponseType.Cancel);
        }

        private void InitUI()
        {
            //Init NoteBook
            _notebook = new Notebook();

            _notebook.BorderWidth = 3;

            //Pack
            //Grey Window : Luis|Muga
            //VBox.PackStart(_notebook, true, true, 5);
            //White Window : Mario
            VBox.PackStart(_notebook, true, true, 0);

            VBox.PackStart(_hboxStatus, false, false, 0);
            this.AddActionWidget(_hboxStatus, -1);
        }

        protected void InitNotes()
        {
            VBox vbox = new VBox(true, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            //Notes
            EntryMultiline entryMultiline = new EntryMultiline();
            entryMultiline.Value.Text = (DataSourceRow as Entity).Notes;
            //Remove ShadowType and Border
            //entryMultiline.ScrolledWindow.ShadowType = ShadowType.None;
            entryMultiline.ScrolledWindow.BorderWidth = 0;
            Label labelMultiline = new Label(GeneralUtils.GetResourceByName("global_notes"));
            vbox.PackStart(entryMultiline, true, true, 0);
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryMultiline, labelMultiline, DataSourceRow, "Notes", RegexUtils.RegexAlfaNumericExtended, false));

            //Append Tab
            _notebook.AppendPage(vbox, new Label(GeneralUtils.GetResourceByName("global_notes")));
        }

        protected override void OnResponse(ResponseType pResponse)
        {
            if (_crudWidgetList != null) _crudWidgetList.ProcessDialogResponse(this, _dialogMode, pResponse);
            //Artigos Compostos [IN:016522]


            if (this.GetType() == (typeof(DialogArticle)))
            {
                try
                {
                    if (_dataSourceRow.GetType() == (typeof(fin_article)))
                    {
                        //Restore Objects before editing if cancel or delete  
                        if ((_dataSourceRow as fin_article).IsComposed)
                        {
                            if (pResponse == ResponseType.Cancel || pResponse == ResponseType.DeleteEvent)
                            {
                                for (int i = (_dataSourceRow as fin_article).ArticleComposition.Count; i > 0; i--)
                                {
                                    var aux = (_dataSourceRow as fin_article).ArticleComposition[i - 1];
                                    (_dataSourceRow as fin_article).ArticleComposition.Remove(aux);
                                }
                                foreach (var item in _articlecompositions)
                                {
                                    (_dataSourceRow as fin_article).ArticleComposition.Add(item);

                                }
                            }
                        }

                        //Process stocks
                        //Gestão de Stocks - Ajuste de Stock diretamente no Artigo (BackOffice) [IN:016530]
                        try
                        {
                            if (pResponse == ResponseType.Ok || pResponse == ResponseType.Apply)
                            {
                                string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", (_dataSourceRow as fin_article).Oid);
                                var getArticleStock = Convert.ToDecimal(_dataSourceRow.Session.ExecuteScalar(stockQuery));
                                if (Convert.ToDecimal(getArticleStock.ToString()) != (_dataSourceRow as fin_article).Accounting)
                                {
                                    var own_customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), XPOSettings.XpoOidUserRecord);
                                    if (own_customer != null)
                                    {
                                        if (string.IsNullOrEmpty(own_customer.Name))
                                        {
                                            //update owner customer for internal stock moviments
                                            own_customer.FiscalNumber = GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"];
                                            own_customer.Name = GeneralSettings.PreferenceParameters["COMPANY_NAME"];
                                            own_customer.Save();
                                        }
                                    }
                                    if ((_dataSourceRow as fin_article).Accounting > getArticleStock)
                                    {
                                        decimal quantity = (_dataSourceRow as fin_article).Accounting - getArticleStock;
                                        ProcessArticleStock.Add(ProcessArticleStockMode.In, own_customer, 1, DateTime.Now, GeneralUtils.GetResourceByName("global_internal_document_footer1"), (_dataSourceRow as fin_article), quantity, GeneralUtils.GetResourceByName("global_internal_document_footer1"));
                                    }
                                    else
                                    {
                                        decimal quantity = getArticleStock - (_dataSourceRow as fin_article).Accounting;
                                        ProcessArticleStock.Add(ProcessArticleStockMode.Out, own_customer, 1, DateTime.Now, GeneralUtils.GetResourceByName("global_internal_document_footer1"), (_dataSourceRow as fin_article), quantity, GeneralUtils.GetResourceByName("global_internal_document_footer1"));
                                    }

                                }
                            }
                        }
                        //New article
                        catch
                        {
                            ProcessArticleStock.Add(ProcessArticleStockMode.In, null, 1, DateTime.Now, GeneralUtils.GetResourceByName("global_internal_document_footer1"), (_dataSourceRow as fin_article), (_dataSourceRow as fin_article).Accounting, GeneralUtils.GetResourceByName("global_internal_document_footer1"));
                        }

                    }
                    //Delete Articles compositions with deleted Parents
                    string sqlDelete = string.Format("DELETE FROM [fin_articlecomposition] WHERE [Article] IS NULL;");
                    XPOSettings.Session.ExecuteQuery(sqlDelete);
                    _logger.Debug("Delete() :: articles composition with null parents'" + "'  ");
                    //Delete Articles SerialNumber emptys 
                    //sqlDelete = string.Format("DELETE FROM [fin_articleserialnumber] WHERE [SerialNumber] IS NULL;");
                    //XPOSettings.Session.ExecuteQuery(sqlDelete);
                    //_logger.Debug("Delete() :: articles serialnumber with null value'" + "'  ");

                    _dataSourceRow.Reload();
                }
                catch (Exception ex)
                {
                    _logger.Error("error Delete() :: articles composition with null parents '" + "' : " + ex.Message, ex);
                }
            }
        }

        private void InitStatusBar()
        {
            if (_crudWidgetList != null)
            {
                _hboxStatus = new HBox(true, 0);
                _hboxStatus.BorderWidth = 3;

                //UpdatedBy
                VBox vboxUpdatedBy = new VBox(true, 0);
                Label labelUpdatedBy = new Label(GeneralUtils.GetResourceByName("global_record_user_update"));
                Label labelUpdatedByValue = new Label(string.Empty);
                labelUpdatedBy.SetAlignment(0.0F, 0.5F);
                labelUpdatedByValue.SetAlignment(0.0F, 0.5F);
                //labelUpdatedBy.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                //labelUpdatedByValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                vboxUpdatedBy.PackStart(labelUpdatedBy);
                vboxUpdatedBy.PackStart(labelUpdatedByValue);

                //CreatedAt
                VBox vboxCreatedAt = new VBox(true, 0);
                Label labelCreatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_created"));
                Label labelCreatedAtValue = new Label(string.Empty);
                //labelCreatedAt.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                //labelCreatedAtValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                labelCreatedAt.SetAlignment(0.5F, 0.5F);
                labelCreatedAtValue.SetAlignment(0.5F, 0.5F);
                vboxCreatedAt.PackStart(labelCreatedAt);
                vboxCreatedAt.PackStart(labelCreatedAtValue);

                //UpdatedAt
                VBox vboxUpdatedAt = new VBox(true, 0);
                Label labelUpdatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_updated_for_base_dialog"));
                Label labelUpdatedAtValue = new Label(string.Empty);
                //labelUpdatedAt.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                //labelUpdatedAtValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                labelUpdatedAt.SetAlignment(1.0F, 0.5F);
                labelUpdatedAtValue.SetAlignment(1.0F, 0.5F);
                vboxUpdatedAt.PackStart(labelUpdatedAt);
                vboxUpdatedAt.PackStart(labelUpdatedAtValue);

                _hboxStatus.PackStart(vboxUpdatedBy);
                _hboxStatus.PackStart(vboxCreatedAt);
                _hboxStatus.PackStart(vboxUpdatedAt);

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(labelUpdatedByValue, (_dataSourceRow as dynamic).UpdatedBy, "Name"));
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(labelCreatedAtValue, _dataSourceRow, "CreatedAt"));
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(labelUpdatedAtValue, _dataSourceRow, "UpdatedAt"));
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DEPRECATED : Protected ReadOnly Records, always initialized : Now use protections in TreeView Events
        protected void ProtectComponents()
        {
            //Works on UPDATE & DELETE
            if (_dialogMode != DialogMode.Insert && _protectedRecords != null && _protectedRecords.Count > 0)
            {
                //Update Reference
                _protectRecord = _protectedRecords.Contains(_dataSourceRow.Oid);

                if (_protectRecord)
                {
                    //Protect Edits in Components
                    foreach (var item in _crudWidgetList)
                    {
                        //_logger.Debug(String.Format("item: [{0}]", item));
                        item.Widget.Sensitive = false;
                    }
                }
            }
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // BO DEPRECATED : Muga Stuff
        /*
        protected void ResetRowManager()
        {
            _rowCurrent = 10;
            _rowHeight = 50;
        }

        protected void JumpRow()
        {
            JumpRow(1);
        }

        protected void JumpRow(int pFactor)
        {
            _rowCurrent += (_rowHeight / pFactor);
        }

        protected Fixed GetNewTabPage(string pLabel)
        {
            Fixed result = new Fixed();

            Label tmpNewLabel = new Label(pLabel);

            _panels.Add(_notebook.AppendPage(result, tmpNewLabel), result);

            ResetRowManager();
            return (result);
        }

        protected Entry AddRowItemText(Fixed pTab, string pCaption, string pFieldName, string pValidationRegex, bool pRequired, bool pFullWidth)
        {
            try
            {
                return AddRowItemText(pTab, pCaption, pFieldName, pValidationRegex, pRequired, pFullWidth, false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        //Add item as rows
        protected Entry AddRowItemText(Fixed pTab, string pCaption, string pFieldName, string pValidationRegex, bool pRequired, bool pFullWidth, bool pReadOnly)
        {
            try
            {
                Label labelObject = new Label(pCaption);
                Entry entryObject = new Entry();

                if (pFullWidth)
                {
                    entryObject.WidthRequest = _widgetMaxWidth;
                }

                if (pReadOnly)
                {
                    entryObject.IsEditable = false;
                }

                pTab.Put(labelObject, _dialogPadding, _rowCurrent);
                pTab.Put(entryObject, _dialogPadding, _rowCurrent + _dialogLabelDistance);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryObject, labelObject, _dataSourceRow, pFieldName, pValidationRegex, pRequired));

                JumpRow();

                return entryObject;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        protected CheckButton AddRowItemCheckButton(Fixed pTab, string pCaption, string pFieldName)
        {
            try
            {
                CheckButton tmpCheckButton = new CheckButton(pCaption);

                pTab.Put(tmpCheckButton, _dialogPadding, _rowCurrent);

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(tmpCheckButton, _dataSourceRow, pFieldName));

                JumpRow(2);

                return tmpCheckButton;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        protected FileChooserButton AddRowItemFileSelect(Fixed pTab, string pCaption, FileFilter pFilters, string pFieldName, string pcurrentValue, bool pRequired, bool pFullWidth)
        {
            try
            {
                Label labelButtonIcon = new Label(pCaption);
                FileChooserButton fileChooserButtonIcon = new FileChooserButton(pCaption, FileChooserAction.Open);
                fileChooserButtonIcon.SetFilename(pcurrentValue);
                fileChooserButtonIcon.Filter = pFilters;

                if (pFullWidth)
                {
                    fileChooserButtonIcon.WidthRequest = _widgetMaxWidth;
                }

                pTab.Put(labelButtonIcon, _dialogPadding, _rowCurrent);
                pTab.Put(fileChooserButtonIcon, _dialogPadding, _rowCurrent + _dialogLabelDistance);

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(fileChooserButtonIcon, labelButtonIcon, _dataSourceRow, pFieldName));

                JumpRow();

                return fileChooserButtonIcon;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        protected FileChooserButton AddRowItemImageSelect(Fixed pTab, string pCaption, FileFilter pFilters, string pFieldName, string pcurrentValue, bool pRequired, bool pFullWidth)
        {
            try
            {
                Label labelIcon = new Label(pCaption);
                FileChooserButton fileChooser = new FileChooserButton("", FileChooserAction.Open);
                fileChooser.SetFilename(pcurrentValue);

                Image imagePreviewButtonIcon = new Image()
                {
                    WidthRequest = _sizefileChooserPreview.Width,
                    HeightRequest = _sizefileChooserPreview.Height
                };

                Frame framePreviewButtonIcon = new Frame();
                framePreviewButtonIcon.Add(imagePreviewButtonIcon);
                fileChooser.Filter = Utils.GetFileFilterImages();
                fileChooser.WidthRequest = _widgetMaxWidth - imagePreviewButtonIcon.WidthRequest - 8;
                fileChooser.SelectionChanged += (sender, eventArgs) => imagePreviewButtonIcon.Pixbuf = Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(imagePreviewButtonIcon.WidthRequest, imagePreviewButtonIcon.HeightRequest));
                pTab.Put(labelIcon, _dialogPadding, _rowCurrent);
                pTab.Put(fileChooser, _dialogPadding, _rowCurrent + _dialogLabelDistance);
                pTab.Put(framePreviewButtonIcon, WidthRequest - _dialogPadding - imagePreviewButtonIcon.WidthRequest - 12, _rowCurrent);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(fileChooser, labelIcon, _dataSourceRow, pFieldName, string.Empty, pRequired));

                JumpRow();

                return fileChooser;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFieldNameChildren, bool pFullWidth)
        {
            return (AddRowItemRelation(pTab, pCaption, pFieldName, pType, pCurrentValue, pFieldNameChildren, pFullWidth, false));
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFieldNameChildren, bool pFullWidth, bool pRequired)
        {
            return (AddRowItemRelation(pTab, pCaption, pFieldName, pType, pCurrentValue, string.Empty, pFieldNameChildren, pFullWidth, pRequired, null));
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFilter, string pFieldNameChildren, bool pFullWidth)
        {
            return (AddRowItemRelation(pTab, pCaption, pFieldName, pType, pCurrentValue, pFilter, pFieldNameChildren, pFullWidth, false, null));
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFilter, string pFieldNameChildren, bool pFullWidth, bool pRequired, SortProperty[] pSortProperty)
        {
            string filterDefault = "(Disabled = 0 OR Disabled is NULL)";
            if (pFilter != string.Empty) filterDefault = string.Format("{0} AND {1}", filterDefault, pFilter);
            SortProperty[] sortProperty = (pSortProperty != null && pSortProperty.Length > 0) ? pSortProperty : null;

            Label labelObject = new Label(pCaption);
            XPOComboBox xpoComboBox = new XPOComboBox(_dataSourceRow.Session, pType, pCurrentValue, pFieldNameChildren, CriteriaOperator.Parse(filterDefault), sortProperty);

            if (pFullWidth)
            {
                xpoComboBox.WidthRequest = _widgetMaxWidth;
            }

            //Image imagePreviewButtonIcon = new Image()
            //{
            //    WidthRequest = _sizefileChooserPreview.Width,
            //    HeightRequest = _sizefileChooserPreview.Height
            //};
            //btn = new Button();

            //Frame framePreviewButtonIcon = new Frame();
            //framePreviewButtonIcon.Add(imagePreviewButtonIcon);
            //btn.Clicked += btn_Clicked;

            pTab.Put(labelObject, _dialogPadding, _rowCurrent);
            pTab.Put(xpoComboBox, _dialogPadding, _rowCurrent + _dialogLabelDistance);
            //pTab.Put(btn, WidthRequest - _dialogPadding - imagePreviewButtonIcon.WidthRequest - 12, _rowCurrent);

            //Commented by Mario: we always need RegexGuid here, else we have problemas on non Required Fields
            //string regExRule = (pRequired) ? SettingsApp.RegexGuid : string.Empty;
            //Fixed Non Required Fields
            string regExRule = SettingsApp.RegexGuid;
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(xpoComboBox, labelObject, _dataSourceRow, pFieldName, regExRule, pRequired));

            _rowCurrent += _rowHeight;

            return (xpoComboBox);
        }

        protected void AddToPositionItemLabel(int pX, int pY, Fixed pTab, string pCaption)
        {
            Label labelButtonIcon = new Label(pCaption);
            pTab.Put(labelButtonIcon, pX, pY);
        }

        protected void AddToPositionItemText(int pX, int pY, int pWidth, Fixed pTab, string pFieldName, string pValidationRegex, bool pRequired, bool pReadOnly)
        {
            Entry entryObject = new Entry();

            entryObject.WidthRequest = pWidth;

            if (pReadOnly)
            {
                entryObject.IsEditable = false;
            }

            pTab.Put(entryObject, pX, pY);
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryObject, _dataSourceRow, pFieldName, pValidationRegex, pRequired));
        }

        protected void AddToPositionItemCheckButton(int pX, int pY, Fixed pTab, string pFieldName)
        {
            CheckButton tmpCheckButton = new CheckButton(string.Empty);
            pTab.Put(tmpCheckButton, pX, pY);
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(tmpCheckButton, _dataSourceRow, pFieldName));
        }

        //Commented: We cant Filter Combo after Render Model, this way we lost selected Item, and the selected item is always the last one, NOW we filter when generating Model
        //protected void FilterCombo(XPOComboBox sender, string pSQLFilter)
        //{
        //  CriteriaOperator criteria = CriteriaOperator.Parse(pSQLFilter);
        //  sender.UpdateModel(criteria);
        //}

        */
        //EO DEPRECATED
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}
