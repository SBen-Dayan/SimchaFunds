$(() => {
    const contrModal = new bootstrap.Modal($("#contributor-modal")[0]);
    const depositModal = new bootstrap.Modal($('#deposit-modal')[0])

    $('#new-contributor').on('click', () => {
        clearModal();
        setUpContModal(true);
        $('#initial-deposit-div').append(`
            <div>
                <label class="form-label">Initial Deposit</label>
                <input type="text" class="form-control" name="amount" placeholder="Initial Deposit">
            </div>`)
        contrModal.show();
    })


    $('.edit-contrib').on('click', function () {
        setUpContModal(false);
        fillModal({
            firstName: $(this).data('first-name'),
            lastName: $(this).data('last-name'),
            cell: $(this).data('cell-number'),
            email: $(this).data('email'),
            alwaysInclude: $(this).data('always-include')
        });
        $('#contributor-modal-form').append(`<input type="hidden" name="id" value=${$(this).data('id')}>`)
        contrModal.show();
    })

    $(".deposit-button").on('click', function () {
        $('#deposit-modal-name').val($(this).data('cont-name'))
        $('#deposit-modal-form').append(`<input type="hidden" name="contributorId" value=${$(this).data('id')}>`)
        depositModal.show();
    })

    $('#search').on('input', function () {
        const text = $(this).val().toLowerCase();
        $('tbody tr').each(function() {
            if ($(this).find('.name').text().toLowerCase().includes(text)) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        })
    })

    $('#clear').on('click', () => {
        $('#search').val('');
        $('tbody tr').each(function () {
            $(this).show();
        });
    })
    function fillModal({ firstName, lastName, cell, email, alwaysInclude }) {
        $("#modal-first-name").val(firstName);
        $('#modal-last-name').val(lastName);
        $('#modal-cell-number').val(cell);
        $('#modal-email').val(email)
        $('#modal-always-include').prop('checked', alwaysInclude.toLowerCase() === 'true');
    }

    const setUpContModal = (newContributor) => {
        $('#initial-deposit-div').find('div').remove();
        $('#contributor-modal-form').attr('action', newContributor ? '/contributor/add' : '/contributor/update');
        $('#contributor-modal-title').text(newContributor? 'New Contributor' : 'Edit Contributor');
    }

    const clearModal = () => {
        $("#modal-first-name").val('');
        $('#modal-last-name').val('');
        $('#modal-cell-number').val('');
        $('#modal-email').val('');
        $('#modal-always-include').removeClass('checked');
    }

})