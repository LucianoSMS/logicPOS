﻿using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_systemaudittype : Entity
    {
        public sys_systemaudittype() : base() { }
        public sys_systemaudittype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(sys_systemaudittype), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(sys_systemaudittype), "Code");
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

        private string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
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

        //SystemAuditType One <> Many SystemAudit
        [Association(@"SystemAuditTypeReferencesSystemAudit", typeof(sys_systemaudit))]
        public XPCollection<sys_systemaudit> Audit
        {
            get { return GetCollection<sys_systemaudit>("Audit"); }
        }
    }
}