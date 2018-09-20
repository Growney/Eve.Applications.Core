$(function () {
    $('.dialog-container').each(
        function (index, element)
        {
            var jqueryEl = $(element);
            var count = 0
            jqueryEl.children('.dialog-close').each( 
                function (cindex, celement) {
                    count++;
                    var jqueryChildEl = $(celement);
                    jqueryChildEl.click(function () {
                        jqueryEl.removeClass('show');
                    })
                }
            );
            if (count == 0) {
                jqueryEl.click(function (clicked) {
                    var clickedJquery = $(clicked.target);
                    if ($(clickedJquery).is(jqueryEl))
                    {
                        jqueryEl.removeClass('show');
                    }
                });
            }
        }
    );
});

function showDialog(dialogID) {
    $('.dialog-container').each(
        function (index, element)
        {
            var jqueryEl = $(element);
            jqueryEl.removeClass('show');
        }
    );
    $('#' + dialogID).addClass('show');
}