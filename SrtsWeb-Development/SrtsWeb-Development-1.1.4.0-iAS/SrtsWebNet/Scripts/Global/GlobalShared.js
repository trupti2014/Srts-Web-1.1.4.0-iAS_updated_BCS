/// <reference path="jquery-1.11.1.min.js" />
/// <reference path="GlobalVal.js" />

function SharedSortOrderNumber(cell, rowObject) {
    if (!(typeof cell === 'string')) return cell;
    var oNum = cell.substr(6, 7);
    var yr = cell.substr(15);
    return parseInt(yr + oNum, 10);
}

function SharedFormatterOrderDate(cellvalue, options, rowObject) {
    if (cellvalue.indexOf(" ") != null || cellvalue.indexOf(" ") != undefined) {
        return cellvalue.substr(0, cellvalue.indexOf(" "));
    }
}

function SharedFormatterPtNameLink(cellvalue, options, rowObject) {
    var patientFullName = rowObject.MiddleName.length > 0 ? cellvalue + ', ' + rowObject.FirstName + ' ' + rowObject.MiddleName.substr(0, 1) : cellvalue + ', ' + rowObject.FirstName;
    //var link = '<a href="../../SrtsPerson/PersonDetails.aspx?id=' + rowObject.IndividualId + '">' + patientFullName +'</a>';        href="#" 
    var link = '<a onclick="GoToDetailsPage(' + rowObject.IndividualId + ');"><u>' + patientFullName +'</u></a>';
    return link;
}

function SharedFormatterPtName(cellvalue, options, rowObject) {
    var patientFullName = rowObject.MiddleName.length > 0 ? cellvalue + ', ' + rowObject.FirstName + ' ' + rowObject.MiddleName.substr(0, 1) : cellvalue + ', ' + rowObject.FirstName;
    return patientFullName;
}
function SharedRedirect(newUrl) {
    if (!UrlVal(newUrl)) return;
    window.location.href = newUrl;
}