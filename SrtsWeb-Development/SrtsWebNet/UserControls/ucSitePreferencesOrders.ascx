<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesOrders.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesOrders" %>
<!-- Site Preferences - Orders Defaults -->
<style type="text/css">
    .preferenceLabel {
        text-align: left;
        margin: 5px 0px;
    }

    .errorStyle {
        color: red;
    }
    /*.successStyle {
        color: green;
    }

    .gvGroupDisabled{
       /*Google's default disabled colors*/
       /*background-color: rgb(217,217,217);
       color: rgb(163,163,163);
       cursor: not-allowed;
    }*/

    .buttonSection {
        clear: both;
        float: right;
        margin: 10px 0px;
    }
</style>
<%--<asp:ScriptManagerProxy ID="ucSitePreferenceOrdersJS" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/Scripts/Validation/TextBoxValidation.js" />
    </Scripts>
</asp:ScriptManagerProxy>--%>

<asp:UpdatePanel ID="upOrderPreferences" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessOrders" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgOrders" runat="server" Value="" ClientIDMode="Static" />

        <%--  Initial  Order Load Preferences--%>
        <div class="w3-row" style="margin-top: -30px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Initial Order Load Preferences</span>
                    <h1 style="float: right; padding-right: 20px">These preferences will autopopulate the order when creating a new order.</h1>
                </div>
                <div class="BeigeBoxContent padding" style="padding-top: 10px; margin-top: 0px; margin-left: 10px; height: auto">
                    <%-- Setup a place for initial order preference, i.e. Priority, Frame, and Frame Items.  Allow users to select if they want to use the frame item preferences set elsewhere in this management page. --%>
                    <div id="divInitialLoadPreferences">
                        <div id="divInitialLoad1">
                            <div class="w3-col" style="width: 200px">
                                <asp:Label ID="ilLabel1" runat="server" CssClass="srtsLabel_medium" Text="Select Priority:"></asp:Label><br />
                                <asp:DropDownList ID="ddlInitialPriority" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium"
                                    OnSelectedIndexChanged="ddlInitialPriority_SelectedIndexChanged" Width="175px" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="w3-rest">
                                <asp:Label ID="ilLabel2" runat="server" CssClass="srtsLabel_medium" Text="Select Frame:"></asp:Label><br />
                                <asp:DropDownList ID="ddlInitialFrame" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="600px"></asp:DropDownList>
                            </div>
                        </div>
                        <div id="divInitialLoadPreferencesError" class="errorStyle"></div>
                    </div>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>

        <%--  Global Order Preferences--%>
        <div class="w3-row" style="margin-top: 5px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Global Order Preferences</span>
                    <h1 style="float: right; padding-right: 20px">The disbursement method and lab selection will be global to all new orders on load of the screen.</h1>
                </div>
                <div class="BeigeBoxContent padding" style="padding-top: 10px; margin-top: 0px; margin-left: 10px; height: auto">
                    <%-- Setup a place to setup a preference for each priority, i.e. Then initial selection for S would be one set of data and the initial selection for R would be some other data. --%>
                    <div id="divGlobalPreferences" class="preferenceSection">
                        <div id="divGlobalPreferencesError" class="errorStyle"></div>
                        <div id="divGlobal1" class="preferenceSection">
                            <div class="w3-col" style="width: 200px">
                                <asp:Label ID="goLabel2" runat="server" CssClass="srtsLabel_medium" Text="Select Distribution:"></asp:Label><br />
                                <asp:DropDownList ID="ddlGlobalDistMethod" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="175px"></asp:DropDownList>
                            </div>
                            <div class="w3-rest">
                                <div class="w3-col" style="width: 170px">
                                    <asp:Label ID="goLabel3" runat="server" CssClass="srtsLabel_medium" Text="Select Lab:"></asp:Label><br />
                                    <asp:DropDownList ID="ddlGlobalLab" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="150px"></asp:DropDownList>
                                    <div style="margin-top: 28px;">
                                        <asp:Label ID="lblLabDiff" runat="server" Style="color: red;"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>
        
         <%-- Clinic Group Preferences
        <div class="w3-row" style="margin-top: 5px">               
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align:left;position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="GroupPreferencesLabel">Group Preferences</span> 
                    <h1 style="float:right;padding-right:20px">These preferences will allow the admin to set up groups for this site.</b></h1>
                </div>
                <div class="BeigeBoxContent padding" style="padding-top:10px;margin-top: 0px; margin-left: 10px; height: auto">
                    <div id="divGroupPreferences" class="preferenceSection">
                        <div id="divAddClinicGroupError" class="errorStyle" runat="server"></div>
                        <div id="divAddClinicGroupSuccess" class="successStyle" runat="server"></div>
                        <div class="w3-col m12">
                            <div class="w3-col m3">
                                <%--<asp:Label ID="cbEnableAllGroupsLabel" runat="server" CssClass="srtsLabel_medium" Text="Enable All Groups:"></asp:Label>--%>
                               <%-- <asp:Button ID="bEnableAllGroups" runat="server" Text="Enable All Groups" ClientIDMode="Static" OnClick="bAllGroupsStatus_Clicked" CssClass="srtsButton" CausesValidation="false" style="margin-bottom:0;" useSubmitBehavior="false" />
                                <asp:Button ID="bDisableAllGroups" runat="server" Text="Disable All Groups" ClientIDMode="Static"  OnClick="bAllGroupsStatus_Clicked" CssClass="srtsButton" CausesValidation="false" useSubmitBehavior="false" />
                                <%--<asp:Checkbox runat="server" ID="cbEnableAllGroupsCheckbox" ClientIDMode="Static" CssClass="srtsCheckBox_medium" style="position:relative; top:2px;" OnCheckedChanged="cbEnableAllGroupsCheckbox_Clicked" AutoPostBack="true"></asp:Checkbox>--%>
                           <%-- </div>
                            <div class="w3-col m6">
                                <asp:Label ID="AddNewGroupLabel" runat="server" CssClass="srtsLabel_medium" Text="Add New Group:"></asp:Label><br />
                                <asp:TextBox runat="server" ID="GroupNameTextBox" ClientIDMode="Static" CssClass="srtsTextBox_medium w3-col m12" value="Group Name" onkeyup="KeyUpTextBoxValidate(this)" onfocus="if(this.value==this.defaultValue)this.value='';" onblur="if(this.value=='')this.value=this.defaultValue;" style="margin-left: 0; position: static;" AutoPostBack="false"></asp:TextBox><br /><br />
                                <asp:TextBox runat="server" ID="GroupDescTextBox" ClientIDMode="Static" CssClass="srtsTextBox_medium w3-col m12" value="Group Description" onkeyup="KeyUpTextBoxValidate(this)" onfocus="if(this.value==this.defaultValue)this.value='';" onblur="if(this.value=='')this.value=this.defaultValue;"  style="margin-left: 0; position: static;" AutoPostBack="false"></asp:TextBox>
                            </div>
                            <div class="w3-rest" style="padding-top: 38px; padding-left:10px;">
                                <asp:Button ID="bCreateGroupBtn" runat="server" Text="Add Group" ClientIDMode="Static" OnClick="bCreateGroupBtn_Click" CssClass="srtsButton" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                    <br />
                    <%--   Current Clinic Groups List 
                    <div id="divClinicGroupsList" runat="server" style="padding-top:80px;">
                        <div class="">
                            <asp:Label ID="phLabelGroupPreferences" runat="server" CssClass="srtsLabel_medium" Text="Current Group Preferences:"></asp:Label>
                        </div>
                        <table class="mGrid" style="margin-bottom:0;">
                            <tr >
                                <th style="width:40px;">
                                    
                                </th>
                                <th style="width:105px; text-align: left;">
                                    Name
                                </th>
                                <th style="width:150px; text-align: left;">
                                    Description
                                </th>
                            </tr>
                        </table>
                        <div class="" style="height: 200px; overflow:scroll;">
                            <asp:GridView ID="gvClinicGroups" runat="server" GridLines="Both"
                                AllowPaging="false" AutoGenerateColumns="false" CssClass="mGrid width75" AllowSorting="true"
                                AlternatingRowStyle-CssClass="alt" HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left"
                                OnRowCommand="gvClinicGroups_RowCommand" OnRowDataBound="gvClinicGroups_RowDataBound" EmptyDataText="None" ShowHeaderWhenEmpty="false" ShowHeader="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="DisableSingleGroup" Visible='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive")) == true %>' Text="Disable" CommandName="Update"  runat="server" OnClick="gvrClinicGroup_Activate"></asp:LinkButton>
                                            <asp:LinkButton ID="EnableSingleGroup" Visible='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive")) == false %>' Text="Enable" CommandName="Update" runat="server" OnClick="gvrClinicGroup_Activate"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Name" DataField="GroupName" />
                                    <asp:BoundField HeaderText="Description" DataField="GroupDesc" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>
 

        <%--  Priority Preferences--%>
           <div class="w3-row" style="margin-top: 5px">               
             <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align:left;position: relative; top: 0px; padding: 12px 10px 3px 15px">
                   <span class="label">Priority Preferences</span> 
                   <h1 style="float:right;padding-right:20px">These preferences will allow the admin to set what frame will load per priority.&nbsp; <b>*At least one is required.</b></h1>
                </div>
                <div class="BeigeBoxContent padding" style="padding-top:10px;margin-top: 0px; margin-left: 10px; height: auto">
               <%-- Setup a spot to set order level preferences, i.e. Initial priority on load will be S, or default order distribution and lab selection are set to CMTP and multivision. --%>
            <div id="divPriorityPreferences" class="preferenceSection">
                <div id="divPriorityPreferencesError" class="errorStyle" runat="server"></div>
                <div class="w3-col" style="width:200px">
                    <asp:Label ID="ppLabel1" runat="server" CssClass="srtsLabel_medium" Text="Select Priority:"></asp:Label><br />
                    <asp:DropDownList runat="server" ID="ddlPpPriority" ClientIDMode="Static" CssClass="srtsDropDown_medium"
                        OnSelectedIndexChanged="ddlPpPriority_SelectedIndexChanged" Width="175px" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="w3-rest">
                    <div class="w3-col" style="width:475px">
                    <asp:Label ID="ppLabel2" runat="server" CssClass="srtsLabel_medium" Text="Select Frame:"></asp:Label><br />
                    <asp:DropDownList runat="server" ID="ddlPpFrame" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="475px" ></asp:DropDownList>
                    </div>
                    <div class="w3-rest" style="padding-top:5px">
                    <asp:Button ID="bSetPreference" runat="server" Text="Set Preference" ClientIDMode="Static" OnClick="bSetPreference_Click" CssClass="srtsButton" CausesValidation="false" />
                    </div>
                </div>
                <br />
             <%--   Current Priority Preferences--%>
                <div id="divPriorityHistory" runat="server">
                    <div class="preferenceLabel">
                        <asp:Label ID="phLabel1" runat="server" CssClass="srtsLabel_medium" Text="Current Priority Preferences:"></asp:Label>
                    </div>
                    <div class="preferenceList">
                        <asp:GridView ID="gvPriorityHistory" runat="server" GridLines="Both"
                            AllowPaging="false" AutoGenerateColumns="false" CssClass="mGrid width75" AllowSorting="true"
                            AlternatingRowStyle-CssClass="alt" HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left"
                            OnRowCommand="gvPriorityHistory_RowCommand" EmptyDataText="None Selected" ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnbPrefSelect" Text="Select" runat="server" CommandName="Select" CausesValidation="false"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Priority" DataField="PriorityDescription" />
                                <asp:BoundField HeaderText="Frame" DataField="FrameDescription" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
      

            <%-- Setup a spot where admins can set if they want labels output in alphabetical order. --%>
            <div id="divLabelSortPreference" class="preferenceSection"></div>

                </div>
                <div class="BeigeBoxFooter"></div>
             </div>
           </div>

        <asp:ScriptManagerProxy ID="smpOrderPreferences" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/OrderSitePreferences.js" />
            </Scripts>
        </asp:ScriptManagerProxy>

    </ContentTemplate>
    <Triggers>
        <%--<asp:AsyncPostBackTrigger ControlID="bCreateGroupBtn" EventName="Click" />--%>
        <asp:AsyncPostBackTrigger ControlID="bSubmitOrderPreferences" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="bSetPreference" EventName="Click" />
       <%-- <asp:AsyncPostBackTrigger ControlID="gvClinicGroups"  EventName="RowCommand" />--%>
        <asp:AsyncPostBackTrigger ControlID="ddlInitialPriority" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="gvPriorityHistory" EventName="RowCommand" />
        <asp:AsyncPostBackTrigger ControlID="ddlPpPriority" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>
<div class="buttonSection">
    <asp:Button ID="bSubmitOrderPreferences" runat="server" CssClass="srtsButton" Text="Submit" OnClick="bSubmitOrderPreferences_Click" CausesValidation="false" />
</div>
