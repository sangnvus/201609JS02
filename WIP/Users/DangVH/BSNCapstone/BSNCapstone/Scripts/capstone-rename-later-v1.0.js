$(document).ready(function () {
    $('#choose-book-tag').popover({
        html: true,
        title: function () {
            return $('#book-tag-select').parent().find('.head').html();
        },
        content: function () {
            return $('#book-tag-select').parent().find('.content').html();
        },
        placement: 'top'
    });
});
$('#choose-book-tag').click(function () {
    console.log("pass btn click");

})