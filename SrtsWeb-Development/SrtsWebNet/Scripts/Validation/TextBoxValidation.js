/// <reference path="../Global/jquery-1.11.1.min.js" />


function KeyUpTextBoxValidate(str)
{
    regex = new RegExp(/[^a-zA-Z0-9\s]/);
    str.value = str.value.replace(regex, '');
}

