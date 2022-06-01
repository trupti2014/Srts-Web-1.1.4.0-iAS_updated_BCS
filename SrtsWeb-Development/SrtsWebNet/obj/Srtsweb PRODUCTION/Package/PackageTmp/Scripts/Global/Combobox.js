/// <reference path="jquery-1.11.1.min.js" />
/// <reference path="jquery-ui.min.js" />

(function ($) {
    $.widget("ui.combobox", {
        _create: function () {
            var self = this,
                select = this.element.hide(),
                selected = select.children(":selected"),
                value = selected.val() ? selected.text() : "";
            var input = this.input = $("<input>")
                .insertAfter(select)
                .val(value)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                        response(select.children("option").map(function () {
                            var text = $(this).text();
                            if (request.term == "-Select-" || (this.value && (!request.term || matcher.test(text))))
                                return {
                                    label: text.replace(
                                        new RegExp(
                                            "(?![^&;]+;)(?!<[^<>]*)(" +
                                            $.ui.autocomplete.escapeRegex(request.term) +
                                            ")(?![^<>]*>)(?![^&;]+;)", "gi"
                                        ), "<strong style='color: red;'>$1</strong>"),
                                    value: text,
                                    option: this
                                };
                        }));
                    },
                    select: function (event, ui) {
                        if (ui.item.option != undefined)
                            ui.item.option.selected = true;
                        self._trigger("selected", event, {
                            item: ui.item.option
                        });
                        select.trigger("change");
                    },
                    change: function (event, ui) {
                        if (!ui.item || input[0].value == "") {
                            var matcher = null;

                            if (!ui.item)
                                matcher = new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + $.ui.autocomplete.escapeRegex(input[0].value) + ")(?![^<>]*>)(?![^&;]+;)", "i");
                            else
                                matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i");

                            //var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),

                            valid = false;

                            select.children("option").each(function () {
                                if ($(this).text().match(matcher)) {
                                    this.selected = valid = true;
                                    input[0].value = this.text;
                                    input.data("ui-autocomplete")._trigger("select", event, { item: $(this) });
                                    return false;
                                }
                            });
                            if (!valid) {
                                // remove invalid value, as it didn't match anything
                                var d = select.children('option')[0];
                                $(this).val(d.text);
                                select.val(d.value);
                                input.data("ui-autocomplete").term = d.text;
                                return false;
                            }
                        }
                    }
                })
            .addClass("ui-widget ui-widget-content ui-corner-left");  // Original
            //.addClass("ui-widget ui-widget-content ui-corner-left");  // New

            input.data("ui-autocomplete")._renderItem = function (ul, item) {
                return $("<li></li>")
                    .data("item.autocomplete", item)
                    .append("<a>" + item.label + "</a>")
                    .appendTo(ul);
            };

            this.button = $("<button type='button'>&nbsp;</button>")
                .attr("tabIndex", -1)
                .attr("title", "Show All Items")
                .insertAfter(input)
                .button({
                    icons: {
                        primary: "ui-icon-triangle-1-s"
                    },
                    text: false
                })
                .removeClass("ui-corner-all")
                .addClass("ui-corner-right ui-button-icon")
                .click(function () {
                    // close if already visible
                    if (input.autocomplete("widget").is(":visible")) {
                        input.autocomplete("close");
                        return;
                    }

                    // pass empty string as value to search for, displaying all results
                    input.autocomplete("search", "");
                    input.focus();
                });
        },

        destroy: function () {
            this.input.remove();
            this.button.remove();
            this.element.show();
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);