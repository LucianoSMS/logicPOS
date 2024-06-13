﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosReportsDialog : PosBaseDialog
    {
        private ScrolledWindow _scrolledWindow;
        // Enable to show Insert Logs on Console
        private readonly bool _showInsertLog = false;
        // Start Ord Code Int
        private int _userpermissionitemOrdAndCode = 1000;

        public PosReportsDialog() { }

        public PosReportsDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("global_reports");
            System.Drawing.Size windowSize = new System.Drawing.Size(500, 509);//454
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_reports.png";

            System.Drawing.Size sizeIcon = new System.Drawing.Size(50, 50);

            //Icons
            string fileIconDefault = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_default.png";

            // InitUI
            InitUI();

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _scrolledWindow, null);
        }

        private void InitUI()
        {
            // Build Accordion
            Accordion accordion = new Accordion(GetAccordionDefinition(), POSSettings.PrivilegesReportDialogFormat);
            //Accordion.Clicked += accordion_Clicked;

            //ViewPort
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(accordion);
            viewport.ResizeMode = ResizeMode.Parent;
            //ScrolledWindow
            _scrolledWindow = new ScrolledWindow();
            _scrolledWindow.ShadowType = ShadowType.EtchedIn;
            _scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            _scrolledWindow.Add(viewport);
            _scrolledWindow.ResizeMode = ResizeMode.Parent;
        }

        /// <summary>
        /// Get Accordion Definition and Generate Privileges (Output to Console)
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, AccordionNode> GetAccordionDefinition()
        {
            //Init accordionDefinition
            Dictionary<string, AccordionNode> accordionDefinition = null;
            try
            {
                string buttonLabelReportTypeString = string.Empty;
                string buttonLabel = string.Empty;
                string buttonLabelString = string.Empty;
                string buttonResourceString = string.Empty;
                string buttoLabelPostfix = string.Empty;
                string userpermissionitemToken = string.Empty;
                Guid userpermissionitemGuid = Guid.Empty;
                Guid userpermissionprofileGuid = Guid.Empty;
                List<string> userpermissionitemInsert = new List<string>();
                List<string> userpermissionprofileInsert = new List<string>();
                string templatePermissionItem = "INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup) VALUES ('{0}',{1},{1},'{2}','{3}','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a');";
                string userpermissionprofileItem = "INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('{0}',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','{1}');";

                // Init Accordion
                accordionDefinition = new Dictionary<string, AccordionNode>();

                // ReportType : Collection
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
                SortProperty[] sortProperty = new SortProperty[2];
                sortProperty[0] = new SortProperty("Ord", SortingDirection.Ascending);
                sortProperty[1] = new SortProperty("Designation", SortingDirection.Ascending);
                XPCollection xpoCollection = new XPCollection(XPOSettings.Session, typeof(rpt_reporttype), criteriaOperator, sortProperty);

                // Report : Collection (ReportType Property Navigations)
                SortingCollection sortingCollection = new SortingCollection
                {
                    new SortProperty("Ord", SortingDirection.Ascending)
                };

                foreach (rpt_reporttype reportType in xpoCollection)
                {
                    // Init AccordionChild
                    Dictionary<string, AccordionNode> accordionChilds = new Dictionary<string, AccordionNode>();

                    buttonLabelReportTypeString = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, reportType.ResourceString);
                    // Try to get Resource
                    if (string.IsNullOrEmpty(buttonLabelReportTypeString))
                    {
                        // Log Before Exchange _ with - to show in button labels
                        _logger.Error(string.Format("Can't find resourceString: [{0}]", reportType.ResourceString));
                        buttonLabelReportTypeString = string.Format("${0}$", reportType.ResourceString.Replace("_", "-"));
                    }

                    // Apply Sorting to childs of ReportType
                    reportType.Report.Sorting = sortingCollection;

                    // Loop ReportType Reports
                    foreach (rpt_report report in reportType.Report)
                    {
                        if (!report.Disabled)
                        {
                            bool reportSensitive = true;
                            userpermissionitemToken = report.Token;
                            //Prevent Reports to non-licenced Module stocks
                            if (report.ReportType.Oid == Guid.Parse("751c9e56-26bb-4fc8-8110-6c1e3b7c84e6") && report.Token != "REPORT_LIST_STOCK_MOVEMENTS" && !LicenseSettings.LicenseModuleStocks)
                            {
                                reportSensitive = false;
                            }

                            if (reportSensitive)
                            {
                                // Generate Insert for userpermissionitem and userpermissionitem                               
                                userpermissionitemGuid = Guid.NewGuid();
                                userpermissionprofileGuid = Guid.NewGuid();
                                userpermissionitemInsert.Add(string.Format(templatePermissionItem, userpermissionitemGuid, _userpermissionitemOrdAndCode, userpermissionitemToken, report.Designation.Replace(" de ", " - ")));
                                userpermissionprofileInsert.Add(string.Format(userpermissionprofileItem, userpermissionprofileGuid, userpermissionitemGuid));
                                _userpermissionitemOrdAndCode += 10;
                            }

                            // Get common resource for all 3 types of Financial Reports
                            Tuple<string, string> tuppleResourceString = LogicPOS.Reporting.Common.FastReport.GetResourceString(report.ResourceString);
                            buttonLabelString = tuppleResourceString.Item1;
                            buttoLabelPostfix = tuppleResourceString.Item2;

                            // Try to get Resource
                            if (string.IsNullOrEmpty(buttonLabelString))
                            {
                                // Log Before Exchange _ with - to show in button labels
                                buttonLabelString = report.ResourceString;
                                _logger.Debug(string.Format("Error Missing resourceString! {0}", buttonLabelString));
                                buttonLabelString = string.Format("${0}$", report.ResourceString.Replace("_", "-"));
                            }
                            // Used this for debug purposes, Add Code, usefull to identify Reports
                            //buttonLabel = string.Format("[{0}] {1}{2}", report.Code, buttonResourceString, resourceStringPostfix);
                            buttonLabel = string.Format("{0}{1}", buttonLabelString, buttoLabelPostfix);

                            // Output Order and Labels 
                            //_logger.Debug(String.Format("Label: [{0}]", buttonLabel));

                            // Add Child Menu
                            accordionChilds.Add(userpermissionitemToken, new AccordionNode(buttonLabel, reportSensitive)
                            {
                                Clicked = PrintReportRouter
                            });
                        }
                    }

                    // Add Main Accordion Parent Buttons
                    accordionDefinition.Add($"TopMenu{reportType.Code}",
                        new AccordionNode(buttonLabelReportTypeString)
                        {
                            Childs = accordionChilds,
                            GroupIcon = new Image($"Assets/Images/Icons/Reports/{reportType.MenuIcon}")
                        });
                }

                // Output Inserts
                if (_showInsertLog)
                {
                    _logger.Debug("Generated Inserts");
                    for (int i = 0; i < userpermissionitemInsert.Count; i++)
                    {
                        _logger.Debug(userpermissionitemInsert[i]);
                    }
                    for (int i = 0; i < userpermissionprofileInsert.Count; i++)
                    {
                        _logger.Debug(userpermissionprofileInsert[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return accordionDefinition;
        }
    }
}
