$(() => {
    const modal = new bootstrap.Modal($(".modal")[0]);
    $('#new-simcha').on('click', () => {
        modal.show();
    })
})