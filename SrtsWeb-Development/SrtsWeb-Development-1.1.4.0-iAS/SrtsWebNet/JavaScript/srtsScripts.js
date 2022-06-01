function onChanging(sender, args) {
    var ae = $find("Animation");
    var an = ae.get_OnLoadBehavior()._animation._animations[0];
    var ao = ae.get_OnLoadBehavior()._animation._animations[1];
    fadein = an;
    fadeout = ao;
    fadein.set_duration(2.0);
    fadeout.set_duration(1.0);
    fadein.play();
    window.setTimeout("fadeout.play()", 11000);
}
function pageLoad() {
    //use the SlideShowExtender's BehaviorID
    var ss = $find("slideshow");
    ss.add_slideChanging(onChanging);
}
function ShowPersonalContent(e, title) {
    document.getElementById("hdrPatientContactInfo").innerHTML = "Patient Personal Information";
}
function HideContactContent() {
    document.getElementById('lidivIDNumber').className = "inactive";
    document.getElementById('lidivAddress').className = "inactive";
    document.getElementById('lidivPhone').className = "inactive";
    document.getElementById('lidivEmail').className = "inactive";
    document.getElementById('divIDNumber').style.display = "none";
    document.getElementById('divAddress').style.display = "none";
    document.getElementById('divPhone').style.display = "none";
    document.getElementById('divEmail').style.display = "none";
}
function HideOrdersContent() {
    document.getElementById('divOrders').style.display = "none";
    document.getElementById('divPrescriptions').style.display = "none";
    document.getElementById('divExams').style.display = "none";
    document.getElementById('lidivOrder').className = "inactive";
    document.getElementById('lidivPrescription').className = "inactive";
    document.getElementById('lidivExam').className = "inactive";
}
function ShowContactContent(d, e, title) {
    HideContactContent();
    document.getElementById(d).style.display = "block";
    document.getElementById(e).className = "current";
    document.getElementById("containertop_header").innerHTML = title;
}
function ShowOrdersContent(d, e, title) {
    HideOrdersContent();
    document.getElementById(d).style.display = "block";
    document.getElementById(e).className = "current";
    document.getElementById("containertop_headerEyewear").innerHTML = title;
}
function AddContactContent(d) {
    document.getElementById(d).style.display = "block";
}
function RefreshIDNumbers() {
    document.getElementByID("uplIDNumbers").Update();
}
function redirectHttpToHttps() {
    var httpURL = window.location.hostname + window.location.pathname + window.location.search;
    var httpsURL = "https://" + httpURL;
    window.location = httpsURL;
}
function HideContent(d) {
    document.getElementById(d).style.display = "none";
}
function ReverseDisplay(d) {
    if (document.getElementById(d).style.display == "none") { document.getElementById(d).style.display = "block"; }
    else { document.getElementById(d).style.display = "none"; }
}
function bookmark(title, url) {
    if (document.all) { // ie
        window.external.AddFavorite(url, title);
    }
    else if (window.sidebar) { // firefox
        window.sidebar.addPanel(title, url, "");
    }
    else if (window.opera && window.print) { // opera
        var elem = document.createElement('a');
        elem.setAttribute('href', url);
        elem.setAttribute('title', title);
        elem.setAttribute('rel', 'sidebar');
        elem.click(); // this.title=document.title;
    }
}