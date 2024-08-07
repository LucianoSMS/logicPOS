﻿using DevExpress.Xpo;
using LogicPOS.Domain.Enums;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentordermain : Entity
    {
        public fin_documentordermain() : base() { }
        public fin_documentordermain(Session session) : base(session) { }

        private DateTime fDateStart;
        public DateTime DateStart
        {
            get { return fDateStart; }
            set { SetPropertyValue<DateTime>("DateStart", ref fDateStart, value); }
        }

        private OrderStatus fOrderStatus;
        public OrderStatus OrderStatus
        {
            get { return fOrderStatus; }
            set { SetPropertyValue("OrderStatus", ref fOrderStatus, value); }
        }

        //DocumentOrderMain One <> Many DocumentOrderTicket
        [Association(@"DocumentOrderMainReferencesDocumentOrderTicket", typeof(fin_documentorderticket))]
        public XPCollection<fin_documentorderticket> OrderTicket
        {
            get { return GetCollection<fin_documentorderticket>("OrderTicket"); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderMain
        private pos_configurationplacetable fPlaceTable;
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderMain")]
        public pos_configurationplacetable PlaceTable
        {
            get { return fPlaceTable; }
            set { SetPropertyValue("PlaceTable", ref fPlaceTable, value); }
        }
    }
}