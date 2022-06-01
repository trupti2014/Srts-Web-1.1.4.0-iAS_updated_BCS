/// <reference path="../Global/jquery-1.11.1.min.js"/>
/// <reference path="../Global/PassFailConfirm.js" />

var currModule = "";
var rowSelectedCount = 0;
var $currentCellText;
var $colName;
var item = "";
var itemType = "";
var statusItem;
var lensType;
var frameType;
var queried = false;
var hdfHoldStockItem;
var hdfHoldStockItemType;
var hdfShowAlert;
var hdfCurrentModule;
var currentGridView;
var currentItem;
var currentItemType;
var currentItemStatus;
var btnOK;
var currentOrderNumber;
var currentPriority;
function pageLoad() {
    currModule = window.location.hash.substr(1, window.location.hash.length);
   // var thisPage = window.location.hash.substr(1, window.location.hash.length);


    if (currModule == "") {
        currModule = "holdforstock";
    }

    hdfHoldStockItem = document.getElementById('hdfHoldStockItem');
    hdfHoldStockItemType = document.getElementById('hdfHoldStockItemType');
    hdfTotalOrdersOnHold = document.getElementById('hdfTotalOrdersOnHold');
    hdfTotalOrdersPending = document.getElementById('hdfTotalOrdersPending');
    hdfCurrentModule = document.getElementById('hdfCurrentModule');
    hdfShowAlert = document.getElementById('hdfShowAlert');
    hdfCheckInPriorities = document.getElementById('hdfCheckInPriorities');


    $(".PageSubMenu").click(function (event) {
      //  if (currModule != ((($(this).text()).replace(/\s+/g, ''))).toLowerCase()) {
      //         window.location.hash = (($(this).text()).replace(/\s+/g, '')).toLowerCase();
      if (currModule != ((($(this).text()).replace(/[ \/]+/g, ''))).toLowerCase()) {
        window.location.hash = (($(this).text()).replace(/[ \/]+/g, '')).toLowerCase();
        }
    });

    $(document).on('change',   //Aldela: 7/17/2018 Added this function
        '[id$=rblRejectRedirect]',
        function () {
            DoRejectRedirect($('#rblRejectRedirect').find('input:checked').val());
        });

    $(window).on('hashchange', function () {
        SetPageSubmenu(currModule);
    });

    SetPageSubmenu(currModule);

    $('[id*=tbSingleReadScan]').keydown(function (e) {
        if (e.which == 13) {
            var orderNum = $(this).val().trim();
            $(this).val('');
            SearchForOrder(orderNum);
            $('#GridData').trigger('reloadGrid');
            $('[id*=tbSingleReadScan]').focus();
        }
    });

    $('[id*=cbCheckInAll]').click(function () {
        $('[id*=ddlOrderClinicCodes]').val('X');
        GetAllOrders(this);
    });

    $('[id*=ddlOrderClinicCodes]').change(function () {
        if ($(this).val() != 'X') {
            $('[id*=cbCheckInAll]').prop('checked', false);
        }
        ClearPriorityStats();
        GetOrdersByClinicCode($(this).val());
    });

    $(".print").hide();

    

    switch (currModule.toLowerCase()) {
        case 'checkin':
            $(".dispense").hide();
            $(".holdstock").hide();
            $(".redirectreject").hide();
            $(".print").hide();
            $(".checkin").show();
            $("#divHoldForStockTotals").hide();
            $("#divInstructionsOrdersPending").hide();
            if (document.getElementById("hdfSiteHasLMS").value == 'True') {
                $('[id*=btnSubmitTop]').hide(); $('[id*=btnSubmitBottom]').hide();
            }
            break;
        case 'redirectreject':
            $(".checkin").hide();
            $(".dispense").hide();
            $(".holdstock").hide();
            $(".redirectreject").show();
            $("#divHoldForStockTotals").hide();
            $("#divInstructionsOrdersPending").hide();
            break;
        case 'dispense':
            $(".checkin").hide();
            $(".holdstock").hide();
            $(".redirectreject").hide();
            $(".dispense").show();
            $('[id*=tbSingleReadScan]').focus();
            $(".print").show();
            $("#divHoldForStockTotals").hide();
            $("#divInstructionsOrdersPending").hide();
            if (document.getElementById("hdfSiteHasLMS").value == 'True') {
                $('[id*=btnSubmitTop]').hide(); $('[id*=btnSubmitBottom]').hide();
            }
            break;
        case 'holdforstock':
            $(".checkin").hide();
            $(".dispense").hide();
            $(".redirectreject").hide();
            $(".holdstock").show();
            $(".print").hide();
            $('[id*=cbCheckInHoldStock]').prop('checked', false);
            hdfCurrentModule.value = "holdforstock";
            $("#divHoldForStockTotals").show();
            $("#spnTotalOrdersOnHold").text(hdfTotalOrdersOnHold.value);
            $("#spnTotalOrdersPending").text(hdfTotalOrdersPending.value);
            break;
    }

    $('[id*=chkHoldFrame]').click(function (e) {
        if ($(this).prop("checked") == true) {
            var msg = "";

            var $cell = $(e.target).closest("td");
            var $colIndex = $cell.parent().children().index($cell);
            $currentCellText = $cell.text();
            $currentCellText = $currentCellText.replace(/(\r\n|\n|\r|)\s/gm, "");
            var $colName = $cell.closest("table").find('th:eq(' + $colIndex + ')').text();

            var orderNumber = $cell.closest("tr").find('td:eq(' + 0 + ')').text();
            orderNumber = orderNumber.replace(/(\r\n|\n|\r|)\s/gm, "");

            var rowIndex;
            var rows = document.getElementById('gvPending').getElementsByTagName('tbody')[0].getElementsByTagName('tr');


            item = "Frame";
            itemType = $currentCellText;
            var items = $colName + " of type " + $currentCellText;
            var msgStart = "<br />Would you like to select and place all orders with a ";
            var msgEnd = " on hold for stock?";
            msg = msgStart + items + msgEnd


            hdfHoldStockItem = document.getElementById('hdfHoldStockItem');
            hdfHoldStockItemType = document.getElementById('hdfHoldStockItemType');
            hdfStatusType = document.getElementById('hdfStatusType');
            hdfHoldStockItem.value = item;
            hdfHoldStockItemType.value = itemType;
            hdfStatusType.value = statusItem;

            currentGridView = "gvPending";
            currentItem = item;
            currentItemType = itemType;
            currentItemStatus = statusItem;
            currentOrderNumber = orderNumber;

            var hdfCommandName = document.getElementById('hdfCommandName');
            hdfCommandName.value = "SelectAllFrameItemType";

            $('#btnYes').show();
            $('#btnNo').show();
            var lblMessage = document.getElementById('lblMessage');
            lblMessage.innerHTML = msg;
            DisplayHoldStockDialog();
        }
    });

    $('[id*=chkHoldLens]').click(function (e) {
        if ($(this).prop("checked") == true) {
            var msg = "";
            $('[id*=chkHoldFrame]').prop('checked', false);

            var $cell = $(e.target).closest("td");
            var $colIndex = $cell.parent().children().index($cell);
            $currentCellText = $cell.text();
            $currentCellText = $currentCellText.replace(/(\r\n|\n|\r|)\s/gm, "");
            var $colName = $cell.closest("table").find('th:eq(' + $colIndex + ')').text();

            var orderNumber = $cell.closest("tr").find('td:eq(' + 0 + ')').text();
            orderNumber = orderNumber.replace(/(\r\n|\n|\r|)\s/gm, "");
            var rowIndex;
            var rows = document.getElementById('gvPending').getElementsByTagName('tbody')[0].getElementsByTagName('tr');


            item = "Lens Material";
            itemType = $currentCellText;
            var items = $colName + " of type " + $currentCellText;
            var msgStart = "<br />Would you like to select and place all orders with a ";
            var msgEnd = " on hold for stock?";
            msg = msgStart + items + msgEnd


            hdfHoldStockItem = document.getElementById('hdfHoldStockItem');
            hdfHoldStockItemType = document.getElementById('hdfHoldStockItemType');
            hdfStatusType = document.getElementById('hdfStatusType');
            hdfHoldStockItem.value = item;
            hdfHoldStockItemType.value = itemType;
            hdfStatusType.value = statusItem;

            currentGridView = "gvPending";
            currentItem = item;
            currentItemType = itemType;
            currentItemStatus = statusItem;
            currentOrderNumber = orderNumber;

            var hdfCommandName = document.getElementById('hdfCommandName');
            hdfCommandName.value = "SelectAllLensItemType";

            var lblMessage = document.getElementById('lblMessage');
            lblMessage.innerHTML = msg;
            DisplayHoldStockDialog();
        }
    });

    $('[id*=chkCheckinHold]').click(function (e) {
        if ($(this).prop("checked") == true) {
            var msg = "";
            var $cell = $(e.target).closest("td");
            var orderNumber = $cell.closest("tr").find('td:eq(' + 1 + ')').text();
            orderNumber = orderNumber.replace(/(\r\n|\n|\r|)\s/gm, "");

            var rowIndex;
            var rows = document.getElementById('gvStockonHold').getElementsByTagName('tbody')[0].getElementsByTagName('tr');

            for (i = 0; i < rows.length; i++) {
                if (i > 1) {
                    if (rows[i].cells[1].innerText == orderNumber) {
                        // get hold item and type
                        rowIndex = rows[i];
                        statusItem = rowIndex.cells[4].innerText.substr(15, 4);
                        lensType = rowIndex.cells[3].innerText;
                        frameType = rowIndex.cells[2].innerText;

                        if (statusItem.startsWith("Fra")) {
                            item = "Frame";
                            itemType = frameType;
                        }
                        if (statusItem.startsWith("Len")) {
                            item = "Lens Material";
                            itemType = lensType;
                        }
                    }
                }
            }


            hdfHoldStockItem = document.getElementById('hdfHoldStockItem');
            hdfHoldStockItemType = document.getElementById('hdfHoldStockItemType');
            hdfStatusType = document.getElementById('hdfStatusType');
            hdfHoldStockItem.value = item;
            hdfHoldStockItemType.value = itemType;
            hdfStatusType.value = statusItem;

            currentGridView = "gvStockonHold";
            currentItem = item;
            currentItemType = itemType;
            currentItemStatus = statusItem;
            currentOrderNumber = orderNumber;

            var hdfCommandName = document.getElementById('hdfCommandName');
            hdfCommandName.value = "SelectAllCheckinItems";

            $('#btnYes').show();
            $('#btnNo').show();

            var lblMessage = document.getElementById('lblMessage');

            var msgStart = "<br />Would you like to select all orders with a ";
            var msgEnd = " to release from hold?";
            msg = msgStart + item + " of type " + itemType + msgEnd
            lblMessage.innerHTML = msg;
            DisplayHoldStockDialog();
        }
    });
}

function GetCodeForDescription(priority) {
    var priorityCode;
    switch (priority.toLowerCase()) {
        case 'downedpilot':
            priorityCode = "P";
            break;
        case 'frameofchoice':
            priorityCode = "F";
            break;
        case 'readiness':
            priorityCode = "R";
            break;
        case 'standard':
            priorityCode = "S";
            break;
        case 'trainee':
            priorityCode = "T";
            break;
        case 'vip':
            priorityCode = "V";
            break;
        case 'woundedwarrior':
            priorityCode = "W";
            break;
        default:

            break;
    }
    return priorityCode;
}

function GetOrdersByPriorityCode(priorityCode) {
    var $grid = $('#GridData'),
        gridData = $grid.jqGrid("getGridParam", "data"),
        ordersByPriority = $.grep(gridData, function (item) {
            return item.Priority == priorityCode;
        });
    $.each(ordersByPriority, function (key, value) {
        SearchForOrder(value.OrderNumber);
    });
    $('#GridData').trigger('reloadGrid');
}

function DoHoldForStock() {
    currModule = "holdforstock";
    SetPageSubmenu(currModule);
}

function SetPageSubmenu(currModule) {
    $("#divSubMenu ul li a").each(function () {
        //if ((($(this).text()).replace(/\s+/g, '')).toLowerCase() == currModule) {
        if ((($(this).text()).replace(/[ \/]+/g, '')).toLowerCase() == currModule) {
            $(this).addClass("active");
        }
    });
    $('[id*=hfPageModule]').val(currModule.toString());
    switch (currModule.toLowerCase()) {
        case 'holdforstock':
            $("#uplHoldforStock").show();
            $("#divHoldOptions").show();
            $("#divGridWrapper").hide();
            $("#divButtonsTop").hide();
            $("#divButtonsBottom").hide();
            $("#divCheckInDispenseHeader").hide();
            $("#divTotalSelected").hide();
            break;
        case 'checkin':
            $("divHoldForStockTotals").hide();
        default:
            $("#uplHoldforStock").hide();
            $("#divHoldOptions").hide();
            $("divHoldForStockTotals").hide();
            break;
    }
    SetPageContent(currModule);
}

function SetPageContent(currModule) {
    // clear Total Selected orders and display count
    rowSelectedCount = 0;
    $("#spnTotalSelectedOrders").text(rowSelectedCount);
    SetPageDataParams(currModule);
}

function SetPageDataParams(currModule) {
    // Get user's sitecode
    var userInfo = $("#spanWrapper").text();
    var siteCode = (userInfo.substr(userInfo.lastIndexOf("-") + 2, userInfo.length)).trim();

    // Build jqGrid
    GetSiteOrderData(currModule, siteCode);
}

function GetSiteOrderData(currModule, siteCode) {
    $("#GridData").jqGrid({
        url: 'ManageOrdersLab.aspx/GetLabOrderData',
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
        colNames: GetColNames(currModule),
        colModel: GetColModel(currModule),
        gridview: true,
        caption: "&nbsp;&nbsp;Available Orders",
        autowidth: false,
        shrinkToFit: true,
        sortname: currModule.toLowerCase() == 'holdforstock' ? 'HfsDate' : 'cb',
        sortorder: "desc",
        viewrecords: true,
        loadonce: true,
        multiselect: true,
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
            // If col 5 (name) is clicked, do nothing unless name clicked. 
            // If name is clicked, the link will direct the user to patient details page.
            if (iCol == 5 && currModule != 'holdforstock') {
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

            if (currModule == "checkin") SetLabCheckinSelections();
            if (currModule == "checkin") SetLabPrioritySelections();
            var orderCount = $("#GridData").getGridParam("records");

            // show total orders on page
            $("#spnTotalOrders").text(orderCount);

            // show total selected orders on page
            $("#spnTotalSelectedOrders").text(rowSelectedCount);

            // set All Orders Checkbox Status
            if (orderCount == rowSelectedCount) {
                $('[id*=cbCheckInAll]').prop('checked', true);
            } else {
                $('[id*=cbCheckInAll]').prop('checked', false);
            }
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
    cbColModel.align = 'center';
    cbColModel.sortable = true;
    cbColModel.sorttype = function (value, item) {
        return typeof (item.cb) === "boolean" && item.cb ? 1 : 0;
    };
}

function GetColNames(currModule) {
    switch (currModule.toLowerCase()) {
        case 'holdforstock':
            return ['Order Number', 'Frame', 'Lens Material', 'Status Comment', 'Hold End Date', 'Order Date', 'Ordering Clinic', 'CurrentStatusId'];
            break;
        case 'checkin':
            return ['Order Number', 'Priority', 'Frame', 'Lens Type', 'Lens Material', 'Patient', 'Order Date', 'Clinic Code', 'IndividualId', 'CurrentStatusId'];
            break;
        case 'dispense':
            return ['Order Number', 'Frame', 'Lens Type', 'Lens Material', 'Patient', 'Order Date', 'Clinic Code', 'IndividualId', 'CurrentStatusId'];
            break;
        case 'redirectreject':
            return ['Order Number', 'Frame', 'Lens Type', 'Lens Material', 'Status Comment', 'Patient', 'Order Date', 'Clinic Code', 'IndividualId', 'CurrentStatusId'];
            break;
    }
}

function GetColModel(currModule) {
    switch (currModule.toLowerCase()) {
        case 'holdforstock':
            return [
            { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 125, resizable: false, sorttype: SharedSortOrderNumber },
            { name: 'FrameCode', index: 'FrameCode', width: 95, resizable: false, sorttype: 'text' },
            { name: 'LensMaterial', index: 'LensMaterial', width: 95, resizable: false, sorttype: 'text' },
            { name: 'StatusComment', index: 'StatusComment', width: 300, resizable: false, sorttype: 'text' },
            { name: 'HfsDate', index: 'HfsDate', width: 90, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
            { name: 'OrderDate', index: 'OrderDate', width: 90, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
            { name: 'ClinicCode', index: 'ClinicCode', width: 100, resizable: false, sorttype: 'text' },
            { name: 'CurrentStatusId', index: 'CurrentStatusId', width: 1 }
            ];
            break;
        case 'dispense':
            return [
            { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 125, resizable: false, sorttype: SharedSortOrderNumber },
            { name: 'FrameCode', index: 'FrameCode', width: 95, resizable: false, sorttype: 'text' },
            { name: 'LensType', index: 'LensType', width: 95, resizable: false, sorttype: 'text' },
            { name: 'LensMaterial', index: 'LensMaterial', width: 95, resizable: false, sorttype: 'text' },
            { name: 'LastName', index: 'LastName', width: 300, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
            { name: 'OrderDate', index: 'OrderDate', width: 90, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
            { name: 'ClinicCode', index: 'ClinicCode', width: 100, resizable: false, sorttype: 'text' },
            { name: 'IndividualId', index: 'IndividualId', width: 1 },
            { name: 'CurrentStatusId', index: 'CurrentStatusId', width: 1 }
            ];
            break;
        case 'checkin':
            return [
                { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 120, resizable: false, sorttype: SharedSortOrderNumber },
                { name: 'Priority', index: 'Priority', width: 40, resizable: false, sorttype: 'text', align: 'center' },
                { name: 'FrameCode', index: 'FrameCode', width: 65, resizable: false, sorttype: 'text' },
                { name: 'LensType', index: 'LensType', width: 70, resizable: false, sorttype: 'text' },
                { name: 'LensMaterial', index: 'LensMaterial', width: 85, resizable: false, sorttype: 'text' },
                { name: 'LastName', index: 'LastName', width: 125, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
                { name: 'OrderDate', index: 'OrderDate', width: 65, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
                { name: 'ClinicCode', index: 'ClinicCode', width: 75, resizable: false, sorttype: 'text' },
                { name: 'IndividualId', index: 'IndividualId', width: 1 },
                { name: 'CurrentStatusId', index: 'CurrentStatusId', width: 1 }
            ];
            break;
        case 'redirectreject':
            return [
                { name: 'OrderNumber', index: 'OrderNumber', key: true, width: 125, resizable: false, sorttype: SharedSortOrderNumber },
                { name: 'FrameCode', index: 'FrameCode', width: 70, resizable: false, sorttype: 'text' },
                { name: 'LensType', index: 'LensType', width: 75, resizable: false, sorttype: 'text' },
                { name: 'LensMaterial', index: 'LensMaterial', width: 95, resizable: false, sorttype: 'text' },
                { name: 'StatusComment', index: 'StatusComment', width: 200, resizable: false, sorttype: 'text' },
                { name: 'LastName', index: 'LastName', width: 190, resizable: false, sorttype: 'text', formatter: SharedFormatterPtNameLink },
                { name: 'OrderDate', index: 'OrderDate', width: 70, resizable: false, sorttype: 'date', formatter: SharedFormatterOrderDate, formatoptions: { newformat: 'm/d/y' }, datefmt: 'm-d-y' },
                { name: 'ClinicCode', index: 'ClinicCode', width: 75, resizable: false, sorttype: 'text' },
                { name: 'IndividualId', index: 'IndividualId', width: 1 },
                { name: 'CurrentStatusId', index: 'CurrentStatusId', width: 1 }
            ];
            break;
    }
}

function GoToDetailsPage(id) {
    var link = '../SrtsPerson/PersonDetails.aspx?id=' + id + '&isP=true';

    if (rowSelectedCount <= 0) {
        window.location.href(link);
        return;
    }

    if (!confirm('There are ' + rowSelectedCount + ' order(s) ready for ' + currModule + '.  Press "Ok" to ' + currModule + ' order(s) or "Cancel" to go to the patient details page.  Note, you will be required to re-select these orders once you leave this page.')) {
        window.location.href(link);
        return;
    }

    DoStatusUpdateOpsAjax(id);
}

function DoStatusUpdateOpsAjax(id) {
    DumpOrderArrays();
    var oig = $('#hfOrdersInGrid').val(),
        selLabel = $('#ddlLabelFormat').find('option:selected').val();

    //var ordersParam = new Array();

    //$.each(JSON.parse(oig), function (key, value) {
    //    ordersParam.push([value._id_.toString(), value.CurrentStatusId.toString()]);
    //});

    $.ajax({
        type: "POST",
        url: 'ManageOrdersLab.aspx/UpdateOrderStatuses',
        data: '{OrdersInGrid: "' + oig + '",CurrentModule: "' + currModule + '",SelectedLabel: "' + selLabel + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.d[0] != null || data.d[0] != "")
                $('hiddenDownload').attr('src', data.d[0].toString());
            window.location.href('../SrtsPerson/PersonDetails.aspx?id=' + id);
        },
        failure: function () {
            alert('An error occured trying to ' + currModule + ' orders.');
        }
    });
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
                if (item.cb == true) {
                    // row is selected so add to rowSelectedCount variable
                    rowSelectedCount = rowSelectedCount + 1;
                }
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
        alert("This order is not ready to be dispensed!");
    }
}

function BtnBulkDone() {
    $("#bulkInput").dialog('close');
    var orderNums = $('[id*=tbOrderNumbers]').val().trim().split('\n');
    for (i = 0; i < orderNums.length; i++) {
        SearchForOrder(orderNums[i]);
    }
    $('#GridData').trigger('reloadGrid');
    $('[id*=tbSingleReadScan]').focus();
}

function ClearSelectedOrders(clearPriorities) {
    ClearGridSelectedArray();
    $('#GridData').trigger('reloadGrid');
    $('[id*=cbCheckInAll]').prop('checked', false);
    $('[id*=ddlOrderClinicCodes]').val('X');
    if (clearPriorities == true) {
        ClearPriorityStats();
    }
}

function DoSubmit() {
    // If the current module is HFS then show the HFS dialog and return false.
    if (currModule.toLowerCase() == 'holdforstock') {
        DoHoldForStockDialog();
        return false;
    }
    else if (currModule.toLowerCase() == 'redirectreject') {
        if (DoRejectRedirectVal()) {
            DumpOrderArrays();
            return true;
        }
        else
            return false;
    }
    else {  // If the current module is not HFS then do DumpOrderArrays function and return true.
        DumpOrderArrays();
        return true;
    }
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
    $('#hfOrdersInGrid').val(selOrders.toString());
    return false;
}

function DoBulkInput() {
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



function GetOrdersByClinicCode(clinicCode) {
    var $grid = $('#GridData'),
         gridData = $grid.jqGrid("getGridParam", "data"),
         ordersByClinic = $.grep(gridData, function (item) {
             return item.ClinicCode == clinicCode;
         });

    ClearGridSelectedArray();
    $.each(ordersByClinic, function (key, value) {
        SearchForOrder(value.OrderNumber);
    });
    $('#GridData').trigger('reloadGrid');
}

function GetAllOrders(cbAllOrders) {
    ClearGridSelectedArray();
    if (cbAllOrders.checked) {
        // select all priorities
        checkAllPriorities("true");
        var i, count;
        gridData = $("#GridData").jqGrid("getGridParam", "data");
        for (i = 0; i < gridData.length; i++) {
            var p = $("#GridData")[0].p, item = p.data[p._index[gridData[i]._id_]];
            if (typeof (item.cb) === "undefined") {
                item.cb = true;
                if (item.cb == true) {
                    // row is selected so add to rowSelectedCount variable
                    rowSelectedCount = rowSelectedCount + 1;
                }
            } else {
                item.cb = !item.cb;
                rowSelectedCount = rowSelectedCount + 1;
            }
            $("#spnTotalSelectedOrders").text(rowSelectedCount);
        }
    }
    else {
        // unselect all priorities
        checkAllPriorities("false");
    }
    $('#GridData').trigger('reloadGrid');
}

function SetLabCheckinSelections() {
    var clinicCodes = [],
        gridData = $("#GridData").jqGrid("getGridParam", "data");

    clinicCodes = $.map(gridData, function (item) { return item.ClinicCode; });
    clinicCodes = $.grep(clinicCodes, function (val, index) {
        return index === $.inArray(val, clinicCodes);
    });

    var oldValue = $('[id*=ddlOrderClinicCodes]').val();
    $('[id*=ddlOrderClinicCodes]').empty();
    jQuery('<option/>', { value: 'X', html: '-Select-' }).appendTo('[id*=ddlOrderClinicCodes]');
    for (i = 0; i < clinicCodes.length; i++) {
        jQuery('<option/>', {
            value: clinicCodes[i],
            html: clinicCodes[i]
        }).appendTo('[id*=ddlOrderClinicCodes]');
    }
    oldValue == null ? $('[id*=ddlOrderClinicCodes]').val('X') : $('[id*=ddlOrderClinicCodes]').val(oldValue);
    //DisplayTotalSelectedRows();
}

function SetLabPrioritySelections() {
    var priorities = [],
        gridData = $("#GridData").jqGrid("getGridParam", "data");

    priorities = $.map(gridData, function (item) { return item.Priority; });
    priorities = $.grep(priorities, function (val, index) {
        return index === $.inArray(val, priorities);
    });
    hdfCheckInPriorities.value = priorities;

    // disable the priorities in the priority list that are not in the orders grid
    var GridView = document.getElementById("gvOrderPrintPriority");
    for (var i = 2; i < GridView.rows.length; i++) {
        var priority = GridView.rows[i].cells[1].innerText;
        var isInGrid = $.inArray(priority.slice(0, 1), priorities);
        if (isInGrid < 0) {
            GridView.rows[i].disabled = true;
        }
    }
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

function SetPageHashValue(pageMode) {
    window.location.hash = pageMode;
    SetPageSubmenu(pageMode);
}

function DisplayProgress() {
    $('[id*=upEditOrder]').show();
    setTimeout(HideProgress, 7000);
}

function HideProgress() {
    $('[id*=upEditOrder]').hide();
}


function DoHoldForStockSubmit() {

    var good = true;
    if ($('#tbHfsDate').val() == '') {
        good = false;
    }
    if (!good) return false;
    return true;
}

function DoStockReason() {
    // X, F, L, O
    var sVal = $('#ddlStockReason').find('option:selected').val();
    var msg = '';
    //var pass = true;

    switch (sVal.toLowerCase()) {
        case 'o':
            msg = '';
            break;
        case 'x':
            //pass = false;
            Fail('ddlStockReason', 'divHoldForStockCheckIn', 'divHfsCiErrorMessage', 'A hold for stock reason selection is required.');
            msg = '';
            break;
        case 'f':
            msg = 'Frame: [Frame Code Here] is unavailable.';
            break;
        case 'l':
            msg = 'Lens: [Lens Code Here] is unavailable.';
            break;
    }

    $('#tbHoldForStockReason').val(msg);
    Pass('ddlStockReason', 'divHoldForStockCheckIn', 'divHfsCiErrorMessage');
    Pass('tbHoldForStockReason', 'divHoldForStockCheckIn', 'divHfsCiErrorMessage');

    //if (!pass) return;
    //Pass('ddlStockReason', 'divOrder', 'divOrderError');
}

function DoStatusDateVal() {
    if ($('#tbHfsDate').val() == '')
        return Fail('tbHfsDate', 'divHoldForStockCheckIn', 'divHfsCiErrorMessage', 'An anticipated return to stock date is required.');

    $('#hfStockDate').val($('#tbHfsDate').val());
    return Pass('tbHfsDate', 'divHoldForStockCheckIn', 'divHfsCiErrorMessage');
}

function DoHoldForStockDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 420,
        height: 'auto',
        resize: 'auto',
        title: 'Hold For Stock',
        dialogClass: 'generic',
        open: function () {
            $('#tbHfsDate').val('');
            $('#tbHoldForStockReason').val('');
            $('#ddlStockReason').val('X');
            $('#ddlStockReason').focus();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();

            ClearMsgs('divHoldForStockCheckIn', 'divHfsCiErrorMessage');
        }
    };
    var dHfs = $('#divHoldForStockCheckIn').dialog(dialogOpts);
    dHfs.parent().appendTo($('form:first'));
    dHfs.dialog('open');

}

function PerformFunction(funcName, para1, para2) {
    window[funcName](para1, para2);
}

function DoHoldForStock() {
    var hdfCurrentModule = document.getElementById('hdfCurrentModule');
    hdfCurrentModule.val("holdforstock");
}

function HideAllButtons(showButton1, showButton2) {
    $("#btnOK").hide();
    $("#btnYes").hide();
    $("#btnNo").hide();
    $("#bHoldStockSubmit").hide();
    $("#btnYesCheckin").hide();
    $("#btnSubmitHold").hide();
    $("#btnCancelHold").hide();
    $("#btnSubmitRelease").hide();
    $("#btnCancelRelease").hide();
    $('.hfsDate').hide();
}

function DisplayHoldStockDialog(showButton1) {
    HideAllButtons(showButton1);
    var hdfCommandName = document.getElementById('hdfCommandName');
    switch (hdfCommandName.value) {
        case "SelectAllLensItemType":
        case "SelectAllFrameItemType":
        case "SelectAllCheckinItems":
            $("#btnYes").show();
            $("#btnNo").show();
            $('.hfsDate').hide();
            break;
        case "HoldSelectedItem":
        case "HoldAllSelectedItems":
        case "SubmitHoldItem":
        case "SubmitHoldItems":
            $("#btnSubmitHold").show();
            $("#btnCancelHold").show();
            $(".hfsDate").show();

            break;
        case "SubmitReleaseHoldItem": 5
        case "SubmitReleaseHoldItems":
            $(".hfsDate").hide();
            $("#btnSubmitRelease").show();
            $("#btnCancelRelease").show();
            break;
    }
    var showDialog = $("#lblMessage").text.length;
    if (showDialog > 0) {
        document.getElementById('lblMessage').style.display = 'block';
        $("#msgHoldforStock").fadeIn(10);
    }
    return false;
}

function CloseHoldStockDialog(sender) {
    document.getElementById('msgHoldforStock').style.display = 'none';
    var hdfCommandName = document.getElementById('hdfCommandName');
    switch (sender) {
        case "ReleaseHoldItems":
            break;
        case "btnCancelHold":
        case "btnCancelRelease":
            currentOrderNumber = "";
            currentItem = "";
            currentItemType = "";
            currentItemStatus = "";
            var hdfCommandName = document.getElementById('hdfCommandName');
            $('#hdfCommandName').val("");
            break;
        case "btnNo": // only process the selected item
            switch (hdfCommandName.value) {
                case "SelectAllFrameItemType":
                case "SelectAllLensItemType":
                    var msg = "Order number " + currentOrderNumber + " with a " + currentItem + " of type " + currentItemType + " will be placed on hold for stock.  <br /><br />Please select an estimated stock date and select 'Submit'. <br /><br /><span style='text-align:center;color:red;font-size:smaller'>( If you choose 'Cancel', your selection will be cleared.)</span>";
                    $('#lblMessage').html(msg);
                    $('#hdfCommandName').val("SubmitHoldItem");
                    $('#hdfOrdersSelected').val(currentOrderNumber);
                    DisplayHoldStockDialog();
                    break;
                case "SelectAllCheckinItems":
                    msg = "Order number " + currentOrderNumber + " with a " + currentItem + " of type " + currentItemType + " will be released from hold for stock. <br /><br />Select 'Submit' to continue.<br /><br /><span style='text-align:center;color:red;font-size:smaller'>( If you choose 'Cancel', all selections will be cleared.)</span>";
                    $('#lblMessage').html(msg);
                    $('#hdfCommandName').val("SubmitReleaseHoldItem");
                    $('#hdfOrdersSelected').val(currentOrderNumber);
                    $('.hfsDate').hide();
                    DisplayHoldStockDialog();
            }
            break;
        case "btnYes_SelectAll":   // select all items of this type to process            
            switch (hdfCommandName.value) {
                case "SelectAllFrameItemType":
                case "SelectAllLensItemType":
                    $('#hdfCommandName').val("SelectAllItemsOfType");
                    break;
                case "SelectAllCheckinItems":
                    $("#btnYes").hide();
                    $("#btnNo").hide();
                    $("#btnSubmitRelease").show();
                    $("#btnCancelRelease").show;
                    break;
            }
    }
    return true;
}

function IsInvalidLab() {
    
    var selLab =  $('[id*=ddlRedirectLab]').val();   
    var invalid = false;
    if (selLab.toLowerCase().startsWith("s") )
    {
        var gridData = $('#GridData').jqGrid("getGridParam").data,
        selRows = $.grep(gridData, function (e) { return e.cb == true; });

        $.each(selRows, function (key, value) {
            var p = $("#GridData")[0].p, item = p.data[p._index[value._id_]];
            var lt = item.LensType;
            if (!lt.toLowerCase().startsWith("sv"))
                invalid = true;
        });
    }
    return invalid;
}


function DoRejectRedirect(action) {
    //if (action == 'none') {
    //    $('#divLabRedirectReject').hide();
    //    return;
    //}

    //if ($('#divLabRedirectReject').is('visible') == false)
    //    $('#divLabRedirectReject').show();

    switch (action) {
        case 'redirect':
            $('#divRedirectLab').show();
            $('[id*=lblJustification]').text("Enter justification for redirecting orders: ");
            break;
        case 'reject':
            $('#divRedirectLab').hide();
            $('[id*=lblJustification]').text("Enter justification for rejecting orders: ");
            break;
    }
}

function DoRejectRedirectVal() {
    var stat = $('#rblRejectRedirect').find('input:checked').val();
    var justMsg = 'Reject/Redirect justification of at least 5 characters is required.';

    if (stat == 'redirect' && !DdlRequriedFieldVal('ddlRedirectLab')) {
        $('[id*=divButtonsBottom]' + ' :input[type=submit]').attr('disabled', true);
        return Fail('ddlRedirectLab', 'divButtonsTop', 'divSubmitError', 'A lab is required to redirect an order.');
    }
    else {
        Pass('ddlRedirectLab', 'divButtonsTop', 'divSubmitError');
        Pass('ddlRedirectLab', 'divButtonsBottom', 'divSubmitError');
    }
    
    if ($.trim($('[id$=tbLabJust]').val()) == '' || $.trim($('[id$=tbLabJust]').val()).length < 5) {
        $('[id*=divButtonsBottom]' + ' :input[type=submit]').attr('disabled', true);
        return Fail('tbLabJust', 'divButtonsTop', 'divSubmitError', justMsg);
    }
    else {

        Pass('tbLabJust', 'divButtonsTop', 'divSubmitError');
        Pass('tbLabJust', 'divButtonsBottom', 'divSubmitError');
    }

    if (stat == 'redirect' && IsInvalidLab()) {
        $('[id*=divButtonsBottom]' + ' :input[type=submit]').attr('disabled', true);
        return Fail('ddlRedirectLab', 'divButtonsTop', 'divSubmitError', 'Multivision orders cannot be sent to single vision labs. Please select a different lab.');
    }
    else {
        Pass('tbLabJust', 'divButtonsTop', 'divSubmitError');
        Pass('tbLabJust', 'divButtonsBottom', 'divSubmitError');
    }

   // $('[id$=bLabStatusChange]').focus();
    return true;

}

function GetPriorityCount(rowIndex) {
    var GridView = document.getElementById("gvOrderPrintPriority");
    // get priority code
    var priorityCode = GetPriorityCode(GridView.rows[rowIndex]);

    // find count of selected priorities in grid
    var $grid = $('#GridData'),
        gridData = $grid.jqGrid("getGridParam", "data"),
        ordersForPriority = $.grep(gridData, function (item) {
            return item.Priority == priorityCode;
        });

    if (ordersForPriority != null && ordersForPriority.length > 0) {
        GridView.rows[rowIndex].cells[2].innerText = ordersForPriority.length;
    }
}

function GetPriorityDate(rowIndex) {
    var GridView = document.getElementById("gvOrderPrintPriority");
    // get priority code
    var priorityCode = GetPriorityCode(rowIndex);

    // get oldest date of selected priority
    selDates = [];
    var $grid = $('#GridData'),
        gridData = $grid.jqGrid("getGridParam", "data"),
        ordersForPriority = $.grep(gridData, function (item) {
            return item.Priority == priorityCode;
        });

    if (ordersForPriority != null && ordersForPriority.length == 1) {
        var d = new Date(ordersForPriority[0].OrderDate);
        switch (isNaN(rowIndex)) {
            case (false):
                if (rowIndex != null) {
                    GridView.rows[rowIndex].cells[3].innerText = d.getMonth() + 1 + '/' + d.getDate() + '/' + d.getFullYear();
                }
                break;
            case (true):
                if (rowIndex != null) {
                    GridView.rows[rowIndex.rowIndex].cells[3].innerText = d.getMonth() + 1 + '/' + d.getDate() + '/' + d.getFullYear();
                }
        }
    }
    else {
        if (ordersForPriority.length > 1) {
            for (var r = 0; r < ordersForPriority.length; r++) {
                newDate = new Date(ordersForPriority[r].OrderDate);
                selDates.push(newDate);
            }
        }
        var d = GetOldestDate(selDates);
        switch (isNaN(rowIndex)) {
            case (false):
                if (rowIndex != null) {
                    GridView.rows[rowIndex].cells[3].innerText = d.getMonth() + 1 + '/' + d.getDate() + '/' + d.getFullYear();
                }
                break;
        }
    }
}

function GetOldestDate(dates) {
    var minDate = new Date(Math.min.apply(null, dates));
    return minDate;
}

function GetPriorityCode(rowIndex) {
    var GridView = document.getElementById("gvOrderPrintPriority");
    // get priority code
    var priorityDescription;
    switch (isNaN(rowIndex)) {
        case (true):
            if (rowIndex != null) {
                priorityDescription = GridView.rows[rowIndex.rowIndex].cells[1].innerText;
            }
            break;
        case (false):
            priorityDescription = GridView.rows[rowIndex].cells[1].innerText;
            break;
    }
    if (rowIndex != null) {
        priorityDescription = priorityDescription.replace(/(\r\n|\n|\r|)\s/gm, "");
        var priorityCode = GetCodeForDescription(priorityDescription);
        return priorityCode;
    }
}

function ClearPriorityStats() {
    var GridView = document.getElementById("gvOrderPrintPriority");

    currentPriority = "";

    // reset count field value and date
    for (var i = 2; i < GridView.rows.length; i++) {
        var chb = GridView.rows[i].cells[0].getElementsByTagName("input")[0];
        chb.checked = false;
        GridView.rows[i].style.background = "#ffffff";
        GridView.rows[i].cells[2].innerText = 0;
        GridView.rows[i].cells[3].innerText = "";
    }
}

function checkAllPriorities(checkall) {
    // clear clinic dropdown selections
    if (checkall == "true") {
        $('[id*=ddlOrderClinicCodes]').val('X');
    }

    // select and set all priority items
    var GridView = document.getElementById("gvOrderPrintPriority");
    for (var i = 2; i < GridView.rows.length; i++) {
        var chb = GridView.rows[i].cells[0].getElementsByTagName("input")[0];
        switch (checkall) {
            case "true":
                if (GridView.rows[i].disabled != true) {
                    chb.checked = true;
                    GridView.rows[i].style.background = "#cccccc";

                    // get priority count
                    GetPriorityCount(i);

                    // get priority oldest date
                    GetPriorityDate(i);
                }
                break;
            case "false":
                ClearPriorityStats();
                break;
        }
    }
}

function checkPriorityItem(rowChecked) {
    // get priority code
    var priority = rowChecked.parentNode.parentNode.parentNode.cells[1].innerText
    priority = priority.replace(/(\r\n|\n|\r|)\s/gm, "");
    currentPriority = GetCodeForDescription(priority);;

    // if the priority is not selected
    if (!rowChecked.checked) {
        // make sure All Orders check box is unchecked
        $('[id*=cbCheckInAll]').prop('checked', false);

        // set the row color to white
        rowChecked.parentNode.parentNode.parentNode.style.background = "#ffffff";

        // reset priority item count to 0
        rowChecked.parentNode.parentNode.parentNode.cells[2].innerText = 0;

        // clear priority item date
        rowChecked.parentNode.parentNode.parentNode.cells[3].innerText = "";

        // clear grid selections
        ClearGridSelectedArray();

        // reload grid
        $('#GridData').trigger('reloadGrid');

        // Get Selected Priorities
        var GridView = document.getElementById("gvOrderPrintPriority");
        for (var i = 2; i < GridView.rows.length; i++) {
            var chb = GridView.rows[i].cells[0].getElementsByTagName("input")[0];
            if (GridView.rows[i].disabled != true) {
                if (chb.checked == true) {
                    currentPriority = GetPriorityCode(i);
                    GetSelectedPriorityItems(i);
                }
            }
        }
    }
    else {
        // Get Selected Priorities
        // make sure by clinic dropdown is not selected
        $('[id*=ddlOrderClinicCodes]').val('X');

        GetOrdersByPriorityCode(currentPriority);

        // set the row color to grey
        rowChecked.parentNode.parentNode.parentNode.style.background = "#cccccc";

        // set priority item count
        GetPriorityCount(rowChecked.parentNode.parentNode.parentNode.rowIndex);

        // set priority item oldest date
        GetPriorityDate(rowChecked.parentNode.parentNode.parentNode.rowIndex);
    }
}

function GetSelectedPriorityItems(rowChecked) {
    // make sure by clinic dropdown is not selected
    $('[id*=ddlOrderClinicCodes]').val('X');

    GetOrdersByPriorityCode(currentPriority);

    var GridView = document.getElementById("gvOrderPrintPriority");
    // set the row color to grey
    GridView.rows[rowChecked].style.background = "#cccccc";

    // set priority item count
    GetPriorityCount(GridView.rows[rowChecked]);

    // set priority item oldest date
    GetPriorityDate(GridView.rows[rowChecked]);
}