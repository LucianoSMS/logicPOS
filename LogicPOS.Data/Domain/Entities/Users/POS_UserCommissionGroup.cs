using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class pos_usercommissiongroup : Entity
    {
        public pos_usercommissiongroup() : base() { }
        public pos_usercommissiongroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(pos_usercommissiongroup), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(pos_usercommissiongroup), "Code");
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
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private decimal fCommission;
        public decimal Commission
        {
            get { return fCommission; }
            set { SetPropertyValue<decimal>("Commission", ref fCommission, value); }
        }

        //UserCommissionGroup One <> Many Family
        [Association(@"UserCommissionGroupReferencesFamily", typeof(fin_articlefamily))]
        public XPCollection<fin_articlefamily> Family
        {
            get { return GetCollection<fin_articlefamily>("Family"); }
        }

        //UserCommissionGroup One <> Many SubFamily
        [Association(@"UserCommissionGroupReferencesSubFamily", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> SubFamily
        {
            get { return GetCollection<fin_articlesubfamily>("SubFamily"); }
        }

        //UserCommissionGroup One <> Many Article
        [Association(@"UserCommissionGroupReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }

        //CommissionGroup One <> Many User
        [Association(@"UserCommissionGroupReferencesUserDetail", typeof(sys_userdetail))]
        public XPCollection<sys_userdetail> Users
        {
            get { return GetCollection<sys_userdetail>("Users"); }
        }
    }
}