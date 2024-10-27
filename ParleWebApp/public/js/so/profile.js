$(document).ready(function() {

    var readURL = function(input) {
        let filename = input.files[0].name;
        if (filename.split(".")[1].toLowerCase() == "jpeg" || filename.split(".")[1].toLowerCase() == "jpg" || filename.split(".")[1].toLowerCase() == "png") {
            var reader = new FileReader();
            reader.onload = function(e) {
                $('.profile-circle').attr('src', e.target.result);
                $('.save-detail').removeClass('d-none');
            }
            reader.readAsDataURL(input.files[0]);
        } else {
            Swal.fire({
                icon: 'error',
                text: ' kindly upload image in jpg,jpeg or png format',
            })
        }
    }
    $("#profile-upload").on('change', function() {
        readURL(this);
    });
});


$(".edit-details").click(function() {
    $(".email-edit,.address-edit").attr('contenteditable', true);
    $(".email-edit,.address-edit").css({ "border-bottom": "1px solid #b5b5b5", "outline": "unset" });
    $('.save-detail').removeClass('d-none');
    $(this).addClass('d-none');
    $('.profile-block').removeClass('pointer-event-none');
    $('.profile-camera').toggleClass('d-none d-flex');
});

profilesuccess = () => {
    Swal.fire({
        icon: 'success',
        title: 'Profile Details Updated Successfully',
        confirmButtonColor: "#2a69b2",
        confirmButtonText: 'Okay',
    }).then((result) => {
        if (result.value) {
            $('#otpmodal').modal('hide');
            $('.save-detail').addClass('d-none')
            $(".email-edit,.address-edit").attr('contenteditable', false);
            $(".email-edit,.address-edit").css({ "border-bottom": "unset", "outline": "unset" });
            $('.edit-details').removeClass('d-none');
            $('.profile-block').addClass('pointer-event-none');
            $('.profile-camera').toggleClass('d-none d-flex');
        }

    })
}

var myModal = new bootstrap.Modal(document.getElementById('otpmodal'), {
    backdrop: "static"
})

function save() {
    myModal.show();
    timer();
}
function hideModal() {
    myModal.hide();
}
$('#resend').click(function() {
    timer();
    $(this).addClass('d-none')
});

function timer() {
    var counter = 59;
    var interval = setInterval(function() {
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