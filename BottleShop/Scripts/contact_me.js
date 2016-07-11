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
                alert('error');
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
