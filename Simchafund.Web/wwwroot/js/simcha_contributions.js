$(() => {
    $('#update').on('click', function ()  {
        $('.amount').each(function () {
            $(this).closest('tr').prepend(`<input name="contributors[${$(this).data('index')}].Amount" value="${$(this).val().replace('$', '')}" type="text" class="form-control" />`)
        });
    })
})