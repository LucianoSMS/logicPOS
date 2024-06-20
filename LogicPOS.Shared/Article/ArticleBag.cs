﻿using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.Orders;
using System;
using System.Collections.Generic;

namespace LogicPOS.Shared.Article
{
    public class ArticleBag : Dictionary<ArticleBagKey, ArticleBagProperties>
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public decimal DiscountGlobal { get; set; } = 0.0m;

        public decimal TotalQuantity { get; set; } = 0.0m;

        public decimal TotalNet { get; set; } = 0;

        public decimal TotalGross { get; set; } = 0;

        public decimal TotalDiscount { get; set; } = 0;

        public decimal TotalTax { get; set; } = 0;

        public decimal TotalFinal { get; set; } = 0;

        public Dictionary<decimal, TaxBagProperties> TaxBag { get; set; } = new Dictionary<decimal, TaxBagProperties>();

        //New Override Dictionary EqualityComparer
        public ArticleBag()
            : base(new ArticleBagKeyEqualityComparer())
        {
            DiscountGlobal = POSSession.GetGlobalDiscount();
        }

        public ArticleBag(decimal pDiscountGlobal)
            : base(new ArticleBagKeyEqualityComparer())
        {
            //Get Discount from Parameter
            DiscountGlobal = pDiscountGlobal;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        /// <summary>
        /// Used to Update Global ArticleBag Totals and Properties
        /// </summary>
        public void UpdateTotals()
        {
            TotalNet = 0.0m;
            TotalGross = 0.0m;
            TotalTax = 0.0m;
            TotalDiscount = 0.0m;
            TotalFinal = 0.0m;
            TotalQuantity = 0.0m;

            //Require to ReCreate TaxBag else in Payment window when we change Discount, 
            //TaxBag Reflect totals without discount appyed, ex after ArticleBag costructed with one Discount and change it after construct
            TaxBag = new Dictionary<decimal, TaxBagProperties>();

            foreach (var item in this)
            {
                UpdateKeyProperties(item.Key);
                TotalNet += item.Value.TotalNet;
                TotalGross += item.Value.TotalGross;
                TotalTax += item.Value.TotalTax;
                TotalDiscount += item.Value.TotalDiscount;
                TotalFinal += item.Value.TotalFinal;
                TotalQuantity += item.Value.Quantity;

                //Required to Update TaxBag Totals
                //TaxBag Add Key
                if (!TaxBag.ContainsKey(item.Key.Vat))
                {
                    TaxBag.Add(item.Key.Vat, new TaxBagProperties(item.Key.Designation, item.Value.TotalTax, item.Value.TotalNet));
                }
                //Update Key, Add Vat
                else
                {
                    TaxBag[item.Key.Vat].Total += item.Value.TotalTax;
                    TaxBag[item.Key.Vat].TotalBase += item.Value.TotalNet;
                }
            }
        }

        /// <summary>
        /// Used to Update Article Bag Price Properties, Prices, Totals etc, All Other Fields that not are Sent via Key and Props
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="pProps"></param>
        /// <returns></returns>
        public void UpdateKeyProperties(ArticleBagKey pKey)
        {
            bool debug = false;

            //Get Fresh PriceProperties Helper Object to Calc
            PriceProperties priceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              this[pKey].Quantity,
              pKey.Discount,
              this.DiscountGlobal,
              pKey.Vat
            );

            this[pKey].PriceWithDiscount = priceProperties.PriceWithDiscount;
            this[pKey].PriceWithDiscountGlobal = priceProperties.PriceWithDiscountGlobal;
            this[pKey].TotalGross = priceProperties.TotalGross;
            this[pKey].TotalNet = priceProperties.TotalNet;
            this[pKey].TotalDiscount = priceProperties.TotalDiscount;
            this[pKey].TotalTax = priceProperties.TotalTax;
            this[pKey].TotalFinal = priceProperties.TotalFinal;
            this[pKey].PriceFinal = priceProperties.PriceFinal;

            if (debug)
            {
                priceProperties.SendToLog("");
                _logger.Debug(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                  this[pKey].Code, pKey.Designation, this[pKey].Quantity, this[pKey].TotalGross, this[pKey].PriceWithDiscount, this[pKey].PriceWithDiscountGlobal, this[pKey].TotalGross, this[pKey].TotalNet, this[pKey].TotalDiscount, this[pKey].TotalTax, this[pKey].TotalFinal));
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Add From ArticleBag Key/Props

        //NEW: Override/Replace Dictionary Add with() Custom Add()
        new public void Add(ArticleBagKey pKey, ArticleBagProperties pProps)
        {
            //Init local vars
            ArticleBagKey key = pKey;
            ArticleBagProperties props = pProps;

            //Get Fresh PriceProperties Helper Object to used for Addition (Vat and Totals)
            PriceProperties addPriceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              pProps.Quantity,
              pKey.Discount,
              this.DiscountGlobal,
              pKey.Vat
            );

            //If Key doesnt exists Add it
            if (!this.ContainsKey(key))
            {
                base.Add(key, props);
            }
            //Else Update Key, Increase Quantity
            else
            {
                this[key].Quantity += props.Quantity;
                if (!string.IsNullOrEmpty(this[key].SerialNumber) && !string.IsNullOrEmpty(props.SerialNumber))
                {
                    this[key].SerialNumber += ";" + props.SerialNumber;
                    this[key].Notes += "; " + props.SerialNumber;
                    props.SerialNumber += ";" + this[key].SerialNumber;
                }
                if (!string.IsNullOrEmpty(this[key].Warehouse) && !string.IsNullOrEmpty(props.Warehouse))
                {
                    this[key].Warehouse += ";" + props.Warehouse;
                    props.Warehouse = this[key].Warehouse;
                }

            }

            //Refresh Current Key Price Properties after Add Quantity)
            UpdateKeyProperties(key);

            //TaxBag Add Key
            if (!TaxBag.ContainsKey(key.Vat))
            {
                //Get Designation from Key
                //Get VatRate formated for filter, in sql server gives error without this it filters 23,0000 and not 23.0000 resulting in null vatRate
                string sql = $"SELECT Designation FROM fin_configurationvatrate WHERE VALUE = '{Utility.DataConversionUtils.DecimalToString(key.Vat)}'";
                string designation = XPOSettings.Session.ExecuteScalar(sql).ToString();
                //Now Add New Key with Designation
                TaxBag.Add(key.Vat, new TaxBagProperties(designation, addPriceProperties.TotalTax, addPriceProperties.TotalNet));
            }
            //Update Key, Add Vat
            else
            {
                TaxBag[key.Vat].Total += addPriceProperties.TotalTax;
                TaxBag[key.Vat].TotalBase += addPriceProperties.TotalNet;
            }

            TotalQuantity += addPriceProperties.Quantity;
            TotalNet += addPriceProperties.TotalNet;
            TotalGross += addPriceProperties.TotalGross;
            TotalTax += addPriceProperties.TotalTax;
            TotalDiscount += addPriceProperties.TotalDiscount;
            TotalFinal += addPriceProperties.TotalFinal;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Used to Remove PartialPayments from ArticleBag
        public void Remove(ArticleBagKey pKey, decimal pRemoveQuantity)
        {
            //Get PriceProperties Helper Object to Remove from current Key
            PriceProperties removePriceProperties = PriceProperties.GetPriceProperties(
              PricePropertiesSourceMode.FromPriceNet,
              false,
              pKey.Price,
              pRemoveQuantity,
              pKey.Discount,
              this.DiscountGlobal,
              pKey.Vat
            );

            //Decrease Quantity
            this[pKey].Quantity -= pRemoveQuantity;

            // SplitPayment : Sometimes we get 0.000000000000001, that makes key dont be removed because its not < 0
            // To prevent this we must round value before compare using DecimalFormatStockQuantity
            string roundedFormat = $"{{0:{CultureSettings.DecimalFormatStockQuantity}}}";//{0:0.00000000}
            decimal roundedQuantity = Convert.ToDecimal(string.Format(roundedFormat, this[pKey].Quantity));

            //if (this[pKey].Quantity <= 0)
            if (roundedQuantity <= 0)
            {
                this.Remove(pKey);
            }
            else
            {
                //Refresh Current Key Price Properties after Add Quantity)
                UpdateKeyProperties(pKey);
            }

            //Calc Article Grand Totals
            TotalQuantity -= removePriceProperties.Quantity;
            TotalNet -= removePriceProperties.TotalNet;
            TotalGross -= removePriceProperties.TotalGross;
            TotalTax -= removePriceProperties.TotalTax;
            TotalDiscount -= removePriceProperties.TotalDiscount;
            TotalFinal -= removePriceProperties.TotalFinal;

            //TaxBag Update 
            TaxBag[pKey.Vat].Total -= removePriceProperties.TotalTax;
            TaxBag[pKey.Vat].TotalBase -= removePriceProperties.TotalNet;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Add From Article
        public void Add(fin_article pArticle, Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, decimal pQuantity)
        {
            Add(pArticle, pPlaceOid, pTableOid, pPriceType, pQuantity, null);
        }

        public void Add(fin_article pArticle, Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, decimal pQuantity, fin_configurationvatexemptionreason pVatExemptionReason)
        {
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

            //Get Place Object to extract TaxSellType Normal|TakeWay
            pos_configurationplace configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), pPlaceOid);
            TaxSellType taxSellType = (configurationPlace.MovementType.VatDirectSelling) ? TaxSellType.TakeAway : TaxSellType.Normal;

            PriceProperties priceProperties = ArticleUtils.GetArticlePrice(pArticle, taxSellType);

            //Prepare articleBag Key and Props
            articleBagKey = new ArticleBagKey(
              pArticle.Oid,
              pArticle.Designation,
              priceProperties.PriceNet,
              priceProperties.DiscountArticle,
              priceProperties.Vat,
              pVatExemptionReason.Oid
            );
            articleBagProps = new ArticleBagProperties(
              pPlaceOid,
              pTableOid,
              pPriceType,
              pArticle.Code,
              pQuantity,
              pArticle.UnitMeasure.Acronym
            );

            //Send to Bag
            Add(articleBagKey, articleBagProps);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Get ArticleBag Articles Splitted by ArticleClass ex Totals Class P,S,O,I

        public Dictionary<string, decimal> GetClassTotals()
        {
            bool debug = false;
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            fin_article article;

            try
            {
                foreach (var item in this)
                {
                    article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), item.Key.ArticleId);
                    if (!result.ContainsKey(article.Class.Acronym))
                    {
                        result.Add(article.Class.Acronym, item.Value.TotalFinal);
                    }
                    else
                    {
                        result[article.Class.Acronym] += item.Value.TotalFinal;
                    }
                    if (debug) _logger.Debug(string.Format("Acronym: [{0}], TotalFinal : [{1}], ClassTotalFinal: [{2}]", article.Class.Acronym, item.Value.TotalFinal, result[article.Class.Acronym]));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public decimal GetClassTotals(string pClassAcronym)
        {
            decimal result = 0.0m;

            try
            {
                Dictionary<string, decimal> articleBagClassTotals = GetClassTotals();
                if (articleBagClassTotals.ContainsKey(pClassAcronym))
                {
                    result = articleBagClassTotals[pClassAcronym];
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Used to Remove Articles from DocumentOrder ex when we Delete Article From TicketList.OrderMain Details

        public decimal DeleteFromDocumentOrder(ArticleBagKey pKey, decimal pRemoveQuantity)
        {
            bool isDone = false;
            decimal resultRemainQuantity = 0;
            string where = string.Empty;
            fin_documentorderdetail deleteOrderDetail = null;
            string articleDesignation = string.Empty;

            //Start UnitOfWork
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                fin_documentordermain xDocumentOrderMain = XPOUtility.GetEntityById<fin_documentordermain>(orderMain.PersistentOid,unitOfWork);

                if (xDocumentOrderMain != null && xDocumentOrderMain.OrderTicket != null)
                {
                    foreach (fin_documentorderticket ticket in xDocumentOrderMain.OrderTicket)
                    {
                        foreach (fin_documentorderdetail detail in ticket.OrderDetail)
                        {
                            try
                            {
                                //Check Equal Key
                                if (pKey.ArticleId == detail.Article.Oid && pKey.Price == detail.Price && pKey.Discount == detail.Discount && pKey.Vat == detail.Vat)
                                {
                                    articleDesignation = pKey.Designation;
                                    resultRemainQuantity += detail.Quantity;
                                    if (!isDone)
                                    {
                                        detail.Quantity -= pRemoveQuantity;
                                        //Assign references to Future Deletes
                                        if (detail.Quantity <= 0) { deleteOrderDetail = detail; }
                                        isDone = true;
                                    }
                                    else
                                    {
                                        where += string.Format(" OR Oid = '{0}'", detail.Oid);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.Message, ex);
                            }
                        }
                    }
                }

                //Debug
                //string sql = @"SELECT * FROM fin_documentorderdetail WHERE 1=0{0};";
                //_logger.Debug(string.Format("Delete(): sql [{0}]", string.Format(sql, where)));

                //Audit
                XPOUtility.Audit("ORDER_ARTICLE_REMOVED", string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_order_article_removed"),
                        articleDesignation,
                        1,
                        resultRemainQuantity - 1,
                        XPOSettings.LoggedUser.Name
                    )
                );

                if (isDone)
                {
                    //Update xDocumentOrderMain UpdatedAt, Required for RealTime Update
                    xDocumentOrderMain.UpdatedAt = XPOUtility.CurrentDateTimeAtomic();

                    //Remove Quantity
                    resultRemainQuantity -= pRemoveQuantity;

                    //Delete Records, OrderMain, OrderTicket and OrderDetails
                    if (deleteOrderDetail != null)
                    {
                        fin_documentorderticket deleteOrderTicket = deleteOrderDetail.OrderTicket;
                        //Store Reference to Future delete Object (After foreach Loop)
                        fin_documentordermain deleteOrderMain = deleteOrderTicket.OrderMain;

                        //Delete Details
                        deleteOrderDetail.Delete();

                        //Check if OrderTicket in Empty, If so Delete it, its not required anymore
                        if (deleteOrderTicket.OrderDetail.Count <= 0)
                        {
                            deleteOrderTicket.Delete();
                        };

                        //Check if OrderMain in Empty, If so Delete it, its not required anymore
                        if (deleteOrderMain.OrderTicket.Count <= 0)
                        {
                            //Before Delete OrderMain, we must UnAssign DocumentMaster SourceOrderMain else we have a CONSTRAINT ERROR on FK_DocumentFinanceMaster_SourceOrderMain trying to delete used OrderMain
                            string sql = string.Format(@"UPDATE fin_documentfinancemaster SET SourceOrderMain = NULL WHERE SourceOrderMain = '{0}';", deleteOrderMain.Oid);
                            unitOfWork.ExecuteScalar(sql);
                            //Open Table
                            deleteOrderMain.PlaceTable.TableStatus = TableStatus.Free;
                            //Audit
                            XPOUtility.Audit("TABLE_OPEN", string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_table_open"), deleteOrderMain.PlaceTable.Designation));
                            //Delete OrderMain
                            deleteOrderMain.Delete();
                        };
                    };
                };

                try
                {
                    //Commit UOW Changes
                    unitOfWork.CommitChanges();
                    //Update OrderMain UpdatedAt, Required to Sync Terminals
                    orderMain.UpdatedAt = XPOUtility.CurrentDateTimeAtomic();

                    //Update ArticleBag Price Properties
                    this[pKey].Quantity = resultRemainQuantity;
                    UpdateKeyProperties(pKey);

                    //SEARCH#001
                    //Require to Remove PartialPayed Items Quantity
                    return resultRemainQuantity - XPOUtility.GetPartialPaymentPayedItems(unitOfWork, xDocumentOrderMain.Oid, pKey.ArticleId);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    unitOfWork.RollbackTransaction();
                    return -1;
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public void ShowInLog()
        {
            _logger.Debug("\tCode\tDesignation\tQuantity\tPriceUser\tDiscount\tVat\tPriceNet\tPriceWithDiscount\tPriceWithDiscountGlobal\tTotalNet\tTotalGross\tTotalDiscount\tTotalTax\tTotalFinal\tPriceFinal");
            foreach (var item in this)
            {
                _logger.Debug(string.Format("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}",
                    item.Value.Code,
                    item.Key.Designation,
                    item.Value.Quantity,
                    string.Empty,//PriceUser no Used
                    item.Key.Discount,
                    item.Key.Vat,
                    item.Key.Price,
                    item.Value.PriceWithDiscount,
                    item.Value.PriceWithDiscountGlobal,
                    item.Value.TotalNet,
                    item.Value.TotalGross,
                    item.Value.TotalDiscount,
                    item.Value.TotalTax,
                    item.Value.TotalFinal,
                    item.Value.PriceFinal
                  ));
            }
            //TaxBag
            _logger.Debug("\tVat\tTotal");
            foreach (var item in this.TaxBag)
            {
                _logger.Debug(string.Format("\t{0}\t{1}", item.Key, item.Value));
            }
            //Totals
            _logger.Debug("\tTotalItems\tTotalNet\tTotalGross\tTotalDiscount\tTotalTax\tTotalFinal");
            _logger.Debug(string.Format("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}", TotalQuantity, TotalNet, TotalGross, TotalDiscount, TotalTax, TotalFinal));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Methods

        //Create ArticleBag From OrderMain.OrderTicket, and Discount PartialPayments for Working OrderMain
        public static ArticleBag TicketOrderToArticleBag(OrderMain pOrderMain)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            bool debug = false;

            //OrderMain
            OrderMain orderMain = pOrderMain;
            //ArticleBag
            ArticleBag articleBag = new ArticleBag();
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

            //Removed, gives problems, Avoid used DropIdentityMap
            //XPOSettings.Session.DropIdentityMap();

            try
            {
                if (orderMain.PersistentOid != Guid.Empty)
                {
                    string sqlOrders = string.Format(@"
                        SELECT 
                            dmOid AS DocumentOrderMain, ddArticle AS Article, ddDesignation AS Designation,ddPrice AS Price,ddDiscount AS Discount,ddVat AS Vat,ddVatExemptionReason AS VatExemptionReason,
                            cpOid AS ConfigurationPlace, ctOid AS ConfigurationPlaceTable, dtPriceType AS PriceType, ddCode AS Code, ddQuantity AS Quantity, ddUnitMeasure AS UnitMeasure,
                            ddToken1 as Token1, ddToken2 as Token2
                        FROM 
                            view_orders 
                        WHERE 
                            dmOid = '{0}'
                        ORDER BY 
                            dtTicketId
                        ;"
                        , orderMain.PersistentOid
                    );
                    SQLSelectResultData selectedDataOrders = XPOUtility.GetSelectedDataFromQuery(sqlOrders);

                    //Process Tickets and Add to ArticleBag
                    if (selectedDataOrders.DataRows.Length > 0)
                    {
                        foreach (SelectStatementResultRow row in selectedDataOrders.DataRows)
                        {
                            //Proteção para artigos do tipo "Sem Preço" [IN:013329]
                            //First check if article have price
                            //if (Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndex("Price")]) > 0.0m)
                            //{
                            //Generate Key/Props
                            articleBagKey = new ArticleBagKey(
                                new Guid(row.Values[selectedDataOrders.GetFieldIndexFromName("Article")].ToString()),                                   //ticketLine.Article.Oid
                                Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Designation")]),                                  //ticketLine.Designation
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Price")]),                                       //ticketLine.Price
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Discount")]),                                    //ticketLine.Discount
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Vat")])                                          //ticketLine.Vat
                            );

                            articleBagProps = new ArticleBagProperties(
                                new Guid(row.Values[selectedDataOrders.GetFieldIndexFromName("ConfigurationPlace")].ToString()),                        //ticket.PlaceTable.Place.Oid
                                new Guid(row.Values[selectedDataOrders.GetFieldIndexFromName("ConfigurationPlaceTable")].ToString()),                   //ticket.PlaceTable.Oid
                                (PriceType)Enum.Parse(typeof(PriceType), row.Values[selectedDataOrders.GetFieldIndexFromName("PriceType")].ToString()), //ticket.PriceType
                                Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Code")]),                                         //ticketLine.Code
                                Convert.ToDecimal(row.Values[selectedDataOrders.GetFieldIndexFromName("Quantity")]),                                    //ticketLine.Quantity
                                Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("UnitMeasure")])                                   //ticketLine.UnitMeasure
                            );

                            //Detect and Assign VatExemptionReason
                            if (row.Values[selectedDataOrders.GetFieldIndexFromName("VatExemptionReason")] != null
                                && Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("VatExemptionReason")]) != Guid.Empty.ToString()
                            )
                            {
                                //Add VatException Reason to Key
                                articleBagKey.VatExemptionReasonOid = new Guid(Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("VatExemptionReason")]));
                            }

                            //Tokens
                            articleBagProps.Token1 = Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Token1")]); //ticketLine.Token1
                            articleBagProps.Token2 = Convert.ToString(row.Values[selectedDataOrders.GetFieldIndexFromName("Token2")]); //ticketLine.Token2

                            //articleBagProps.SerialNumber = Convert.ToString(row.Values[selectedDataOrders.GetFieldIndex("SerialNumber")]); //SerialNumber

                            //Send to Bag
                            if (articleBagProps.Quantity > 0)
                            {
                                articleBag.Add(articleBagKey, articleBagProps);
                            }

                            //if (debug) log.Debug(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", ticket.PlaceTable.Place.Oid, ticket.PlaceTable.Designation, ticket.PriceType, ticketLine.Article.Oid, ticketLine.Code, ticketLine.Designation, ticketLine.Price, ticketLine.Quantity, ticketLine.UnitMeasure, ticketLine.Discount, ticketLine.Vat));
                            //}

                        }
                    }

                    //Process PartialPayed Items and Remove From ArticleBag
                    string sqlDocuments = string.Format(@"
                        SELECT 
                            ftOid AS DocumentType,fdArticle AS Article,fdDesignation AS Designation,fdPrice AS Price,fdQuantity AS Quantity, fdDiscount AS Discount, fdVat AS Vat, fdVatExemptionReason AS VatExemptionReason
                        FROM 
                            view_documentfinance 
                        WHERE 
                            fmSourceOrderMain = '{0}'
                        ORDER BY 
                            ftOid,fmOid
                        ;"
                        , orderMain.PersistentOid
                    );

                    SQLSelectResultData selectedDataDocuments = XPOUtility.GetSelectedDataFromQuery(sqlDocuments);
                    if (selectedDataDocuments.DataRows.Length > 0)
                    {
                        foreach (SelectStatementResultRow row in selectedDataDocuments.DataRows)
                        {
                            // If Not ConferenceDocument or TableConsult
                            //Proteção para artigos do tipo "Sem Preço" [IN:013329]
                            //First check if article have price
                            //&& (Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndex("Price")]) > 0.0m)
                            if (row.Values[selectedDataDocuments.GetFieldIndexFromName("DocumentType")].ToString() != DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument.ToString()
                                && row.Values[selectedDataDocuments.GetFieldIndexFromName("Price")] != null)
                            {

                                //Generate Key/Props
                                articleBagKey = new ArticleBagKey(
                                    new Guid(row.Values[selectedDataDocuments.GetFieldIndexFromName("Article")].ToString()),
                                    Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndexFromName("Designation")]),
                                    Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Price")]),
                                    Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Discount")]),
                                    Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Vat")])
                                );
                                //Detect and Assign VatExemptionReason
                                if (row.Values[selectedDataDocuments.GetFieldIndexFromName("VatExemptionReason")] != null
                                    && Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndexFromName("VatExemptionReason")]) != Guid.Empty.ToString()
                                )
                                {
                                    //Add VatException Reason to Key
                                    articleBagKey.VatExemptionReasonOid = new Guid(Convert.ToString(row.Values[selectedDataDocuments.GetFieldIndexFromName("VatExemptionReason")]));
                                }
                                if (articleBag.ContainsKey(articleBagKey))
                                {
                                    //Remove PartialPayed Item Quantity from ArticleBag
                                    articleBag.Remove(articleBagKey, Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Quantity")]));
                                }
                                else
                                {
                                    //Protecções de integridade das BD's e funcionamento da aplicação [IN:013327]
                                    //Remove PartialPayed Item Quantity from ArticleBag that price was changed
                                    foreach (var article in articleBag)
                                    {
                                        if (article.Key.ArticleId == articleBagKey.ArticleId)
                                        {
                                            //Remove PartialPayed Item Quantity from ArticleBag
                                            articleBag.Remove(article.Key, Convert.ToDecimal(row.Values[selectedDataDocuments.GetFieldIndexFromName("Quantity")]));
                                        }
                                    }
                                    if (debug) log.Debug(string.Format("articleBagKey: [{0}]", articleBagKey));
                                }
                            }
                        }
                    }
                    if (debug) articleBag.ShowInLog();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return articleBag;
        }

        //Create ArticleBag From DocumentFinanceMaster
        public static ArticleBag DocumentFinanceMasterToArticleBag(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Init Global ArticleBag
            ArticleBag articleBag = new ArticleBag();
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

            try
            {
                if (pDocumentFinanceMaster != null
                    && pDocumentFinanceMaster.DocumentDetail != null
                    && pDocumentFinanceMaster.DocumentDetail.Count > 0
                )
                {
                    foreach (fin_documentfinancedetail detail in pDocumentFinanceMaster.DocumentDetail)
                    {
                        //Prepare articleBag Key and Props
                        articleBagKey = new ArticleBagKey(
                          detail.Article.Oid,
                          detail.Designation,
                          detail.Price,
                          detail.Discount,
                          detail.Vat
                        );
                        articleBagProps = new ArticleBagProperties(
                          detail.DocumentMaster.SourceOrderMain.PlaceTable.Place.Oid,
                          detail.DocumentMaster.SourceOrderMain.PlaceTable.Oid,
                          (PriceType)detail.DocumentMaster.SourceOrderMain.PlaceTable.Place.PriceType.EnumValue,
                          detail.Code,
                          detail.Quantity,
                          detail.UnitMeasure
                        );
                        //Send to Bag
                        articleBag.Add(articleBagKey, articleBagProps);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return articleBag;
        }
    }
}
