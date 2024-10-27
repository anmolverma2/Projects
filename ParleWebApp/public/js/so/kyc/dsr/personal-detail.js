$(".bank-verify").click(function() {
    $(".verified-msg").removeClass('d-none');
    timer();
});
//  profile upload
let readURL = function(input) {
    let filename = input.files[0].name;
    if (filename.split(".")[1].toLowerCase() == "jpeg" || filename.split(".")[1].toLowerCase() == "jpg" || filename.split(".")[1].toLowerCase() == "png") {
        var reader = new FileReader();
        reader.onload = function(e) {
            $('.profile-circle').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    } else {
        Swal.fire({
            icon: 'error',
            text: ' kindly upload image in jpg,jpeg or png format',
        })
    }
}
$("#personal-image-upload").on('change', function() {
    readURL(this);
});

const currentdate = new Date();
const dob = document.getElementById('dob');
const datepicker = new Datepicker(dob, {
    format: 'dd/mm/yyyy',
    autohide: true,
    minDate: '01/01/1940',
    maxDate: currentdate,
});
const anniversarydob = document.getElementById('anniversary-date');
const datepicker1 = new Datepicker(anniversarydob, {
    'format': 'dd/mm/yyyy',
    autohide: true,
    minDate: '01/01/1940',
    maxDate: currentdate,
});

const doj = document.getElementById('doj');
const datepicker2 = new Datepicker(doj, {
    'format': 'dd/mm/yyyy',
    autohide: true,
    minDate: '01/01/1940',
    maxDate: currentdate,
});

function previewFile(input) {
    let current = input.previousElementSibling;
    let preview = input.nextElementSibling;
    let file = input.files[0];
    let filename = input.files[0].name;
    let fileid = input.nextElementSibling.getAttribute("id").substr(5);
    let deleteimage = input.nextElementSibling.nextElementSibling;

    let reader = new FileReader();

    if (filename.split(".")[1].toLowerCase() == "jpeg" || filename.split(".")[1].toLowerCase() == "jpg" || filename.split(".")[1].toLowerCase() == "png") {
        reader.onloadend = function() {
            current.classList.add('d-none');
            preview.classList.remove('d-none');
            preview.nextElementSibling.classList.remove('d-none');
            preview.src = reader.result;
            $("#filename" + fileid).text(filename)
        }
    } else {
        Swal.fire({
            icon: 'error',
            text: ' kindly upload image in jpg,jpeg or png format',
        })
    }
    if (file) {
        reader.readAsDataURL(file);
    } else {
        preview.src = "";
    }

    $(deleteimage).on("click", function() {
        let imageID = $(this).prev().prev().attr("id").substr(7);
        let fileID = imageID.substr(5);
        console.log(fileID);
        $("#" + imageID).attr("src", "");
        $("#filename" + fileID).text(" ");
        current.classList.remove('d-none');
        preview.classList.add('d-none');
        preview.nextElementSibling.classList.add('d-none');
    });
}