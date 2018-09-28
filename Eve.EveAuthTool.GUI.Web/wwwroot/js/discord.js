function GetLink() {
    $.ajax({
        url: "/Discord/GenerateLink/",
        success: function (data) {
            $('#keyQuery').html('(Generated !)');
            $('#keyResult').html(data);
        }
    });
}