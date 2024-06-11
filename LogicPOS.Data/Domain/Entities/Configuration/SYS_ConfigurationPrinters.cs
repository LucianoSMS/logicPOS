﻿using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_configurationprinters : Entity
    {
        public sys_configurationprinters() : base() { }
        public sys_configurationprinters(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(sys_configurationprinters), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(sys_configurationprinters), "Code");
            ShowInDialog = true;
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fDesignation;
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fNetworkName;
        public string NetworkName
        {
            get { return fNetworkName; }
            set { SetPropertyValue<string>("NetworkName", ref fNetworkName, value); }
        }

        private string fThermalEncoding;
        public string ThermalEncoding
        {
            get { return fThermalEncoding; }
            set { SetPropertyValue<string>("ThermalEncoding", ref fThermalEncoding, value); }
        }

        private bool fThermalPrintLogo;
        public bool ThermalPrintLogo
        {
            get { return fThermalPrintLogo; }
            set { SetPropertyValue<bool>("ThermalPrintLogo", ref fThermalPrintLogo, value); }
        }

        private string fThermalImageCompanyLogo;
        public string ThermalImageCompanyLogo
        {
            get { return fThermalImageCompanyLogo; }
            set { SetPropertyValue<string>("ThermalImageCompanyLogo", ref fThermalImageCompanyLogo, value); }
        }

        private int fThermalMaxCharsPerLineNormal;
        public int ThermalMaxCharsPerLineNormal
        {
            get { return fThermalMaxCharsPerLineNormal; }
            set { SetPropertyValue<int>("ThermalMaxCharsPerLineNormal", ref fThermalMaxCharsPerLineNormal, value); }
        }

        private int fThermalMaxCharsPerLineNormalBold;
        public int ThermalMaxCharsPerLineNormalBold
        {
            get { return fThermalMaxCharsPerLineNormalBold; }
            set { SetPropertyValue<int>("ThermalMaxCharsPerLineNormalBold", ref fThermalMaxCharsPerLineNormalBold, value); }
        }

        private int fThermalMaxCharsPerLineSmall;
        public int ThermalMaxCharsPerLineSmall
        {
            get { return fThermalMaxCharsPerLineSmall; }
            set { SetPropertyValue<int>("ThermalMaxCharsPerLineSmall", ref fThermalMaxCharsPerLineSmall, value); }
        }

        private string fThermalCutCommand;
        public string ThermalCutCommand
        {
            get { return fThermalCutCommand; }
            set { SetPropertyValue<string>("ThermalCutCommand", ref fThermalCutCommand, value); }
        }

        private int fThermalOpenDrawerValueM;
        public int ThermalOpenDrawerValueM
        {
            get { return fThermalOpenDrawerValueM; }
            set { SetPropertyValue<int>("ThermalOpenDrawerValueM", ref fThermalOpenDrawerValueM, value); }
        }

        private int fThermalOpenDrawerValueT1;
        public int ThermalOpenDrawerValueT1
        {
            get { return fThermalOpenDrawerValueT1; }
            set { SetPropertyValue<int>("ThermalOpenDrawerValueT1", ref fThermalOpenDrawerValueT1, value); }
        }

        private int fThermalOpenDrawerValueT2;
        public int ThermalOpenDrawerValueT2
        {
            get { return fThermalOpenDrawerValueT2; }
            set { SetPropertyValue<int>("ThermalOpenDrawerValueT2", ref fThermalOpenDrawerValueT2, value); }
        }

        private bool fShowInDialog;
        public bool ShowInDialog
        {
            get { return fShowInDialog; }
            set { SetPropertyValue<bool>("ShowInDialog", ref fShowInDialog, value); }
        }

        //ConfigurationPrintersType One <> Many ConfigurationPlace
        private sys_configurationprinterstype fPrinterType;
        [Association(@"ConfigurationPrintersTypeConfigurationPrinters")]
        public sys_configurationprinterstype PrinterType
        {
            get { return fPrinterType; }
            set { SetPropertyValue("PrinterType", ref fPrinterType, value); }
        }

        // Impressoras - Diferenciação entre Tipos TK016249
        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationPrintersReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> Terminals
        {
            get { return GetCollection<pos_configurationplaceterminal>("Terminals"); }
        }

        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationThermalPrintersReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> ThermalPrinter
        {
            get { return GetCollection<pos_configurationplaceterminal>("ThermalPrinter"); }
        }

        //ConfigurationPrinters One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"ConfigurationPrintersTerminalReferencesDFYearSerieTerminal", typeof(fin_documentfinanceyearserieterminal))]
        public XPCollection<fin_documentfinanceyearserieterminal> YearSerieTerminal
        {
            get { return GetCollection<fin_documentfinanceyearserieterminal>("YearSerieTerminal"); }
        }

        //ConfigurationPrinters One <> Many Article
        [Association(@"ConfigurationPrintersReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }

        //ConfigurationPrinters One <> Many ArticleFamily
        [Association(@"ConfigurationPrintersReferencesArticleFamily", typeof(fin_articlefamily))]
        public XPCollection<fin_articlefamily> ArticleFamily
        {
            get { return GetCollection<fin_articlefamily>("ArticleFamily"); }
        }

        //ConfigurationPrinters One <> Many ArticleSubFamily
        [Association(@"ConfigurationPrintersReferencesArticleSubFamily", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> ArticleSubFamily
        {
            get { return GetCollection<fin_articlesubfamily>("ArticleSubFamily"); }
        }

        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationPrintersReferencesDocumentFinanceType", typeof(fin_documentfinancetype))]
        public XPCollection<fin_documentfinancetype> DocumentType
        {
            get { return GetCollection<fin_documentfinancetype>("DocumentType"); }
        }
    }
}
