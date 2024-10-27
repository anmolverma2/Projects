function previous() {
    window.history.back();
}



var myModal = new bootstrap.Modal(document.getElementById('resign'))
function save() {
    myModal.show();
}



const elem = document.querySelector('#DateFrom');
const datepicker = new Datepicker(elem, {
autohide: true
});


approve = () => {
Swal.fire({
    icon: 'question',
    title: 'Are You sure  want to Resign ?',
    confirmButtonColor: "#2a69b2",
    confirmButtonText: 'Okay',
    showDenyButton: true,
    confirmButtonText: 'Yes',
    denyButtonText: `No`,

}).then((result) => {
    if (result.value) {

        Swal.fire({
            icon: 'success',
            title: 'Salesman Resigned Successfully',
            confirmButtonText: 'Okay',
        }).then((result) => {
          
            if (result.isConfirmed) {
                location.href = '../../dsr/dsr.html';
            }
        })

    }

})
}


