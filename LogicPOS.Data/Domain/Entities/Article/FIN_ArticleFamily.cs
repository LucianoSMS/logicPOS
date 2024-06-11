using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_articlefamily : Entity
    {
        public fin_articlefamily() : base() { }
        public fin_articlefamily(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(fin_articlefamily), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(fin_articlefamily), "Code");
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

        private string fButtonLabel;
        [Size(35)]
        public string ButtonLabel
        {
            get { return fButtonLabel; }
            set { SetPropertyValue<string>("ButtonLabel", ref fButtonLabel, value); }
        }

        private bool fButtonLabelHide;
        public bool ButtonLabelHide
        {
            get { return fButtonLabelHide; }
            set { SetPropertyValue<bool>("ButtonLabelHide", ref fButtonLabelHide, value); }
        }

        private string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        private string fButtonIcon;
        [Size(255)]
        public string ButtonIcon
        {
            get { return fButtonIcon; }
            set { SetPropertyValue<string>("ButtonIcon", ref fButtonIcon, value); }
        }

        //UserCommissionGroup One <> Many Family
        private pos_usercommissiongroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesFamily")]
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many SubFamily
        private erp_customerdiscountgroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesFamily")]
        public erp_customerdiscountgroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationPrinters One <> Many ArticleFamily
        private sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticleFamily")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue("Printer", ref fPrinter, value); }
        }

        //ArticleFamily One <> Many ConfigurationPrintersTemplates
        private sys_configurationprinterstemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleFamily")]
        public sys_configurationprinterstemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue("Template", ref fTemplate, value); }
        }

        //Family One <> Many Article
        [Association(@"FamilyReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }

        //Family One <> Many SubFamily
        [Association(@"FamilyReferencesSubFamily", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> SubFamily
        {
            get { return GetCollection<fin_articlesubfamily>("SubFamily"); }
        }
    }
}