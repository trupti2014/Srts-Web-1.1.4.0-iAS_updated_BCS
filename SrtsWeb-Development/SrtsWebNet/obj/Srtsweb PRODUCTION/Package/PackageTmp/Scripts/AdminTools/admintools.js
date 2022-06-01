/// <reference path="../Global/jquery-1.11.1.min.js" />

$(document).ready(function () {
    $('[id*=udpSOT]').hide();
    $('[id*=udpGZT]').hide();

    $(".AdminToolsSelect").click(function () {
        switch (this.id.substring(15, 18)) {
            case "SOT":
                $('[id*=udpSOT]').show();
                $('[id*=udpGZT]').hide();
                BindSOTEvents();
                break;
            case "GZT":
                $('[id*=udpSOT]').hide();
                $('[id*=udpGZT]').show();
                break;
        }
    });
});

function BindSOTEvents() {
    $('[id*=dpkStartDate]').datepicker({
        dateFormat: 'yy-mm-dd',
        autoclose: true
    });
    $('[id*=dpkEndDate]').datepicker({
        dateFormat: 'yy-mm-dd',
        autoclose: true
    });

    $.ajax({
        type: "POST",
        url: 'SrtsAdminTools.aspx/GetSiteDataFromDB',
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var listItems = "";
            listItems += "<option value='------'>-Select-</option>";
            for (i in data.d) {
                listItems += "<option value='" + data.d[i].toString().substring(0, 6) + "'>" + data.d[i] + "</option>";
            }
            $('[id*=ddlSiteCode]').html(listItems);
        },
        failure: function () {
            alert("Failed!");
        }
    });
}

function LoadSiteOrdersGrid() {
    jQuery("#GridData").jqGrid({
        url: 'SrtsAdminTools.aspx/GetSiteOrdersFromDB',
        datatype: 'local',
        mtype: 'POST',
        ajaxGridOptions: { contentType: 'application/json; charset=utf-8' },
        serializeGridData: function (postData) {
            return JSON.stringify(postData);
        },
        colNames: ['Order Number', 'Site', 'Last Updated', 'Current Status', 'Frame', 'Lense', 'Last Name', 'First Name', 'Middle Name', 'Last 4'],
        colModel: [
                      { name: 'OrderNumber', index: 'OrderNumber', width: 100, sorttype: 'text' },
                      { name: 'SiteType', index: 'SiteType', width: 50, sorttype: 'text' },
                      { name: 'StatusDate', index: 'StatusDate', width: 115, sorttype: 'date' },
                      { name: 'StatusComment', index: 'StatusComment', width: 250, sorttype: 'text' },
                      { name: 'FrameCode', index: 'FrameCode', width: 40, sorttype: 'text' },
                      { name: 'LenseType', index: 'LenseType', width: 40, sorttype: 'text' },
                      { name: 'LastName', index: 'LastName', width: 90, sorttype: 'text' },
                      { name: 'FirstName', index: 'FirstName', width: 90, sorttype: 'text' },
                      { name: 'MiddleName', index: 'MiddleName', width: 90, sorttype: 'text' },
                      { name: 'LastFour', index: 'LastFour', width: 55, sorttype: 'text' }
        ],
        sortname: 'OrderNumber',
        sortorder: "asc",
        viewrecords: true,
        loadonce: true,
        autowidth: true,
        shrinkToFit: true,
        height: '100%',
        rowNum: 25,
        rowList: [25, 50, 100],
        pager: '#GridPager',
        caption: "Orders currently open at site",
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
    $("#GridData").jqGrid('setGridParam', {
        datatype:'json',
        postData: {
            SiteCode: $('[id*=ddlSiteCode]').val(),
            StartDate: $('[id*=dpkStartDate]').val(),
            EndDate: $('[id*=dpkEndDate]').val()
        }
    }).trigger('reloadGrid');
}

function BindGZTEvents() {
}