function bindSearchResults(id, bindto) {
    $("[searchResultId$='" + id + "']").each(
        function (i, el) {
            el.onclick = function () {
                bindto($(el));
            }
        }
    );
}