﻿using System;
using DevExpress.Xpo;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentorderticket : Entity
    {
        public fin_documentorderticket() : base() { }
        public fin_documentorderticket(Session session) : base(session) { }

        private int fTicketId;
        public int TicketId
        {
            get { return fTicketId; }
            set { SetPropertyValue<int>("TicketId", ref fTicketId, value); }
        }

        private DateTime fDateStart;
        public DateTime DateStart
        {
            get { return fDateStart; }
            set { SetPropertyValue<DateTime>("DateStart", ref fDateStart, value); }
        }

        private PriceType fPriceType;
        public PriceType PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue("PriceType", ref fPriceType, value); }
        }

        private decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        //DocumentOrderTicket One <> Many DocumentOrderDetail
        [Association(@"DocumentOrderTicketReferencesDocumentOrderDetail", typeof(fin_documentorderdetail))]
        public XPCollection<fin_documentorderdetail> OrderDetail
        {
            get { return GetCollection<fin_documentorderdetail>("OrderDetail"); }
        }

        //DocumentOrderMain One <> Many DocumentOrderTicket
        private fin_documentordermain fOrderMain;
        [Association(@"DocumentOrderMainReferencesDocumentOrderTicket")]
        public fin_documentordermain OrderMain
        {
            get { return fOrderMain; }
            set { SetPropertyValue("OrderMain", ref fOrderMain, value); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderTicket
        private pos_configurationplacetable fPlaceTable;
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderTicket")]
        public pos_configurationplacetable PlaceTable
        {
            get { return fPlaceTable; }
            set { SetPropertyValue("PlaceTable", ref fPlaceTable, value); }
        }
    }
}