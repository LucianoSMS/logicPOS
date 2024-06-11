﻿using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class rpt_reporttype : Entity
    {
        public rpt_reporttype() : base() { }
        public rpt_reporttype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(rpt_reporttype), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(rpt_reporttype), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        private string fMenuIcon;
        [Size(255)]
        public string MenuIcon
        {
            get { return fMenuIcon; }
            set { SetPropertyValue<string>("MenuIcon", ref fMenuIcon, value); }
        }

        //ReportType One <> Many Report
        [Association(@"ReportTypeReferencesReport", typeof(rpt_report))]
        public XPCollection<rpt_report> Report
        {
            get { return GetCollection<rpt_report>("Report"); }
        }
    }
}