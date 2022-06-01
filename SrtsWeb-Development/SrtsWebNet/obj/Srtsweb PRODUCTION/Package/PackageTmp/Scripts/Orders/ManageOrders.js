/// <reference path="ManageOrders.js" />
/// <reference path="../Global/jquery-1.11.1.min.js"/>

var OrderNumbersNotInGrid = [], currModule = "";
var rowSelectedCount = 0;
var notAddedCount = 0;

function pageLoad() {
    //var dialogOpts = {
    //    autoOpen: false,
    //    modal: true,
    //    width: 550,
    //    height: 400,
    //    title: 'Bulk Order Input',
    //    dialogClass: 'generic'
    //},
    //d = $("#bulkInput").dialog(dialogOpts);

    //d.parent().appendTo($("form:first"));

    $('[id*=tbSingleReadScan]').keydown(function (e) {
        if (e.which == 13) {
            var orderNum = $(this).val().trim().toString().toUpperCase();
            $(this).val('');
            if (orderNum.substr(0, 6) != $("#spanWrapper").text().substr($("#spanWrapper").text().lastIndexOf("-") + 2, $("#spanWrapper").text().length).trim()) {
                alert("This order does not belong to your clinic!");
                return;
            }
            SearchForOrder(orderNum);
            $('#GridData').trigger('reloadGrid');
            $('[id*=tbSingleReadScan]').focus();
            //DisplayTotalSelectedRows();
            $("#spnTotalSelectedOrders").text(rowSelectedCount);
        }
    });

    currModule = window.location.hash.substr(1, window.location.hash.length);
    $(".PageSubMenu").click(function (event) {
        if (currModule != (($(this).text()).replace(/\s+/g, '')).toLowerCase()) {
            window.location.hash = (($(this).text()).replace(/\s+/g, '')).toLowerCase();
        }
    });
    $(window).on('hashchange', function () {
        SetPageSubmenu(currModule);
    });

    SetPageSubmenu(currModule);
    ClearGridSelectedArray();
    if (currModule.toLowerCase() == "problem" || currModule.toLowerCase() == "overdue") {
        $("#GridData").jqGrid('hideCol', 'cb');
        $(".submit,.clear,.print,.bulk").hide();
        $('#divInstruction').text('Select the grid row or scan 771 into single order textbox to view the order\'s details.');
    }
    else if (currModule.toLowerCase() == 'dispense') {
        $("#GridData").jqGrid('showCol', 'cb');
        $(".submit,.clear,.bulk").show();
        $(".print").hide();
        $('#divInstruction').text('Select the grid row checkbox, scan the 771 into single order textbox, or use bulk input to select order for dispense.');
    }
    else if (currModule.toLowerCase() == 'pending') {
        $("#GridData").jqGrid('hideCol', 'cb');
        $(".submit,.clear,.print,.bulk").show();
        $('#divInstruction').text('Scan the 771 into single order textbox, or use bulk input to select order to force check-in the order.');
    }
    else {
        $("#GridData").jqGrid('showCol', 'cb');
        $(".submit,.clear,.print,.bulk").show();
        $('#divInstruction').text('Select the grid row checkbox, scan the 771 into single order textbox, or use bulk input to select order for check-in.');
    }

    $('[id*=tbSingleReadScan]').focus();
}

function SetPageSubmenu(currModule) {
    $("#divSubMenu ul li a").each(function () {
        if ((($(this).text()).replace(/\s+/g, '')).toLowerCase() == currModule) {
            $(this).addClass("active");
        }
    });
    $('[id*=hfPageModule]').val(currModule.toString());
    SetPageContent(currModule);
}

function SetPageContent(currModule) {
    switch (currModule) {
        case "pending":
            $("#spnPageSubModule").text("Pending Orders");
            break;
        case "checkin":
            $("#spnPageSubModule").text("Order Check-In");
            break;
        case "dispense":
            $("#spnPageSubModule").text("Order Dispense");
            break;
        case "problem":
            $("#spnPageSubModule").text("Problem Orders");
            $("#divTotalSelected").addClass("hide");
            break;
        case "overdue":
            $("#spnPageSubModule").text("Overdue Orders");
            $("#divTotalSelected").addClass("hide");
            break;
    }
    SetPageDataParams(currModule);
}

function SetPageDataParams(currModule) {
    // Clear Selected Order Array
    OrderNumbersNotInGrid = [];

    // Get user's sitecode
    var userInfo = $("#spanWrapper").text();
    var siteCode = (userInfo.substr(userInfo.lastIndexOf("-") + 2, userInfo.length)).trim();

    // Build columns for jqGrid
    var modeColNames = []
    modeColNames = SetPageDataColNames(currModule);
    var modeColModel = []
    modeColModel = SetPageDataColModel(currModule);

    // Build jqGrid
    GetSiteOrderData(currModule, modeColNames, modeColModel, siteCode);
}

function SetPageDataColNames(currModule) {
    switch (currModule) {
        case "pending":
            return ['Order Number', 'Patient', 'Frame', 'Lens Type', 'Ship To Patient', 'Lab', 'Date Created', 'Status Description', 'Individual ID'];
            break;
        case "checkin":
            return ['Order Number', 'Patient', 'Frame', 'Lens Type', 'Ship To Patient', 'Lab', 'Date Created', 'Individual ID'];
            break;
        case "dispense":
            return ['Order Number', 'Patient', 'Frame', 'Lens Type', 'Lab', 'Date Received', 'Individual ID'];
            break;
        case "problem":
            $("#divTotalSelected").addClass("hide");
            return ['Order Number', 'Patient', 'Frame', 'Lens Type', 'Lab', 'Date Created', 'Status Description', 'Individual ID'];
            break;
        case "overdue":
            $("#divTotalSelected").addClass("hide");
            return ['Order Number', 'Patient', 'Frame', 'Lens Type', 'Lab', 'Days Overdue', 'Date Created', 'Status Description', 'Individual ID'];
            break;
    }
}

function SetPageDataColModel(currModule) {
    switch (currModule) {
        case "pending":
            return [
                { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 105, resizable: false, sorttype: SharedSortOrderNumber },
                { name: 'LastName', index: 'LastName', width: 175, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
                { name: 'FrameCode', index: 'FrameCode', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LensType', index: 'LensType', width: 65, resizable: false, sorttype: 'text' },
                { name: 'ShipToPatient', index: 'ShipToPatient', width: 60, resizable: false, sorttype: 'text', formatter: FormatterSTP },
                { name: 'LabSiteCode', index: 'LabSiteCode', width: 60, resizable: false, sorttype: 'text' },
                { name: 'DateOrderCreated', index: 'DateOrderCreated', width: 80, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
                { name: 'OrderStatusDescription', index: 'OrderStatusDescription', width: 220, resizable: false, sorttype: 'text' },
                { name: 'IndividualId', index: 'IndividualId', width: 1 }
            ];
            break;
        case "checkin":
            return [
                { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 105, resizable: false, sorttype: SharedSortOrderNumber },
                { name: 'LastName', index: 'LastName', width: 175, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
                { name: 'FrameCode', index: 'FrameCode', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LensType', index: 'LensType', width: 65, resizable: false, sorttype: 'text' },
                { name: 'ShipToPatient', index: 'ShipToPatient', width: 60, resizable: false, sorttype: 'text', formatter: FormatterSTP },
                { name: 'LabSiteCode', index: 'LabSiteCode', width: 60, resizable: false, sorttype: 'text' },
                { name: 'DateOrderCreated', index: 'DateOrderCreated', width: 80, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
                { name: 'IndividualId', index: 'IndividualId', width: 1 }
            ];
            break;
        case "dispense":
            return [
                { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 105, resizable: false, sorttype: SharedSortOrderNumber },
                { name: 'LastName', index: 'LastName', width: 195, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
                { name: 'FrameCode', index: 'FrameCode', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LensType', index: 'LensType', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LabSiteCode', index: 'LabSiteCode', width: 60, resizable: false, sorttype: 'text' },
                { name: 'DateOrderCreated', index: 'DateOrderCreated', width: 80, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
                { name: 'IndividualId', index: 'IndividualId', width: 1 }
            ];
            break;
        case "problem":
            return [
                { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 105, resizable: false, sorttype: SharedSortOrderNumber },
                { name: 'LastName', index: 'LastName', width: 195, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
                { name: 'FrameCode', index: 'FrameCode', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LensType', index: 'LensType', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LabSiteCode', index: 'LabSiteCode', width: 60, resizable: false, sorttype: 'text' },
                { name: 'DateOrderCreated', index: 'DateOrderCreated', width: 80, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
                { name: 'OrderStatusDescription', index: 'OrderStatusDescription', width: 220, resizable: false, sorttype: 'text' },
                { name: 'IndividualId', index: 'IndividualId', width: 1 }
            ];
            break;
        case "overdue":
            return [
                { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 105, resizable: false, sorttype: SharedSortOrderNumber },
                { name: 'LastName', index: 'LastName', width: 195, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
                { name: 'FrameCode', index: 'FrameCode', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LensType', index: 'LensType', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LabSiteCode', index: 'LabSiteCode', width: 60, resizable: false, sorttype: 'text' },
                { name: 'DaysPastDue', index: 'DaysPastDue', width: 85, resizable: false, sorttype: 'text', align: 'center' },
                { name: 'DateOrderReceived', index: 'DateOrderReceived', width: 80, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
                { name: 'OrderStatusDescription', index: 'OrderStatusDescription', width: 220, resizable: false, sorttype: 'text' },
                { name: 'IndividualId', index: 'IndividualId', width: 1 }
            ];
            break;
    }
}

function GoToDetailsPage(id) {
    var totOrders = OrderNumbersNotInGrid.length + rowSelectedCount;
    var link = '../../SrtsPerson/PersonDetails.aspx?id=' + id + '&isP=true';

    if (totOrders <= 0) {
        window.location.href(link);
        return;
    }

    if (!confirm('There are ' + totOrders + ' order(s) ready for ' + currModule + '.  Press "Ok" to ' + currModule + ' order(s) or "Cancel" to go to the patient details page.  Note, you will be required to re-select these orders once you leave this page.')) {
        window.location.href(link);
        return;
    }

    DoStatusUpdateOpsAjax(id);
}

function GoToPatientOrder(id) {
    $('[id$=hfOrderNumber]').val(id);
    __doPostBack('__Page', 'JQUERY_PB');
    return;
}

function DoStatusUpdateOpsAjax(id) {
    DumpOrderArrays();
    var oig = $('[id*=hfOrdersInGrid]').val(),
        onig = $('[id*=hfOrdersNotInGrid]').val(),
        selLabel = $('#ddlLabelFormat').find('option:selected').val();

    $.ajax({
        type: "POST",
        url: 'ManageOrders.aspx/UpdateOrderStatuses',
        data: '{OrdersInGrid: "' + oig + '",OrdersNotInGrid: "' + onig + '",CurrentModule: "' + currModule + '",SelectedLabel: "' + selLabel + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.d[0] != undefined && data.d[0] != null && data.d[0] != "")
                $('hiddenDownload').attr('src', data.d[0].toString());
            window.location.href('../../SrtsPerson/PersonDetails.aspx?id=' + id);
        },
        failure: function () {
            alert('An error occured trying to ' + currModule + ' orders.');
        }
    });
}

function GetSiteOrderData(currModule, modeColNames, modeColModel, siteCode) {
    $("#GridData").jqGrid({
        url: 'ManageOrders.aspx/GetClinicOrderData',
        datatype: 'json',
        mtype: 'POST',
        postData: {
            siteCode: siteCode,
            pageMode: currModule
        },
        ajaxGridOptions: { contentType: 'application/json; charset=utf-8' },
        serializeGridData: function (postData) {
            return JSON.stringify(postData);
        },
        colNames: modeColNames,
        colModel: modeColModel,
        gridview: true,
        caption: "&nbsp;&nbsp;Available Orders",
        sortname: "cb",
        sortorder: "desc",
        viewrecords: true,
        loadonce: true,
        multiselect: true,
        //multiPageSelection: true,
        autowidth: true,
        height: '100%',
        rowNum: 25,
        rowList: [25, 50, 100],
        pager: '#GridPager',
        toppager: true,
        additionalProperties: ["LastName", "FirstName", "MiddleName"],
        beforeSelectRow: function (rowid, e) {
            return false;
        },
        onCellSelect: function (rowid, iCol, cellcontent, e) {
            if (currModule.toLowerCase() == 'problem' || currModule.toLowerCase() == 'overdue') {
                // If col 2 (name) is clicked, do nothing unless name clicked. 
                // If name is clicked, the link will direct the user to patient details page.
                $('#hfSkipOverdueProblem').val(iCol == 2);
                if ($('#hfSkipOverdueProblem').val() == 'true') return;

                // if any other column is clicked, go to patient order detail.
                GoToPatientOrder(rowid);
            }
            else if (currModule.toLowerCase() == 'pending') {
                if (iCol == 2) return;

                var p = this.p, item = p.data[p._index[rowid]];

                // If the row is currently not selected then leave.
                if (typeof (item.cb) === "undefined") return;

                // Otherwise un check it
                item.cb = false;

                // if rowSelectedCount is negative multiply by negative one to reset...
                var posRowSelectedCount = (rowSelectedCount < 0) ? rowSelectedCount * -1 : rowSelectedCount;
                // row is unselected so subtract from rowSelectedCount variable
                rowSelectedCount = rowSelectedCount - 1;
                $('#GridData').trigger('reloadGrid');
            }
            else {
                // if checkin or dispense
                // If col 2 (name) is clicked, do nothing unless name clicked. 
                // If name is clicked, the link will direct the user to patient details page.
                if (iCol == 2) {
                    return;
                }
                else {
                    // if any other column is clicked, select the row
                    var p = this.p, item = p.data[p._index[rowid]];
                    if (typeof (item.cb) === "undefined") {
                        item.cb = true;
                    } else {
                        item.cb = !item.cb;
                    }
                    // if rowSelectedCount is negative multiply by negative one to reset...
                    var posRowSelectedCount = (rowSelectedCount < 0) ? rowSelectedCount * -1 : rowSelectedCount;
                    if (item.cb == true) {
                        // row is selected so add to rowSelectedCount variable
                        rowSelectedCount = rowSelectedCount + 1;
                    }
                    else {
                        // row is unselected so subtract from rowSelectedCount variable
                        rowSelectedCount = rowSelectedCount - 1;
                    }
                    $('#GridData').trigger('reloadGrid');
                }
            }
        },
        loadComplete: function () {
            var $this = $(this), i, count, p = this.p, data = p.data, item, index = p._index, rowid;

            for (rowid in index) {
                if (index.hasOwnProperty(rowid)) {
                    item = data[index[rowid]];
                    if (typeof (item.cb) === "boolean" && item.cb) {
                        $this.jqGrid('setSelection', rowid, false);
                    }
                }
            }
            var orderCount = $("#GridData").getGridParam("records");
            $("#spnSubModuleOrderCount").text(" - Total Orders Available: ( " + orderCount + " )");

            // show total orders on page
            $("#spnTotalOrders").text(orderCount);

            // show total selected orders on page
            $("#spnTotalSelectedOrders").text(rowSelectedCount);
        },
        jsonReader: {
            root: function (obj) { return obj.d; },
            page: function (obj) { return 1; },
            total: function (obj) { return 1; },
            records: function (obj) { return obj.d.length; },
            repeatitems: false
        },
        loadError: function (jqXHR, textStatus, errorThrown) {
            alert('HTTP status code: ' + jqXHR.status + '\n' +
            'textStatus: ' + textStatus + '\n' +
            'errorThrown: ' + errorThrown);
            alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
        }
    });
    $("#cb_" + $("#GridData")[0].id).hide();
    $("#jqgh_GridData_cb").addClass("ui-jqgrid-sortable");
    cbColModel = $("#GridData").jqGrid("getColProp", "cb");
    cbColModel.sortable = true;
    cbColModel.sorttype = function (value, item) {
        return typeof (item.cb) === "boolean" && item.cb ? 1 : 0;
    };
}

function SearchForOrder(orderNumber) {
    var gridData = $('#GridData').jqGrid('getGridParam', 'data');
    var gridDataRow = $.grep(gridData, function (e) { return e._id_ == orderNumber; });

    if (gridDataRow.length > 0) {
        var selRowIds = $('#GridData').jqGrid("getGridParam", "selarrrow");
        if ($.inArray(gridDataRow[0]._id_, selRowIds) < 0) {
            var p = $("#GridData")[0].p, item = p.data[p._index[gridDataRow[0]._id_]];
            if (typeof (item.cb) === "undefined") {
                item.cb = true;
                rowSelectedCount = rowSelectedCount + 1;
            } else {
                item.cb = !item.cb;
                rowSelectedCount = rowSelectedCount + 1;
            }
            $("#spnTotalSelectedOrders").text(rowSelectedCount);
        }
        else {
            return;
        }
    }
    else {
        if (currModule == "checkin" && $.inArray(orderNumber, OrderNumbersNotInGrid) < 0) {
            // Compare first 6 of order number against the logged in users site code.
            if (orderNumber.substr(0, 6) == $('[id*=hfSiteCode]').val()) {
                OrderNumbersNotInGrid.push(orderNumber);
                SetOrderNumbersNotInGrid();
            }
            else
                notAddedCount++;
        }
        else if (currModule == "pending") {
            alert("The order number " + orderNumber + " is not in the pending orders list.");
        }
        else {
            alert("This order is not ready to be dispensed!");
        }
    }
}

function FormatterSTP(cellvalue, options, rowObject) {
    return cellvalue == "Clinic Distribution" ? "No" : "Yes";
}

function BtnBulkDone() {
    $("#bulkInput").dialog('close');
    var orderNums = $('[id*=tbOrderNumbers]').val().trim().split('\n');
    for (i = 0; i < orderNums.length; i++) {
        SearchForOrder(orderNums[i]);
    }
    $('#GridData').trigger('reloadGrid');
    $('[id*=tbSingleReadScan]').focus();
    //DisplayTotalSelectedRows();
    $("#spnTotalSelectedOrders").text(rowSelectedCount);
    if (notAddedCount == 0) return;
    alert('You attempted to add ' + notAddedCount + ' order(s) that do not belong to your clinic and were removed from the bulk input attempt.');
}

function ClearSelectedOrders() {
    ClearGridSelectedArray();
    $('#GridData').trigger('reloadGrid');
    OrderNumbersNotInGrid = [];
}

function DumpOrderArrays() {
    var gridData = $('#GridData').jqGrid("getGridParam").data,
        selRows = $.grep(gridData, function (e) { return e.cb == true; }),
        selOrders = [];

    // Reset and show total selected orders on page
    rowSelectedCount = 0;
    $("#GridData").jqGrid("getGridParam").selarrrow = [];
    $("#spnTotalSelectedOrders").text(rowSelectedCount);

    $.each(selRows, function (key, value) {
        selOrders.push(value._id_);
    });
    $('[id*=hfOrdersInGrid]').val(selOrders.toString());
    $('[id*=hfOrdersNotInGrid]').val(OrderNumbersNotInGrid.toString());
}

function DoBulkInput() {
    notAddedCount = 0;

    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 550,
        height: 350,
        title: 'Bulk Order Input',
        dialogClass: 'generic',
        closeOnEscape: true
    };
    var doh = $('#bulkInput').dialog(dialogOpts);
    doh.parent().appendTo($('form:first'));

    doh.dialog('open');

    $('[id*=lblOrderScanCount]').empty();

    var d = $("#bulkInput");
    $('[id*=tbOrderNumbers]').val("");
    $('[id*=tbOrderNumbers]').focus();
    d.click(function () { $('[id*=tbOrderNumbers]').focus() });
}

function SetOrderNumbersNotInGrid() {
    // Add the items in the OrderNumbersNotInGrid array to the lblOrdersNotInLabDispense label
    var ons = '';
    if (OrderNumbersNotInGrid.length > 0 && OrderNumbersNotInGrid != "")
        ons = 'Order Numbers Not In Grid:';
    for (a = 0; a < OrderNumbersNotInGrid.length; a++) {
        ons += '  ' + OrderNumbersNotInGrid[a];
    }
    $('#lblOrdersNotInLabDispense').text(ons);
}

function ClearGridSelectedArray() {
    var selRows = $("#GridData").jqGrid("getGridParam", "data");

    $.each(selRows, function (key, value) {
        var p = $("#GridData")[0].p, item = p.data[p._index[value._id_]];
        item.cb = false;
    });

    // clear Total Selected orders and reset count
    rowSelectedCount = 0;
    $("#GridData").jqGrid("getGridParam").selarrrow = [];
}

// To be deleted along with calls to this function
//function DisplayTotalSelectedRows() {
//    // get count of total selected rows
//    var totalRowsSelected = jQuery('#GridData').jqGrid('getGridParam', 'selarrrow');
//    var totalRowsCount = totalRowsSelected.length;

//    $("#GridData").jqGrid("getGridParam").selarrrow = [];
//    rowSelectedCount = totalRowsCount;
//    $("#spnTotalSelectedOrders").text(rowSelectedCount);
//}
