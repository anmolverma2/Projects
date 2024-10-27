let OTPModal = new bootstrap.Modal(document.getElementById('otpModal'));
SaveSignature = () => {
    OTPModal.show();
    timer();
}


success = () => {
    Swal.fire({
        icon: 'success',
        title: 'KYC Submitted Successfully',
        confirmButtonColor: "#2a69b2",
        confirmButtonText: 'Okay',
    }).then((result) => {
        if (result.value) {
            location = '../../dsr/dsr.html';
        }
    })
}

// signature
let canvas = document.querySelector("canvas");

let signaturePad = new SignaturePad(canvas);


signaturePad.toDataURL(); // save image as PNG

document.getElementById('clear').addEventListener('click', function() {
    signaturePad.clear();
});

// timer

$("#resend").click(function() {
    $("#resend").addClass('d-none');
    timer();
});
let interval;
timer = () => {
    let counter = 59;
    interval = setInterval(function() {
        counter--;
        if (counter <= 0) {
            clearInterval(interval);
            $('.timer').text('00');
            $('#resend').removeClass('d-none');
            return;
        } else {
            if (counter >= 10) {
                $('.timer').text(" " + counter);
            } else {
                $('.timer').text(" 0" + counter);
            }
        }
    }, 1000);
    $('.close-modal').click(function() {
        clearInterval(interval);
        $('.timer').text('59');
    });
}