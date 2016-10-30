
$('#myModal').on('show.bs.modal', function (e) {

    //get data-id attribute of the clicked element
    var fileSlide = $(e.relatedTarget).data('fileSlide');

    //populate the textbox
    $(e.currentTarget).find('input[name="fileSlide"]').val(fileSlide);
});