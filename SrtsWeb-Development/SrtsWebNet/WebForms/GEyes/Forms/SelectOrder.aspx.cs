using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.GEyes;
using SrtsWeb.Views.GEyes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GEyes.Forms
{
    public partial class SelectOrder : PageBase, ISelectOrderView
    {
        private SelectOrderPresenter _presenter;

        public SelectOrder()
        {
            _presenter = new SelectOrderPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = myInfo.Patient.Individual.NameFiL;
            Session["OrderSubmitted"] = null;

            //if (myInfo != null)
            //{
            //    if (myInfo.SecurityAcknowledged)
            //    {
            //        Master._MainPageFooter.Visible = true;
            //    }
            //}
            //else
            //{
            //    Response.Redirect("~/WebForms/Account/Login.aspx");
            //}

            if (myInfo.IsNull())
                Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");

            if (!IsPostBack)
            {
                _presenter.InitView();
                OrdersSortData = _ordersData;

                if (_ordersData != null)
                {
                    Gridview2.DataSource = OrdersSortData;
                    Gridview2.DataBind();

                }

                PanelSet = true;

                if (SearchSortDirection == null)
                {
                    SearchSortDirection = "DESC";
                }

                if (CurrentSortField == null)
                {
                    CurrentSortField = "OrderNumber";
                }
            }
            else
            {
                if (myInfo == null)
                {
                    Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
                }
            }
        }



        //protected void gvOrders_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    _presenter.FillSelection(gvOrders.SelectedDataKey.Value.ToString());
        //    PanelSet = false;
        //    tbComment.Focus();
        //}


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                _presenter.SetData();
                Response.Redirect("~/WebForms/GEyes/Forms/AddressUpdate.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlDisplay.Visible = true;
            pnlSelected.Visible = false;
        }

        protected void Orders_Sorting(object sender, GridViewSortEventArgs e)
        {
            string SortField = e.SortExpression;

            if (SortField != CurrentSortField)
            {
                SearchSortDirection = "DESC";
            }

            switch (SortField)
            {
                case "OrderNumber":
                    if (SearchSortDirection == "DESC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o1.OrderNumber.CompareTo(o2.OrderNumber);
                        });
                        SearchSortDirection = "ASC";
                        CurrentSortField = SortField;
                    }
                    else if (SearchSortDirection == "ASC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o2.OrderNumber.CompareTo(o1.OrderNumber);
                        });
                        SearchSortDirection = "DESC";
                        CurrentSortField = SortField;
                    }
                    break;

                case "FrameDescription":
                    if (SearchSortDirection == "DESC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o1.FrameDescription.CompareTo(o2.FrameDescription);
                        });
                        SearchSortDirection = "ASC";
                        CurrentSortField = SortField;
                    }
                    else if (SearchSortDirection == "ASC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o2.FrameDescription.CompareTo(o1.FrameDescription);
                        });
                        SearchSortDirection = "DESC";
                        CurrentSortField = SortField;
                    }
                    break;

                case "LensTint":
                    if (SearchSortDirection == "DESC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o1.LensTint.CompareTo(o2.LensTint);
                        });
                        SearchSortDirection = "ASC";
                        CurrentSortField = SortField;
                    }
                    else if (SearchSortDirection == "ASC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o2.LensTint.CompareTo(o1.LensTint);
                        });
                        SearchSortDirection = "DESC";
                        CurrentSortField = SortField;
                    }
                    break;

                case "LensCoating":
                    if (SearchSortDirection == "DESC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o1.LensCoating.CompareTo(o2.LensCoating);
                        });
                        SearchSortDirection = "ASC";
                        CurrentSortField = SortField;
                    }
                    else if (SearchSortDirection == "ASC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o2.LensCoating.CompareTo(o1.LensCoating);
                        });
                        SearchSortDirection = "DESC";
                        CurrentSortField = SortField;
                    }
                    break;

                case "LensTypeLong":
                    if (SearchSortDirection == "DESC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o1.LensTypeLong.CompareTo(o2.LensTypeLong);
                        });
                        SearchSortDirection = "ASC";
                        CurrentSortField = SortField;
                    }
                    else if (SearchSortDirection == "ASC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o2.LensTypeLong.CompareTo(o1.LensTypeLong);
                        });
                        SearchSortDirection = "DESC";
                        CurrentSortField = SortField;
                    }
                    break;

                case "DateCreated":
                    if (SearchSortDirection == "DESC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o1.DateCreated.CompareTo(o2.DateCreated);
                        });
                        SearchSortDirection = "ASC";
                        CurrentSortField = SortField;
                    }
                    else if (SearchSortDirection == "ASC")
                    {
                        OrdersSortData.Sort(delegate(OrderDisplayEntity o1, OrderDisplayEntity o2)
                        {
                            return o2.DateCreated.CompareTo(o1.DateCreated);
                        });
                        SearchSortDirection = "DESC";
                        CurrentSortField = SortField;
                    }
                    break;

                default:

                    break;
            }

            bind_gvOrders();
        }

        private void bind_gvOrders()
        {
            //gvOrders.DataSource = OrdersSortData;
            //gvOrders.DataBind();

            Gridview2.DataSource = OrdersSortData;
            Gridview2.DataBind();
        }

        protected void ValidateCommentFormat(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length > 0)
            {
                int limit = 90;
                args.IsValid = args.Value.ValidateCommentLength(limit);
                args.IsValid = args.Value.ValidateCommentFormat();
            }
        }

        public string SearchSortDirection
        {
            get { return (string)ViewState["SearchSortDirection"]; }
            set { ViewState.Add("SearchSortDirection", value); }
        }

        public string CurrentSortField
        {
            get { return (string)ViewState["CurrentSortField"]; }
            set { ViewState.Add("CurrentSortField", value); }
        }

        private List<OrderDisplayEntity> _ordersData;

        public List<OrderDisplayEntity> OrdersData
        {
            get { return _ordersData; }
            set
            {
                _ordersData = value;
                //gvOrders.DataSource = _ordersData;
                //gvOrders.DataBind();

                Gridview2.DataSource = _ordersData;
                Gridview2.DataBind();

            }
        }

        public GEyesSession myInfo
        {
            get { return (GEyesSession)Session["MyInfo"]; }
            set { Session["MyInfo"] = value; }
        }

        private bool PanelSet
        {
            set
            {
                pnlDisplay.Visible = value;
                pnlSelected.Visible = !value;
            }
        }

        public string LensTint
        {
            get { return tbLensTint.Text; }
            set { tbLensTint.Text = value; }
        }

        public string LensCoating
        {
            get { return tbLensCoating.Text; }
            set { tbLensCoating.Text = value; }
        }

        public string LensTypeLong
        {
            get { return tbLensType.Text; }
            set { tbLensType.Text = value; }
        }

        public string Comment
        {
            get { return tbComment.Text; }
            set { tbComment.Text = value; }
        }

        public string FrameDescription
        {
            get { return tbFrameDesc.Text; }
            set { tbFrameDesc.Text = value; }
        }

        public List<OrderDisplayEntity> OrdersSortData
        {
            get
            {
                return (List<OrderDisplayEntity>)ViewState["OrdersSortData"];
            }
            set
            {
                ViewState.Add("OrdersSortData", value);
            }
        }

        protected void Gridview2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                _presenter.FillSelection(e.CommandArgument.ToString());
                PanelSet = false;
                //tbComment.Focus();
            }
        }

        protected void Gridview2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                OrderDisplayEntity entity = (OrderDisplayEntity)e.Row.DataItem;              
                if (!String.IsNullOrEmpty(entity.FrameDescription))
                {
                    entity.FrameDescription = Helpers.ToTitleCase(entity.FrameDescription.ToLower());
                }
                if (!String.IsNullOrEmpty(entity.LensTypeLong))
                {
                    entity.LensTypeLong = Helpers.ToTitleCase(entity.LensTypeLong.ToLower());
                }
                if (!String.IsNullOrEmpty(entity.LensTint))
                {
                    entity.LensTint = Helpers.ToTitleCase(entity.LensTint.ToLower());
                }
                else
                {
                    entity.LensCoating = "No Tint";
                }
                if (!String.IsNullOrEmpty(entity.LensCoating))
                {
                    entity.LensCoating = Helpers.ToTitleCase(entity.LensCoating.ToLower());
                }
                else
                {
                    entity.LensCoating = "No Coating";
                }
            }
        }
    }
}