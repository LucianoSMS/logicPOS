using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Xpo;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_configurationpaymentcondition : XPGuidObject
    {
        public fin_configurationpaymentcondition() : base() { }
        public fin_configurationpaymentcondition(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(fin_configurationpaymentcondition), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(fin_configurationpaymentcondition), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fAcronym;
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        //ConfigurationPaymentCondition One <> Many DocumentFinanceMaster
        [Association(@"ConfigurationPaymentConditionReferencesDocumentFinanceMaster", typeof(fin_documentfinancemaster))]
        public XPCollection<fin_documentfinancemaster> DocumentMaster
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentMaster"); }
        }
    }
}