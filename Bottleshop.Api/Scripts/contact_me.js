// Contact Form Scripts

$(function() {

    
});


/*When clicking on Full hide fail/success boxes */
$('#name').focus(function() {
    $('#success').html('');
});

var Common = {
    Ajax: function (url, data) {
        var returnData;
        $.ajax({
            url: url,
            data: data,
            type: 'POST',
            async: false,
            success: function (data) {
                returndata = data;
            },
            error: function (e) {
                console.log(e);
                $("#divNError").show();
                $("#spanNetError").text(e.responseText);
            }
        });
        return returndata;
    },
    Nullcheck: function (id, message) {
        var text = $('#' + id).val();
        if (text == '') {
            alert(message);
            return false;
        }
        else {
            return true;
        }
    },
    NullcheckSelector: function (selector, message) {
        var result = true;
        selector.each(function () {
            if ($(this).val() == '') {
                result = false;
            }
        });
        if (!result) {
            Common.Alert(message, message, 'I');
        }
        return result;
    }
}

$.fn.rowspan = function (colIdx, isStats) {
    return this.each(function () {
        var that;
        $('tr', this).each(function (row) {
            $('td:eq(' + colIdx + ')', this).filter(':visible').each(function (col) {

                if ($(this).html() == $(that).html()
					&& (!isStats
							|| isStats && $(this).prev().html() == $(that).prev().html()
							)
					) {
                    rowspan = $(that).attr("rowspan") || 1;
                    rowspan = Number(rowspan) + 1;

                    $(that).attr("rowspan", rowspan);

                    // do your action for the colspan cell here            
                    $(this).hide();

                    //$(this).remove(); 
                    // do your action for the old cell here

                } else {
                    that = this;
                }

                // set the that if not already set
                that = (that == null) ? this : that;
            });
        });
    });
};

